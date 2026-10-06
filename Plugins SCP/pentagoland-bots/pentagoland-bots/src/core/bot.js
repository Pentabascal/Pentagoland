'use strict';
const fs = require('node:fs');
const path = require('node:path');
const crypto = require('node:crypto');
const { Client, GatewayIntentBits, Events, MessageFlags } = require('discord.js');
const { createLogger } = require('./logger');
const { Settings } = require('./db');
const { hasTier } = require('./perms');

const EPH = MessageFlags.Ephemeral;

function loadCommands(dirs) {
  const map = new Map();
  for (const dir of dirs) {
    if (!fs.existsSync(dir)) continue;
    for (const f of fs.readdirSync(dir).filter((x) => x.endsWith('.js') && !x.startsWith('_'))) {
      const mod = require(path.join(dir, f));
      for (const cmd of Array.isArray(mod) ? mod : [mod]) {
        if (!cmd?.data || typeof cmd.execute !== 'function') throw new Error(`Comando inválido en ${f}`);
        map.set(cmd.data.name, cmd);
      }
    }
  }
  return map;
}

async function safeReply(i, payload) {
  try {
    if (i.deferred) {
      const { flags, ...rest } = payload; // eslint-disable-line no-unused-vars
      return await i.editReply(rest);
    }
    if (i.replied) return await i.followUp({ ...payload, flags: EPH });
    return await i.reply(payload);
  } catch {
    return null;
  }
}

/**
 * Arranca un bot con: carga de comandos, permisos por nivel, cooldowns, manejo de errores,
 * servicios en segundo plano y apagado limpio.
 * opts: { name, token, guildId, db, commandDirs, handlers, services, configSchema, extraCtx }
 */
async function startBot(opts) {
  const { name, token, guildId, db, commandDirs = [], handlers = [], services = [], configSchema = [], extraCtx = {} } = opts;
  const log = createLogger(name);
  const settings = new Settings(db);
  const client = new Client({
    intents: [GatewayIntentBits.Guilds],
    allowedMentions: { parse: [], repliedUser: false }, // nunca menciones masivas por defecto
  });
  const commands = loadCommands([path.join(__dirname, 'commands'), ...commandDirs]);
  const ctx = { client, db, settings, log, commands, guildId, configSchema, botName: name, startedAt: Date.now(), ...extraCtx };
  const cooldowns = new Map();
  let stopping = false;

  client.on(Events.Error, (e) => log.error('Error del cliente:', e));
  client.on(Events.Warn, (m) => log.warn(m));

  client.on(Events.InteractionCreate, async (i) => {
    try {
      if (i.guildId && i.guildId !== guildId) return; // bot de un solo servidor

      if (i.isChatInputCommand()) {
        const cmd = commands.get(i.commandName);
        if (!cmd) return;
        if (!i.inGuild()) {
          return safeReply(i, { content: 'Este comando solo funciona dentro del servidor.', flags: EPH });
        }
        const sub = i.options.getSubcommand(false);
        const need = (sub && cmd.subTiers?.[sub]) ?? cmd.tier;
        if (!hasTier(i.member, settings, need)) {
          return safeReply(i, { content: '🚫 No tienes permiso para usar este comando.', flags: EPH });
        }
        if (cmd.cooldown) {
          const k = `${i.commandName}:${i.user.id}`;
          const until = cooldowns.get(k) || 0;
          if (Date.now() < until) {
            const s = Math.ceil((until - Date.now()) / 1000);
            return safeReply(i, { content: `⏳ Espera ${s}s antes de volver a usarlo.`, flags: EPH });
          }
          cooldowns.set(k, Date.now() + cmd.cooldown * 1000);
        }
        await cmd.execute(i, ctx);
      } else if (i.isAutocomplete()) {
        await commands.get(i.commandName)?.autocomplete?.(i, ctx);
      } else if (i.isMessageComponent() || i.isModalSubmit()) {
        const h = handlers.find((x) => i.customId.startsWith(x.prefix));
        if (h) await h.execute(i, ctx);
      }
    } catch (err) {
      const ref = crypto.randomBytes(3).toString('hex');
      log.error(`Error en interacción (ref ${ref}):`, err);
      if (!i.isAutocomplete()) {
        await safeReply(i, { content: `⚠️ Algo ha fallado (ref \`${ref}\`). Inténtalo de nuevo; si sigue igual, avisa al staff.`, flags: EPH });
      }
    }
  });

  async function stop(code = 0) {
    if (stopping) return;
    stopping = true;
    log.info('Apagando...');
    for (const s of services) { try { await s.stop?.(); } catch (e) { log.warn('Error al parar servicio:', e.message); } }
    try { await client.destroy(); } catch { /* nada */ }
    try { db.close(); } catch { /* nada */ }
    process.exit(code);
  }
  process.on('SIGINT', () => stop(0));
  process.on('SIGTERM', () => stop(0));
  process.on('unhandledRejection', (r) => log.error('unhandledRejection:', r));
  process.on('uncaughtException', (e) => { log.error('uncaughtException:', e); stop(1); });

  client.once(Events.ClientReady, async () => {
    try {
      if (!client.guilds.cache.has(guildId)) log.warn('El bot no está en el servidor GUILD_ID. ¿Lo has invitado?');
      await client.application.commands.set([...commands.values()].map((c) => c.data.toJSON()), guildId);
      log.info(`Conectado como ${client.user.tag} · ${commands.size} comandos registrados`);
      for (const s of services) await s.start?.(ctx);
    } catch (e) {
      log.error('Fallo al iniciar:', e);
      stop(1);
    }
  });

  await client.login(token);
  return { client, ctx, stop };
}

module.exports = { startBot, safeReply };

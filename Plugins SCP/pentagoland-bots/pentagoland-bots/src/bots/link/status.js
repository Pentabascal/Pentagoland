'use strict';
const env = require('../../core/env');
const { buildServerEmbed } = require('../../core/serverEmbed');

/** Convierte la última fila guardada en el objeto público (sin lista de jugadores ni IPs). */
function toPublicStatus(db, staleMs, now = Date.now()) {
  const row = db.prepare('SELECT payload, received_at FROM server_status WHERE id = 1').get();
  if (!row) return { available: false, message: 'Sin datos por ahora.' };
  const p = JSON.parse(row.payload);
  const updatedAt = new Date(row.received_at).toISOString();
  if (now - row.received_at > staleMs) {
    return { available: true, stale: true, online: null, updatedAt, message: 'Sin datos recientes.' };
  }
  return {
    available: true, stale: false, online: p.online,
    players: p.players ?? null, maxPlayers: p.maxPlayers ?? null,
    version: p.version ?? null, name: p.name ?? null, round: p.round ?? null,
    updatedAt, message: null,
  };
}

/** Panel de estado vivo: un único mensaje que se edita (nunca spamea canales). */
function createStatusService() {
  let ctx; let timer; let pending = null; let lastHash = null; let lastEdit = 0; let busy = false;
  const minGap = env.int('PANEL_MIN_INTERVAL_SEC', 45) * 1000;
  const staleMs = () => env.int('STATUS_STALE_SEC', 180) * 1000;

  const publicStatus = () => toPublicStatus(ctx.db, staleMs());

  async function refresh(force = false) {
    if (busy) return;
    busy = true;
    try {
      const row = ctx.db.prepare('SELECT channel_id, message_id FROM panel WHERE guild_id = ?').get(ctx.guildId);
      if (!row) return;
      const address = ctx.settings.get(ctx.guildId, 'server_address');
      const payload = { embeds: [buildServerEmbed(publicStatus(), { address })] };
      const hash = JSON.stringify(payload.embeds[0].toJSON());
      if (!force && hash === lastHash) return;
      const channel = await ctx.client.channels.fetch(row.channel_id).catch(() => null);
      if (!channel?.isTextBased()) return;
      let msg = await channel.messages.fetch(row.message_id).catch(() => null);
      if (msg && msg.author.id === ctx.client.user.id) {
        await msg.edit(payload);
      } else {
        msg = await channel.send(payload);
        ctx.db.prepare('UPDATE panel SET message_id = ? WHERE guild_id = ?').run(msg.id, ctx.guildId);
      }
      lastHash = hash;
      lastEdit = Date.now();
    } catch (e) {
      ctx.log.warn('No se pudo actualizar el panel de estado:', e.message);
    } finally {
      busy = false;
    }
  }

  /** Programa una actualización respetando el intervalo mínimo. */
  function schedule() {
    if (pending) return;
    const wait = Math.max(2000, minGap - (Date.now() - lastEdit));
    pending = setTimeout(() => { pending = null; refresh(); }, wait);
  }

  async function install(channel) {
    const msg = await channel.send({ embeds: [buildServerEmbed(publicStatus(), { address: ctx.settings.get(ctx.guildId, 'server_address') })] });
    ctx.db.prepare(`INSERT INTO panel (guild_id, channel_id, message_id) VALUES (?,?,?)
      ON CONFLICT(guild_id) DO UPDATE SET channel_id = excluded.channel_id, message_id = excluded.message_id`)
      .run(ctx.guildId, channel.id, msg.id);
    lastHash = null;
    return msg;
  }

  return {
    start(c) { ctx = c; timer = setInterval(() => refresh(), env.int('PANEL_TICK_SEC', 60) * 1000); },
    stop() { clearInterval(timer); clearTimeout(pending); },
    refresh, schedule, install, publicStatus,
  };
}

module.exports = { createStatusService, toPublicStatus };

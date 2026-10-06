'use strict';
const { SlashCommandBuilder, MessageFlags } = require('discord.js');
const { simple } = require('../../../core/embeds');
const { tierIndex, TIERS } = require('../../../core/perms');
const { parseDuration, formatDuration } = require('../../../core/util');
const { createCase, logCase, dmUser, canActOn } = require('../modlib');

const EPH = MessageFlags.Ephemeral;
const HELP = 'Si crees que es un error, contacta con el staff desde el canal de soporte.';
const userOpt = (o) => o.setName('usuario').setDescription('Usuario').setRequired(true);
const reasonOpt = (o) => o.setName('motivo').setDescription('Motivo (obligatorio)').setRequired(true).setMinLength(5).setMaxLength(500);
const err = (i, msg) => i.reply({ embeds: [simple('error', 'No se puede', msg)], flags: EPH });
const auditReason = (i, reason) => `${reason} — por ${i.user.tag}`.slice(0, 500);

async function fetchMember(i, user) { return i.guild.members.fetch(user.id).catch(() => null); }

async function finish(i, ctx, type, user, reason, extra = {}) {
  const id = createCase(ctx, { userId: user.id, modId: i.user.id, type, reason, ...extra });
  const row = ctx.db.prepare('SELECT * FROM cases WHERE id = ?').get(id);
  await logCase(ctx, row);
  return id;
}

const warn = {
  tier: 'trial',
  data: new SlashCommandBuilder().setName('warn').setDescription('Avisa formalmente a un usuario.')
    .addUserOption(userOpt).addStringOption(reasonOpt),
  async execute(i, ctx) {
    const user = i.options.getUser('usuario', true);
    const reason = i.options.getString('motivo', true);
    const m = await fetchMember(i, user);
    if (!m) return err(i, 'Ese usuario no está en el servidor.');
    const e = canActOn(i, ctx, m, 'warn');
    if (e) return err(i, e);
    const id = await finish(i, ctx, 'warn', user, reason);
    const dm = await dmUser(user, 'Has recibido un aviso en Pentagoland', `**Motivo:** ${reason}\n\n${HELP}`);
    return i.reply({ embeds: [simple('ok', `Aviso registrado (caso #${id})`, `${user} · MD ${dm ? 'enviado' : 'no entregado'}.`)], flags: EPH });
  },
};

const timeout = {
  tier: 'trial',
  data: new SlashCommandBuilder().setName('timeout').setDescription('Aísla temporalmente a un usuario (timeout nativo).')
    .addUserOption(userOpt)
    .addStringOption((o) => o.setName('duracion').setDescription('Ej: 10m, 2h, 1d (máx. 28d; Trial Moderator máx. 1h)').setRequired(true))
    .addStringOption(reasonOpt),
  async execute(i, ctx) {
    const user = i.options.getUser('usuario', true);
    const reason = i.options.getString('motivo', true);
    const sec = parseDuration(i.options.getString('duracion', true));
    if (!sec || sec < 60 || sec > 28 * 86400) return err(i, 'Duración no válida. Usa por ejemplo `10m`, `2h` o `1d` (entre 1 minuto y 28 días).');
    if (tierIndex(i.member, ctx.settings) < TIERS.indexOf('mod') && sec > 3600) return err(i, 'Como Trial Moderator el máximo es 1 hora.');
    const m = await fetchMember(i, user);
    if (!m) return err(i, 'Ese usuario no está en el servidor.');
    const e = canActOn(i, ctx, m, 'timeout');
    if (e) return err(i, e);
    await m.timeout(sec * 1000, auditReason(i, reason));
    const id = await finish(i, ctx, 'timeout', user, reason, { durationSec: sec });
    await dmUser(user, 'Has sido aislado temporalmente en Pentagoland', `**Duración:** ${formatDuration(sec)}\n**Motivo:** ${reason}\n\n${HELP}`);
    return i.reply({ embeds: [simple('ok', `Timeout aplicado (caso #${id})`, `${user} · ${formatDuration(sec)}`)], flags: EPH });
  },
};

const untimeout = {
  tier: 'trial',
  data: new SlashCommandBuilder().setName('untimeout').setDescription('Retira el timeout de un usuario.')
    .addUserOption(userOpt).addStringOption(reasonOpt),
  async execute(i, ctx) {
    const user = i.options.getUser('usuario', true);
    const reason = i.options.getString('motivo', true);
    const m = await fetchMember(i, user);
    if (!m) return err(i, 'Ese usuario no está en el servidor.');
    if (!m.isCommunicationDisabled()) return err(i, 'Ese usuario no tiene un timeout activo.');
    if (!m.moderatable) return err(i, 'No puedo actuar sobre ese usuario.');
    await m.timeout(null, auditReason(i, reason));
    const id = await finish(i, ctx, 'untimeout', user, reason);
    return i.reply({ embeds: [simple('ok', `Timeout retirado (caso #${id})`, `${user}`)], flags: EPH });
  },
};

const kick = {
  tier: 'mod',
  data: new SlashCommandBuilder().setName('kick').setDescription('Expulsa a un usuario del servidor.')
    .addUserOption(userOpt).addStringOption(reasonOpt),
  async execute(i, ctx) {
    const user = i.options.getUser('usuario', true);
    const reason = i.options.getString('motivo', true);
    const m = await fetchMember(i, user);
    if (!m) return err(i, 'Ese usuario no está en el servidor.');
    const e = canActOn(i, ctx, m, 'kick');
    if (e) return err(i, e);
    await dmUser(user, 'Has sido expulsado de Pentagoland', `**Motivo:** ${reason}`);
    await m.kick(auditReason(i, reason));
    const id = await finish(i, ctx, 'kick', user, reason);
    return i.reply({ embeds: [simple('ok', `Expulsado (caso #${id})`, `${user}`)], flags: EPH });
  },
};

const ban = {
  tier: 'head',
  data: new SlashCommandBuilder().setName('ban').setDescription('Banea a un usuario.')
    .addUserOption(userOpt).addStringOption(reasonOpt)
    .addIntegerOption((o) => o.setName('borrar_mensajes').setDescription('Días de mensajes a borrar').addChoices(
      { name: 'Ninguno', value: 0 }, { name: '1 día', value: 1 }, { name: '7 días', value: 7 })),
  async execute(i, ctx) {
    const user = i.options.getUser('usuario', true);
    const reason = i.options.getString('motivo', true);
    const days = i.options.getInteger('borrar_mensajes') ?? 0;
    const m = await fetchMember(i, user);
    if (m) { const e = canActOn(i, ctx, m, 'ban'); if (e) return err(i, e); }
    else if (user.id === i.user.id) return err(i, 'No puedes banearte a ti mismo.');
    await dmUser(user, 'Has sido baneado de Pentagoland', `**Motivo:** ${reason}\n\n${HELP}`);
    await i.guild.members.ban(user.id, { reason: auditReason(i, reason), deleteMessageSeconds: days * 86400 });
    const id = await finish(i, ctx, 'ban', user, reason);
    return i.reply({ embeds: [simple('ok', `Baneado (caso #${id})`, `${user}`)], flags: EPH });
  },
};

const unban = {
  tier: 'head',
  data: new SlashCommandBuilder().setName('unban').setDescription('Retira el ban de un usuario.')
    .addStringOption((o) => o.setName('id').setDescription('ID de usuario').setRequired(true).setMinLength(17).setMaxLength(20))
    .addStringOption(reasonOpt),
  async execute(i, ctx) {
    const id = i.options.getString('id', true).trim();
    const reason = i.options.getString('motivo', true);
    if (!/^\d{17,20}$/.test(id)) return err(i, 'ID no válida.');
    const b = await i.guild.bans.fetch(id).catch(() => null);
    if (!b) return err(i, 'Ese usuario no está baneado.');
    await i.guild.members.unban(id, auditReason(i, reason));
    const n = await finish(i, ctx, 'unban', b.user, reason);
    return i.reply({ embeds: [simple('ok', `Ban retirado (caso #${n})`, `<@${id}>`)], flags: EPH });
  },
};

const nota = {
  tier: 'trial',
  data: new SlashCommandBuilder().setName('nota').setDescription('Añade una nota interna sobre un usuario (no se le notifica).')
    .addUserOption(userOpt).addStringOption(reasonOpt),
  async execute(i, ctx) {
    const user = i.options.getUser('usuario', true);
    const id = await finish(i, ctx, 'note', user, i.options.getString('motivo', true));
    return i.reply({ embeds: [simple('ok', `Nota guardada (caso #${id})`, `${user}`)], flags: EPH });
  },
};

module.exports = [warn, timeout, untimeout, kick, ban, unban, nota];

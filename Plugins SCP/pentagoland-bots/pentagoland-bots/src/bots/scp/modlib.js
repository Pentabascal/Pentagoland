'use strict';
const { embed } = require('../../core/embeds');
const { sendToChannel, formatDuration, truncate } = require('../../core/util');

const TYPE_LABEL = {
  warn: '⚠️ Aviso', timeout: '⏳ Timeout', untimeout: '✅ Timeout retirado', kick: '👢 Expulsión',
  ban: '🔨 Ban', unban: '♻️ Unban', note: '📝 Nota interna',
};

function createCase(ctx, { userId, modId, type, reason, durationSec = null }) {
  const r = ctx.db.prepare(`INSERT INTO cases (guild_id, user_id, moderator_id, type, reason, duration_sec, created_at)
    VALUES (?,?,?,?,?,?,?)`).run(ctx.guildId, userId, modId, type, reason, durationSec, Date.now());
  return Number(r.lastInsertRowid);
}

function caseEmbed(c) {
  const e = embed('brand').setTitle(`${TYPE_LABEL[c.type]} · caso #${c.id}`).addFields(
    { name: 'Usuario', value: `<@${c.user_id}> (\`${c.user_id}\`)`, inline: true },
    { name: 'Staff', value: `<@${c.moderator_id}>`, inline: true },
  );
  if (c.duration_sec) e.addFields({ name: 'Duración', value: formatDuration(c.duration_sec), inline: true });
  e.addFields({ name: 'Motivo', value: truncate(c.reason, 1000) }).setTimestamp(new Date(c.created_at));
  return e;
}

/** Registra en #logs (solo staff). Nunca incluye IPs ni datos privados. */
const logCase = (ctx, c) => sendToChannel(ctx, 'channel_logs', { embeds: [caseEmbed(c)] });

async function dmUser(user, title, description) {
  try {
    await user.send({ embeds: [embed('brand').setTitle(title).setDescription(description).setFooter({ text: 'Pentagoland' })] });
    return true;
  } catch { return false; }
}

/** Devuelve un texto de error o null si se puede actuar sobre el miembro. */
function canActOn(i, ctx, target, action) {
  if (target.id === i.user.id) return 'No puedes aplicarte una acción a ti mismo.';
  if (target.id === ctx.client.user.id) return 'No puedo aplicarme una acción a mí mismo.';
  if (target.id === i.guild.ownerId) return 'No se puede actuar sobre el propietario del servidor.';
  if (i.user.id !== i.guild.ownerId && target.roles.highest.position >= i.member.roles.highest.position) {
    return 'Ese usuario tiene un rol igual o superior al tuyo.';
  }
  const ok = { warn: true, timeout: target.moderatable, kick: target.kickable, ban: target.bannable }[action];
  return ok ? null : 'No puedo actuar sobre ese usuario: su rol está por encima del mío o me faltan permisos.';
}

module.exports = { TYPE_LABEL, createCase, caseEmbed, logCase, dmUser, canActOn };

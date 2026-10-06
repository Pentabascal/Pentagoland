'use strict';
const {
  ActionRowBuilder, ButtonBuilder, ButtonStyle, ChannelType, PermissionFlagsBits: P,
  MessageFlags, AttachmentBuilder,
} = require('discord.js');
const { embed, simple } = require('../../core/embeds');
const { hasTier } = require('../../core/perms');
const { sendToChannel, ts } = require('../../core/util');

const EPH = MessageFlags.Ephemeral;
const ALLOW = [P.ViewChannel, P.SendMessages, P.ReadMessageHistory, P.AttachFiles, P.EmbedLinks];

// Qué roles ven cada tipo de ticket (los apelaciones, solo Head Staff y Administración).
const TYPES = {
  report: { label: 'Reporte', emoji: '🚨', prefix: 'reporte', roles: ['role_support', 'role_trial', 'role_mod', 'role_head', 'role_admin'] },
  tech: { label: 'Problema técnico', emoji: '🛠️', prefix: 'tecnico', roles: ['role_support', 'role_dev', 'role_head', 'role_admin'] },
  appeal: { label: 'Apelación', emoji: '⚖️', prefix: 'apelacion', roles: ['role_head', 'role_admin'] },
};

function panelPayload() {
  const row = new ActionRowBuilder().addComponents(...Object.entries(TYPES).map(([k, t]) =>
    new ButtonBuilder().setCustomId(`ticket:open:${k}`).setLabel(t.label).setEmoji(t.emoji).setStyle(ButtonStyle.Secondary)));
  return {
    embeds: [embed('brand').setTitle('🛟 Ayuda privada').setDescription(
      'Abre un ticket para **reportes**, **problemas técnicos** o **apelaciones**. Solo lo verán tú y el equipo autorizado.\n\n' +
      '• No publiques reportes en canales públicos.\n' +
      '• Nunca compartas contraseñas, códigos de Steam ni tokens.')],
    components: [row],
  };
}

async function ensureCategory(ctx, guild) {
  const id = ctx.settings.get(guild.id, 'tickets_category');
  const existing = id ? await guild.channels.fetch(id).catch(() => null) : null;
  if (existing?.type === ChannelType.GuildCategory) return existing;
  const cat = await guild.channels.create({
    name: 'TICKETS', type: ChannelType.GuildCategory,
    permissionOverwrites: [
      { id: guild.id, deny: [P.ViewChannel] },
      { id: ctx.client.user.id, allow: [...ALLOW, P.ManageChannels] },
    ],
  });
  ctx.settings.set(guild.id, 'tickets_category', cat.id);
  return cat;
}

async function openTicket(i, ctx, typeKey) {
  const type = TYPES[typeKey];
  if (!type) return;
  await i.deferReply({ flags: EPH });
  const g = i.guild;
  const max = Number(ctx.settings.get(g.id, 'ticket_max_open', '1'));
  let category;
  try { category = await ensureCategory(ctx, g); } catch (e) {
    ctx.log.error('No se pudo crear la categoría de tickets:', e);
    return i.editReply({ embeds: [simple('error', 'No se pudo abrir el ticket', 'Falta configuración. Avisa al staff.')] });
  }
  // Comprobación + inserción sin await entre medias: no hay condición de carrera.
  const open = ctx.db.prepare("SELECT channel_id FROM tickets WHERE guild_id = ? AND opener_id = ? AND status = 'open'").all(g.id, i.user.id);
  if (open.length >= max) {
    return i.editReply({ embeds: [simple('warn', 'Ya tienes un ticket abierto', open.map((t) => `<#${t.channel_id}>`).join(' '))] });
  }
  const ins = ctx.db.prepare('INSERT INTO tickets (guild_id, opener_id, type, created_at) VALUES (?,?,?,?)').run(g.id, i.user.id, typeKey, Date.now());
  const id = Number(ins.lastInsertRowid);

  try {
    const staffIds = [...new Set(type.roles.flatMap((k) => ctx.settings.getList(g.id, k)))];
    const channel = await g.channels.create({
      name: `${type.prefix}-${String(id).padStart(4, '0')}`,
      type: ChannelType.GuildText,
      parent: category.id,
      topic: `Ticket #${id} · ${type.label} · usuario ${i.user.id}`,
      permissionOverwrites: [
        { id: g.id, deny: [P.ViewChannel] },
        { id: ctx.client.user.id, allow: [...ALLOW, P.ManageChannels] },
        { id: i.user.id, allow: ALLOW },
        ...staffIds.map((r) => ({ id: r, allow: ALLOW })),
      ],
    });
    ctx.db.prepare('UPDATE tickets SET channel_id = ? WHERE id = ?').run(channel.id, id);
    const close = new ActionRowBuilder().addComponents(new ButtonBuilder().setCustomId('ticket:close').setLabel('Cerrar ticket').setStyle(ButtonStyle.Danger));
    await channel.send({
      content: `<@${i.user.id}>`,
      embeds: [embed('brand').setTitle(`${type.emoji} ${type.label} · #${id}`).setDescription(
        'Cuéntanos qué ha pasado con el mayor detalle posible. El equipo responderá cuando pueda.\n\n' +
        '**No compartas contraseñas, códigos de Steam ni tokens.**')],
      components: [close],
      allowedMentions: { users: [i.user.id] },
    });
    await sendToChannel(ctx, 'channel_modqueue', { embeds: [simple('info', `Nuevo ticket #${id}`, `${type.label} · <#${channel.id}> · ${ts(Date.now())}`)] });
    return i.editReply({ embeds: [simple('ok', 'Ticket creado', `<#${channel.id}>`)] });
  } catch (e) {
    ctx.log.error('No se pudo crear el ticket:', e);
    ctx.db.prepare('DELETE FROM tickets WHERE id = ?').run(id);
    return i.editReply({ embeds: [simple('error', 'No se pudo abrir el ticket', 'Inténtalo de nuevo o avisa al staff.')] });
  }
}

function ticketOf(ctx, channelId) {
  return ctx.db.prepare("SELECT * FROM tickets WHERE channel_id = ? AND status = 'open'").get(channelId);
}

async function askClose(i, ctx) {
  const t = ticketOf(ctx, i.channelId);
  if (!t) return i.reply({ content: 'Este canal no es un ticket abierto.', flags: EPH });
  if (i.user.id !== t.opener_id && !hasTier(i.member, ctx.settings, 'support') && !hasTier(i.member, ctx.settings, 'dev')) {
    return i.reply({ content: '🚫 Solo quien abrió el ticket o el staff pueden cerrarlo.', flags: EPH });
  }
  const row = new ActionRowBuilder().addComponents(
    new ButtonBuilder().setCustomId('ticket:confirm').setLabel('Confirmar cierre').setStyle(ButtonStyle.Danger),
    new ButtonBuilder().setCustomId('ticket:cancel').setLabel('Cancelar').setStyle(ButtonStyle.Secondary));
  return i.reply({ content: 'Se guardará una transcripción para el staff y el canal se borrará. ¿Cerrar?', components: [row], flags: EPH });
}

async function transcript(channel) {
  const all = [];
  let before;
  for (let n = 0; n < 10; n++) { // hasta 1000 mensajes
    const batch = await channel.messages.fetch({ limit: 100, before });
    if (!batch.size) break;
    all.push(...batch.values());
    before = batch.last().id;
  }
  all.reverse();
  const text = all.map((m) => {
    const files = m.attachments.size ? ` [adjuntos: ${[...m.attachments.values()].map((a) => a.url).join(' ')}]` : '';
    return `[${m.createdAt.toISOString()}] ${m.author.tag} (${m.author.id}): ${m.content}${files}`;
  }).join('\n');
  return { text: text || '(sin mensajes)', count: all.length };
}

async function confirmClose(i, ctx) {
  const t = ticketOf(ctx, i.channelId);
  if (!t) return i.update({ content: 'Este ticket ya estaba cerrado.', components: [] });
  if (!ctx.settings.get(i.guildId, 'channel_logs')) {
    return i.update({ content: '⚠️ Configura el canal de logs (`/config canal`) para guardar la transcripción antes de cerrar.', components: [] });
  }
  await i.update({ content: 'Cerrando…', components: [] });
  const channel = i.channel;
  const { text, count } = await transcript(channel);
  const file = new AttachmentBuilder(Buffer.from(text, 'utf8'), { name: `ticket-${t.id}.txt` });
  const sent = await sendToChannel(ctx, 'channel_logs', {
    embeds: [embed('brand').setTitle(`Ticket #${t.id} cerrado`).addFields(
      { name: 'Tipo', value: TYPES[t.type]?.label ?? t.type, inline: true },
      { name: 'Abierto por', value: `<@${t.opener_id}>`, inline: true },
      { name: 'Cerrado por', value: `<@${i.user.id}>`, inline: true },
      { name: 'Mensajes', value: String(count), inline: true })],
    files: [file],
  });
  if (!sent) return i.editReply({ content: '⚠️ No pude guardar la transcripción, así que NO he borrado el canal. Revisa los permisos del canal de logs.' });
  ctx.db.prepare("UPDATE tickets SET status = 'closed', closed_at = ?, closed_by = ? WHERE id = ?").run(Date.now(), i.user.id, t.id);
  await channel.delete(`Ticket #${t.id} cerrado por ${i.user.tag}`).catch((e) => ctx.log.warn('No se pudo borrar el canal:', e.message));
  return null;
}

const handler = {
  prefix: 'ticket:',
  async execute(i, ctx) {
    if (i.customId.startsWith('ticket:open:')) return openTicket(i, ctx, i.customId.split(':')[2]);
    if (i.customId === 'ticket:close') return askClose(i, ctx);
    if (i.customId === 'ticket:confirm') return confirmClose(i, ctx);
    if (i.customId === 'ticket:cancel') return i.update({ content: 'Cancelado.', components: [] });
    return null;
  },
};

module.exports = { handler, panelPayload, ticketOf, askClose, ALLOW };

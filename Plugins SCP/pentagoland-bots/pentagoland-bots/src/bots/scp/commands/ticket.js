'use strict';
const { SlashCommandBuilder, MessageFlags } = require('discord.js');
const { simple } = require('../../../core/embeds');
const { panelPayload, ticketOf, askClose } = require('../tickets');

const EPH = MessageFlags.Ephemeral;

module.exports = {
  tier: 'support',
  subTiers: { panel: 'admin' },
  data: new SlashCommandBuilder().setName('ticket').setDescription('Gestión de tickets privados.')
    .addSubcommand((s) => s.setName('panel').setDescription('Publica el panel para abrir tickets en este canal.'))
    .addSubcommand((s) => s.setName('cerrar').setDescription('Cierra este ticket (guarda transcripción).'))
    .addSubcommand((s) => s.setName('anadir').setDescription('Añade a un usuario a este ticket.')
      .addUserOption((o) => o.setName('usuario').setDescription('Usuario').setRequired(true)))
    .addSubcommand((s) => s.setName('quitar').setDescription('Quita a un usuario de este ticket.')
      .addUserOption((o) => o.setName('usuario').setDescription('Usuario').setRequired(true))),

  async execute(i, ctx) {
    const sub = i.options.getSubcommand();
    if (sub === 'panel') {
      await i.channel.send(panelPayload());
      return i.reply({ embeds: [simple('ok', 'Panel publicado')], flags: EPH });
    }
    const t = ticketOf(ctx, i.channelId);
    if (!t) return i.reply({ embeds: [simple('error', 'Aquí no', 'Usa este comando dentro de un ticket abierto.')], flags: EPH });
    if (sub === 'cerrar') return askClose(i, ctx);
    const user = i.options.getUser('usuario', true);
    if (sub === 'anadir') {
      await i.channel.permissionOverwrites.edit(user.id, {
        ViewChannel: true, SendMessages: true, ReadMessageHistory: true, AttachFiles: true, EmbedLinks: true,
      });
      return i.reply({ embeds: [simple('ok', 'Usuario añadido', `${user}`)] });
    }
    if (user.id === t.opener_id) return i.reply({ embeds: [simple('error', 'No se puede', 'No puedes quitar a quien abrió el ticket.')], flags: EPH });
    await i.channel.permissionOverwrites.delete(user.id);
    return i.reply({ embeds: [simple('ok', 'Usuario quitado', `${user}`)] });
  },
};

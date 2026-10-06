'use strict';
const { SlashCommandBuilder, MessageFlags } = require('discord.js');
const { simple } = require('../../../core/embeds');
const { sendToChannel } = require('../../../core/util');

const EPH = MessageFlags.Ephemeral;

module.exports = {
  tier: 'mod',
  cooldown: 5,
  data: new SlashCommandBuilder().setName('purge').setDescription('Borra mensajes recientes de este canal (máx. 100, menos de 14 días).')
    .addIntegerOption((o) => o.setName('cantidad').setDescription('1-100').setRequired(true).setMinValue(1).setMaxValue(100)),
  async execute(i, ctx) {
    await i.deferReply({ flags: EPH });
    const n = i.options.getInteger('cantidad', true);
    try {
      const deleted = await i.channel.bulkDelete(n, true);
      await sendToChannel(ctx, 'channel_logs', { embeds: [simple('info', 'Purge', `${i.user} borró **${deleted.size}** mensajes en <#${i.channelId}>.`)] });
      return i.editReply({ embeds: [simple('ok', 'Hecho', `Se borraron ${deleted.size} mensajes.`)] });
    } catch (e) {
      ctx.log.warn('purge falló:', e.message);
      return i.editReply({ embeds: [simple('error', 'No se pudo borrar', 'Comprueba que el bot tenga permiso de Gestionar mensajes en este canal.')] });
    }
  },
};

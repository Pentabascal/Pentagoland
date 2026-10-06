'use strict';
const { SlashCommandBuilder, MessageFlags } = require('discord.js');

module.exports = {
  data: new SlashCommandBuilder().setName('ping').setDescription('Comprueba que el bot responde.'),
  async execute(i, ctx) {
    const ws = Math.round(ctx.client.ws.ping);
    await i.reply({
      content: `🏓 Latencia: **${ws >= 0 ? ws : '?'} ms** · Activo desde <t:${Math.floor(ctx.startedAt / 1000)}:R>`,
      flags: MessageFlags.Ephemeral,
    });
  },
};

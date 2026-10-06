'use strict';
const { SlashCommandBuilder, MessageFlags } = require('discord.js');
const { embed } = require('../embeds');
const { hasTier } = require('../perms');

module.exports = {
  data: new SlashCommandBuilder().setName('ayuda').setDescription('Lista los comandos que puedes usar con este bot.'),
  async execute(i, ctx) {
    const lines = [...ctx.commands.values()]
      .filter((c) => hasTier(i.member, ctx.settings, c.tier))
      .sort((a, b) => a.data.name.localeCompare(b.data.name))
      .map((c) => `**/${c.data.name}** — ${c.data.description}`);
    await i.reply({
      embeds: [embed('brand').setTitle(`Comandos de ${ctx.botName}`).setDescription(lines.join('\n') || 'Sin comandos disponibles.')
        .setFooter({ text: 'Los subcomandos aparecen al escribir el comando.' })],
      flags: MessageFlags.Ephemeral,
    });
  },
};

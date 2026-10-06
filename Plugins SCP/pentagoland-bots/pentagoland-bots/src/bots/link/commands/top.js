'use strict';
const { SlashCommandBuilder, escapeMarkdown } = require('discord.js');
const { embed } = require('../../../core/embeds');

module.exports = {
  cooldown: 10,
  data: new SlashCommandBuilder().setName('top').setDescription('Ranking de kills (solo jugadores con perfil público).'),
  async execute(i, ctx) {
    const rows = ctx.db.prepare(`SELECT l.display_name AS name, s.kills, s.deaths FROM links l
      JOIN player_stats s ON s.steam_id = l.steam_id
      WHERE l.public_stats = 1 AND l.display_name IS NOT NULL ORDER BY s.kills DESC LIMIT 10`).all();
    const text = rows.map((r, n) => `**${n + 1}.** ${escapeMarkdown(r.name)} — ${r.kills} kills · KDR ${(r.kills / Math.max(r.deaths, 1)).toFixed(2)}`).join('\n')
      || 'Todavía no hay jugadores con perfil público.';
    await i.reply({ embeds: [embed('brand').setTitle('🏆 Ranking de kills').setDescription(text).setFooter({ text: 'Solo perfiles públicos' })] });
  },
};

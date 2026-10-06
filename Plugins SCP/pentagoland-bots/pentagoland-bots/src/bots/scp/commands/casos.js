'use strict';
const { SlashCommandBuilder, MessageFlags } = require('discord.js');
const { embed, simple } = require('../../../core/embeds');
const { truncate, ts } = require('../../../core/util');
const { TYPE_LABEL, caseEmbed } = require('../modlib');

const EPH = MessageFlags.Ephemeral;

const casos = {
  tier: 'trial',
  data: new SlashCommandBuilder().setName('casos').setDescription('Historial de casos de un usuario.')
    .addUserOption((o) => o.setName('usuario').setDescription('Usuario').setRequired(true)),
  async execute(i, ctx) {
    const user = i.options.getUser('usuario', true);
    const rows = ctx.db.prepare('SELECT * FROM cases WHERE guild_id = ? AND user_id = ? ORDER BY id DESC LIMIT 15').all(i.guildId, user.id);
    const text = rows.map((r) => `**#${r.id}** ${TYPE_LABEL[r.type]} · ${ts(r.created_at, 'd')} · ${truncate(r.reason, 80)}`).join('\n') || 'Sin casos registrados.';
    return i.reply({ embeds: [embed('brand').setTitle(`Casos de ${user.username}`).setDescription(text)], flags: EPH });
  },
};

const caso = {
  tier: 'trial',
  data: new SlashCommandBuilder().setName('caso').setDescription('Detalle de un caso.')
    .addIntegerOption((o) => o.setName('id').setDescription('Número de caso').setRequired(true).setMinValue(1)),
  async execute(i, ctx) {
    const row = ctx.db.prepare('SELECT * FROM cases WHERE id = ? AND guild_id = ?').get(i.options.getInteger('id', true), i.guildId);
    if (!row) return i.reply({ embeds: [simple('error', 'No existe', 'No hay ningún caso con ese número.')], flags: EPH });
    return i.reply({ embeds: [caseEmbed(row)], flags: EPH });
  },
};

module.exports = [casos, caso];

'use strict';
const { SlashCommandBuilder, MessageFlags } = require('discord.js');
const { embed } = require('../../../core/embeds');
const { ts } = require('../../../core/util');

module.exports = {
  tier: 'dev',
  data: new SlashCommandBuilder().setName('vinculo').setDescription('Diagnóstico de la integración con la web y el servidor (developers).')
    .addSubcommand((s) => s.setName('info').setDescription('Estado de la API y contadores.')),
  async execute(i, ctx) {
    const { db } = ctx;
    const st = db.prepare('SELECT received_at FROM server_status WHERE id = 1').get();
    const count = (sql) => db.prepare(sql).get().c;
    const e = embed('brand').setTitle('🔌 Diagnóstico del vínculo').addFields(
      { name: 'Último estado recibido', value: st ? ts(st.received_at) : 'nunca', inline: true },
      { name: 'Cuentas vinculadas', value: String(count('SELECT COUNT(*) c FROM links')), inline: true },
      { name: 'Perfiles públicos', value: String(count('SELECT COUNT(*) c FROM links WHERE public_stats = 1')), inline: true },
      { name: 'Jugadores con estadísticas', value: String(count('SELECT COUNT(*) c FROM player_stats')), inline: true },
      { name: 'Códigos pendientes', value: String(count('SELECT COUNT(*) c FROM link_codes WHERE expires_at > ' + Date.now())), inline: true },
    );
    await i.reply({ embeds: [e], flags: MessageFlags.Ephemeral });
  },
};

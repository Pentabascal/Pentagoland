'use strict';
const { SlashCommandBuilder, MessageFlags } = require('discord.js');
const env = require('../../../core/env');
const { simple } = require('../../../core/embeds');
const { buildServerEmbed } = require('../../../core/serverEmbed');

const EPH = MessageFlags.Ephemeral;

// Módulo SCP: por ahora solo lee el estado que publica el bot Vínculo.
// Las acciones remotas sobre el servidor de juego NO existen todavía.
module.exports = {
  cooldown: 5,
  data: new SlashCommandBuilder().setName('scp').setDescription('Utilidades de SCP: Secret Laboratory.')
    .addSubcommand((s) => s.setName('servidor').setDescription('Estado del servidor de juego.')),
  async execute(i, ctx) {
    const base = env.str('LINK_API_URL', '');
    if (!base) return i.reply({ embeds: [simple('info', 'Aún no conectado', 'El estado del servidor todavía no está configurado.')], flags: EPH });
    await i.deferReply({ flags: EPH });
    try {
      const ac = new AbortController();
      const t = setTimeout(() => ac.abort(), 5000);
      const res = await fetch(`${base.replace(/\/$/, '')}/public/status.json`, { signal: ac.signal }).finally(() => clearTimeout(t));
      if (!res.ok) throw new Error(`HTTP ${res.status}`);
      const pub = await res.json();
      return i.editReply({ embeds: [buildServerEmbed(pub, { address: ctx.settings.get(i.guildId, 'server_address') })] });
    } catch (e) {
      ctx.log.warn('No se pudo leer el estado:', e.message);
      return i.editReply({ embeds: [simple('warn', 'Sin datos', 'No pude consultar el estado del servidor ahora mismo.')] });
    }
  },
};

'use strict';
const { SlashCommandBuilder, MessageFlags, escapeMarkdown } = require('discord.js');
const { embed, simple } = require('../../../core/embeds');
const { hasTier } = require('../../../core/perms');

const EPH = MessageFlags.Ephemeral;
const NAMES = { chaos: 'Caos', mtf: 'MTF', scp: 'SCP', guard: 'Guardia', classd: 'Clase D', scientist: 'Científico' };

function statsEmbed(user, link, s) {
  const e = embed('brand').setTitle(`Perfil de ${escapeMarkdown(user.displayName ?? user.username)}`)
    .setFooter({ text: 'Datos registrados por el servidor de juego' });
  if (!s) return e.setDescription('Todavía no hay estadísticas registradas por el servidor.');
  const kdr = Math.round((s.kills / Math.max(s.deaths, 1)) * 100) / 100;
  e.addFields(
    { name: 'Kills', value: String(s.kills), inline: true },
    { name: 'Muertes', value: String(s.deaths), inline: true },
    { name: 'KDR', value: String(kdr), inline: true },
    { name: 'Horas', value: (s.seconds_played / 3600).toFixed(1), inline: true },
    { name: 'SCPs asesinados', value: String(s.scp_kills), inline: true },
  );
  const b = JSON.parse(s.breakdown || '{}');
  const lines = Object.keys(NAMES).filter((k) => b[k] !== undefined).map((k) => `${NAMES[k]}: **${b[k]}**`);
  if (lines.length) e.addFields({ name: 'Desglose por facción', value: lines.join(' · ') });
  return e.setTimestamp(new Date(s.updated_at));
}

module.exports = {
  data: new SlashCommandBuilder()
    .setName('perfil')
    .setDescription('Tu perfil y estadísticas de SCP:SL.')
    .addSubcommand((s) => s.setName('ver').setDescription('Muestra un perfil.')
      .addUserOption((o) => o.setName('usuario').setDescription('Otro usuario (solo si su perfil es público)')))
    .addSubcommand((s) => s.setName('publico').setDescription('Muestra u oculta tu perfil en el ranking y para otros.')
      .addBooleanOption((o) => o.setName('activar').setDescription('true = público, false = privado').setRequired(true))),

  async execute(i, ctx) {
    const { db } = ctx;
    if (i.options.getSubcommand() === 'publico') {
      const on = i.options.getBoolean('activar', true);
      const link = db.prepare('SELECT 1 FROM links WHERE discord_id = ?').get(i.user.id);
      if (!link) return i.reply({ embeds: [simple('info', 'Primero vincula tu cuenta', 'Usa `/vincular` con el código del juego.')], flags: EPH });
      db.prepare('UPDATE links SET public_stats = ?, display_name = ? WHERE discord_id = ?')
        .run(on ? 1 : 0, on ? (i.member?.displayName ?? i.user.username).slice(0, 32) : null, i.user.id);
      return i.reply({ embeds: [simple('ok', on ? 'Perfil público' : 'Perfil privado',
        on ? 'Tu nombre y tus estadísticas pueden aparecer en el ranking y en la web. Puedes ocultarlo cuando quieras.' : 'Ya no apareces en el ranking ni en la web.')], flags: EPH });
    }

    const target = i.options.getUser('usuario') ?? i.user;
    const link = db.prepare('SELECT * FROM links WHERE discord_id = ?').get(target.id);
    const self = target.id === i.user.id;
    if (!link) {
      return i.reply({ embeds: [simple('info', self ? 'No tienes cuenta vinculada' : 'Sin perfil', self ? 'Usa `/vincular` con el código que te da el servidor.' : 'Ese usuario no tiene perfil disponible.')], flags: EPH });
    }
    if (!self && !link.public_stats && !hasTier(i.member, ctx.settings, 'mod')) {
      return i.reply({ embeds: [simple('info', 'Sin perfil', 'Ese usuario no tiene perfil disponible.')], flags: EPH });
    }
    const s = db.prepare('SELECT * FROM player_stats WHERE steam_id = ?').get(link.steam_id);
    const member = i.guild.members.cache.get(target.id);
    return i.reply({ embeds: [statsEmbed(member ?? target, link, s)], flags: self ? EPH : undefined });
  },
};

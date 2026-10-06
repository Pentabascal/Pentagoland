'use strict';
const { SlashCommandBuilder, MessageFlags } = require('discord.js');
const { embed, simple } = require('../../../core/embeds');
const { hasTier } = require('../../../core/perms');
const { normalizeHttpsUrl, ts } = require('../../../core/util');

const EPH = MessageFlags.Ephemeral;
const LINKS = [
  ['Invitación de Discord', 'discord'], ['Normas', 'normas'], ['Estado del servidor', 'estado'],
  ['Cómo jugar', 'como-jugar'], ['Galería', 'galeria'], ['Panel', 'panel'],
  ['Comunidad', 'comunidad'], ['Contacto', 'contacto'], ['Plugins', 'plugins'],
].map(([name, value]) => ({ name, value }));
const linkOpt = (o) => o.setName('enlace').setDescription('Qué enlace').setRequired(true).setChoices(...LINKS);

module.exports = {
  subTiers: { 'enlace-poner': 'admin', 'enlace-quitar': 'admin' },
  data: new SlashCommandBuilder()
    .setName('web')
    .setDescription('Información y ajustes de la web de Pentagoland.')
    .addSubcommand((s) => s.setName('estado').setDescription('Comprueba si la web responde.'))
    .addSubcommand((s) => s.setName('enlaces').setDescription('Enlaces oficiales de la web.'))
    .addSubcommand((s) => s.setName('enlace-poner').setDescription('Fija un enlace oficial (https).')
      .addStringOption(linkOpt)
      .addStringOption((o) => o.setName('url').setDescription('URL https').setRequired(true).setMaxLength(300)))
    .addSubcommand((s) => s.setName('enlace-quitar').setDescription('Quita un enlace oficial.').addStringOption(linkOpt)),

  async execute(i, ctx) {
    const sub = i.options.getSubcommand();

    if (sub === 'estado') {
      const url = ctx.settings.get(i.guildId, 'web_url');
      if (!url) return i.reply({ embeds: [simple('info', 'Sin URL configurada', 'Un administrador puede fijarla con `/config texto`.')], flags: EPH });
      const m = ctx.db.prepare('SELECT * FROM monitor WHERE id = 1').get();
      if (!m.last_check) return i.reply({ embeds: [simple('info', 'Aún sin datos', 'Todavía no se ha hecho ninguna comprobación.')], flags: EPH });
      const e = embed(m.up ? 'ok' : 'error').setTitle(m.up ? '🟢 La web responde' : '🔴 La web no responde')
        .addFields(
          { name: 'URL', value: url },
          { name: 'Latencia', value: `${m.latency_ms} ms`, inline: true },
          { name: 'HTTP', value: m.http_status ? String(m.http_status) : '—', inline: true },
          { name: 'Última comprobación', value: ts(m.last_check), inline: true },
        );
      return i.reply({ embeds: [e], flags: EPH });
    }

    if (sub === 'enlaces') {
      const rows = Object.fromEntries(ctx.db.prepare('SELECT key, url FROM links').all().map((r) => [r.key, r.url]));
      const staff = hasTier(i.member, ctx.settings, 'mod');
      const lines = LINKS.filter((l) => rows[l.value] || staff)
        .map((l) => (rows[l.value] ? `**${l.name}:** ${rows[l.value]}` : `**${l.name}:** _pendiente_`));
      if (!lines.length) return i.reply({ embeds: [simple('info', 'Enlaces', 'Todavía no hay enlaces oficiales publicados.')], flags: EPH });
      return i.reply({ embeds: [embed('brand').setTitle('🔗 Enlaces oficiales').setDescription(lines.join('\n'))], flags: EPH });
    }

    const key = i.options.getString('enlace', true);
    if (sub === 'enlace-quitar') {
      ctx.db.prepare('DELETE FROM links WHERE key = ?').run(key);
      return i.reply({ embeds: [simple('ok', 'Enlace quitado', key)], flags: EPH });
    }
    const url = normalizeHttpsUrl(i.options.getString('url', true));
    if (!url) return i.reply({ embeds: [simple('error', 'URL no válida', 'Debe ser https y sin usuario ni contraseña.')], flags: EPH });
    ctx.db.prepare(`INSERT INTO links (key, url, updated_at) VALUES (?, ?, ?)
      ON CONFLICT(key) DO UPDATE SET url = excluded.url, updated_at = excluded.updated_at`).run(key, url, Date.now());
    return i.reply({ embeds: [simple('ok', 'Enlace guardado', `${key}: ${url}`)], flags: EPH });
  },
};

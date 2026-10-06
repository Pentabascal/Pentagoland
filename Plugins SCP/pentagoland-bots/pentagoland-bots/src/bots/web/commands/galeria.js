'use strict';
const { SlashCommandBuilder, MessageFlags } = require('discord.js');
const { embed, simple } = require('../../../core/embeds');
const { normalizeHttpsUrl, truncate } = require('../../../core/util');

const EPH = MessageFlags.Ephemeral;
const idOpt = (o) => o.setName('id').setDescription('Número del elemento').setRequired(true).setMinValue(1);

module.exports = {
  tier: 'mod',
  subTiers: { aprobar: 'head', rechazar: 'head' },
  data: new SlashCommandBuilder()
    .setName('galeria')
    .setDescription('Galería de la web: solo material propio o autorizado.')
    .addSubcommand((s) => s.setName('proponer').setDescription('Propone una imagen (queda pendiente de aprobación).')
      .addStringOption((o) => o.setName('titulo').setDescription('Título').setRequired(true).setMaxLength(100))
      .addStringOption((o) => o.setName('credito').setDescription('Crédito / autor').setRequired(true).setMaxLength(100))
      .addStringOption((o) => o.setName('url').setDescription('URL https de la imagen (alojada en tu web)').setRequired(true).setMaxLength(500))
      .addBooleanOption((o) => o.setName('autorizada').setDescription('Confirmo que es propia o tengo permiso para publicarla').setRequired(true))
      .addStringOption((o) => o.setName('descripcion').setDescription('Texto opcional').setMaxLength(200)))
    .addSubcommand((s) => s.setName('aprobar').setDescription('Aprueba y publica en la web.').addIntegerOption(idOpt))
    .addSubcommand((s) => s.setName('rechazar').setDescription('Rechaza un elemento.').addIntegerOption(idOpt))
    .addSubcommand((s) => s.setName('lista').setDescription('Elementos pendientes y aprobados.')),

  async execute(i, ctx) {
    const sub = i.options.getSubcommand();
    const { db } = ctx;

    if (sub === 'proponer') {
      if (!i.options.getBoolean('autorizada', true)) {
        return i.reply({ embeds: [simple('error', 'No se puede proponer', 'Solo se admite material propio o con permiso.')], flags: EPH });
      }
      const url = normalizeHttpsUrl(i.options.getString('url', true), 500);
      if (!url) return i.reply({ embeds: [simple('error', 'URL no válida', 'Debe ser https y sin credenciales.')], flags: EPH });
      const r = db.prepare('INSERT INTO gallery (title, credit, image_url, caption, submitted_by, created_at) VALUES (?,?,?,?,?,?)')
        .run(i.options.getString('titulo', true), i.options.getString('credito', true), url, i.options.getString('descripcion'), i.user.id, Date.now());
      return i.reply({ embeds: [simple('ok', 'Propuesta registrada', `Elemento **#${r.lastInsertRowid}** pendiente de aprobación.`)], flags: EPH });
    }

    if (sub === 'lista') {
      const rows = db.prepare("SELECT id, title, credit, status FROM gallery WHERE status != 'rejected' ORDER BY id DESC LIMIT 15").all();
      const text = rows.map((r) => `**#${r.id}** · ${r.status === 'approved' ? '✅' : '⏳'} · ${truncate(r.title, 50)} — ${truncate(r.credit, 30)}`).join('\n') || 'No hay elementos.';
      return i.reply({ embeds: [embed('brand').setTitle('Galería').setDescription(text)], flags: EPH });
    }

    const id = i.options.getInteger('id', true);
    const row = db.prepare('SELECT * FROM gallery WHERE id = ?').get(id);
    if (!row) return i.reply({ embeds: [simple('error', 'No existe', `No hay ningún elemento #${id}.`)], flags: EPH });
    const status = sub === 'aprobar' ? 'approved' : 'rejected';
    db.prepare('UPDATE gallery SET status = ? WHERE id = ?').run(status, id);
    return i.reply({ embeds: [simple('ok', sub === 'aprobar' ? 'Aprobado' : 'Rechazado', `Elemento #${id}.`)], flags: EPH });
  },
};

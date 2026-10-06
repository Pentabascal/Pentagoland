'use strict';
const { SlashCommandBuilder, MessageFlags } = require('discord.js');
const { embed, simple } = require('../../../core/embeds');
const { sendToChannel, truncate } = require('../../../core/util');

const EPH = MessageFlags.Ephemeral;
const LABEL = { draft: '📝 borrador', published: '✅ publicada', archived: '📦 archivada' };
const idOpt = (o) => o.setName('id').setDescription('Número de la novedad').setRequired(true).setMinValue(1);

function newsEmbed(r) {
  return embed('brand').setTitle(truncate(r.title, 250)).setDescription(truncate(r.body, 3800))
    .setFooter({ text: 'Pentagoland · Novedades' }).setTimestamp(r.published_at ? new Date(r.published_at) : new Date());
}

module.exports = {
  tier: 'mod',
  subTiers: { publicar: 'head', archivar: 'head' },
  data: new SlashCommandBuilder()
    .setName('novedad')
    .setDescription('Novedades del servidor y de la web (borrador → publicación).')
    .addSubcommand((s) => s.setName('crear').setDescription('Crea un borrador.')
      .addStringOption((o) => o.setName('titulo').setDescription('Título').setRequired(true).setMaxLength(120))
      .addStringOption((o) => o.setName('texto').setDescription('Contenido').setRequired(true).setMaxLength(1800)))
    .addSubcommand((s) => s.setName('editar').setDescription('Edita una novedad.').addIntegerOption(idOpt)
      .addStringOption((o) => o.setName('titulo').setDescription('Nuevo título').setMaxLength(120))
      .addStringOption((o) => o.setName('texto').setDescription('Nuevo contenido').setMaxLength(1800)))
    .addSubcommand((s) => s.setName('publicar').setDescription('Publica en Discord y en el feed de la web.').addIntegerOption(idOpt))
    .addSubcommand((s) => s.setName('archivar').setDescription('Retira una novedad del feed de la web.').addIntegerOption(idOpt))
    .addSubcommand((s) => s.setName('lista').setDescription('Últimas novedades.')),

  async execute(i, ctx) {
    const sub = i.options.getSubcommand();
    const { db } = ctx;
    const get = (id) => db.prepare('SELECT * FROM news WHERE id = ?').get(id);

    if (sub === 'crear') {
      const r = db.prepare('INSERT INTO news (title, body, author_id, created_at) VALUES (?,?,?,?)')
        .run(i.options.getString('titulo', true), i.options.getString('texto', true), i.user.id, Date.now());
      const row = get(Number(r.lastInsertRowid));
      return i.reply({ content: `Borrador **#${row.id}** creado. Vista previa (nada publicado todavía):`, embeds: [newsEmbed(row)], flags: EPH });
    }

    if (sub === 'lista') {
      const rows = db.prepare('SELECT id, title, status FROM news ORDER BY id DESC LIMIT 15').all();
      const text = rows.map((r) => `**#${r.id}** · ${LABEL[r.status]} · ${truncate(r.title, 60)}`).join('\n') || 'No hay novedades.';
      return i.reply({ embeds: [embed('brand').setTitle('Novedades').setDescription(text)], flags: EPH });
    }

    const id = i.options.getInteger('id', true);
    const row = get(id);
    if (!row) return i.reply({ embeds: [simple('error', 'No existe', `No hay ninguna novedad #${id}.`)], flags: EPH });

    if (sub === 'editar') {
      const title = i.options.getString('titulo') ?? row.title;
      const body = i.options.getString('texto') ?? row.body;
      db.prepare('UPDATE news SET title = ?, body = ? WHERE id = ?').run(title, body, id);
      const upd = get(id);
      if (upd.status === 'published' && upd.message_id) {
        try {
          const chId = ctx.settings.get(ctx.guildId, 'channel_novedades');
          const msg = await (await ctx.client.channels.fetch(chId)).messages.fetch(upd.message_id);
          await msg.edit({ embeds: [newsEmbed(upd)] });
        } catch (e) { ctx.log.warn('No se pudo editar el mensaje publicado:', e.message); }
      }
      return i.reply({ content: `Novedad **#${id}** actualizada.`, embeds: [newsEmbed(upd)], flags: EPH });
    }

    if (sub === 'archivar') {
      db.prepare("UPDATE news SET status = 'archived' WHERE id = ?").run(id);
      return i.reply({ embeds: [simple('ok', 'Archivada', `La novedad #${id} ya no aparece en el feed de la web.`)], flags: EPH });
    }

    // publicar
    if (row.status === 'published') return i.reply({ embeds: [simple('warn', 'Ya publicada', `La #${id} ya estaba publicada.`)], flags: EPH });
    await i.deferReply({ flags: EPH });
    const now = Date.now();
    const msg = await sendToChannel(ctx, 'channel_novedades', { embeds: [newsEmbed({ ...row, published_at: now })] });
    if (!msg) {
      return i.editReply({ embeds: [simple('error', 'No se ha publicado nada',
        'No pude enviar el mensaje. Configura el canal con `/config canal` y revisa que el bot pueda escribir allí.')] });
    }
    db.prepare("UPDATE news SET status = 'published', published_at = ?, message_id = ? WHERE id = ?").run(now, msg.id, id);
    return i.editReply({ embeds: [simple('ok', 'Publicada', `La novedad #${id} está en Discord y en el feed de la web.`)] });
  },
};

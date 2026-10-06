'use strict';
const { SlashCommandBuilder, ChannelType, MessageFlags } = require('discord.js');
const { simple, embed } = require('../embeds');

const EPH = MessageFlags.Ephemeral;
const CH_TYPES = [ChannelType.GuildText, ChannelType.GuildAnnouncement, ChannelType.GuildCategory];
const keyOpt = (o) => o.setName('clave').setDescription('Qué ajuste').setRequired(true).setAutocomplete(true);
const TYPE_BY_SUB = { canal: 'channel', rol: 'role', texto: 'text' };

module.exports = {
  tier: 'admin',
  data: new SlashCommandBuilder()
    .setName('config')
    .setDescription('Configura este bot (solo administración).')
    .addSubcommand((s) => s.setName('ver').setDescription('Muestra la configuración actual.'))
    .addSubcommand((s) => s.setName('autodetectar').setDescription('Detecta canales y roles por su nombre habitual.'))
    .addSubcommand((s) => s.setName('canal').setDescription('Asigna un canal o categoría.')
      .addStringOption(keyOpt)
      .addChannelOption((o) => o.setName('canal').setDescription('Canal o categoría').addChannelTypes(...CH_TYPES).setRequired(true)))
    .addSubcommand((s) => s.setName('rol').setDescription('Añade un rol a un ajuste (se suma a los existentes).')
      .addStringOption(keyOpt)
      .addRoleOption((o) => o.setName('rol').setDescription('Rol').setRequired(true)))
    .addSubcommand((s) => s.setName('texto').setDescription('Asigna un valor de texto o número.')
      .addStringOption(keyOpt)
      .addStringOption((o) => o.setName('valor').setDescription('Valor').setRequired(true).setMaxLength(200)))
    .addSubcommand((s) => s.setName('quitar').setDescription('Borra un ajuste.').addStringOption(keyOpt)),

  async autocomplete(i, ctx) {
    const need = TYPE_BY_SUB[i.options.getSubcommand()];
    const q = String(i.options.getFocused()).toLowerCase();
    const out = ctx.configSchema
      .filter((e) => (!need || e.type === need) && (e.label.toLowerCase().includes(q) || e.key.includes(q)))
      .slice(0, 25)
      .map((e) => ({ name: e.label.slice(0, 100), value: e.key }));
    await i.respond(out);
  },

  async execute(i, ctx) {
    const g = i.guildId;
    const sub = i.options.getSubcommand();

    if (sub === 'ver') {
      const lines = ctx.configSchema.map((e) => {
        let v = '—';
        if (e.type === 'channel') { const id = ctx.settings.get(g, e.key); if (id) v = `<#${id}>`; }
        else if (e.type === 'role') { const l = ctx.settings.getList(g, e.key); if (l.length) v = l.map((r) => `<@&${r}>`).join(' '); }
        else { const t = ctx.settings.get(g, e.key); if (t) v = `\`${t}\``; }
        return `**${e.label}:** ${v}`;
      });
      return i.reply({ embeds: [embed('brand').setTitle(`Configuración · ${ctx.botName}`).setDescription(lines.join('\n') || 'Nada configurable.')], flags: EPH });
    }

    if (sub === 'autodetectar') {
      await i.deferReply({ flags: EPH });
      await i.guild.roles.fetch();
      await i.guild.channels.fetch();
      const found = [];
      const missing = [];
      for (const e of ctx.configSchema.filter((x) => x.detect && x.type !== 'text')) {
        if (ctx.settings.get(g, e.key)) continue;
        const name = e.detect.toLowerCase();
        const hit = e.type === 'role'
          ? i.guild.roles.cache.find((r) => r.name.toLowerCase() === name)
          : i.guild.channels.cache.find((c) => c.name.toLowerCase() === name);
        if (hit) { ctx.settings.set(g, e.key, hit.id); found.push(e.label); } else missing.push(e.label);
      }
      return i.editReply({ embeds: [simple('ok', 'Autodetección terminada',
        `**Asignados:** ${found.join(', ') || 'ninguno'}\n**No encontrados:** ${missing.join(', ') || 'ninguno'}`)] });
    }

    const key = i.options.getString('clave', true);
    const entry = ctx.configSchema.find((e) => e.key === key);
    const need = TYPE_BY_SUB[sub];
    if (!entry || (need && entry.type !== need)) {
      return i.reply({ embeds: [simple('error', 'Ajuste no válido', 'Elige uno de la lista desplegable.')], flags: EPH });
    }

    if (sub === 'quitar') {
      ctx.settings.delete(g, key);
      return i.reply({ embeds: [simple('ok', 'Ajuste borrado', entry.label)], flags: EPH });
    }
    if (sub === 'canal') {
      const ch = i.options.getChannel('canal', true);
      ctx.settings.set(g, key, ch.id);
      return i.reply({ embeds: [simple('ok', entry.label, `Asignado a <#${ch.id}>.`)], flags: EPH });
    }
    if (sub === 'rol') {
      const role = i.options.getRole('rol', true);
      ctx.settings.add(g, key, role.id);
      return i.reply({ embeds: [simple('ok', entry.label, `Añadido <@&${role.id}>. Usa \`/config quitar\` para vaciar la lista.`)], flags: EPH });
    }
    // texto
    let value = i.options.getString('valor', true).trim();
    if (entry.validate) {
      value = entry.validate(value);
      if (value === null) return i.reply({ embeds: [simple('error', 'Valor no válido', entry.hint || 'Revisa el formato.')], flags: EPH });
    }
    ctx.settings.set(g, key, value);
    return i.reply({ embeds: [simple('ok', entry.label, `Guardado: \`${value}\``)], flags: EPH });
  },
};

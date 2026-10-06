'use strict';
const { SlashCommandBuilder, MessageFlags, ChannelType, PermissionFlagsBits } = require('discord.js');
const { simple } = require('../../../core/embeds');
const { buildServerEmbed } = require('../../../core/serverEmbed');

const EPH = MessageFlags.Ephemeral;

module.exports = {
  subTiers: { panel: 'admin', actualizar: 'mod' },
  data: new SlashCommandBuilder()
    .setName('servidor')
    .setDescription('Estado del servidor de SCP:SL.')
    .addSubcommand((s) => s.setName('estado').setDescription('Muestra el estado actual.'))
    .addSubcommand((s) => s.setName('panel').setDescription('Crea el mensaje de estado que se actualiza solo.')
      .addChannelOption((o) => o.setName('canal').setDescription('Por defecto, el configurado').addChannelTypes(ChannelType.GuildText, ChannelType.GuildAnnouncement)))
    .addSubcommand((s) => s.setName('actualizar').setDescription('Fuerza la actualización del panel.')),

  async execute(i, ctx) {
    const sub = i.options.getSubcommand();
    const address = ctx.settings.get(i.guildId, 'server_address');

    if (sub === 'estado') {
      return i.reply({ embeds: [buildServerEmbed(ctx.status.publicStatus(), { address })], flags: EPH });
    }
    if (sub === 'actualizar') {
      await ctx.status.refresh(true);
      return i.reply({ embeds: [simple('ok', 'Panel actualizado')], flags: EPH });
    }
    // panel
    const chId = i.options.getChannel('canal')?.id ?? ctx.settings.get(i.guildId, 'channel_estado');
    const channel = chId ? await i.guild.channels.fetch(chId).catch(() => null) : null;
    if (!channel) return i.reply({ embeds: [simple('error', 'Falta el canal', 'Indica un canal o configúralo con `/config canal`.')], flags: EPH });
    const perms = channel.permissionsFor(i.guild.members.me);
    if (!perms?.has([PermissionFlagsBits.ViewChannel, PermissionFlagsBits.SendMessages, PermissionFlagsBits.EmbedLinks])) {
      return i.reply({ embeds: [simple('error', 'Sin permisos', 'El bot necesita ver el canal, escribir y usar embeds.')], flags: EPH });
    }
    await ctx.status.install(channel);
    return i.reply({ embeds: [simple('ok', 'Panel creado', `Se actualizará automáticamente en <#${channel.id}>.`)], flags: EPH });
  },
};

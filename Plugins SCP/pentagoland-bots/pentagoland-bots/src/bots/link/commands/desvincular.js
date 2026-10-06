'use strict';
const { SlashCommandBuilder, MessageFlags } = require('discord.js');
const { simple } = require('../../../core/embeds');

const EPH = MessageFlags.Ephemeral;

module.exports = {
  data: new SlashCommandBuilder().setName('desvincular').setDescription('Desvincula tu cuenta de SCP:SL y retira tu perfil público.'),
  async execute(i, ctx) {
    const r = ctx.db.prepare('DELETE FROM links WHERE discord_id = ?').run(i.user.id);
    if (!r.changes) return i.reply({ embeds: [simple('info', 'No estabas vinculado')], flags: EPH });
    const roleId = ctx.settings.get(i.guildId, 'role_player');
    if (roleId) await i.member.roles.remove(roleId, 'Cuenta desvinculada').catch(() => {});
    return i.reply({ embeds: [simple('ok', 'Cuenta desvinculada', 'Se ha eliminado el vínculo, tu nombre público y tu presencia en el ranking.')], flags: EPH });
  },
};

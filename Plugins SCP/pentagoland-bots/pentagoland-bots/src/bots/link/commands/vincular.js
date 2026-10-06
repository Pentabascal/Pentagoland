'use strict';
const { SlashCommandBuilder, MessageFlags } = require('discord.js');
const { simple } = require('../../../core/embeds');

const EPH = MessageFlags.Ephemeral;
const mask = (id) => { const [n, p] = id.split('@'); return `••••${n.slice(-4)}@${p}`; };

module.exports = {
  cooldown: 5,
  data: new SlashCommandBuilder()
    .setName('vincular')
    .setDescription('Vincula tu Discord con tu cuenta de SCP:SL usando el código del juego.')
    .addStringOption((o) => o.setName('codigo').setDescription('Código que te da el servidor dentro del juego').setRequired(true).setMinLength(6).setMaxLength(12)),

  async execute(i, ctx) {
    const { db } = ctx;
    const code = i.options.getString('codigo', true).toUpperCase().replace(/[^A-Z0-9]/g, '');
    if (db.prepare('SELECT 1 FROM links WHERE discord_id = ?').get(i.user.id)) {
      return i.reply({ embeds: [simple('warn', 'Ya estás vinculado', 'Usa `/desvincular` si quieres cambiar de cuenta.')], flags: EPH });
    }
    const result = db.transaction(() => {
      const row = db.prepare('SELECT steam_id, expires_at FROM link_codes WHERE code = ?').get(code);
      if (!row || row.expires_at < Date.now()) return { error: 'invalid' };
      if (db.prepare('SELECT 1 FROM links WHERE steam_id = ?').get(row.steam_id)) {
        db.prepare('DELETE FROM link_codes WHERE steam_id = ?').run(row.steam_id);
        return { error: 'taken' };
      }
      db.prepare('INSERT INTO links (discord_id, steam_id, linked_at) VALUES (?,?,?)').run(i.user.id, row.steam_id, Date.now());
      db.prepare('DELETE FROM link_codes WHERE steam_id = ?').run(row.steam_id);
      return { steamId: row.steam_id };
    })();

    if (result.error === 'taken') {
      return i.reply({ embeds: [simple('error', 'Cuenta ya vinculada', 'Esa cuenta de juego ya está vinculada a otro usuario.')], flags: EPH });
    }
    if (result.error) {
      return i.reply({ embeds: [simple('error', 'Código no válido o caducado', 'Pide uno nuevo desde el juego y vuelve a probar.')], flags: EPH });
    }
    const roleId = ctx.settings.get(i.guildId, 'role_player');
    if (roleId) await i.member.roles.add(roleId, 'Cuenta de SCP:SL vinculada').catch((e) => ctx.log.warn('No se pudo dar el rol:', e.message));
    return i.reply({ embeds: [simple('ok', 'Cuenta vinculada', `Tu Discord queda vinculado a \`${mask(result.steamId)}\`.\nPuedes ver tus datos con \`/perfil ver\` y desvincular cuando quieras con \`/desvincular\`.`)], flags: EPH });
  },
};

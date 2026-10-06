'use strict';
const { embed, COLORS } = require('./embeds');
const { ts } = require('./util');

/** `pub` es el objeto de /public/status.json. Solo muestra lo que el servidor haya enviado de verdad. */
function buildServerEmbed(pub, { address } = {}) {
  const e = embed('brand').setTitle('📡 Estado del servidor SCP:SL').setFooter({ text: 'Pentagoland' });
  if (!pub || pub.available === false) {
    return e.setColor(COLORS.soft).setDescription('Sin datos por ahora. Aparecerán aquí cuando el servidor envíe información.');
  }
  const updated = pub.updatedAt ? Date.parse(pub.updatedAt) : null;
  const when = updated ? ts(updated) : 'desconocida';
  if (pub.stale) {
    return e.setColor(COLORS.soft).setDescription(`⚠️ **Sin datos recientes.** Última actualización: ${when}.`);
  }
  e.addFields({ name: 'Estado', value: pub.online ? '🟢 En línea' : '🔴 Fuera de línea', inline: true });
  if (Number.isInteger(pub.players)) {
    const v = Number.isInteger(pub.maxPlayers) ? `${pub.players}/${pub.maxPlayers}` : `${pub.players}`;
    e.addFields({ name: 'Jugadores', value: v, inline: true });
  }
  if (pub.version) e.addFields({ name: 'Versión', value: String(pub.version), inline: true });
  if (address) e.addFields({ name: 'Dirección', value: `\`${String(address).replace(/`/g, '')}\`` });
  e.addFields({ name: 'Última actualización', value: when });
  if (updated) e.setTimestamp(new Date(updated));
  return e;
}

module.exports = { buildServerEmbed };

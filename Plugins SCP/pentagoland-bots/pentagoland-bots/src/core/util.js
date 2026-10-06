'use strict';

function truncate(str, max) {
  const s = String(str ?? '');
  return s.length > max ? `${s.slice(0, max - 1)}…` : s;
}

/** "10m", "2h", "1d", "1w" → segundos (o null). */
function parseDuration(input) {
  const m = /^(\d{1,4})\s*([smhdw])$/i.exec(String(input).trim());
  if (!m) return null;
  const mult = { s: 1, m: 60, h: 3600, d: 86400, w: 604800 }[m[2].toLowerCase()];
  return Number(m[1]) * mult;
}

function formatDuration(sec) {
  const parts = [];
  for (const [u, n] of [['d', 86400], ['h', 3600], ['m', 60], ['s', 1]]) {
    const v = Math.floor(sec / n);
    if (v) { parts.push(`${v}${u}`); sec -= v * n; }
  }
  return parts.join(' ') || '0s';
}

/** Solo https, sin credenciales embebidas. Devuelve la URL normalizada o null. */
function normalizeHttpsUrl(value, maxLen = 300) {
  try {
    const u = new URL(String(value).trim());
    if (u.protocol !== 'https:' || u.username || u.password || u.href.length > maxLen) return null;
    return u.href;
  } catch {
    return null;
  }
}

const ts = (ms, style = 'R') => `<t:${Math.floor(ms / 1000)}:${style}>`;

/** Envía a un canal configurado (clave de ajustes). Devuelve el mensaje o null. */
async function sendToChannel(ctx, key, payload) {
  const id = ctx.settings.get(ctx.guildId, key);
  if (!id) return null;
  try {
    const ch = await ctx.client.channels.fetch(id);
    if (!ch || !ch.isTextBased()) return null;
    return await ch.send(payload);
  } catch (e) {
    ctx.log.warn(`No se pudo enviar a ${key}:`, e.message);
    return null;
  }
}

module.exports = { truncate, parseDuration, formatDuration, normalizeHttpsUrl, ts, sendToChannel };

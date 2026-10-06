'use strict';
const { EmbedBuilder } = require('discord.js');

// Paleta Pentagoland: negro, rosa claro, morado, blanco y azul claro
const COLORS = {
  brand: 0x9b5de5,
  soft: 0xf8b4d9,
  info: 0x8fd3ff,
  ok: 0x8fd3ff,
  warn: 0xf4b860,
  error: 0xe5484d,
  dark: 0x0d0d12,
};

const embed = (kind = 'brand') => new EmbedBuilder().setColor(COLORS[kind] ?? COLORS.brand);

const ICONS = { ok: '✅', error: '❌', warn: '⚠️', info: 'ℹ️', brand: '', soft: '' };
function simple(kind, title, description) {
  const e = embed(kind).setTitle(`${ICONS[kind] ? ICONS[kind] + ' ' : ''}${title}`);
  if (description) e.setDescription(description);
  return e;
}

module.exports = { COLORS, embed, simple };

'use strict';
const { PermissionFlagsBits } = require('discord.js');

// De menor a mayor. Cada nivel incluye a los anteriores.
const TIERS = ['support', 'trial', 'mod', 'head', 'admin'];

function tierIndex(member, settings) {
  if (!member?.guild) return -1;
  const top = TIERS.length - 1;
  if (member.id === member.guild.ownerId) return top;
  if (member.permissions?.has(PermissionFlagsBits.Administrator)) return top;
  let best = -1;
  TIERS.forEach((tier, idx) => {
    const ids = settings.getList(member.guild.id, `role_${tier}`);
    if (ids.some((id) => member.roles.cache.has(id))) best = Math.max(best, idx);
  });
  return best;
}

function isDev(member, settings) {
  if (tierIndex(member, settings) >= TIERS.indexOf('admin')) return true;
  return settings.getList(member.guild.id, 'role_dev').some((id) => member.roles.cache.has(id));
}

function hasTier(member, settings, tier) {
  if (!tier) return true;
  if (tier === 'dev') return isDev(member, settings);
  return tierIndex(member, settings) >= TIERS.indexOf(tier);
}

module.exports = { TIERS, tierIndex, hasTier, isDev };

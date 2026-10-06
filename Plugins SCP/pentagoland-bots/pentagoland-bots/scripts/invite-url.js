'use strict';
// Uso: npm run invite -- web | link | scp     (necesita WEB_CLIENT_ID / LINK_CLIENT_ID / SCP_CLIENT_ID en .env)
require('dotenv').config();
const { PermissionFlagsBits: P, PermissionsBitField } = require('discord.js');

const BOTS = {
  web: { env: 'WEB_CLIENT_ID', perms: [P.ViewChannel, P.SendMessages, P.EmbedLinks, P.ReadMessageHistory] },
  link: { env: 'LINK_CLIENT_ID', perms: [P.ViewChannel, P.SendMessages, P.EmbedLinks, P.ReadMessageHistory, P.ManageRoles] },
  scp: {
    env: 'SCP_CLIENT_ID',
    perms: [P.ViewChannel, P.SendMessages, P.EmbedLinks, P.AttachFiles, P.ReadMessageHistory, P.ManageMessages,
      P.ManageChannels, P.ModerateMembers, P.KickMembers, P.BanMembers],
  },
};

const which = process.argv[2];
const bot = BOTS[which];
if (!bot) { console.error('Uso: npm run invite -- web|link|scp'); process.exit(1); }
const clientId = process.env[bot.env];
if (!clientId) { console.error(`Falta ${bot.env} en .env`); process.exit(1); }
const perms = new PermissionsBitField(bot.perms).bitfield.toString();
console.log(`https://discord.com/oauth2/authorize?client_id=${clientId}&scope=bot%20applications.commands&permissions=${perms}`);

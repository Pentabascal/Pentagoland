'use strict';
const path = require('node:path');
const env = require('../../core/env');
const { openDb } = require('../../core/db');
const { startBot } = require('../../core/bot');
const migrations = require('./migrations');
const configSchema = require('./schema');
const { handler: ticketHandler } = require('./tickets');

(async () => {
  const db = openDb(path.join(env.str('DATA_DIR', './data'), 'scp.db'), migrations);
  await startBot({
    name: 'sentinel',
    token: env.str('SCP_BOT_TOKEN'),
    guildId: env.str('GUILD_ID'),
    db,
    commandDirs: [path.join(__dirname, 'commands')],
    handlers: [ticketHandler],
    configSchema,
  });
})().catch((e) => { console.error(e); process.exit(1); });

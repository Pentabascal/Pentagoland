'use strict';
const path = require('node:path');
const env = require('../../core/env');
const { openDb } = require('../../core/db');
const { startBot } = require('../../core/bot');
const migrations = require('./migrations');
const configSchema = require('./schema');
const { createMonitor } = require('./monitor');
const { createApi } = require('./api');

(async () => {
  const db = openDb(path.join(env.str('DATA_DIR', './data'), 'web.db'), migrations);
  await startBot({
    name: 'web',
    token: env.str('WEB_BOT_TOKEN'),
    guildId: env.str('GUILD_ID'),
    db,
    commandDirs: [path.join(__dirname, 'commands')],
    services: [createMonitor(), createApi()],
    configSchema,
  });
})().catch((e) => { console.error(e); process.exit(1); });

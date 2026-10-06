'use strict';
const path = require('node:path');
const env = require('../../core/env');
const { openDb } = require('../../core/db');
const { startBot } = require('../../core/bot');
const migrations = require('./migrations');
const configSchema = require('./schema');
const { createStatusService } = require('./status');
const { createApi } = require('./api');

(async () => {
  const db = openDb(path.join(env.str('DATA_DIR', './data'), 'link.db'), migrations);
  const status = createStatusService();
  await startBot({
    name: 'link',
    token: env.str('LINK_BOT_TOKEN'),
    guildId: env.str('GUILD_ID'),
    db,
    commandDirs: [path.join(__dirname, 'commands')],
    services: [status, createApi({ status })],
    configSchema,
    extraCtx: { status },
  });
})().catch((e) => { console.error(e); process.exit(1); });

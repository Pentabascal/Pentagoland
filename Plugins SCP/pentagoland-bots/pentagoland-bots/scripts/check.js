'use strict';
// Comprueba la configuración antes de arrancar. Uso: npm run check
require('dotenv').config();

const common = ['GUILD_ID'];
const per = {
  web: ['WEB_BOT_TOKEN'],
  link: ['LINK_BOT_TOKEN', 'API_SECRET_SERVER', 'API_SECRET_WEB'],
  scp: ['SCP_BOT_TOKEN'],
};
let bad = 0;
const need = (k, min = 1) => {
  const v = process.env[k];
  if (!v || v.length < min) { console.log(`✗ ${k}${min > 1 ? ` (mín. ${min} caracteres)` : ''}`); bad++; } else console.log(`✓ ${k}`);
};
common.forEach((k) => need(k));
for (const [bot, keys] of Object.entries(per)) {
  console.log(`\n[${bot}]`);
  keys.forEach((k) => need(k, k.startsWith('API_SECRET') ? 32 : 1));
}
if (process.env.API_SECRET_SERVER && process.env.API_SECRET_SERVER === process.env.API_SECRET_WEB) {
  console.log('✗ API_SECRET_SERVER y API_SECRET_WEB deben ser distintos'); bad++;
}
console.log(bad ? `\n${bad} problema(s).` : '\nTodo correcto.');
process.exit(bad ? 1 : 0);

'use strict';
const { staffRoleSchema } = require('../../core/schema');

module.exports = [
  ...staffRoleSchema,
  { key: 'channel_logs', type: 'channel', label: 'Canal: logs (staff)', detect: 'logs' },
  { key: 'channel_modqueue', type: 'channel', label: 'Canal: mod-queue (avisos de tickets)', detect: 'mod-queue' },
  { key: 'tickets_category', type: 'channel', label: 'Categoría de tickets (se crea sola si falta)' },
  { key: 'ticket_max_open', type: 'text', label: 'Tickets abiertos máximos por usuario', validate: (v) => (/^[1-5]$/.test(v) ? v : null), hint: 'Un número del 1 al 5.' },
];

'use strict';
const { staffRoleSchema } = require('../../core/schema');

module.exports = [
  ...staffRoleSchema,
  { key: 'channel_estado', type: 'channel', label: 'Canal: estado del servidor', detect: 'estado-servidor' },
  { key: 'channel_anuncios', type: 'channel', label: 'Canal: anuncios', detect: 'anuncios' },
  { key: 'channel_novedades', type: 'channel', label: 'Canal: novedades del servidor', detect: 'novedades-del-servidor' },
  { key: 'channel_operaciones', type: 'channel', label: 'Canal: operaciones (staff)', detect: 'operaciones' },
  { key: 'role_player', type: 'role', label: 'Rol al vincular (Jugador SCP:SL)', detect: 'Jugador SCP:SL' },
  { key: 'server_address', type: 'text', label: 'Dirección pública del servidor (opcional)', validate: (v) => (/^[\w.\-:]{3,80}$/.test(v) ? v : null), hint: 'Solo IP/dominio y puerto, p. ej. ejemplo.com:7777' },
];

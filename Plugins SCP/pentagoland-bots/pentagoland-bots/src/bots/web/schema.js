'use strict';
const { staffRoleSchema } = require('../../core/schema');
const { normalizeHttpsUrl } = require('../../core/util');

module.exports = [
  ...staffRoleSchema,
  { key: 'channel_novedades', type: 'channel', label: 'Canal: novedades del servidor', detect: 'novedades-del-servidor' },
  { key: 'channel_anuncios', type: 'channel', label: 'Canal: anuncios', detect: 'anuncios' },
  { key: 'channel_operaciones', type: 'channel', label: 'Canal: operaciones (staff)', detect: 'operaciones' },
  { key: 'web_url', type: 'text', label: 'URL de la web (https)', validate: (v) => normalizeHttpsUrl(v), hint: 'Debe ser una URL https válida, sin usuario ni contraseña.' },
];

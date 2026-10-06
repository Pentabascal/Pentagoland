'use strict';
require('dotenv').config();

function str(name, def) {
  const v = process.env[name];
  if (v === undefined || v === '') {
    if (def === undefined) throw new Error(`Falta la variable de entorno ${name} (mira .env.example).`);
    return def;
  }
  return v;
}
function int(name, def) {
  const n = Number.parseInt(str(name, String(def)), 10);
  if (Number.isNaN(n)) throw new Error(`${name} debe ser un número.`);
  return n;
}
function bool(name, def = false) {
  return ['1', 'true', 'yes', 'si', 'sí', 'on'].includes(str(name, String(def)).toLowerCase());
}
function secret(name) {
  const v = str(name);
  if (v.length < 32) throw new Error(`${name} debe tener al menos 32 caracteres.`);
  return v;
}
module.exports = { str, int, bool, secret };

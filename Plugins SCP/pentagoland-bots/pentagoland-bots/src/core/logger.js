'use strict';
const LEVELS = { debug: 10, info: 20, warn: 30, error: 40 };
const TOKEN_RE = /[A-Za-z0-9_-]{20,}\.[A-Za-z0-9_-]{5,}\.[A-Za-z0-9_-]{20,}/g;

function fmt(a) {
  if (a instanceof Error) return a.stack || a.message;
  if (typeof a === 'object' && a !== null) {
    try { return JSON.stringify(a); } catch { return '[objeto]'; }
  }
  return String(a);
}

function createLogger(name, level = process.env.LOG_LEVEL || 'info') {
  const min = LEVELS[level] ?? LEVELS.info;
  const out = (lvl, args) => {
    if (LEVELS[lvl] < min) return;
    const line = `${new Date().toISOString()} [${name}] ${lvl.toUpperCase()} ${args.map(fmt).join(' ')}`.replace(TOKEN_RE, '[TOKEN]');
    (lvl === 'error' ? console.error : console.log)(line);
  };
  return {
    debug: (...a) => out('debug', a),
    info: (...a) => out('info', a),
    warn: (...a) => out('warn', a),
    error: (...a) => out('error', a),
  };
}
module.exports = { createLogger };

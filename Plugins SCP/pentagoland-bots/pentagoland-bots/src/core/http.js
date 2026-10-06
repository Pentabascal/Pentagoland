'use strict';
const crypto = require('node:crypto');
const express = require('express');

function createApp({ trustProxy = false } = {}) {
  const app = express();
  app.disable('x-powered-by');
  if (trustProxy) app.set('trust proxy', 1);
  app.use(express.json({ limit: '200kb', verify: (req, _res, buf) => { req.rawBody = buf; } }));
  return app;
}

/** Límite simple por IP en memoria. */
function rateLimiter({ windowMs = 60_000, max = 60 } = {}) {
  const hits = new Map();
  setInterval(() => {
    const now = Date.now();
    for (const [k, v] of hits) if (v.reset < now) hits.delete(k);
  }, windowMs).unref();
  return (req, res, next) => {
    const now = Date.now();
    let e = hits.get(req.ip);
    if (!e || e.reset < now) { e = { count: 0, reset: now + windowMs }; hits.set(req.ip, e); }
    if (++e.count > max) {
      res.set('Retry-After', String(Math.ceil((e.reset - now) / 1000)));
      return res.status(429).json({ error: 'rate_limited' });
    }
    return next();
  };
}

/**
 * Autenticación HMAC-SHA256: firma = HMAC(secreto, `${timestamp}.${cuerpo}`).
 * Cabeceras: X-Pentagoland-Timestamp (segundos unix) y X-Pentagoland-Signature (hex).
 * Ventana de 60 s y caché anti-repetición de firmas.
 */
function signatureAuth(secret, { toleranceSec = 60 } = {}) {
  const key = Buffer.from(secret, 'utf8');
  const seen = new Map();
  setInterval(() => {
    const now = Date.now();
    for (const [k, exp] of seen) if (exp < now) seen.delete(k);
  }, 30_000).unref();

  return (req, res, next) => {
    const t = req.get('x-pentagoland-timestamp');
    const sig = req.get('x-pentagoland-signature');
    if (!t || !sig || !/^\d{9,12}$/.test(t) || !/^[a-f0-9]{64}$/i.test(sig)) {
      return res.status(401).json({ error: 'unauthorized' });
    }
    if (Math.abs(Date.now() / 1000 - Number(t)) > toleranceSec) {
      return res.status(401).json({ error: 'stale_timestamp' });
    }
    const body = req.rawBody || Buffer.alloc(0);
    const expected = crypto.createHmac('sha256', key).update(`${t}.`).update(body).digest();
    const given = Buffer.from(sig, 'hex');
    if (given.length !== expected.length || !crypto.timingSafeEqual(given, expected)) {
      return res.status(401).json({ error: 'unauthorized' });
    }
    const id = sig.toLowerCase();
    if (seen.has(id)) return res.status(401).json({ error: 'replayed' });
    seen.set(id, Date.now() + (toleranceSec + 5) * 1000);
    return next();
  };
}

/** Cabeceras para JSON públicos de solo lectura. Sin origen configurado no se permite CORS. */
function publicHeaders(origin) {
  return (_req, res, next) => {
    if (origin) res.set('Access-Control-Allow-Origin', origin);
    res.set('Vary', 'Origin');
    res.set('Cache-Control', 'public, max-age=30');
    res.set('X-Content-Type-Options', 'nosniff');
    next();
  };
}

function errorHandler(log) {
  // eslint-disable-next-line no-unused-vars
  return (err, _req, res, _next) => {
    if (err.type === 'entity.parse.failed') return res.status(400).json({ error: 'invalid_json' });
    if (err.type === 'entity.too.large') return res.status(413).json({ error: 'too_large' });
    log.error('Error HTTP:', err);
    return res.status(500).json({ error: 'internal_error' });
  };
}

function listen(app, { port, host, log }) {
  return new Promise((resolve, reject) => {
    const server = app.listen(port, host, () => {
      log.info(`API escuchando en ${host}:${port}`);
      resolve(server);
    });
    server.on('error', reject);
  });
}

module.exports = { createApp, rateLimiter, signatureAuth, publicHeaders, errorHandler, listen };

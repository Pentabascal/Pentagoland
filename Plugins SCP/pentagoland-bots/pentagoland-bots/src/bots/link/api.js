'use strict';
const crypto = require('node:crypto');
const env = require('../../core/env');
const { createApp, rateLimiter, signatureAuth, publicHeaders, errorHandler, listen } = require('../../core/http');
const { embed } = require('../../core/embeds');
const { sendToChannel, truncate } = require('../../core/util');

const isInt = (v, min, max) => Number.isInteger(v) && v >= min && v <= max;
const STEAM_RE = /^[A-Za-z0-9]{3,40}@[a-z]{3,12}$/;
const FACTIONS = ['chaos', 'mtf', 'scp', 'guard', 'classd', 'scientist'];
const CODE_ALPHABET = 'ABCDEFGHJKMNPQRSTUVWXYZ23456789'; // sin 0/O/1/I/L
const CODE_TTL_MS = 10 * 60 * 1000;
const NOTIFY_CHANNELS = { anuncios: 'channel_anuncios', novedades: 'channel_novedades', operaciones: 'channel_operaciones' };

const bad = (res, msg) => res.status(400).json({ error: 'invalid_request', message: msg });

function validateStatus(b) {
  if (!b || typeof b !== 'object' || typeof b.online !== 'boolean') return { error: 'online (boolean) es obligatorio' };
  const out = { online: b.online };
  for (const k of ['players', 'maxPlayers']) {
    if (b[k] !== undefined) { if (!isInt(b[k], 0, 2000)) return { error: `${k} inválido` }; out[k] = b[k]; }
  }
  for (const [k, max] of [['version', 32], ['name', 64]]) {
    if (b[k] !== undefined) { if (typeof b[k] !== 'string' || b[k].length > max) return { error: `${k} inválido` }; out[k] = b[k]; }
  }
  if (b.round !== undefined) {
    const r = b.round;
    if (!r || typeof r.state !== 'string' || r.state.length > 32) return { error: 'round.state inválido' };
    out.round = { state: r.state };
    if (r.elapsedSeconds !== undefined) { if (!isInt(r.elapsedSeconds, 0, 1e7)) return { error: 'round.elapsedSeconds inválido' }; out.round.elapsedSeconds = r.elapsedSeconds; }
  }
  return { value: out };
}

function validatePlayer(p, idx) {
  if (!p || typeof p.steamId !== 'string' || !STEAM_RE.test(p.steamId)) return { error: `players[${idx}].steamId inválido` };
  const out = { steamId: p.steamId, breakdown: {} };
  for (const [k, field] of [['kills', 'kills'], ['deaths', 'deaths'], ['secondsPlayed', 'secondsPlayed'], ['scpKills', 'scpKills']]) {
    const v = p[field] ?? 0;
    if (!isInt(v, 0, 1e9)) return { error: `players[${idx}].${k} inválido` };
    out[k] = v;
  }
  if (p.breakdown !== undefined) {
    if (typeof p.breakdown !== 'object' || p.breakdown === null) return { error: `players[${idx}].breakdown inválido` };
    for (const f of FACTIONS) {
      if (p.breakdown[f] !== undefined) {
        if (!isInt(p.breakdown[f], 0, 1e9)) return { error: `players[${idx}].breakdown.${f} inválido` };
        out.breakdown[f] = p.breakdown[f];
      }
    }
  }
  return { value: out };
}

function genCode() {
  let s = '';
  for (let n = 0; n < 8; n++) s += CODE_ALPHABET[crypto.randomInt(CODE_ALPHABET.length)];
  return s;
}

function createApi({ status }) {
  let server; let purge;
  return {
    async start(ctx) {
      const { db } = ctx;
      const app = createApp({ trustProxy: env.bool('TRUST_PROXY') });
      const limit = rateLimiter({ max: 120 });
      const serverAuth = signatureAuth(env.secret('API_SECRET_SERVER'));
      const webAuth = signatureAuth(env.secret('API_SECRET_WEB'));
      const pub = [limit, publicHeaders(env.str('WEB_ORIGIN', ''))];

      app.get('/health', (_req, res) => res.json({ ok: true }));

      // ── Públicos (solo lectura, sin datos sensibles) ──
      app.get('/public/status.json', pub, (_req, res) => res.json(status.publicStatus()));

      app.get('/public/leaderboard.json', pub, (_req, res) => {
        const items = db.prepare(`SELECT l.display_name AS name, s.kills, s.deaths, s.seconds_played, s.scp_kills
          FROM links l JOIN player_stats s ON s.steam_id = l.steam_id
          WHERE l.public_stats = 1 AND l.display_name IS NOT NULL
          ORDER BY s.kills DESC LIMIT 20`).all()
          .map((r) => ({ name: r.name, kills: r.kills, deaths: r.deaths, kdr: Math.round((r.kills / Math.max(r.deaths, 1)) * 100) / 100,
            hours: Math.round((r.seconds_played / 3600) * 10) / 10, scpKills: r.scp_kills }));
        res.json({ generatedAt: new Date().toISOString(), items });
      });

      // ── Servidor de juego → bot (firmados con API_SECRET_SERVER) ──
      app.post('/v1/server/status', limit, serverAuth, (req, res) => {
        const { value, error } = validateStatus(req.body);
        if (error) return bad(res, error);
        db.prepare(`INSERT INTO server_status (id, payload, received_at) VALUES (1, ?, ?)
          ON CONFLICT(id) DO UPDATE SET payload = excluded.payload, received_at = excluded.received_at`)
          .run(JSON.stringify(value), Date.now());
        status.schedule();
        return res.json({ ok: true });
      });

      app.post('/v1/server/stats', limit, serverAuth, (req, res) => {
        const list = req.body?.players;
        if (!Array.isArray(list) || list.length === 0 || list.length > 200) return bad(res, 'players debe ser un array de 1 a 200 elementos');
        const parsed = [];
        for (let n = 0; n < list.length; n++) {
          const r = validatePlayer(list[n], n);
          if (r.error) return bad(res, r.error);
          parsed.push(r.value);
        }
        const up = db.prepare(`INSERT INTO player_stats (steam_id, kills, deaths, seconds_played, scp_kills, breakdown, updated_at)
          VALUES (?,?,?,?,?,?,?)
          ON CONFLICT(steam_id) DO UPDATE SET kills=excluded.kills, deaths=excluded.deaths,
            seconds_played=excluded.seconds_played, scp_kills=excluded.scp_kills,
            breakdown=excluded.breakdown, updated_at=excluded.updated_at`);
        const now = Date.now();
        db.transaction(() => parsed.forEach((p) => up.run(p.steamId, p.kills, p.deaths, p.secondsPlayed, p.scpKills, JSON.stringify(p.breakdown), now)))();
        return res.json({ ok: true, updated: parsed.length });
      });

      // El juego identifica al jugador (no el usuario escribiendo un ID): así la vinculación es verificada.
      app.post('/v1/link/code', limit, serverAuth, (req, res) => {
        const steamId = req.body?.steamId;
        if (typeof steamId !== 'string' || !STEAM_RE.test(steamId)) return bad(res, 'steamId inválido');
        if (db.prepare('SELECT 1 FROM links WHERE steam_id = ?').get(steamId)) return res.status(409).json({ error: 'already_linked' });
        const recent = db.prepare('SELECT created_at FROM link_codes WHERE steam_id = ? ORDER BY created_at DESC LIMIT 1').get(steamId);
        if (recent && Date.now() - recent.created_at < 30_000) return res.status(429).json({ error: 'too_soon' });
        const now = Date.now();
        let code;
        db.transaction(() => {
          db.prepare('DELETE FROM link_codes WHERE steam_id = ?').run(steamId);
          for (;;) { // reintenta si colisiona (improbable)
            code = genCode();
            if (!db.prepare('SELECT 1 FROM link_codes WHERE code = ?').get(code)) break;
          }
          db.prepare('INSERT INTO link_codes (code, steam_id, created_at, expires_at) VALUES (?,?,?,?)').run(code, steamId, now, now + CODE_TTL_MS);
        })();
        return res.json({ code, expiresInSeconds: CODE_TTL_MS / 1000 });
      });

      // ── Web (backend) → Discord (firmado con API_SECRET_WEB). Nunca usar webhooks en el HTML público. ──
      app.post('/v1/web/notify', limit, webAuth, async (req, res) => {
        const { channel, title, description } = req.body || {};
        const key = NOTIFY_CHANNELS[channel];
        if (!key) return bad(res, `channel debe ser uno de: ${Object.keys(NOTIFY_CHANNELS).join(', ')}`);
        if (typeof title !== 'string' || !title.trim() || title.length > 200) return bad(res, 'title inválido');
        if (typeof description !== 'string' || !description.trim() || description.length > 3000) return bad(res, 'description inválida');
        const msg = await sendToChannel(ctx, key, { embeds: [embed('brand').setTitle(truncate(title, 200)).setDescription(description).setFooter({ text: 'Pentagoland' })] });
        if (!msg) return res.status(409).json({ error: 'channel_not_configured_or_unreachable' });
        return res.json({ ok: true });
      });

      app.use(errorHandler(ctx.log));
      purge = setInterval(() => db.prepare('DELETE FROM link_codes WHERE expires_at < ?').run(Date.now()), 5 * 60 * 1000);
      server = await listen(app, { port: env.int('LINK_API_PORT', 3102), host: env.str('API_HOST', '127.0.0.1'), log: ctx.log });
    },
    stop() { clearInterval(purge); return new Promise((r) => (server ? server.close(() => r()) : r())); },
  };
}

module.exports = { createApi };

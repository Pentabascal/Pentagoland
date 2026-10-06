'use strict';
const env = require('../../core/env');
const { simple } = require('../../core/embeds');
const { sendToChannel, ts } = require('../../core/util');

const DOWN_AFTER_FAILS = 2; // evita falsas alarmas por un fallo puntual

function createMonitor() {
  let timer;
  let first;
  let busy = false;
  let ctx;

  async function check() {
    if (busy) return;
    busy = true;
    try {
      const url = ctx.settings.get(ctx.guildId, 'web_url');
      if (!url) return;
      const started = Date.now();
      let up = false;
      let status = null;
      const ac = new AbortController();
      const t = setTimeout(() => ac.abort(), 8000);
      try {
        const res = await fetch(url, { redirect: 'follow', signal: ac.signal, headers: { 'user-agent': 'PentagolandBot-Monitor/1.0' } });
        status = res.status;
        up = res.status < 500;
      } catch { up = false; } finally { clearTimeout(t); }
      const latency = Date.now() - started;

      const prev = ctx.db.prepare('SELECT * FROM monitor WHERE id = 1').get();
      const fails = up ? 0 : prev.fails + 1;
      const now = Date.now();
      const changed = prev.up === null || prev.up !== (up ? 1 : 0);
      ctx.db.prepare(`UPDATE monitor SET up=?, http_status=?, latency_ms=?, last_check=?, fails=?,
        last_change = CASE WHEN ? THEN ? ELSE last_change END WHERE id = 1`)
        .run(up ? 1 : 0, status, latency, now, fails, changed ? 1 : 0, now);

      if (!up && fails >= DOWN_AFTER_FAILS && !prev.alerted_down) {
        ctx.db.prepare('UPDATE monitor SET alerted_down = 1 WHERE id = 1').run();
        await sendToChannel(ctx, 'channel_operaciones', { embeds: [simple('error', 'La web no responde', `${url}\nFallos consecutivos: ${fails} · ${ts(now)}`)] });
      } else if (up && prev.alerted_down) {
        ctx.db.prepare('UPDATE monitor SET alerted_down = 0 WHERE id = 1').run();
        await sendToChannel(ctx, 'channel_operaciones', { embeds: [simple('ok', 'La web vuelve a responder', `${url}\nLatencia: ${latency} ms · ${ts(now)}`)] });
      }
    } catch (e) {
      ctx.log.warn('Fallo en la comprobación de la web:', e.message);
    } finally {
      busy = false;
    }
  }

  return {
    start(c) {
      ctx = c;
      timer = setInterval(check, env.int('MONITOR_INTERVAL_SEC', 120) * 1000);
      first = setTimeout(check, 10_000);
    },
    stop() { clearInterval(timer); clearTimeout(first); },
  };
}

module.exports = { createMonitor };

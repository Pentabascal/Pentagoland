'use strict';
const env = require('../../core/env');
const { createApp, rateLimiter, publicHeaders, errorHandler, listen } = require('../../core/http');

/** JSON públicos de solo lectura para la web estática. Solo contenido publicado/aprobado. */
function createApi() {
  let server;
  return {
    async start(ctx) {
      const app = createApp({ trustProxy: env.bool('TRUST_PROXY') });
      const pub = [rateLimiter({ max: 120 }), publicHeaders(env.str('WEB_ORIGIN', ''))];
      const { db } = ctx;

      app.get('/health', (_req, res) => res.json({ ok: true }));

      app.get('/public/news.json', pub, (_req, res) => {
        const items = db.prepare(`SELECT id, title, body, published_at FROM news
          WHERE status = 'published' ORDER BY published_at DESC LIMIT 50`).all()
          .map((r) => ({ id: r.id, title: r.title, body: r.body, publishedAt: new Date(r.published_at).toISOString() }));
        res.json({ generatedAt: new Date().toISOString(), items });
      });

      app.get('/public/gallery.json', pub, (_req, res) => {
        const items = db.prepare(`SELECT id, title, credit, image_url, caption FROM gallery
          WHERE status = 'approved' ORDER BY id DESC LIMIT 100`).all()
          .map((r) => ({ id: r.id, title: r.title, credit: r.credit, imageUrl: r.image_url, caption: r.caption }));
        res.json({ generatedAt: new Date().toISOString(), items });
      });

      app.get('/public/links.json', pub, (_req, res) => {
        const links = Object.fromEntries(db.prepare('SELECT key, url FROM links').all().map((r) => [r.key, r.url]));
        res.json({ generatedAt: new Date().toISOString(), links });
      });

      app.use(errorHandler(ctx.log));
      server = await listen(app, { port: env.int('WEB_API_PORT', 3101), host: env.str('API_HOST', '127.0.0.1'), log: ctx.log });
    },
    stop() { return new Promise((r) => (server ? server.close(() => r()) : r())); },
  };
}

module.exports = { createApi };

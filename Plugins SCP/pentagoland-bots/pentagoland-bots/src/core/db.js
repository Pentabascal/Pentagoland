'use strict';
const fs = require('node:fs');
const path = require('node:path');
const Database = require('better-sqlite3');

/** Abre SQLite (WAL) y aplica migraciones pendientes (array de SQL, en orden). */
function openDb(file, migrations = []) {
  fs.mkdirSync(path.dirname(file), { recursive: true });
  const db = new Database(file);
  db.pragma('journal_mode = WAL');
  db.pragma('foreign_keys = ON');
  db.pragma('busy_timeout = 5000');
  db.exec(`CREATE TABLE IF NOT EXISTS settings (
    guild_id TEXT NOT NULL, key TEXT NOT NULL, value TEXT NOT NULL,
    PRIMARY KEY (guild_id, key))`);
  const current = db.pragma('user_version', { simple: true });
  for (let i = current; i < migrations.length; i++) {
    db.transaction(() => {
      db.exec(migrations[i]);
      db.pragma(`user_version = ${i + 1}`);
    })();
  }
  return db;
}

class Settings {
  constructor(db) {
    this._get = db.prepare('SELECT value FROM settings WHERE guild_id = ? AND key = ?');
    this._set = db.prepare(`INSERT INTO settings (guild_id, key, value) VALUES (?, ?, ?)
      ON CONFLICT(guild_id, key) DO UPDATE SET value = excluded.value`);
    this._del = db.prepare('DELETE FROM settings WHERE guild_id = ? AND key = ?');
    this._all = db.prepare('SELECT key, value FROM settings WHERE guild_id = ?');
  }
  get(g, k, def = null) { const r = this._get.get(g, k); return r ? r.value : def; }
  getList(g, k) { const v = this.get(g, k); return v ? v.split(',').filter(Boolean) : []; }
  set(g, k, v) { this._set.run(g, k, String(v)); }
  /** Añade un ID a una lista separada por comas (sin duplicar). */
  add(g, k, id) { const l = this.getList(g, k); if (!l.includes(id)) l.push(id); this.set(g, k, l.join(',')); }
  delete(g, k) { this._del.run(g, k); }
  all(g) { return Object.fromEntries(this._all.all(g).map((r) => [r.key, r.value])); }
}

module.exports = { openDb, Settings };

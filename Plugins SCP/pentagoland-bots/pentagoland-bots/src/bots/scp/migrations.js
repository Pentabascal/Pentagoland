'use strict';
module.exports = [`
CREATE TABLE cases (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  guild_id TEXT NOT NULL,
  user_id TEXT NOT NULL,
  moderator_id TEXT NOT NULL,
  type TEXT NOT NULL CHECK (type IN ('warn','timeout','untimeout','kick','ban','unban','note')),
  reason TEXT NOT NULL,
  duration_sec INTEGER,
  created_at INTEGER NOT NULL
);
CREATE INDEX idx_cases_user ON cases (guild_id, user_id);
CREATE TABLE tickets (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  guild_id TEXT NOT NULL,
  channel_id TEXT UNIQUE,
  opener_id TEXT NOT NULL,
  type TEXT NOT NULL,
  status TEXT NOT NULL DEFAULT 'open' CHECK (status IN ('open','closed')),
  created_at INTEGER NOT NULL,
  closed_at INTEGER,
  closed_by TEXT
);
CREATE INDEX idx_tickets_open ON tickets (guild_id, opener_id, status);
`];

'use strict';
module.exports = [`
CREATE TABLE news (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  title TEXT NOT NULL,
  body TEXT NOT NULL,
  status TEXT NOT NULL DEFAULT 'draft' CHECK (status IN ('draft','published','archived')),
  author_id TEXT NOT NULL,
  created_at INTEGER NOT NULL,
  published_at INTEGER,
  message_id TEXT
);
CREATE TABLE gallery (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  title TEXT NOT NULL,
  credit TEXT NOT NULL,
  image_url TEXT NOT NULL,
  caption TEXT,
  status TEXT NOT NULL DEFAULT 'pending' CHECK (status IN ('pending','approved','rejected')),
  submitted_by TEXT NOT NULL,
  created_at INTEGER NOT NULL
);
CREATE TABLE links (
  key TEXT PRIMARY KEY,
  url TEXT NOT NULL,
  updated_at INTEGER NOT NULL
);
CREATE TABLE monitor (
  id INTEGER PRIMARY KEY CHECK (id = 1),
  up INTEGER,
  http_status INTEGER,
  latency_ms INTEGER,
  last_check INTEGER,
  last_change INTEGER,
  fails INTEGER NOT NULL DEFAULT 0,
  alerted_down INTEGER NOT NULL DEFAULT 0
);
INSERT INTO monitor (id) VALUES (1);
`];

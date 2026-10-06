'use strict';
module.exports = [`
CREATE TABLE server_status (
  id INTEGER PRIMARY KEY CHECK (id = 1),
  payload TEXT NOT NULL,
  received_at INTEGER NOT NULL
);
CREATE TABLE link_codes (
  code TEXT PRIMARY KEY,
  steam_id TEXT NOT NULL,
  created_at INTEGER NOT NULL,
  expires_at INTEGER NOT NULL
);
CREATE INDEX idx_link_codes_steam ON link_codes (steam_id);
CREATE TABLE links (
  discord_id TEXT PRIMARY KEY,
  steam_id TEXT NOT NULL UNIQUE,
  linked_at INTEGER NOT NULL,
  public_stats INTEGER NOT NULL DEFAULT 0,
  display_name TEXT
);
CREATE TABLE player_stats (
  steam_id TEXT PRIMARY KEY,
  kills INTEGER NOT NULL DEFAULT 0,
  deaths INTEGER NOT NULL DEFAULT 0,
  seconds_played INTEGER NOT NULL DEFAULT 0,
  scp_kills INTEGER NOT NULL DEFAULT 0,
  breakdown TEXT NOT NULL DEFAULT '{}',
  updated_at INTEGER NOT NULL
);
CREATE TABLE panel (
  guild_id TEXT PRIMARY KEY,
  channel_id TEXT NOT NULL,
  message_id TEXT NOT NULL
);
`];

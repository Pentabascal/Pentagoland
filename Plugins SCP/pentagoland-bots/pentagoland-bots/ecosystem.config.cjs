// pm2 start ecosystem.config.cjs
module.exports = {
  apps: ['web', 'link', 'scp'].map((n) => ({
    name: `pentagoland-${n}`,
    script: `src/bots/${n}/index.js`,
    autorestart: true,
    restart_delay: 5000,
    max_restarts: 30,
    env: { NODE_ENV: 'production' },
  })),
};

# Security policy

Server Doctor is an administrative tool that can expose configuration and, when explicitly enabled, write selected YAML files. Treat its permission and writable-root configuration as security-sensitive.

## Reporting a vulnerability

Do not publish server paths, credentials, tokens, IP addresses, player identifiers, or private logs in a public issue. Contact the repository maintainer privately and include a minimal reproduction, affected version, and impact. Rotate any credential that may have appeared in a diagnostic preview.

## Deployment guidance

- Grant `pentagoland.serverdoctor` only to trusted administrators.
- Keep `AllowConfigChanges` disabled unless there is a specific need.
- If enabled, authorize only the narrow configuration root required.
- Keep external backups and validate the plugin on a staging server first.

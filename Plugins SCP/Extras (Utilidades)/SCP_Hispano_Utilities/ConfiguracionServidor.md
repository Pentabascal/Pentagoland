# Configuración de Pentagoland Core

EXILED genera estos valores en `%AppData%/EXILED/Configs/Plugins/pentagoland_core/7777.yml`.

| Clave | Función |
| --- | --- |
| `is_enabled` | Activa el proveedor auxiliar de estado. |
| `debug` | Activa el registro de depuración de EXILED. |
| `collect_round_statistics` | Cuenta rondas iniciadas y finalizadas; no recopila datos de jugadores. |
| `persist_statistics` | Conserva los contadores agregados de rondas en `estadisticas.json`. |
| `enable_json_integration_snapshot` | Genera `estado.json`, sin nombres, identificadores ni estadísticas individuales. |
| `server_name` / `server_tagline` | Identidad pública incluida en el snapshot. |
| `data_directory` | Directorio de salida; las rutas relativas se resuelven desde el servidor. |
| `statistics_file_name` | Nombre del archivo de contadores de rondas. |
| `integration_snapshot_file_name` | Nombre del snapshot público. |
| `snapshot_interval_seconds` | Heartbeat del snapshot; se limita automáticamente a 10–300 segundos. |

La capacidad y el número de jugadores se consultan desde la API activa de EXILED, no se duplican en esta configuración.

## Integraciones y privacidad

La web necesita un proxy/backend de solo lectura que sirva `estado.json` por HTTPS y con `UpdatedAtUtc` reciente. Pentagoland Core no abre puertos ni inicia un listener HTTP. No publiques `estadisticas.json`, configuraciones internas ni copias de datos antiguos.

## Funciones que pertenecen a otros plugins

- `StatsSystem`: playtime, kills, deaths y seguimiento por rol.
- `MvpSystem`: puntos y puntuaciones de MVP.
- `AdminTools`: comandos y moderación administrativa.
- `UltimateHUD`, `ScpsInfoDisplay` y RueI: overlays HUD.

El núcleo no tiene comandos de chat, advertencias, reportes ni filtros de mensajes. No actives otra implementación de esas funciones sin comprobar primero qué plugin las gestiona en la instalación activa.

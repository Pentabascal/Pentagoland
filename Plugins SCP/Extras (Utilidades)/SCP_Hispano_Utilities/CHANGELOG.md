# Changelog

## [4.3.0] - 2026-09-29

### Changed
- Pentagoland Core queda limitado a estadísticas agregadas del servidor y exportación de estado para la web.
- La capacidad se consulta a EXILED y los snapshots incluyen disponibilidad, recuento, capacidad y timestamp UTC ISO 8601.
- `estado.json` se actualiza al iniciar, cambiar jugadores o rondas y mediante heartbeat.

### Removed
- Retirada la recopilación y persistencia paralela de kills, deaths, puntos y advertencias, ya cubiertas por StatsSystem y MvpSystem.
- Retirados el registro de reportes y los helpers de comandos no conectados a comandos reales; AdminTools mantiene la moderación.
- Retirado el tipo de compatibilidad que podía descubrirse como una segunda clase de plugin.

### Security
- Los snapshots ya no exportan UserId, SteamID ni estadísticas individuales.
- Los archivos de estadísticas antiguos conservan únicamente sus contadores agregados de rondas tras la migración.

## [4.2.0] - 2026-09-29

### Changed
- Compilación de servidor contra EXILED 9.14.2; los stubs ya no se enlazan de forma predeterminada.
- Seguimiento de bajas desactivado por defecto para evitar duplicarlo con LabAPI StatsSystem.
- Desplazamiento horizontal de ScpsInfoDisplay reducido y limitado para evitar HUD fuera de pantalla.
- Separadas las posiciones superiores del reloj, TPS y duración de ronda.

### Fixed
- La lista de identificadores administrativos ya no acepta el apodo como sustituto del UserId.
- La bandera `EnableAdminOnlyRestrictions` controla las comprobaciones de permiso.
- El nombre del servidor configurado se conserva en el snapshot.
- Un archivo de estadísticas inválido ya no se sobrescribe al iniciar o apagar el plugin.
- Los límites de longitud de reportes se normalizan antes de registrar el enfriamiento.
- Los timestamps sin valor se normalizan a UTC para que la serialización funcione en Windows/Mono.

## [4.1.0] - 2026-09-29

### Added
- Compatibilidad mejorada con aliases y nombres legacy.
- Protección de permisos por administrador para comandos sensibles.
- Configuración ampliada para personalización del servidor.
- Estado del servidor y snapshot de integración.
- Preparación para uso en servidor real con arquitectura modular.

### Changed
- Se reforzó la estructura del plugin para reducir conflictos y facilitar mantenimiento.
- Se ajustó la versión base del núcleo para reflejar el estado de consolidación.

### Security
- Se limitaron comandos administrativos a roles o usuarios autorizados.
- Se reforzó la validación de reportes y tamaños de entrada.

# Plugins y configuración de LabAPI

Esta carpeta conserva una copia de los componentes de LabAPI instalados en el servidor local:

- `LabApi-7777.yml` — rutas de carga y política de compatibilidad del cargador.
- `plugins/global/` y `plugins/7777/` — plugins instalados para todas las instancias y para el puerto 7777.
- `dependencies/global/` y `dependencies/7777/` — dependencias instaladas para esos plugins.
- `configs/` — configuración, traducciones y archivos auxiliares de los plugins.
- `plugins/disabled/` y `backups/` — ubicaciones de plugins desactivados y copias locales, si contienen archivos.
- `externally-loaded/EXILED/` — copia del binario `Axwabo.CommandSystem`, que LabAPI registra mediante el cargador de compatibilidad de EXILED.
- `configs/EXILED/Plugins/` — configuraciones auxiliares del cargador de compatibilidad para `Axwabo.CommandSystem` y `SER`.

La copia procede de `%AppData%\SCP Secret Laboratory\LabAPI`. No incluye la instalación del juego ni del servidor dedicado.

## Plugins instalados

- Globales: AntiSuicidalZombies, CedMod, Exiled Loader, LateSpawn, RueI, SecretAPI y Timers.
- Puerto 7777: MvpSystem, ProjectMER, RespawnTimer, SCP Proximity Chat, SecretLabNAudio, SER y StatsSystem.
- El cargador de compatibilidad también registra `Axwabo.CommandSystem` desde la carpeta de plugins de EXILED.
- `0Harmony` y `LabApiExtensions` se conservan como componentes de soporte; los archivos DLL restantes se mantienen en `dependencies/`.

## Datos locales

- El valor de `ced_mod_api_key` en `configs/7777/CedMod/config.yml` se deja vacío en esta copia. Introduce la clave solo en la configuración local del servidor; no la publiques en Git.
- Los datos generados de jugadores de `configs/7777/StatsSystem/player_stats.json` se conservan en la copia local y se excluyen del control de versiones.
- Los archivos de registro, las bases de datos locales, los archivos de respaldo y las salidas de compilación están excluidos por el `.gitignore` de la raíz.

Esta carpeta representa los archivos instalados actualmente, no una distribución limpia para instalar los plugins en otro servidor. Comprueba sus dependencias y licencias antes de redistribuir binarios de terceros.

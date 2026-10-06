# Pentagoland Core 4.3.0

Plugin auxiliar de EXILED para publicar un estado agregado del servidor que puede consumir la web de Pentagoland. No registra comandos de chat, no muestra otro HUD y no guarda estadísticas individuales.

## Funciones

- Cuenta rondas iniciadas y finalizadas, y conserva solo esos contadores agregados.
- Genera `estado.json` con el nombre del servidor, disponibilidad, jugadores conectados, capacidad y fecha UTC.
- Actualiza el estado al iniciar, al cambiar la lista de jugadores, al cambiar de ronda y mediante un heartbeat configurable.
- Escribe los archivos de forma atómica para evitar JSON incompleto si el proceso se interrumpe.

`estado.json` no contiene nombres, UserId, SteamID, puntos, bajas, advertencias ni estadísticas individuales. La web recibe solo el contrato público; no se incluye un servidor HTTP ni se expone una ruta de red automáticamente. Un proxy/backend de solo lectura debe servir ese archivo por HTTPS antes de configurar `data-server-status-endpoint` en la web.

## Funciones delegadas a plugins existentes

- LabAPI `StatsSystem` conserva el tiempo jugado, kills, muertes y estadísticas por rol.
- LabAPI `MvpSystem` gestiona puntos y puntuaciones de MVP.
- EXILED `AdminTools` se ocupa de comandos y moderación administrativa.
- `UltimateHUD`, `ScpsInfoDisplay` y los plugins basados en RueI son responsables de los HUD.

Por eso Pentagoland Core ya no mantiene sistemas paralelos de combate, puntos, advertencias, reportes ni comandos. Los tipos auxiliares que no registraban comandos reales se retiraron.

## Compilación e instalación

1. Compila `SCP_Hispano_Utilities.csproj` en Release para .NET Framework 4.8.
2. La compilación usa `ExMod.Exiled` 9.14.2; los stubs de desarrollo no se enlazan por defecto.
3. Copia `bin/Release/net48/PentagolandCore.dll` a `%AppData%/EXILED/Plugins`.
4. Reinicia el servidor y revisa la configuración generada en `%AppData%/EXILED/Configs/Plugins/pentagoland_core`.

## Datos y migración

- `estadisticas.json` conserva únicamente contadores agregados de rondas.
- `estado.json` es el snapshot público de integración.
- Ambos archivos se guardan bajo `PentagolandData` en el directorio del servidor, salvo que se configure una ruta distinta.
- Al cargar un archivo antiguo de estadísticas, se recuperan los contadores de rondas, pero se ignoran los registros individuales; el siguiente guardado elimina esos datos duplicados de la salida.
- Si un archivo no se puede leer, el plugin no lo sobrescribe y lo informa en el log.

## Contrato de `estado.json`

```json
{
  "SchemaVersion": 1,
  "ServerName": "Pentagoland",
  "ServerTagline": "Servidor hispano de SCP: Secret Laboratory",
  "Online": true,
  "OnlinePlayers": 0,
  "MaxPlayers": 20,
  "UpdatedAtUtc": "2026-09-29T13:00:00.0000000Z"
}
```

Los valores del bloque son ilustrativos. `MaxPlayers` se obtiene del servidor activo; `UpdatedAtUtc` es UTC en formato ISO 8601. La web no debe tratar un snapshot antiguo como estado actual ni publicar archivos de estadísticas internas.

## Configuración

Consulta [`ConfiguracionServidor.md`](./ConfiguracionServidor.md) para ver los ajustes disponibles. No hay comandos de jugador ni módulos de moderación activables desde esta configuración.

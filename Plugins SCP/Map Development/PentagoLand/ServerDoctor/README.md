# Pentagoland Server Doctor

Herramientas administrativas de diagnóstico y mantenimiento para servidores de SCP: Secret Laboratory con LabAPI.

El plugin no modifica rondas, jugadores, balance, archivos del juego ni otros plugins al iniciarse. En modo de solo lectura se declara transparente ante LabAPI; si se habilita la escritura de configuraciones, retira esa marca. No registra eventos de juego, no aplica Harmony patches, no carga DLL de terceros y no intenta recargar plugins. Las operaciones se solicitan explícitamente desde Remote Admin o la consola del servidor.

## Funciones

- Diagnóstico básico del proceso, espacio libre, memoria administrada y directorios reconocidos.
- Estado en vivo desde LabAPI: puerto, jugadores, TPS, versión de LabAPI, tiempo activo y memoria del proceso. Si el runtime no expone las métricas de memoria, se muestran como no disponibles en vez de informar cero.
- Inventario en vivo de plugins reconocidos/habilitados y dependencias cargadas según el estado de `PluginLoader`, separado del inventario de DLL en disco.
- Generación bajo demanda de un informe local con diagnóstico, inventarios y las últimas líneas de hasta tres logs; aplica redacción antes de guardar.
- Comprobación de solo lectura para ajustes de conexión de `config_gameplay.txt`, basada en la guía de soporte de SCP:SL.
- Inventario superficial de DLL en carpetas conocidas de LabAPI y EXILED. Solo informa de archivos presentes; no afirma que estén cargados o funcionando.
- Inspección de las rutas activas de plugins y dependencias declaradas en la configuración de LabAPI, además de los directorios base oficiales.
- Inspección estática de DLL bajo demanda: hash SHA-256, tamaño, fecha y metadatos de versión declarados, sin cargar ni ejecutar el código.
- Acceso de solo lectura a los datos del juego y a la configuración del servidor ubicada en la ruta de datos de SCP:SL.
- Exploración acotada de archivos de texto, vista previa limitada y lectura del final de logs.
- Redacción de valores que parecen contraseñas, tokens, claves o direcciones IPv4 antes de mostrar texto en Remote Admin.
- Copias SHA-256 identificadas, auditoría local y restauración con copia de rollback.
- Cambio opcional de valores escalares existentes en YAML de LabAPI/EXILED y en archivos de configuración SCP:SL con sintaxis `clave: valor`. Antes de escribir crea una copia automática; preserva BOM, salto de línea y comentarios inline de la propiedad modificada.
- Rutas restringidas a raíces configuradas; rechaza traversal, rutas absolutas, enlaces simbólicos y puntos de reanálisis.

No existe una API común que permita editar con seguridad cualquier parámetro de cualquier plugin. La edición incluida está deliberadamente limitada a claves escalares existentes y no ambiguas: YAML en raíces autorizadas y `config_gameplay.txt` SCP:SL con sintaxis `clave: valor`, dentro de `scpsl-config`/`scpsl-user-config`. Otros archivos SCP:SL —incluidos Remote Admin y permisos— permanecen fuera del editor. No cambia JSON, listas, objetos, expresiones, código, valores en memoria ni configuraciones privadas de un plugin con formatos distintos de YAML. Se pueden añadir formatos mediante `IConfigEditorAdapter` en `src\PentagolandServerDoctor`; el adaptador recibe también la raíz y la ruta para poder limitar su alcance. Los nuevos adaptadores deben seguir usando raíces autorizadas, confirmación y backup. Después de un cambio hay que seguir el método de recarga documentado por ese plugin; Server Doctor nunca decide reiniciar o recargar otro componente.

## Requisitos y compatibilidad

- SCP: Secret Laboratory Dedicated Server con LabAPI habilitado.
- .NET Framework 4.8 para ejecutar el plugin.
- Para compilar: .NET SDK compatible con C# 12 y referencias del servidor disponibles en `SL_REFERENCES`.

El plugin usa LabAPI y no requiere EXILED. En una instalación mixta no interfiere intencionadamente con plugins EXILED, pero no se puede garantizar compatibilidad con todas las versiones, hosters o plugins de terceros sin probar esa combinación concreta. No inspecciona el estado interno de plugins de forma invasiva.

Las rutas estándar se obtienen de `PathManager` de LabAPI y su política `gamedir_for_configs`. También se revisan `PluginPaths` y `DependencyPaths` activos de `PluginLoader.Config`, resolviendo `$port` y manteniendo cada raíz dentro de su carpeta oficial. Las rutas absolutas o que salen de esas carpetas se omiten con una advertencia; si una instalación necesita inspeccionar una ruta externa, declárala explícitamente en `AdditionalRoots` y en `PluginScanRoots` o `DependencyScanRoots`. Las ubicaciones alternativas bajo el directorio del servidor son detección de hoster, no una ruta oficial universal. No se recorren discos enteros ni se siguen enlaces.

## Compilar

En PowerShell, apunta `SL_REFERENCES` a la carpeta `SCPSL_Data\Managed` del servidor:

```powershell
.\build.cmd "D:\SCP Server\SCPSL_Data\Managed"
```

El DLL se genera en `src\PentagolandServerDoctor\bin\Release\net48\PentagolandServerDoctor.dll`. El proyecto no descarga DLL del juego ni las incluye en Git.

Para ejecutar solo las pruebas sin referencias del juego:

```powershell
.\build.cmd test
```

En VS Code, abre la paleta de comandos y ejecuta **Tasks: Run Task**. Están disponibles `Server Doctor: safety tests` y `Server Doctor: build plugin`. La tarea de compilación necesita que `SL_REFERENCES` apunte a `SCPSL_Data\Managed` o que se configure la ruta al invocar `build.cmd`.

Las pruebas automatizadas compilan el código del plugin con stubs de API para probar la lógica aislada. No reemplazan una compilación contra los ensamblados reales ni una prueba en un servidor de staging.

## Instalar

1. Compila con las referencias de la misma instalación/versión de servidor que vas a usar.
2. Copia `PentagolandServerDoctor.dll` en una de las rutas activas de `PathManager.Plugins` definidas en `PluginPaths` (por defecto `global` y `$port`). En una instalación estándar suele ser `%AppData%\SCP Secret Laboratory\LabAPI\plugins\global\` en Windows y `~/.config/SCP Secret Laboratory/LabAPI/plugins/global/` en Linux. Si `hoster_policy.txt` contiene `gamedir_for_configs: true`, LabAPI usa `AppData\SCP Secret Laboratory\LabAPI\plugins\global\` bajo el directorio de trabajo del servidor.
3. Arranca el servidor y verifica la consola de LabAPI.
4. LabAPI genera la configuración del plugin al cargarlo. Ejecuta `pdoctor help` desde Remote Admin para ver los comandos.
5. Concede `pentagoland.serverdoctor` solo al grupo de administradores que deba usarlo. La consola del servidor puede utilizar comandos administrativos.

Para instalar o quitar el plugin hay que reiniciar el proceso del servidor.

## Comandos

Los comandos están disponibles desde Remote Admin y la consola del servidor. Remote Admin requiere el permiso configurado en `AdminPermission`; la consola local del proceso se trata como consola de confianza por LabAPI.

```text
pdoctor help
pdoctor status
pdoctor scan
pdoctor health
pdoctor loaded-plugins
pdoctor report
pdoctor check-gameplay scpsl-config:7777/config_gameplay.txt
pdoctor plugins
pdoctor inspect-plugin labapi-plugins:global/ExamplePlugin.dll
pdoctor compat-plugin labapi-plugins:global/ExamplePlugin.dll
pdoctor dependencies
pdoctor compatibility
pdoctor roots
pdoctor files <raíz> [directorio-relativo]
pdoctor read <raíz>:<archivo>
pdoctor tail logs:<archivo.log> [líneas]
pdoctor backup <raíz>:<archivo>
pdoctor backups
pdoctor set <raíz>:<archivo.config> <clave.ruta> <valor> --confirm
pdoctor restore <id-de-copia> confirm
```

Ejemplo:

```text
pdoctor files labapi-configs
pdoctor read labapi-configs:MiPlugin/global.yml
pdoctor backup labapi-configs:MiPlugin/global.yml
```

`pdoctor plugins` y `pdoctor dependencies` recorren de forma acotada las carpetas conocidas y sus subdirectorios (máximo 256 directorios, 10 000 archivos y 500 DLL por raíz), incluidas las rutas activas de LabAPI, las habituales `global` y por puerto, y las raíces adicionales declaradas; no muestran componentes efectivamente cargados ni detectan dependencias faltantes. Las rutas configuradas por LabAPI se limitan a 32 por tipo. Para una ubicación externa, declara una `AdditionalRoot` y añádela a `PluginScanRoots` o `DependencyScanRoots`. `pdoctor compatibility` explica qué se pudo detectar y qué no puede certificar. `pdoctor tail` solo acepta la raíz `logs` y limita la salida a 100 líneas. La lectura normal también tiene un tamaño máximo y trunca respuestas largas.

`pdoctor loaded-plugins` consulta `PluginLoader.Plugins`, `EnabledPlugins` y `Dependencies` de LabAPI al momento de ejecutar el comando. Distingue instancias conocidas de las habilitadas y lista las dependencias que LabAPI registró como cargadas. No llama a propiedades virtuales ni métodos de plugins de terceros, ni inspecciona DLL en disco; su lista complementa `pdoctor plugins`, que solo inventaría archivos.

`pdoctor inspect-plugin <raíz>:<archivo.dll>` calcula una huella reproducible y muestra los metadatos de archivo declarados por el proveedor. `pdoctor compat-plugin <raíz>:<archivo.dll>` lee metadatos PE/.NET sin cargar ni ejecutar el plugin, contrasta las referencias declaradas con DLL que encuentra en `SCPSL_Data\Managed`, raíces de plugin y dependencias, e informa de referencias no encontradas o versiones distintas. Es un indicio útil, no un veredicto: puede haber dependencias del sistema, carga dinámica, binding redirects o APIs incompatibles aunque todas las DLL estén. No toca ni repara los binarios.

El análisis usa `System.Reflection.Metadata`. Instala el plugin en una ruta incluida en `PluginPaths` y sus dependencias runtime en una ruta incluida en `DependencyPaths` (por ejemplo, las carpetas activas `$port`). El build puede generar `System.Reflection.Metadata.dll`, `System.Collections.Immutable.dll`, `System.Memory.dll`, `System.Buffers.dll`, `System.Runtime.CompilerServices.Unsafe.dll` y `System.Numerics.Vectors.dll`. Antes de copiarlas, compara sus nombres y versiones con las DLL de `SCPSL_Data\Managed` y con las dependencias ya instaladas: no sobrescribas ni rebajes ensamblados compartidos por el servidor u otros plugins. Si hacen falta versiones privadas, guárdalas en una carpeta dedicada y añade esa ruta relativa a `dependency_paths` sin quitar las rutas existentes. No copies DLL del juego ni de LabAPI. En el arranque, comprueba que LabAPI cargó el plugin y las dependencias sin errores; ejecuta `pdoctor compat-plugin` sobre una DLL conocida para probar el analizador en runtime.

`pdoctor health` consulta métricas en vivo expuestas por LabAPI y el proceso, sin mantener un sondeo ni cambiar el estado del servidor. `pdoctor report` guarda un archivo de soporte en `PentagolandServerDoctor\reports`, limitado a 1 MiB y 30 informes, con el estado, inventarios y hasta 40 líneas de los tres logs más recientes. El contenido de logs pasa por redacción; no es infalible, así que revisa el informe antes de compartirlo. El informe se crea solo al solicitarlo.

`pdoctor check-gameplay` comprueba de forma orientativa `server_ip`, `ipv4_bind_ip` e `ipv6_bind_ip` en un `config_gameplay.txt`. No prueba el firewall, NAT ni la conexión externa y nunca escribe cambios; trata los hallazgos como pistas y verifica la configuración de tu hoster/versión antes de corregirla.

## Configuración

Valores importantes (el archivo lo genera LabAPI):

```yaml
admin_permission: pentagoland.serverdoctor
server_root_path: ""
allow_config_changes: false
writable_roots: []
plugin_scan_roots:
  - labapi-plugins
  - labapi-user-plugins
  - labapi-server-plugins
  - exiled-plugins
  - exiled-server-plugins
dependency_scan_roots:
  - labapi-dependencies
  - labapi-user-dependencies
  - labapi-server-dependencies
maximum_backup_count: 200
maximum_config_file_bytes: 1048576
additional_roots: []
```

La configuración inicial es de solo lectura. Los comandos `backup` y `backups` siguen disponibles sin habilitar la edición. Para permitir `set` o `restore`, hay que activar la opción y autorizar explícitamente una raíz:

```yaml
allow_config_changes: true
writable_roots:
  - labapi-configs
```

Para modificar, por ejemplo, `friendly_fire` en un archivo existente del servidor, autoriza `scpsl-config`, crea una copia y confirma la operación:

```yaml
allow_config_changes: true
writable_roots:
  - scpsl-config
```

```text
pdoctor backup scpsl-config:7777/config_gameplay.txt
pdoctor set scpsl-config:7777/config_gameplay.txt friendly_fire true --confirm
```

La edición también genera su propia copia antes de escribir. No cambia la configuración en memoria. Hazlo en una ventana de mantenimiento: respalda el archivo fuera del servidor, revisa el resultado y verifica el ajuste tras el siguiente arranque. Los valores admitidos dependen de la versión del juego. El editor no puede enumerar valores que el juego no haya guardado todavía ni validar rangos específicos de una versión.

Usa la raíz más estrecha posible. No autorices `server` si basta `labapi-configs`. Los nombres disponibles aparecen con `pdoctor roots`. Las raíces habituales son `server`, `scpsl-data`, `scpsl-config`, `labapi-configs`, `labapi-plugins`, `labapi-dependencies`, `labapi-data`, `labapi-server-plugins`, `labapi-server-dependencies`, `scpsl-user-data`, `scpsl-user-config`, `labapi-user-configs`, `labapi-user-plugins`, `labapi-user-dependencies`, `labapi-user-data`, `exiled-configs`, `exiled-plugins`, `exiled-data`, `exiled-server-plugins`, `configs` y `logs`; algunas solo aparecen cuando corresponden a la política de rutas activa. `scpsl-data` sigue la ruta de datos de LabAPI; `scpsl-config` apunta a los archivos de configuración por puerto (por ejemplo `7777/config_gameplay.txt`). El editor solo cambia un valor escalar que ya existe, con `clave: valor`; no valida los rangos de cada ajuste y no aplica cambios en memoria. La raíz `scpsl-config` no se añade automáticamente a `WritableRoots`: autorízala expresamente solo si quieres editar ajustes del servidor. Las raíces `*-user-*` permiten inspeccionar datos de usuario cuando el hoster activa `gamedir_for_configs`. EXILED usa su propia ubicación y los hosters pueden diferir; las raíces adicionales permiten cubrir instalaciones personalizadas. Las raíces personalizadas se declaran así:

```yaml
additional_roots:
  - name: mi-plugin
    path: D:\SCP Server\MiPlugin\Configs
```

Las rutas relativas de `AdditionalRoots` se resuelven desde el directorio del servidor. Reinicia el servidor después de cambiar esta configuración.

El plugin detecta la raíz buscando `SCPSL_Data\Managed` desde el directorio de trabajo y la ruta base del proceso. Si el hoster usa otra estructura, define `ServerRootPath` explícitamente; una ruta relativa se resuelve desde el directorio de trabajo del servidor.

### Edición escalar YAML

La edición admite claves sencillas separadas por puntos, por ejemplo `plugin.enabled`. Si la clave no existe, es ambigua, pertenece a una colección o no es escalar, la operación se cancela sin escribir. Los valores `true`, `false`, `null`, `~`, enteros y decimales simples se guardan como valores YAML; otros valores se guardan como texto entre comillas. No se interpreta sintaxis YAML avanzada. `--confirm` es obligatorio en cada edición y `confirm` en cada restauración.

Cada edición genera una copia previa y una entrada en `PentagolandServerDoctor\audit.log`. La copia se puede listar y restaurar:

```text
pdoctor backups
pdoctor restore <id> confirm
```

Escribir o restaurar exige el token explícito de confirmación, `AllowConfigChanges` habilitado y que la raíz original esté autorizada. Antes de cada cambio, se guarda una copia del estado actual; antes de restaurar, se guarda también un rollback. Las copias no se borran automáticamente; limpia las antiguas desde el gestor de archivos del host cuando hayas verificado que ya no hacen falta. El límite por defecto es 200 copias, 256 MiB en total y 1 MiB por archivo (configurable hasta 16 MiB por archivo). El registro de auditoría se limita a 5 MiB y requiere archivado manual al llenarse.

## Seguridad y alcance

- Las acciones de lectura y escritura exigen el permiso de administrador configurado. No hay comandos de jugador ni de consola cliente.
- Se rechazan rutas absolutas, `..`, enlaces simbólicos y puntos de reanálisis.
- Solo se pueden leer extensiones de texto conocidas; no se pueden descargar DLL ni ficheros binarios mediante Remote Admin.
- La redacción es una protección de conveniencia, no un detector perfecto de secretos. Revisa la salida antes de compartirla.
- El plugin no incluye servidor HTTP, acceso remoto nuevo, telemetría, llamadas web ni dependencias de terceros.
- La escritura está desactivada por defecto y nunca activa una recarga automática.
- Mantén copias fuera del servidor antes de tocar configuraciones importantes. Prueba los cambios primero en un servidor de staging.

## Desarrollo

La lógica de rutas, redacción, copias y edición YAML tiene pruebas en `tests/`. Para comprobar el plugin completo, compílalo también con los ensamblados reales del servidor e instálalo en una instancia de pruebas. No se deben añadir DLL privadas del juego o de plugins de terceros al repositorio.

## Licencia

MIT. Consulta [LICENSE](./LICENSE).

## Referencias oficiales

- [LabAPI `PathManager`](https://github.com/northwood-studios/LabAPI/blob/7b837be2ce8012f12b2d1dd80c52c4c11ba4e0c3/LabApi/Loader/Features/Paths/PathManager.cs): datos del juego, `AppData` y política de hoster.
- [LabAPI `LabApiConfig`](https://github.com/northwood-studios/LabAPI/blob/7b837be2ce8012f12b2d1dd80c52c4c11ba4e0c3/LabApi/Loader/Features/Configuration/LabApiConfig.cs) y [`PluginLoader`](https://github.com/northwood-studios/LabAPI/blob/7b837be2ce8012f12b2d1dd80c52c4c11ba4e0c3/LabApi/Loader/PluginLoader.cs): rutas configurables de plugins y dependencias.
- [LabAPI `Server`](https://github.com/northwood-studios/LabAPI/blob/7b837be2ce8012f12b2d1dd80c52c4c11ba4e0c3/LabApi/Features/Wrappers/Server/Server.cs): telemetría de TPS, puerto y jugadores usada por `pdoctor health`.
- [LabAPI example command](https://github.com/northwood-studios/LabAPI/blob/7b837be2ce8012f12b2d1dd80c52c4c11ba4e0c3/LabApi.Examples/Commands/CommandsPlugin2/Commands/RemoteAdminCommands/ChangeDefaultGravityCommand.cs): registro de comandos para Remote Admin y consola de servidor.
- [LabAPI `Plugin`](https://github.com/northwood-studios/LabAPI/blob/7b837be2ce8012f12b2d1dd80c52c4c11ba4e0c3/LabApi/Loader/Features/Plugins/Plugin.cs) y [documentación oficial de SCP:SL sobre servidores modificados](https://scpslgame.com/csg): alcance de `IsTransparent` para utilidades administrativas no intrusivas.
- [SCP:SL Tech Support Wiki: Gameplay Config Setup](https://techwiki.scpslgame.com/books/server-guides/page/2-gameplay-config-setup): ubicación y función de `config_gameplay.txt`.
- [SCP:SL Tech Support Wiki: Server Verification troubleshooting](https://techwiki.scpslgame.com/books/server-issues/page/i-need-help-with-server-verification): ejemplos de sintaxis `clave: valor` para el archivo de configuración.

Como referencia de producto se revisó el listado de funciones del repositorio [HelpSense](https://github.com/XLittleLeft/HelpSense). Incluye limpieza de objetos y cambios de rondas/personajes además de administración; esas funciones pueden cambiar la partida, usan otra base de modding y no se importaron a este plugin de diagnóstico pasivo. Server Doctor toma como integración la telemetría disponible directamente en LabAPI, los logs y los archivos de configuración documentados, no los mecanismos internos de plugins ajenos.

La compatibilidad con plugins actuales y futuros no puede garantizarse de forma universal: un plugin puede cambiar API, código, configuración o estado en memoria, y puede ejecutarse bajo otro framework. Server Doctor es deliberadamente independiente de esos detalles: permite declarar raíces personalizadas, inventariar instalaciones, respaldar y editar escalares YAML cuando se autorice, generar informes redactados y comparar hashes estáticos. Para confirmar compatibilidad real se necesita la versión exacta del juego/LabAPI, el plugin, sus dependencias y una prueba de esa combinación en un servidor de staging.

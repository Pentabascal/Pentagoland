# Servidor SCP: Secret Laboratory — contenido y mantenimiento

Última revisión: 28 de septiembre de 2026  
Versión del servidor: **v2.0.0**  
Versión del juego: 14.2.7  
EXILED: 9.14.2  
Puerto de juego: 7777

Este documento describe el contenido que está instalado en el servidor, qué
plugins están activos, qué módulos se han excluido deliberadamente y qué
comandos o sistemas son importantes para su mantenimiento.

## Plugins activos

### Plugins de EXILED y LabAPI

- **Exiled.CreditTags**: muestra las etiquetas de crédito disponibles de los
  jugadores.
- **Exiled.CustomItems**: dependencia de runtime de VVUP. Su plugin se
  mantiene cargado para inicializar correctamente la configuración de la API,
  pero no contiene objetos propios ni muestra hints; `VVUP.CustomItems` es el
  único propietario activo del registro y de los textos.
- **Exiled.CustomRoles**: API base de roles personalizados. No se han instalado
  roles personalizados de este paquete.
- **Exiled.Events**: eventos de EXILED.
- **Exiled.Permissions**: permisos de Remote Admin y comandos.
- **CustomItemsAPI**: API adicional de objetos personalizados de LabAPI.
- **SubclassFramework**: sistema independiente de subclases con 101 subclases.
- **ScpProximityChat.Core** y **ScpProximityChat.SecretAPI**: fuentes y
  referencias disponibles, pero no confirmados como DLL activos en la
  auditoría del 24/09/2026.
- **SCP-457**: configuración conservada pero desactivada porque no hay una DLL
  de SCP-457 instalada en la carpeta de plugins actual.

### HUD y avisos

- **RueI 3.1.2** es el renderizador común de los elementos persistentes.
- **UltimateHUD** es propietario del HUD general. Su lista de espectadores
  integrada permanece desactivada porque `SpectatorListRueI` es el único
  propietario de esa lista.
- **ScpsInfoDisplay** añade información de SCP como otro elemento RueI; no
  reemplaza el HUD general.
- **ServerWatermark** muestra la marca inferior y **SpectatorListRueI** muestra
  quién observa al jugador. Sus configuraciones están sincronizadas con el
  servidor.
- **UsefulHints** y **UsefulHints.Addons** se mantienen desactivados: sus hints
  nativos `ShowHint` pueden tapar temporalmente todos los elementos RueI y sus
  mensajes se duplican con los avisos propios de VVUP.
- `UltimateHUD`, `ScpsInfoDisplay`, `SpectatorListRueI`, `ServerWatermark` y
  `VVUP.CustomItemRueIBridge` usan elementos persistentes RueI. Se retiró el
  alineado automático por resolución tras encontrar una excepción de rango en
  `ElementCombiner.SplitUntil`; el contador de munición ya no usa espacios
  negativos extremos. La información de espectador se centra y la fila
  inferior conserva la coordenada del bloque que siga habilitado.
- Los cinco proyectos se recompilaron y desplegaron; el último arranque cargó
  los plugins y no mostró nuevas excepciones del combinador. Aún se necesita
  revisar visualmente el HUD con un jugador conectado durante una ronda.

### Paquete VVUP de Snivy

- **VVUP.Base**: núcleo compartido y efectos visuales de objetos.
- **VVUP.CustomItems**: objetos, armas, granadas, armaduras y herramientas
  personalizados.
- **VVUP.CustomEscapes**: escapes personalizados.
  La regla por defecto de Class-D no esposado usa `NtfPrivate`; se eliminó una
  entrada duplicada que bloqueaba ese resultado.
- **VVUP.MicroDamageReduction**: reducción configurable del daño de MicroHID
  contra determinados SCP.
- **VVUP.PostNukeEnhancements**: mejoras posteriores a la detonación de la
  ojiva.
- **VVUP.RoundStart**: ajustes aplicados al comienzo de la ronda.
- **VVUP.ScpChanges**: cambios de comportamiento de los SCP.
- **VVUP.ServerEvents**: eventos de servidor configurables.
- **VVUP.SubclassCompatibility**: puente entre `SubclassFramework` y
  `VVUP.CustomItems`.
- **VVUP.Votes**: votaciones de mapa.
- **VVUP.WeaponEvaporate**: reglas configurables de evaporación por tipo de
  daño.

La auditoría de configuraciones del 25/09/2026 mantuvo intacta la separación
de propietarios de Custom Items y desactivó el señuelo experimental de SCP-173.

### Sistema de experiencia

`XPSystem` queda activo con almacenamiento LiteDB y cálculo cuadrático:
`XP = 100 * nivel^2`. Los comandos de jugador son `.xp get`, `.xp leaderboard`
y los alias `.nivel` y `.topxp`. El sistema recompensa kills, escapes, victorias,
generadores, reanimaciones y armas especiales, con límites para puertas,
recogidas y drops para evitar farmeo.

Las recompensas de eventos se mantienen silenciosas para no sobrescribir los
hints de Custom Items. Solo los ascensos de nivel muestran un aviso. El sistema
no sustituye `StatsSystem`, no modifica sus archivos y mantiene DNT respetado.
`StatsSystem` está instalado como plugin LabAPI independiente para estadísticas
persistentes de carrera. El último arranque registrado confirmó `StatsSystem`
2.1.1 y `MvpSystem` 1.2.0 activos; `MvpSystem` debe tratarse como cargado y
revisarse junto a `VVUP.ServerEvents` si se cambia el resumen de ronda.

El arranque registrado el 28/09/2026 a las 00:08 confirmó RueI 3.1.2,
UltimateHUD, ScpsInfoDisplay, SpectatorListRueI y ServerWatermark. Ese log es
anterior al despliegue HUD del 28/09 por la tarde; hace falta revisar el próximo
arranque para verificar los binarios recién compilados y la desactivación de
UsefulHints.

## Integración de subclases y armas

### Comandos de jugador

Los comandos de cliente no dependen del registro SSSS y están disponibles
desde la consola del juego:

- `.ayuda` (`.help`, `.comandos`): muestra las funciones disponibles.
- `.subclase`: muestra la subclase actual, descripción y cooldown.
- `.subclases`: lista las subclases registradas y sus roles compatibles.
- `.hab`: activa la habilidad de la subclase cuando corresponda.
- `.preferencias` (`.prefs`, `.ajustes`): permite ocultar o mostrar el aviso de
  introducción y el contador de habilidad.
- `.vvuphelp` (`.vvupayuda`, `.cihelp`): muestra los comandos y acciones de
  objetos personalizados.
- `.cihints` (`.itemhints`, `.avisositems`): activa o desactiva por jugador el
  aviso al seleccionar un objeto personalizado.

Se mantienen como funciones informativas y de comodidad; no conceden permisos
ni objetos y no alteran el equilibrio por sí mismos.

`VVUP.SubclassCompatibility` aplica automáticamente estos objetos:

| Subclase | Objeto | ID |
|---|---|---:|
| Demoledor | Lanzagranadas ADATS | 46 |
| Francotirador MTF | Fusil de francotirador EL-95 | 58 |

También puede entregar objetos de apoyo a subclases existentes, sin crear
roles paralelos:

| Subclase | Objeto | ID |
|---|---|---:|
| Radiotelegrafista | Intercomunicador portátil | 47 |
| Bioquímico | Desfibrilador (DEA) | 56 |
| Escudero MTF | Granada Guardián | 55 |

Estas entregas se controlan con `enable_extra_subclass_items` en
`VVUP.SubclassCompatibility`. El puente comprueba que el jugador no tenga ya
el mismo Custom Item, por lo que una reasignación o respawn no duplica objetos.
También valida que los IDs no sean cero ni estén repetidos y cancela entregas
pendientes cuando el jugador abandona la partida.

El puente espera a que la subclase termine de asignar el inventario, elimina
el arma vanilla que entra en conflicto y entrega el objeto personalizado.

## Objetos personalizados activos

### Estado de los ajustes SSSS

Los ajustes se registran una sola vez desde `VVUP.Base` mediante
`Exiled.API.Features.Core.UserSettings.SettingBase.Register`. Este es el punto
de entrada soportado por EXILED y evita reasignar directamente
`ServerSpecificSettingsSync.DefinedSettings`, que era la causa de los
duplicados persistentes de SecretAPI.

En la configuración estable del servidor, `ssss_enabled` de `VVUP.CI` y
`use_server_specific_keybind` de `subclass_framework` permanecen desactivados.
La combinación actual de SecretAPI y el registro central todavía puede
notificar duplicados al publicar estos ajustes, incluso cuando los IDs son
únicos. Las habilidades siguen disponibles mediante `.hab`, y los objetos
personalizados siguen funcionando sin SSSS. El bind configurable se mantiene
desactivado hasta validar la versión exacta de SecretAPI, porque algunas
versiones duplican o rechazan ajustes SSSS al combinar varios proveedores.

`SubclassFramework` aporta el bind de habilidad y `VVUP.CustomItems` aporta
el catálogo, los binds de C4/F4/ADATS y los colores del Rayo Helios. Antes de
registrarlos se comprueba que todos los IDs sean únicos. Si
`ssss_enabled` está desactivado o la configuración contiene IDs inválidos, el
módulo no aporta ajustes, pero los objetos personalizados siguen funcionando.

No se añaden IDs SSSS nuevos mientras el registro central permanezca
desactivado en producción. Los comandos de jugador no dependen de SSSS y son
la vía estable de accesibilidad. Los IDs actuales reservados para una futura
prueba aislada son 11000-11006; cualquier ampliación deberá usar otro bloque y
validarse contra SecretAPI antes de activarse.

FacilityManagement conserva una configuración endurecida y permanece
desactivado: no se activa sin revisar su DLL y sus handlers contra EXILED
9.14.2, porque modifica globalmente generadores, Tesla, SCP-914, puertas,
inventario, intercom y la limpieza de ojiva. Sus secciones de cambios globales
están vacías y `warhead_cleanup` es `false` para evitar pérdida de objetos.

El catálogo activo contiene 32 objetos con nombre y descripción configurados.

| ID | Objeto |
|---:|---|
| 20 | Obscurus Veil-5, granada de humo |
| 22 | Granada de agente nervioso |
| 23 | Dispositivo señuelo fantasma |
| 24 | Linterna fantasma |
| 25 | Armadura resistente a explosivos |
| 26 | LJ-429 |
| 27 | Pulso fantasma |
| 28 | Serenata silenciosa |
| 29 | SCP-1499 |
| 30 | PB-42 |
| 31 | Amnesioflux |
| 33 | SCP-2818 |
| 34 | Píldoras infinitas |
| 36 | Granada de racimo |
| 37 | SCP-207 de salud adicional |
| 38 | Coraza Nebulosa |
| 39 | Viper |
| 40 | Pathfinder |
| 41 | Rayo Helios X-57 |
| 42 | Echo Flare |
| 43 | Granada Pathfinder |
| 46 | Lanzagranadas ADATS |
| 47 | Intercomunicador portátil |
| 50 | Televarita |
| 51 | Lunar Lob |
| 53 | Botella de ron |
| 55 | Granada Guardián |
| 56 | Desfibrilador externo automático (DEA) |
| 57 | Cuchillo |
| 58 | Fusil EL-95 de cerrojo |
| 59 | SCP-427 |
| 60 | Estimulante experimental |

Al recoger o seleccionar cualquier objeto personalizado se muestra su nombre y
descripción traducidos durante unos segundos mediante los handlers propios de
`VVUP.CustomItems`. El hint se muestra una sola vez durante exactamente 4
segundos, después de que terminen los eventos de inventario, para evitar
parpadeos y reinicios visuales. El texto usa un tamaño reducido (`22px`) y los
hints equivalentes de `CustomItemsAPI` y `Exiled.CustomItems` deben permanecer
con `show_picked_up_hint: false` y `show_selected_hint: false`; VVUP es el
único propietario de los avisos. En consola de jugador, `.detonate` (alias `.det`) detona las cargas
C4/F4 propias dentro del alcance y exige el detonador configurado; aparece en
`.help`. En Remote Admin, `vvupci` es el comando principal y
`customitems`/`ci`/`items` son alias. Admite `list`, `info`, `give`, `spawn` y
`help`, y requiere el permiso `vvup.customitems`. El nombre principal evita
colisiones con el comando de Custom Items incluido en EXILED.

Ejemplos:

```text
vvupci list
vvupci info 39
vvupci give 2 39
vvupci give viper
vvupci spawn 2 46
```

## Subclases

`SubclassFramework` incluye 101 subclases repartidas entre:

- Insurgencia del Caos.
- Fundación: MTF, guardias y científicos.
- Clase D.
- SCP.
- Variantes especiales de SCP-173 y SCP-096.

La configuración se encuentra en:

`EXILED\Configs\Plugins\subclass_framework\7777.yml`

Comandos principales:

```text
subclass list
subclass spawn <id-jugador> <id-subclase>
scspawn <id-jugador> <id-subclase>
.hab
.habilidad
.subclase
.miclase
```

Permiso administrativo:

```text
subclass.spawn
```

## Integraciones opcionales y desactivadas deliberadamente

### RemoteKeycard

`RemoteKeycard` 3.4.2 se ha actualizado manualmente para EXILED 9.14.2.
La versión preparada corrige la liberación no determinista de los eventos al
recargar el plugin y está instalada en `Plugins\7777`.

La configuración efectiva es `Configs\Plugins\remote_keycard\7777.yml` y está
activa para puertas, generadores, panel de ojiva y taquillas SCP. Por tanto,
RemoteKeycard pertenece al conjunto de plugins activos, no al conjunto de
integraciones opcionales. La copia de staging se conserva en
`EXILED\Plugins\staging\RemoteKeycard-3.4.2\` como referencia y rollback.

### KillMessage

KillMessage 1.0.8 se ha actualizado a EXILED 9.14.2 y se ha compilado con las
referencias locales del servidor. Se corrigieron el orden de limpieza al
desactivar, el cierre seguro de LiteDB y el acceso nulo durante `Died`.
Permanece en `Plugins\staging\KillMessage-1.0.8` porque añade persistencia y
mensajes de muerte que deben probarse sin mezclarlo con cambios de HUD.

### Uncomplicated Custom Items

UCI está desactivado para evitar duplicar objetos, comandos, teclas y
configuraciones con `VVUP.CustomItems`. Sus archivos se conservan fuera de la
carpeta de plugins en:

`EXILED\DisabledPlugins\`

No se deben mover de nuevo a `Plugins` sin desactivar antes los objetos
equivalentes de Snivy.

### Módulos Snivy no instalados

No se han instalado los siguientes módulos para evitar duplicidades o contenido
no solicitado:

- `VVUP.CustomRoles`
- `VVUP.FreeCustomRoles`
- `VVUP.HuskInfection`
- `VVUP.CreditTags`
- `VVUP.OperationCrossfireServerEvent`

La razón principal es conservar las subclases existentes, evitar añadir roles
paralelos y no duplicar el sistema de etiquetas de crédito de EXILED.

### Integración LabAPI de ScpProximityChat

Los dos módulos se cargan desde:

`%AppData%\SCP Secret Laboratory\LabAPI\plugins\7777\`

Sus dependencias están aisladas en:

`%AppData%\SCP Secret Laboratory\LabAPI\dependencies\7777\`

- `SecretLabNAudio.Core.dll` `2.0.0`.
- `SecretAPI.dll` `3.4.0`.
- `0Harmony.dll` `2.2.2.0`.

La configuración propia de ScpProximityChat todavía no existe porque el
servidor debe generarla en su siguiente arranque. No se ha inventado un YAML
manual; se revisará después de que LabAPI lo cree.

### Registro central de Server-Specific

En el arranque, `VVUP.Base` recopila los ajustes expuestos por los módulos
cargados (`VVUP.CustomItems`, roles compatibles y `SubclassFramework`) y los
publica una sola vez. Las teclas y preferencias de cada módulo siguen
recibiendo sus eventos propios; el registro central solo administra la
configuración visible para el jugador.

## Configuración visible en español

Se han traducido al castellano los mensajes visibles de:

- Recogida y selección de objetos.
- Votaciones.
- Objetos personalizados de Snivy.
- Modos del lanzagranadas.
- Detonador de C4/F4.
- Colores del rayo Helios.
- Integración de equipamiento de subclases.

Los identificadores técnicos, nombres de enums, tipos de daño, IDs, permisos y
rutas no se traducen porque el servidor los necesita exactamente con su nombre
original.

## Rebalance vigente

- Fusil EL-95: daño `110`.
- SCP-2818: daño `350`.
- Radiación contra SCP: multiplicador `3`.
- SCP-106: resistencia `0.25`, sin ejecución instantánea.
- SCP-049: sin ejecución instantánea.
- SCP-096: sin furia ilimitada durante la ojiva y `50` de curación por baja.
- Probabilidades de subclases: `30/45/80` para humanos y `55/70/92` para SCP.

### Mejoras ligeras para humanos

- La armadura resistente a explosivos reduce ahora un `55 %` del daño recibido.
- SCP-207 de salud añade `20` puntos de vida máxima.
- El DEA revive con `55` puntos de vida.
- SCP-427 regenera `0,3` puntos por segundo.
- Al apuntar con el EL-95 se conserva una ralentización leve, no incapacitante.

## Buenas prácticas de mantenimiento

1. Ejecutar una sola instancia de `SCPSL.exe`; el servidor debe iniciarse desde
   LocalAdmin.
2. No activar UCI mientras `VVUP.CustomItems` esté activo.
3. No cambiar los IDs de los objetos sin actualizar
   `VVUP.SubclassCompatibility`.
4. Tras modificar una DLL, reiniciar el servidor una sola vez y revisar que no
   haya excepciones de carga.
5. Mantener `SubclassFramework` y los módulos VVUP compilados para EXILED 9.14.2.
6. Probar las subclases con `subclass spawn` antes de modificar sus
   probabilidades globales.

## Auditoría técnica del 24/09/2026

- Se reconstruyeron la solución Snivy, `SubclassFramework` y
  `VVUP.SubclassCompatibility` con EXILED 9.14.2; las tres compilaciones
  terminaron sin errores.
- `Pathfinder` ahora distingue correctamente equipos SCP/Tutorial y no permite
  que un temporizador antiguo quite una marca renovada.
- `VVUP.PostNukeEnhancements` desregistra el mismo evento que registra y no
  aplica colores sentinel inválidos al finalizar la ojiva.
- `Exiled.CustomItems.dll` debe estar físicamente disponible como dependencia
  de runtime para `VVUP.Base` y `VVUP.CustomItems`, pero su configuración
  `exiled_custom_items/7777.yml` permanece desactivada. El registro activo de
  objetos pertenece únicamente a `VVUP.CustomItems`.
- El ID del keybind de `SubclassFramework` conserva `774201`
  y el keybind queda desactivado en la
  configuración actual porque el registro combinado EXILED/SecretAPI duplica
  el ajuste incluso con un ID exclusivo. El comando `.hab` permanece activo.
- La configuración efectiva de subclases usa `30/45/80` para humanos y
  `55/70/92` para SCP; el multiplicador de radiación contra SCP es `3`.
- Persisten advertencias de resolución de referencias Unity durante el build;
  proceden del conjunto completo de DLL del juego y no generaron errores C#.
- Se instalaron y verificaron los DLL oficiales de ScpProximityChat v1.1.1,
  SecretAPI 3.4.0 y SecretLabNAudio.Core 2.0.0 en las carpetas LabAPI del
  puerto 7777.
- Se retiró el duplicado global de `UncomplicatedCustomItems-Exiled.dll`;
  su copia idéntica permanece en `LabAPI\plugins\disabled`.
- El arranque anterior no mostró excepciones nuevas en `ServerLogs`; falta
  validar en partida la transmisión de voz y revisar la configuración que se
  genere en el próximo inicio.
- Se ejecutó el servidor mediante `LocalAdmin.exe` en tres arranques de
  validación. El tercero terminó con `SCPSL.exe` vivo y cargó todos los módulos
  VVUP, `Exiled.CustomItems`, `SubclassFramework` y ScpProximityChat.
- El tercer arranque no produjo `TypeLoadException`, `duplicate ID`, errores de
  dependencias ni deshabilitaciones de VVUP. Solo permanecen tres avisos
  `gpath` de Mono del cargador, sin impacto observable en la disponibilidad.

## Ubicaciones importantes

- Plugins activos: `EXILED\Plugins\7777\`
- Configuración: `EXILED\Configs\Plugins\`
- Configuración de SCP-457 pendiente: `EXILED\Configs\Plugins\s_c_p457\`
- Plugins LabAPI activos: `%AppData%\SCP Secret Laboratory\LabAPI\plugins\7777\`
- Dependencias LabAPI: `%AppData%\SCP Secret Laboratory\LabAPI\dependencies\7777\`
- Plugins desactivados conservados: `EXILED\DisabledPlugins\`
- Código fuente Snivy:
  `EXILED\zCreacionPlugins\SnivysUltimatePackage-master\`
- Código fuente de subclases:
  `EXILED\zCreacionPlugins\SUBCLASES\SubclassFramework\`
- Dependencias: `EXILED\Plugins\dependencies\`

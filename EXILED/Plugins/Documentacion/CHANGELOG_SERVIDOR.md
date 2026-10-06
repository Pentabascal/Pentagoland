# Historial de cambios del servidor

## 2026-09-28 — Estabilización del renderizador HUD RueI

- Desactivados `UsefulHints` y `UsefulHints.Addons`: enviaban hints nativos que
  podían ocultar temporalmente todos los elementos persistentes de RueI.
- Recompilados y desplegados `UltimateHUD`, `ScpsInfoDisplay`,
  `SpectatorListRueI`, `ServerWatermark` y `VVUP.CustomItemRueIBridge`.
- Quitado el alineado automático basado en resolución de los elementos RueI:
  el log registraba `ArgumentOutOfRangeException` en
  `RueI.API.Parsing.ElementCombiner.SplitUntil` al combinar las capas.
- Reemplazado el espacio negativo de `-900` en el contador de munición por
  texto centrado normal.
- Centrada la información del jugador observado y la fila combinada de mapa y
  servidor; si se desactiva uno de sus bloques, se usa la posición del bloque
  que permanece activo.
- Corregida la opción TPS `BOTH` para que también se muestre a espectadores.
- Respaldados los cinco binarios previos y la configuración HUD en
  `%AppData%\EXILED\Backups\hud-ruei-parser-fix-20260928`.
- Los cinco proyectos compilaron sin errores ni avisos; el servidor volvió a
  iniciar y cargó los complementos RueI corregidos.
- No se observan nuevas excepciones de combinación RueI en el arranque
  posterior. Falta confirmar la presentación visual con un cliente dentro de
  una ronda.

## 2026-09-25 — Activación del sistema XPSystem

- Activado `XPSystem-EXILED.dll` 2.1.2 con almacenamiento LiteDB.
- Configurados niveles cuadráticos con 100 XP por nivel al cuadrado.
- Habilitados `.xp get`, `.xp leaderboard`, `.nivel` y `.topxp`.
- Añadidos badges y formato de nivel en español.
- Ajustadas recompensas para evitar farmeo de drops, puertas, uso genérico y
  granadas.
- Las recogidas de Custom Items siguen concediendo XP, pero sin mostrar un
  hint adicional que pueda sobrescribir las descripciones de VVUP.
- Se conservaron DNT, `StatsSystem` y los propietarios actuales de Custom Items.

## 2026-09-25 — Auditoría general de configuraciones EXILED

- Corregida una regla duplicada de `VVUP.CustomEscapes` que bloqueaba el escape
  normal de Class-D no esposados antes de alcanzar la regla NTF.
- Desactivado el señuelo experimental de SCP-173 en `SubclassFramework` para
  evitar comportamiento inestable mientras se mantiene el resto de subclases.
- Desactivado el modo debug de `AdminTools`.
- Corregida una etiqueta de formato sobrante en el texto de SCP-939.
- Se mantuvieron intactos los propietarios de Custom Items, los hints de VVUP,
  RemoteKeycard y FacilityManagement.

## 2026-09-25 — Endurecimiento de FacilityManagement

- Mantenido desactivado porque no hay código fuente local ni una compilación
  verificable contra EXILED 9.14.2.
- Desactivada la limpieza automática de objetos y cadáveres tras la ojiva.
- Vaciadas las secciones de Tesla, SCP-914, ventanas, puertas, Hume Shield,
  objetos y roles para que no modifiquen sistemas que ya controla el servidor.
- Eliminada la munición infinita de ParticleDisruptor del perfil preparado.
- La configuración queda lista para una prueba aislada, pero la DLL no se
  despliega en producción.

## 2026-09-25 — Desactivación de hints duplicados de CustomItemsAPI

- Corregidos `show_picked_up_hint` y `show_selected_hint` en
  `Configs\Plugins\CustomItemsAPI\7777.yml`: estaban en `true` y competían con
  el sistema de hints de `VVUP.CustomItems`.
- `VVUP.CustomItems` queda como único sistema que muestra descripciones al
  recoger o seleccionar objetos.
- `Exiled.CustomItems` ya tenía sus dos avisos internos desactivados.

## 2026-09-25 — Revisión de robustez y ciclo de vida

- `.detonate` ya soporta correctamente instalaciones donde solo esté registrado
  C4 o F4 y evita acceder a instancias no inicializadas.
- Las cargas inválidas o pickups destruidos se omiten antes de calcular la
  distancia, evitando excepciones durante la detonación.
- Las preferencias de `.cihints` se limpian al desconectarse el jugador para
  evitar crecimiento de memoria y estados heredados por otro usuario.
- `VVUP.SubclassCompatibility` cancela entregas diferidas cuando un jugador
  abandona y valida `apply_delay` e IDs de Custom Items al iniciar.
- Los IDs cero o duplicados desactivan únicamente las entregas extra, no todo
  el framework ni las subclases.

## 2026-09-25 — Compatibilidad ampliada entre subclases y Custom Items

- El puente de compatibilidad ahora equipa de forma opcional a tres subclases
  existentes con objetos coherentes: Radiotelegrafista (47), Bioquímico (56)
  y Escudero MTF (55).
- Añadida la opción `enable_extra_subclass_items`, activada por defecto pero
  completamente reversible desde la configuración.
- Añadidos IDs configurables para cada entrega, con valores validados contra
  el catálogo activo de VVUP.
- Evitadas duplicaciones durante reasignaciones o respawns comprobando si el
  jugador ya posee el mismo Custom Item.
- No se crearon nuevas subclases ni se modificó la selección de roles: se
  reutilizan subclases existentes y probadas para reducir riesgo.

## 2026-09-25 — Accesibilidad y comandos de Custom Items

- Corregida la resolución del jugador en `.detonate`/`.det`: ahora funciona
  con el `CommandSender` real de la consola de cliente y devuelve un mensaje
  claro si se ejecuta durante la autenticación.
- Añadida `.vvuphelp` (`.vvupayuda`, `.cihelp`, `.ciayuda`) con las acciones
  disponibles de objetos personalizados.
- Añadida `.cihints` (`.itemhints`, `.avisositems`) para ocultar o recuperar
  por jugador los avisos de selección.
- Añadidas comprobaciones para evitar excepciones si C4/F4 aún no están
  registrados.
- No se reutilizaron ni ampliaron IDs SSSS; el registro continúa siendo
  opt-in hasta resolver las colisiones históricas con SecretAPI.

## 2026-09-25 — Corrección del spawn inválido de SCP-427

- Eliminado `AntiScp207Pedestal` de los puntos de aparición de SCP-427. Aunque
  el enum existe en la API, los mapas activos no contienen ese locker y EXILED
  registraba `No locker found of type AntiScp207Pedestal`.
- Actualizados el valor por defecto del plugin y la configuración efectiva del
  puerto 7777 para que no vuelva a generarse esa referencia.

## 2026-09-25 — Hint de Custom Items estable y sin parpadeo

- Eliminada la renovación periódica del hint, que reiniciaba el HUD y
  provocaba 6-7 destellos visibles.
- El texto se presenta una sola vez durante exactamente 4 segundos después de
  estabilizar los eventos de inventario.
- Aplicado tamaño reducido `<size=22>` para mejorar la lectura sin ocupar tanto
  espacio.
- Desactivado también el aviso de recogida interno de `Exiled.CustomItems`;
  `CustomItemsAPI` ya tenía sus dos avisos desactivados. VVUP queda como único
  sistema que presenta la descripción.

## 2026-09-24 — Corrección crítica de Custom Items y comandos de detonación

- Corregida la excepción `NullReferenceException` de
  `Exiled.CustomItems.ShowPickedUpMessage` y `ShowSelectedMessage`: la
  dependencia se inicializa con su configuración, pero sus mensajes propios
  permanecen ocultos y VVUP es el único presentador.
- Las entregas de `VVUP.SubclassCompatibility`, `vvupci` y reposiciones
  internas ya no solicitan el mensaje inseguro de la API; VVUP muestra el
  nombre y la descripción de forma controlada.
- Los hints de recogida/selección se renuevan cada 0,75 segundos durante al
  menos 4 segundos y se limpian al desactivar el plugin.
- `.detonate` y `.det` quedan documentados en español y aparecen en la ayuda
  de comandos del cliente. Solo afectan a las cargas del jugador, respetan la
  distancia máxima y exigen el detonador cuando así lo indique el objeto.

## 2026-09-24 — Versión final v2.0.0: base estable y reproducible

- Consolidado el paquete de producción con VVUP, SubclassFramework,
  RemoteKeycard, CustomItemsAPI y sus dependencias necesarias.
- Separadas las integraciones opcionales de la carga estable: no se activan
  plugins experimentales, duplicados de HUD, sistemas alternativos de objetos,
  parches Harmony o bases de datos adicionales sin validación individual.
- Corregida la documentación para distinguir RemoteKeycard activo de las
  integraciones opcionales desactivadas.
- Añadidos un manifiesto de versión y reglas de mantenimiento reproducibles.
- Verificado el estado de la carpeta activa: 15 DLL, sin DLL pendientes,
  una sola instancia dedicada y sin errores críticos de carga en los logs
  revisados.

## 2026-09-24 — Comando seguro y entrega directa de Custom Items

- Cambiado el nombre principal del comando propio a `vvupci` para evitar la
  colisión con el comando homónimo de `Exiled.CustomItems`.
- Conservados `customitems`, `ci`, `items` y `customitem` como alias cuando el
  registro de comandos permite usarlos.
- La entrega mediante `give` muestra explícitamente el nombre y la
  descripción del objeto, incluso cuando la entrega no genera un evento de
  recogida física.
- Compilado `VVUP.CustomItems` correctamente y desplegado con copia de
  seguridad previa.

## 2026-09-24 — Limpieza de duplicado y preparación de KillMessage

- Restaurado `Exiled.CustomItems.dll` en `Plugins\7777` como dependencia de
  runtime requerida por `VVUP.Base` y `VVUP.CustomItems`; su configuración
  propia sigue desactivada para evitar un segundo registrador.
- Instalada la DLL de RemoteKeycard en `Plugins\7777` con
  `is_enabled: false`, para que el siguiente arranque pueda verificar la carga
  sin introducir todavía su comportamiento en partida.
- Confirmado que `exiled_custom_items/7777.yml` sigue desactivado y que
  `VVUP.CustomItems` es el único registrador activo de objetos.
- Actualizado KillMessage 1.0.8 a EXILED 9.14.2 con referencias locales
  verificables.
- Corregidos el cierre seguro de LiteDB, la limpieza del ciclo de vida y el
  acceso potencialmente nulo en el evento de muerte.
- Compilado con `0` errores y `0` advertencias; aislado en staging con
  configuración desactivada.

## 2026-09-24 — Actualización segura de RemoteKeycard

- Adaptado el objetivo EXILED de RemoteKeycard 3.4.2 a `9.14.2`.
- Corregida la liberación de eventos para que ocurra de forma determinista en
  `OnDisabled`, evitando handlers duplicados durante recargas.
- Compilado con `0` errores y `0` advertencias.
- Aislado en `Plugins\staging\RemoteKeycard-3.4.2` con configuración
  desactivada hasta probarlo en una ronda real.

## 2026-09-24 — Desactivación preventiva de SSSS duplicado tras el arranque

- El arranque con SecretAPI volvió a notificar duplicados para los ajustes
  `11000`–`11006` y `774201`.
- Se desactivaron `ssss_enabled` y `use_server_specific_keybind` en la
  configuración efectiva del puerto `7777`.
- Se mantuvieron `.hab`, los comandos de subclases y los objetos
  personalizados; no se desactivó ningún plugin de gameplay.
- El valor predeterminado de `SubclassFramework` ahora también es seguro para
  instalaciones nuevas.

## 2026-09-24 — Registro SSSS mediante la API soportada de EXILED

- Reactivado el registro de Server-Specific con `SettingBase.Register`, sin
  reasignar `DefinedSettings` ni mantener una segunda colección global.
- Centralizados el bind de habilidades de subclases y los controles de Custom
  Items en `VVUP.Base`, con deduplicación por ID antes del registro.
- `VVUP.CustomItems` ya respeta `ssss_enabled` también al generar sus ajustes;
  una configuración desactivada o inválida no deja controles huérfanos.
- Corregida una reutilización insegura de `StringBuilderPool` al generar el
  catálogo de objetos.
- El HUD de habilidad se dibuja inmediatamente después de asignar la
  subclase y se desplaza ligeramente hacia abajo para no tapar el centro de la
  pantalla.

## 2026-09-24 — Eliminación del duplicado persistente de SecretAPI

- Retirado el coordinador dinámico de `VVUP.Base`: SecretAPI mantenía las
  definiciones anteriores internamente y producía duplicados aunque el código
  recibiera solo ajustes nuevos.
- Desactivado `ssss_enabled` de `VVUP.CustomItems`. Esto no desactiva los
  objetos, únicamente evita registrar los controles globales que provocaban
  errores en consola.
- Corregidos los textos restantes en inglés y las descripciones rotas del
  catálogo de Custom Items en la configuración activa.
- Reiniciado el servidor después de desplegar la DLL final.

## 2026-09-24 — Corrección definitiva de Custom Items y SSSS

- Migrados los IDs de ajustes de Custom Items de `10000-10006` a
  `11000-11006`; los IDs antiguos colisionaban con el registro interno de
  SecretAPI y provocaban errores `duplicate ID`.
- El coordinador de `VVUP.Base` ya no reasigna `DefinedSettings` completo:
  envía únicamente ajustes nuevos y conserva los de todos los demás plugins.
- Organizado Server-Specific con las secciones `HABILIDADES DE SUBCLASES` y
  `ARMAS Y OBJETOS PERSONALIZADOS`.
- Activados los avisos de selección de `CustomItemsAPI` y eliminado el handler
  paralelo de VVUP que podía mostrar mensajes duplicados o lanzar excepciones.
- Traducidos al español los nombres, descripciones y opciones visibles de los
  objetos C4, F4, Píldoras infinitas y Rayo X-57 Helios.

## 2026-09-24 — Restauración previa a CLASS SWAP y auditoría

- Retirada completamente la lógica de CLASS SWAP y sus opciones de
  configuración para volver al estado estable anterior.
- Restaurado el identificador histórico de habilidad de SubclassFramework
  (`774201`) y mantenido el registro SSSS propio desactivado, evitando que
  SecretAPI vuelva a generar duplicados.
- Desactivada la configuración huérfana de SCP-457: no existe una DLL activa
  correspondiente en `Plugins\7777`.

## 2026-09-24 — Comandos de calidad de vida sin SSSS

- Añadidos `.ayuda`, `.subclases` y alias equivalentes para descubrir las
  funciones del servidor desde la consola del cliente.
- Retirado `.reglas`, que no aportaba una función práctica.
- Retirados `.estado`, `.ronda` y `.objetos` por no aportar control real al
  jugador.
- Añadido `.preferencias` para controlar de forma individual la introducción
  de subclase y el contador de habilidad.
- Añadido `.objetosavisos` para controlar las descripciones de Custom Items al
  seleccionarlos.
- Las preferencias se guardan durante la sesión por SteamID y no muestran
  información de ronda, posiciones ni estados de otros jugadores.
- Reactivada de forma protegida la opción Server-Specific de habilidad de
  subclase (`774201`), con comprobación previa de IDs duplicados y sin borrar
  ajustes existentes.
- Sustituido el registro separado de Server-Specific por un coordinador central
  en `VVUP.Base`; `VVUP.CustomItems` vuelve a aportar sus controles propios sin
  reasignar `DefinedSettings` durante su activación.
- `.subclases` muestra nombres y roles compatibles de todas las subclases
  registradas.
- Se conserva `.subclase` para consultar la clase propia y `.hab` para usar
  habilidades.
- Estas funciones no modifican `DefinedSettings` y no pueden provocar los
  duplicados de SecretAPI.

## 2026-09-24 — Auditoría general del modpack

- Corregido el registro SSSS de `VVUP.CustomItems` para que se ejecute al
  habilitar el plugin y para que `OnVerified` solo envíe los ajustes al
  jugador. Ya no se reasigna la colección global en cada conexión.
- Ajustado el registro para entregar únicamente entradas ausentes: SecretAPI
  concatena el setter de `DefinedSettings`, por lo que reasignar la colección
  completa provocaba duplicados aunque el contenido local estuviera filtrado.
- Retirada la copia de respaldo `.dll` de la carpeta activa
  `Plugins\7777`; se conserva fuera de la carpeta cargable.
- Confirmado que `Exiled.CustomItems`, `UncomplicatedCustomItems` y sus
  integraciones están desactivados; `VVUP.CustomItems` queda como propietario
  activo de los objetos personalizados.
- El log sigue mostrando warnings reflexivos de `Exiled.CustomRoles` con
  `SecretLabNAudio` y referencias serializadas antiguas de RemoteAdmin. No
  desactivé esos componentes porque no bloquean el arranque y no hay evidencia
  de que sean causados por el modpack activo.

## 2026-09-24 — Restauración urgente del HUD

- Retirados los placeholders `{tps}` y `{maxTps}` de las plantillas
  `player_hud` y `spectator_hud`: UltimateHUD 7.1.1 no los admite dentro de
  esas plantillas y podía dejar todo el HUD sin renderizar.
- Restauradas las plantillas que funcionaban correctamente, manteniendo el
  HUD normal y de espectador activos.
- El TPS independiente continúa desactivado hasta implementar una integración
  compatible con la API real de UltimateHUD.

## 2026-09-24 — Aumento de aparición de subclases

- Aumentada la probabilidad humana base de `20%` a `30%`, la de dos opciones
  de `35%` a `45%` y el techo humano de `70%` a `80%`.
- Añadida una escala específica para SCP: `55%` base, `70%` con dos opciones
  y hasta `92%` por pool. Esto corrige especialmente los roles SCP con una
  sola subclase compatible, que antes se quedaban limitados al 20%.
- Se mantienen los límites `MaxPerRound` y `MaxPerWave` de cada subclase para
  evitar que una variante única monopolice una ronda.

## 2026-09-24 — Estado del TPS del HUD

- Se mantiene desactivado el TPS permanente: UltimateHUD 7.1.1 no admite los
  placeholders necesarios dentro de `player_hud` y `spectator_hud`.
- No se añadió un placeholder no soportado, porque hacía que desapareciera
  todo el HUD. El HUD funcional se conserva sin TPS hasta disponer de una
  integración compatible con la API real del plugin.

## 2026-09-24 — Reparación de objetos personalizados y Demoledor

- Demoledor ahora recibe dos granadas HE, chaleco de combate, botiquín y una
  tarjeta de Chaos Insurgency; si se asigna manualmente a Fundación recibe una
  tarjeta MTF Operative.
- Al seleccionar un objeto personalizado se muestra brevemente su nombre y
  descripción mediante el objeto entrante del evento, evitando leer el slot
  anterior.
- Añadido el comando de Remote Admin `customitems` (alias `ci` e `items`) para
  listar ID, nombre y descripción. Requiere el permiso `vvup.customitems`.

## 2026-09-24 — Corrección crítica de excepciones de CustomItems y SSSS

- Desactivado el mensaje automático de selección de `Exiled.CustomItems` y de
  `CustomItemsAPI`; ambos podían ejecutarse aunque el sistema de configuración
  estuviera desactivado y provocar `NullReferenceException` en
  `CustomItem.ShowSelectedMessage`.
- Se conserva únicamente el mensaje de selección controlado por
  `VVUP.CustomItems`, con texto limitado y seguro para el HUD.
- `OnVerified` de VVUP ya no registra ajustes SSSS; solo sincroniza los ajustes
  ya registrados durante `OnEnabled`.
- El estado interno de registro SSSS se reinicia correctamente al desactivar el
  plugin para evitar estados inconsistentes durante una recarga.

## 2026-09-24 — Desactivación preventiva de SSSS duplicado

- Desactivado SSSS de `VVUP.CustomItems` en la configuración activa y por
  defecto en código: SecretAPI ya contiene ajustes con los IDs `10000–10006`
  y rechaza el registro de VVUP como duplicado durante `OnEnabled`.
- Los Custom Items permanecen activos; solo se desactivan los controles
  opcionales de C4/F4, ADATS y color Helios mediante Server-Specific Settings.
- Esta medida elimina el error crítico del arranque sin afectar la entrega ni
  el uso normal de los objetos personalizados.

## 2026-09-24 — CLASS SWAP de subclases

- Añadida una opción Server-Specific configurable por el jugador para cambiar
  de subclase con una tecla elegida por él (por defecto `J`).
- Solo está disponible para SCP excepto SCP-079 y SCP-049-2 durante los
  primeros 30 segundos de cada ronda.
- El límite se almacena por jugador y ronda: una muerte, respawn o cambio de
  vida no lo reinicia.
- El nuevo ID SSSS es `10007`, separado de los IDs conflictivos de
  `VVUP.CustomItems`, y el registro comprueba duplicados antes de asignarlo.

## 2026-09-24 — Restauración y orden de Server-Specific Settings

- Corregido el fallo que hacía desaparecer la mayoría de opciones: CLASS SWAP
  ya no reasigna `ServerSpecificSettingsSync.DefinedSettings`.
- La tecla de habilidad y CLASS SWAP se registran mediante `SettingBase.Register`,
  que concatena sin borrar ajustes de EXILED ni de otros plugins.
- Reactivada la tecla configurable de habilidades (`X` por defecto).
- Añadido un encabezado común y descriptivo para agrupar las opciones de
  subclases y facilitar su configuración por cada jugador.

## 2026-09-24 — Contención de duplicados SSSS de SubclassFramework

- Eliminado el registro SSSS de `SubclassFramework`: SecretAPI rechazaba los
  IDs `774202` y `10007` como duplicados durante `SettingBase.Register`.
- Desactivados temporalmente `use_server_specific_keybind` y
  `enable_class_swap` en la configuración activa.
- El comando de cliente `.hab` permanece disponible como método seguro para
  usar las habilidades sin tocar los ajustes globales.
- CLASS SWAP queda pausado hasta poder reutilizar el registro existente de
  SecretAPI sin volver a asignar ni duplicar `DefinedSettings`.

## 2026-09-24 — Corrección de Server-Specific Settings y mute

- Corregido el registro de SSSS de `VVUP.CustomItems`: ya no se reasigna la
  lista completa de ajustes cuando SecretAPI concatena los valores asignados a
  `DefinedSettings`, evitando duplicados de los IDs `10000+`.
- Protegido el registro frente a verificaciones simultáneas y hecho idempotente
  cuando varios jugadores entran a la vez.
- Añadida validación de IDs configurables: si hay una colisión interna, SSSS se
  desactiva explícitamente y se registra el error en lugar de dejar ajustes
  ambiguos.
- Creado el archivo vacío `config\7777\mutes.txt`, que LocalAdmin esperaba al
  arrancar y cuya ausencia generaba `Can't load the mute file!`.

## 2026-09-24 — Ajuste final del HUD

- Eliminado el ID numérico del HUD de jugador y del HUD de espectador.
- Reducidos los tamaños del reloj, tiempo de ronda, nombre, rol, bajas,
  munición y datos de espectador para evitar solapamientos y distracciones.
- Aplicada una paleta de colores suave para separar visualmente cada dato sin
  saturar la pantalla.
- Desactivado el TPS permanente: la versión instalada de UltimateHUD no ofrece
  un ajuste para vincularlo directamente a la tecla `N`; mantenerlo activo
  continuamente era más molesto que útil.
- Reducidos el banner introductorio de subclase y el HUD de habilidad para que
  no compitan con la interfaz nativa.

## 2026-09-24 — Revisión final de estabilidad

- Invalidada la asignación diferida de subclases al cambiar de ronda o
  recargar el framework, evitando que callbacks antiguos modifiquen una vida
  nueva.
- Limpiados los hints y broadcasts internos al desconectarse un jugador y al
  reiniciar la ronda para evitar referencias retenidas durante sesiones largas.
- Eliminado un `catch` global del bucle de actualización de subclases que podía
  ocultar fallos y dejar el estado sin actualizar silenciosamente.
- Hecha transaccional la activación de `SubclassFramework`: si falla una fase
  posterior, se desregistran los componentes ya inicializados.
- Corregida la documentación de transferencia para reflejar el multiplicador
  real de radiación (`3`) y las probabilidades efectivas (`20/35/70`).

## 2026-09-24 — Integración LabAPI de chat de proximidad

- Instalados `ScpProximityChat.Core` y `ScpProximityChat.SecretAPI` v1.1.1 en
  `LabAPI\plugins\7777`.
- Instalados `SecretLabNAudio.Core` v2.0.0, `SecretAPI` v3.4.0 y Harmony
  `2.2.2.0` en `LabAPI\dependencies\7777`.
- Retirado el DLL global activo de `UncomplicatedCustomItems-Exiled`; la copia
  idéntica de respaldo permanece en `LabAPI\plugins\disabled` para evitar
  duplicidad con `VVUP.CustomItems`.
- Pendiente de comprobar tras el siguiente arranque: YAML generado por
  ScpProximityChat y audio real dentro de una ronda.

## 2026-09-24 — Correcciones detectadas en el arranque real

- Restaurado `Exiled.CustomItems.dll` como dependencia de API de VVUP; el
  plugin de objetos de EXILED continúa deshabilitado mediante configuración.
- Cambiado el ID del keybind de SubclassFramework de `774201` a `774202` para
  eliminar la colisión reportada por SecretAPI.
- Desactivado el keybind Server-Specific de SubclassFramework en el servidor:
  SecretAPI/SSSS duplicaba el ajuste al combinarse con EXILED 9.14.2 incluso
  con un ID nuevo; el comando `.hab` sigue disponible.
- Validado mediante tres arranques reales con `LocalAdmin.exe`: el tercer
  arranque mantuvo `SCPSL.exe` operativo y cargó VVUP, Exiled.CustomItems,
  SubclassFramework y ScpProximityChat sin errores de dependencias.

## v1.0.0 — 24 de septiembre de 2026

### Lanzamiento estable

- El servidor deja la fase experimental y queda identificado como `v1.0.0`.
- Añadido `VERSION_SERVIDOR.txt` y documento de transferencia para futuras
  sesiones de mantenimiento.
- Desactivado el registrador duplicado de `Exiled.CustomItems`; VVUP pasa a ser
  el único propietario del registro de objetos personalizados.

### Estabilidad y código

- Cancelados callbacks diferidos de efectos visuales de VVUP.Base al cambiar de
  ronda o desactivar el plugin.
- Añadido control de ciclo de vida al puente de compatibilidad de subclases.
- Corregido el recuento de la granada de racimo para evitar una explosión extra.
- Compilados y desplegados VVUP.Base, VVUP.CustomItems y
  VVUP.SubclassCompatibility sin errores.

### Balance de lanzamiento

- SCP-457 preparado con valores menos extremos si se activa su módulo.
- Reducida la radiación contra SCP a multiplicador `2` cada `1,5` segundos.
- Aumentada la presencia de subclases con probabilidades globales `30/45/80`.
- Límite de SCP-207 de salud reducido a `120`.
- Se mantienen las reducciones anteriores del PB-42, agente nervioso,
  Pathfinder, EL-95 y SCP-2818.

## 2026-09-24 — Sesión de rebalance y mantenimiento

### Correcciones

- Corregido el cálculo del tiempo restante del enfriamiento del cuchillo.
- Evitada la doble suscripción de eventos si `VVUP.CustomItems` se reactiva sin
  una desactivación completa.
- Traducidos los mensajes predeterminados del cuchillo y del lanzagranadas.
- Traducidos los mensajes de accesorios de armas y recarga del ADATS.

### Rebalance

- Retirada la COM-18 de munición explosiva (ID `21`): su daño de área y
  capacidad de alcanzar a varios jugadores la hacían desproporcionada y además
  competía con Pathfinder por el mismo arma base.
- Eliminada también su clase y referencia de compilación para impedir que
  vuelva a registrarse accidentalmente.
- Reducido el PB-42 de cinco a tres unidades y su duración de veinte a doce
  segundos para que no bloquee puertas y luces durante demasiado tiempo.
- Reducida la duración del agente nervioso a `20` segundos y su veneno a `25`.
- Pathfinder pasa de un multiplicador de daño de `1,10` a `1,05`.
- Corregida la combinación de accesorios del EL-95 que provocaba controles
  laterales invertidos al apuntar; se mantiene la mira y se elimina el
  accesorio conflictivo.
- Reducida la penalización de movimiento al apuntar con el EL-95 para que sea
  más manejable sin convertirlo en un arma móvil.
- Ligero apoyo a los humanos mediante la armadura explosiva, SCP-207 de salud,
  el DEA y SCP-427, sin aumentar directamente la vida base de los roles.
- Reducido el daño del fusil EL-95 de `150` a `110`.
- Reducido el daño de SCP-2818 de `1000` a `350`.
- Reducido el multiplicador de radiación contra SCP de `25` a `3`.
- Ajustada la resistencia de SCP-106 a `0.25` y desactivados los ataques de
  ejecución instantánea de SCP-106 y SCP-049.
- Desactivada la furia ilimitada de SCP-096 durante la detonación de la ojiva.
- Reducida la curación de SCP-096 por baja a `50`.
- Ajustadas las probabilidades globales de subclases a `20/35/70` para evitar
  que las subclases aparezcan con demasiada frecuencia.
- Se mantiene el daño de MicroHID contra SCP-096 en `0.5`, una reducción
  moderada que evita que el arma lo elimine instantáneamente sin volverla
  inútil.
- Se mantiene la radiación posterior a la ojiva con daño bajo por ciclo y una
  espera de 300 segundos.
- C4 y F4 siguen desactivados hasta disponer de una implementación de colisión
  compatible con la versión actual de LabAPI.

### Compatibilidad

- Se conserva `VVUP.SubclassCompatibility` como puente único para entregar el
  ADATS al Demoledor y el EL-95 al Francotirador MTF.
- Se mantiene UCI fuera de las carpetas activas para evitar duplicidad de
  objetos, comandos y ajustes de servidor.

## 2026-09-24 — Integración y traducción

- Añadido el inventario completo en `README_SERVIDOR.md`.
- Separado el prefijo de `VVUP.SubclassCompatibility` para evitar la colisión
  con `VVUP.ScpChanges`.
- Añadida la traducción al castellano de los mensajes visibles principales.
- Añadidos controles de ciclo de vida seguros a `VVUP.CustomItems`.
- 2026-09-24 — Validación posterior a la restauración de Custom Items:
  - Reiniciado el servidor dedicado una sola vez después de restaurar
    `Exiled.CustomItems.dll` 9.14.2.0 como dependencia de runtime.
  - El proceso dedicado quedó operativo y único (`SCPSL.exe` del servidor,
    puerto 7777); no aparecen nuevas excepciones ni errores de dependencia en
    los logs de EXILED.
  - Confirmado que `VVUP.CI` permanece habilitado y
    `exiled_custom_items` permanece deshabilitado, evitando doble registro.
  - Corregida la configuración efectiva de RemoteKeycard:
    `Configs\Plugins\remote_keycard\7777.yml` queda deshabilitada. Se eliminó
    la carpeta redundante con mayúsculas que no consumía EXILED.
- 2026-09-24 — Corrección de diagnóstico SSSS:
  - `VVUP.CustomItems` ya no emite un `WARN` cuando `ssss_enabled: false`
    está desactivado intencionadamente para evitar conflictos de SecretAPI.
  - La desactivación intencionada se registra como `INFO`; una configuración
    realmente inválida se registra por separado como error y conserva la
    validación de IDs duplicados.
  - Compilado y desplegado `VVUP.CustomItems` tras el cambio, con 0 errores.
  - Reiniciado el servidor y confirmado que queda una única instancia
    dedicada activa; la búsqueda posterior no muestra errores de dependencia
    ni excepciones en los logs disponibles.
- 2026-09-24 — Preparación de versión estable:
  - Activado RemoteKeycard únicamente después de respaldar su DLL y
    configuración; se conserva el registro de rollback en
    `Plugins\backups\pre-remote-keycard-enable-20260924-2218`.
  - Corregidos mensajes restantes en inglés y errores gramaticales visibles de
    ADATS, Cuchillo, EL-95 y SCP-427 en la configuración activa de VVUP.CI.
  - Se mantiene UCI, Talky, UsefulHints, XP, EffectKeeper, ScpSwap,
    ScpsInfoDisplay, Sunrise y SCPUtils fuera de producción por conflictos o
    falta de validación contra las referencias actuales.
- 2026-09-24 — Validación final de RemoteKeycard:
  - La primera orden de compilación sin la propiedad de referencias del juego
    falló por referencias Unity/Mirror no resueltas; no se usó ese resultado
    para producción.
  - Recompilado con las referencias reales de
    `SubclassFramework\References`: 0 advertencias y 0 errores.
  - Desplegada la DLL resultante `RemoteKeycard-EXILED` 3.4.2 y reiniciado el
    servidor.
  - Confirmada una única pareja `LocalAdmin.exe`/`SCPSL.exe` dedicada y sin
    errores de dependencia, excepciones de carga o duplicados en los logs
    disponibles.
- 2026-09-24 — Revisión final de Custom Items:
  - Confirmados 32 IDs configurados, todos únicos, entre 20 y 60; se
    conservaron los huecos intencionados del catálogo.
  - `CustomItemsAPI` está desplegado y habilitado para mostrar tanto el nombre
    (`{0}`) como la descripción (`{1}`) al recoger y seleccionar objetos.
  - Corregido el texto de selección para distinguirlo de la recogida.
  - Corregida la cadena de descripción de la Linterna fantasma, que contenía
    comillas triples heredadas y podía afectar al parseo del YAML.
  - Reiniciado el servidor después de los cambios; quedó una única instancia
    dedicada activa y no se detectaron excepciones, dependencias ausentes ni
    errores de carga en los logs disponibles.
- 2026-09-24 — Corrección integral de Custom Items:
  - VVUP ahora muestra directamente los textos descriptivos al recoger y
    seleccionar sus objetos mediante los eventos de EXILED, sin depender de
    que CustomItemsAPI detecte internamente los objetos VVUP.
  - El comando `customitems`/`ci`/`items` admite `list`, `info`, `give`,
    `spawn` y `help`, con IDs numéricos o nombres, objetivos por ID/UserID o
    nombre y mensajes de error explícitos.
  - Añadidas opciones de configuración para activar/desactivar los hints y
    personalizar sus textos y duración.
  - Desactivados los hints automáticos de `CustomItemsAPI` para que no se
    dupliquen: la ruta de VVUP es ahora la única que muestra el nombre y la
    descripción.
  - Compilación de VVUP.CustomItems completada con 0 errores; las 16
    advertencias restantes son heredadas de APIs obsoletas/anotaciones
    nullable y no afectan a esta integración.
  - Reinicio final completado con una única instancia dedicada y sin errores
    de carga o excepciones detectables en los logs disponibles.

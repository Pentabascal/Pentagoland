# Transferencia del proyecto del servidor SCP: Secret Laboratory

## Identidad y objetivo final

Este proyecto es un servidor dedicado de **SCP: Secret Laboratory** para
Windows, instalado bajo:

`C:\Users\migue\AppData\Roaming\EXILED`

El objetivo actual es sacar el servidor de una fase experimental/alpha y
mantenerlo como **v1.0.0**, con una base estable, jugable y documentada. La
prioridad no es añadir contenido indiscriminadamente: cada cambio debe mejorar
la experiencia, evitar abusos, respetar la compatibilidad entre plugins y no
introducir errores en consola o en las rondas.

## Historial resumido de decisiones

1. Se intentaron objetos personalizados mediante UCI y configuraciones YAML,
   pero los comandos y armas no fueron fiables. UCI se conserva desactivado.
2. Se analizó una implementación externa llamada Big Gun/Grenade Launcher. Se
   probó una réplica independiente, pero el trabajo anterior de lanzagranadas y
   sniper se eliminó cuando se decidió adoptar el paquete Snivy.
3. Se integró `SnivysUltimatePackage-master`, adaptándolo a EXILED 9.14.2 y
   LabAPI. Los módulos de roles personalizados de Snivy se excluyeron para no
   reemplazar ni duplicar las aproximadamente 100 subclases ya existentes.
4. C4 y F4 se dejaron fuera porque el código antiguo dependía de miembros de
   colisión que ya no existen en la API actual.
5. Se creó `VVUP.SubclassCompatibility` para conectar Demoledor con ADATS y
   Francotirador MTF con EL-95, sin mezclar los prefijos de configuración de
   `VVUP.ScpChanges`.
6. Se corrigieron problemas de munición, recarga, detonación, cooldown del
   cuchillo, textos visibles y registros duplicados.
7. Se tradujo al castellano visible todo lo que no sea un identificador técnico,
   enum, permiso, ruta, nombre de API o nombre interno necesario para el juego.
8. El servidor debe mantenerse entretenido pero no basarse en objetos capaces de
   matar o controlar zonas sin contrajuego. La COM-18 explosiva es el ejemplo
   de contenido que debe retirarse aunque sea vistoso.

## Criterios permanentes de trabajo

- Leer e inspeccionar el código y la configuración antes de editar.
- Preferir cambios quirúrgicos, compatibles y reversibles.
- No reactivar plugins descartados solo para aprovechar contenido existente.
- No añadir subclases paralelas si ya existe una subclase equivalente.
- No solucionar un bug de gameplay con una duplicación de plugins.
- No ocultar excepciones ni usar capturas amplias que silencien fallos.
- Comprobar que los IDs personalizados no se dupliquen.
- Mantener mensajes visibles en castellano de España, usando `ñ` cuando
  corresponda y vocabulario natural.
- Dar pequeños apoyos a humanos mediante utilidad, curación limitada,
  información o movilidad, no acumulando vida, armadura, daño y munición.
- Todo objeto potente debe tener límite de spawn, duración, munición o
  contrajuego explícito.
- No declarar una actualización terminada hasta compilar, desplegar y revisar
  el arranque real.

## Entorno técnico

- Juego: SCP: Secret Laboratory `14.2.7`.
- EXILED: `9.14.2`.
- LabAPI incluido en el entorno de EXILED.
- .NET Framework objetivo: `4.8`.
- Puerto de juego: `7777`.
- Sistema operativo: Windows.
- El servidor debe iniciarse y reiniciarse desde LocalAdmin.
- No se deben iniciar instancias manuales adicionales de `SCPSL.exe`.
- Debe existir una sola instancia del proceso del servidor.

No se deben guardar contraseñas, tokens ni credenciales en este documento.

## Arquitectura activa

### Plugins desplegados en `Plugins\7777`

- `CustomItemsAPI.dll`.
- `SubclassFramework.dll`.
- `VVUP.Base.dll`.
- `VVUP.CustomEscapes.dll`.
- `VVUP.CustomItems.dll`.
- `VVUP.MicroDamageReduction.dll`.
- `VVUP.PostNukeEnhancements.dll`.
- `VVUP.RoundStart.dll`.
- `VVUP.ScpChanges.dll`.
- `VVUP.ServerEvents.dll`.
- `VVUP.SubclassCompatibility.dll`.
- `VVUP.Votes.dll`.
- `VVUP.WeaponEvaporate.dll`.

### Plugins LabAPI desplegados en el puerto 7777

- `ScpProximityChat.Core.dll` v1.1.1.
- `ScpProximityChat.SecretAPI.dll` v1.1.1.

Dependencias instaladas en `LabAPI\dependencies\7777`:

- `SecretLabNAudio.Core.dll` v2.0.0.
- `SecretAPI.dll` v3.4.0.
- `0Harmony.dll` v2.2.2.0.

La configuración de ScpProximityChat se dejará generar por LabAPI en el
próximo arranque; no se ha creado manualmente para evitar claves inventadas.

### Función de cada módulo

- **CustomItemsAPI**: API adicional para objetos personalizados.
- **SubclassFramework**: sistema de subclases, aproximadamente 101 subclases.
- **VVUP.Base**: núcleo compartido y efectos visuales de objetos.
- **VVUP.CustomItems**: armas, granadas, armaduras, utilidades y objetos
  médicos personalizados.
- **VVUP.CustomEscapes**: escapes configurables.
- **VVUP.MicroDamageReduction**: reduce el daño de MicroHID contra SCP
  concretos.
- **VVUP.PostNukeEnhancements**: radiación y efectos posteriores a la ojiva.
- **VVUP.RoundStart**: ajustes al comienzo de ronda.
- **VVUP.ScpChanges**: cambios de comportamiento de SCP.
- **VVUP.ServerEvents**: eventos especiales configurables.
- **VVUP.SubclassCompatibility**: entrega ADATS y EL-95 a las subclases
  correspondientes después de que SubclassFramework termine el inventario.
- **VVUP.Votes**: votaciones de mapa.
- **VVUP.WeaponEvaporate**: reglas de evaporación según tipo de daño.

## Estado de componentes y exclusiones deliberadas

### RemoteKeycard (activo)

RemoteKeycard 3.4.2 fue adaptado a EXILED 9.14.2 y se corrigió su ciclo de
vida para desregistrar los eventos en `OnDisabled`, no únicamente desde el
finalizador del objeto. La DLL está instalada en `Plugins\7777` y la
configuración efectiva `Configs\Plugins\remote_keycard\7777.yml` tiene
`is_enabled: true`. La copia de staging se conserva para rollback. La primera
validación de arranque con el plugin activo dejó una única instancia dedicada
operativa y sin errores de carga en los logs disponibles.

### KillMessage

KillMessage 1.0.8 está compilado contra EXILED 9.14.2 y .NET Framework 4.8.1.
Se corrigió el orden de desactivación para retirar eventos antes de cerrar
LiteDB, se hizo `Close` idempotente y se protegió el evento `Died` frente a
jugadores nulos. Está preparado en `Plugins\staging\KillMessage-1.0.8` con
`is_enabled: false`; no se activa junto con cambios de HUD sin una ronda
específica de validación.

### Uncomplicated Custom Items

UCI se conserva, pero está desactivado y fuera de la carpeta activa:

`C:\Users\migue\AppData\Roaming\EXILED\DisabledPlugins`

No se debe volver a activar mientras `VVUP.CustomItems` esté funcionando,
porque ambos sistemas pueden registrar objetos, IDs, comandos y ajustes
superpuestos.

El DLL global que se encontró en `LabAPI\plugins\global` fue retirado tras
compararlo byte a byte con la copia de `LabAPI\plugins\disabled`.

### Dependencia Exiled.CustomItems

`Exiled.CustomItems.dll` debe permanecer en la carpeta de plugins como
dependencia de runtime de VVUP:

`EXILED\Plugins\7777\Exiled.CustomItems.dll`

La configuración `EXILED\Configs\Plugins\exiled_custom_items\7777.yml` queda
con `is_enabled: false`, por lo que no se inicia como registrador independiente.
Solo `VVUP.CustomItems` debe ser propietario del registro activo de objetos
personalizados. La copia de `DisabledPlugins` se conserva como respaldo.

### Módulos Snivy no desplegados

Se mantienen fuera de las carpetas activas para evitar contenido duplicado o
roles paralelos:

- `VVUP.CustomRoles`.
- `VVUP.FreeCustomRoles`.
- `VVUP.HuskInfection`.
- `VVUP.CreditTags`.
- `VVUP.OperationCrossfireServerEvent`.

C4 y F4 siguen sin registrarse porque su antiguo hook de colisión no es
compatible con la API física actual. No deben activarse parcialmente.

## Objetos personalizados activos

IDs principales:

- `20`: Obscurus Veil-5, humo.
- `22`: granada de agente nervioso.
- `23`: dispositivo señuelo fantasma.
- `24`: linterna fantasma.
- `25`: armadura resistente a explosivos.
- `26`: LJ-429.
- `27`: Pulso fantasma.
- `28`: Serenata silenciosa.
- `29`: SCP-1499.
- `30`: PB-42.
- `31`: Amnesioflux.
- `33`: SCP-2818.
- `34`: Píldoras infinitas.
- `36`: granada de racimo.
- `37`: SCP-207 de salud adicional.
- `38`: Coraza Nebulosa.
- `39`: Viper.
- `40`: Pathfinder.
- `41`: Rayo Helios X-57.
- `42`: Echo Flare.
- `43`: granada Pathfinder.
- `46`: lanzagranadas ADATS.
- `47`: intercomunicador portátil.
- `50`: Televarita.
- `51`: Lunar Lob.
- `53`: botella de ron.
- `55`: granada Guardián.
- `56`: desfibrilador externo automático.
- `57`: cuchillo.
- `58`: fusil EL-95 de cerrojo.
- `59`: SCP-427.
- `60`: estimulante experimental.

La antigua COM-18 de munición explosiva, ID `21`, fue eliminada por ser
claramente desproporcionada: causaba explosiones de área desde un arma,
permitía golpear a varios jugadores y competía con Pathfinder usando la misma
arma base.

## Integración de subclases y armas

- `demoledor` recibe el ADATS, ID `46`.
- `francotirador_mtf` recibe el EL-95, ID `58`.
- El puente espera al inventario de SubclassFramework y elimina las armas
  vanilla conflictivas antes de entregar el objeto personalizado.
- El puente usa el prefijo `VVUP.SubclassCompatibility`; no debe cambiarse al
  prefijo `VVUP.SC`, que pertenece a VVUP.ScpChanges.
- El callback diferido del puente tiene ahora un token de ciclo de vida para no
  aplicar inventarios después de desactivar el plugin.

## Balance vigente de v1.0.0

### Armas y objetos

- EL-95: daño `110`, cargador de `1`.
- Se eliminó el accesorio `ExtendedStock` del EL-95 por el problema de controles
  laterales invertidos al apuntar. Se conserva la mira `ScopeSight`.
- La ralentización al apuntar con EL-95 es leve, intensidad `20`.
- SCP-2818: daño `350`, sigue siendo un arma de riesgo porque puede matar al
  tirador si falla.
- Pathfinder: multiplicador de daño marcado `1.05`.
- PB-42: duración `12` segundos y máximo `3` unidades.
- Granada de agente nervioso: duración `20` segundos y veneno `25` segundos.
- SCP-207 de salud: límite máximo del usuario `120`.
- Armadura explosiva: reducción multiplicativa `0.55`.
- DEA: revive con `55` de vida.
- SCP-427: regeneración `0.3` por segundo y AHP limitado por configuración.
- Granada de racimo: el bucle fue corregido para crear exactamente el número
  configurado, sin una explosión secundaria adicional.

### SCP, radiación y subclases

- SCP-106: resistencia `0.25`, sin ejecución instantánea.
- SCP-049: sin ejecución instantánea.
- SCP-096: sin furia ilimitada durante la ojiva.
- Curación de SCP-096 por baja: `50`.
- Radiación contra SCP: multiplicador `3`, intervalo `1.5` segundos y retraso
  de `300` segundos.
- Probabilidades globales de subclase: `30/45/80` para humanos y `55/70/92`
  para SCP, con aumento por opción `3`. Esto mantiene las subclases especiales
  sin garantizarlas en todos los jugadores.
- SCP-457, si su módulo se activa en el futuro, tiene valores preparados más
  razonables: `590` de vida, `18` de contacto, `5` de aura y multiplicador de
  escalado `1.5`. Actualmente no se debe asumir que su DLL esté activa solo
  porque exista el YAML.

## Correcciones técnicas realizadas

- Corregido el cálculo del cooldown restante del cuchillo.
- Añadida protección contra doble registro/reactivación de VVUP.CustomItems.
- Eliminada la clase y la referencia de compilación de la COM-18 explosiva.
- Corregida la condición del bucle de granada de racimo.
- VVUP.Base cancela el callback diferido de luces al comenzar una nueva ronda o
  al desactivarse, evitando que una recarga cree luces con el plugin apagado.
- VVUP.Base limpia los efectos visuales durante la desactivación.
- El puente de subclases evita callbacks antiguos después de una recarga.
- Se conservan C4/F4 en código, pero sin registro ni spawn hasta migrar la
  colisión.

## Comandos y permisos conocidos

Subclasses:

```text
subclass list
subclass spawn <id-jugador> <id-subclase>
scspawn <id-jugador> <id-subclase>
.hab
.habilidad
.subclase
.miclase
```

Permiso administrativo principal:

```text
subclass.spawn
```

El comando de módulos activos de VVUP se encuentra en VVUP.Base; debe usarse
para comprobar qué módulos han comunicado correctamente su estado. Los nombres
de IDs, prefijos, enums y permisos no deben traducirse porque forman parte de
la API o del sistema de comandos.

## Rutas importantes

- Plugins activos: `EXILED\Plugins\7777\`.
- Configuración: `EXILED\Configs\Plugins\`.
- Fuentes Snivy: `EXILED\zCreacionPlugins\SnivysUltimatePackage-master\`.
- Fuentes SubclassFramework:
  `EXILED\zCreacionPlugins\SUBCLASES\SubclassFramework\`.
- Plugins desactivados:
  `EXILED\DisabledPlugins\`.
- Versión del servidor: `EXILED\Plugins\VERSION_SERVIDOR.txt`.
- Documentación pública:
  `EXILED\Plugins\README_SERVIDOR.md`.
- Changelog:
  `EXILED\Plugins\CHANGELOG_SERVIDOR.md`.

## Proceso de build y despliegue

Los proyectos apuntan a .NET Framework 4.8 y EXILED 9.14.2. Los builds
individuales que sí tienen todas sus referencias locales deben ejecutarse con:

```powershell
dotnet build <ruta-del-proyecto>.csproj --configuration Release --no-restore
```

El build completo de la solución puede fallar si se incluyen proyectos
desactivados que no tienen instalados los ensamblados de referencia de .NET
Framework 4.8. Eso no implica que los proyectos activos hayan fallado; se
deben compilar individualmente y comprobar `0 Errores`.

Después de compilar, copiar solo las DLL activas a `Plugins\7777` y reiniciar
una única vez mediante LocalAdmin. No reemplazar DLL mientras el servidor las
tenga cargadas si el sistema mantiene archivos bloqueados.

## Comprobaciones obligatorias antes de declarar una versión estable

1. Confirmar que solo exista un proceso `SCPSL.exe`.
2. Revisar el arranque buscando excepciones, prefijos duplicados y errores de
   registro de objetos.
3. Confirmar que `Exiled.CustomItems.dll` esté en la carpeta de plugins como
   dependencia y que `exiled_custom_items/7777.yml` permanezca desactivado.
4. Probar `subclass list`, el spawn de Demoledor y el spawn de
   Francotirador MTF.
5. Verificar ADATS con un solo proyectil, recarga y detonación.
6. Verificar EL-95, movimiento lateral normal y recarga.
7. Probar Pathfinder, PB-42, granada nerviosa y granada de racimo.
8. Probar DEA, SCP-207, armadura explosiva y SCP-427.
9. Confirmar que C4/F4 no aparecen.
10. Revisar el log completo de una ronda y eliminar cualquier error nuevo
    antes de añadir más contenido.

## Mejoras futuras razonables

- Migrar C4/F4 a la API de colisión actual con pruebas específicas, sin usar
  Harmony o hooks antiguos.
- Añadir una tarjeta de ingeniero solo si tiene spawn limitado a una unidad y
  accesos cuidadosamente definidos.
- Mejorar subclases humanas débiles mediante información, radio, apoyo médico o
  movilidad, no acumulando vida, armadura y armas simultáneamente.
- Añadir pruebas automáticas de conteo de objetos, IDs duplicados y registro de
  plugins.
- Mantener el idioma visible en castellano de España y revisar textos nuevos.
- No activar módulos no desplegados sin auditar sus nulos, callbacks diferidos,
  ciclo de vida y duplicidades.

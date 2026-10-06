# Pentagoland Web

Web de presentación del proyecto Pentagoland para SCP: Secret Laboratory. Está hecha con HTML, CSS y JavaScript sin framework ni proceso de compilación, para que sea fácil de abrir, editar y subir a un hosting estático.

## Abrir la web

Abre `index.html` en un navegador moderno. No hace falta instalar dependencias ni arrancar un servidor.

## Qué incluye

- Navegación adaptable a móvil, teclado y ratón
- Enlace para saltar al contenido principal y facilitar la navegación con teclado
- Guía de entrada, principios de comunidad y una página de normas con una propuesta claramente marcada como borrador
- Catálogo filtrable de plugins, con versiones del último arranque comprobado y estados activo/desactivado/pendiente
- Mejoras previstas sin fechas ni promesas inventadas
- Preguntas frecuentes desplegables
- Estado del servidor con diseño inicial sin datos ficticios y publicación de solo el contador
- Vista “Servidor” dentro del panel, con estado, jugadores y hora de actualización
- Panel de jugador independiente, con vistas de estadísticas, equipo y perfil
- Vistas enlazables con fragmentos (`panel/index.html#stats`, `#server`, `#account`, etc.) y accesos al sitio principal desde cada sección
- Stat tracker preparado para datos reales cuando exista una API autenticada; todavía no lee datos del servidor
- Desglose de kills por Caos, MTF, SCP, Guardia, Clase D y Científico
- Apartado de cuenta futura con verificación Steam prevista y vinculación Discord opcional; sin credenciales ni autenticación simulada
- Apartado de normas en `normas.html`, con propuesta para revisión y aprobación antes de aplicarse
- Plantilla del servidor Discord en `../Discord/servidor-template.md`; no se ha configurado una cuenta, invitación, bot ni webhook
- Banner proporcionado por el propietario en `assets/pentagoland-banner.jpg`, integrado en la portada sin modificar el original
- Ilustraciones SVG originales locales, sin hotlinks a imágenes de terceros
- Galería de ilustraciones locales; se sustituirán o ampliarán con capturas propias cuando estén disponibles
- Navegación simplificada con indicador de sección actual
- Animaciones discretas con alternativa para usuarios que prefieren movimiento reducido
- Formulario de contacto de muestra con validación HTML
- Diseño adaptable, foco visible y respeto a la preferencia de movimiento reducido
- Metadatos básicos para compartir el sitio y favicon SVG
- Sin analítica, backend, cookies ni dependencias de fuentes externas

## Contacto: estado actual

El formulario **no envía ni almacena información**. Comprueba los campos obligatorios en el navegador y muestra un aviso de que todavía no tiene un destino. Para ponerlo en producción, hay que conectarlo a un servicio de formularios o a un backend propio y actualizar el aviso y la política de privacidad. No se debe presentar como funcional hasta completar esa conexión.

## Panel y cuentas

El panel se abre desde `panel/index.html`. Permite introducir un SteamID64 de 17 cifras (formato `7656119…`) y recordarlo en `localStorage` para volver a tenerlo disponible en el mismo navegador, dispositivo y origen del sitio. El usuario puede cambiarlo o quitarlo. El panel también enlaza al perfil público de Steam correspondiente. Si se abre como archivo local (`file://`), en modo privado o con el almacenamiento del sitio bloqueado, el navegador puede no conservarlo.

Esto **no es un inicio de sesión ni una verificación de identidad**: cualquiera puede escribir un SteamID ajeno. El identificador solo se guarda localmente y no se envía a Pentagoland. No carga estadísticas. Steam no ofrece desde un sitio estático una forma segura de obtener estadísticas propias del servidor; para mostrarlas, el servidor debe registrar las partidas y exponerlas mediante una API con autorización adecuada. No se debe confiar en el SteamID enviado por el cliente para proteger cuentas o estadísticas privadas, ni guardar claves/API secrets en JavaScript.

La autenticación real con Steam requiere verificar Steam OpenID en un backend y emitir una sesión segura. Los perfiles, imágenes y estadísticas compartidas también necesitan persistencia, validación y controles de privacidad en servidor. El borrador del perfil actual puede guardarse localmente en el navegador, pero no se publica ni sincroniza.

### Estadísticas y cuentas futuras

Los indicadores del tracker están vacíos hasta que exista telemetría real y una API autorizada. `StatsSystem` y `MvpSystem` están instalados en el servidor local, pero el panel no lee sus datos. KDR solo se calculará con kills y muertes verificadas; rondas, victorias, rachas y desglose completo de armas/roles requieren registro explícito y no deben fingirse. El tiempo jugado debe acumular sesiones reales del servidor, no el tiempo que la web permanezca abierta.

La pantalla de cuenta solo presenta opciones futuras desactivadas. No pide correo, contraseñas ni tokens y no crea cuentas locales inseguras. La asociación de estadísticas requerirá verificar Steam OpenID en un backend; Discord OAuth podrá añadirse como vínculo opcional, con scopes mínimos, consentimiento y opción de desvincular. Discord por sí solo no demuestra la titularidad de una cuenta Steam. La API privada no debe confiar en un SteamID escrito por el cliente.

Los datos aún no disponibles se muestran con guiones o como pendientes: no se presentan nombres de staff, estados de actividad, estadísticas, rangos ni normas como si fueran reales.

## Normas

`normas.html` contiene una propuesta de convivencia permisiva, no vigente hasta que el propietario la apruebe. El procedimiento de apelación, contacto, sanciones y aplicación en el servidor de juego quedan pendientes. La propuesta resume, pero no sustituye, las directrices oficiales de Northwood y Discord.

El sitio y la plantilla de Discord identifican Pentagoland como comunidad independiente, sin afirmar afiliación con Northwood Studios o SCP Wiki. Las reglas del juego deben incorporarse a Server Info de acuerdo con la versión vigente de las directrices de Northwood.

## Plugins y Discord

`plugins.html` refleja las cargas confirmadas por el log del servidor del 29/09/2026 y la configuración revisada. Se actualizará al comprobar un nuevo arranque. SCPDiscord aparece como pendiente: no está habilitado ni se presenta como compatible.

`../Discord/servidor-template.md` contiene la estructura propuesta de categorías, canales, roles, permisos mínimos, mensajes y pruebas. La configuración real requiere que el propietario aplique los cambios desde Discord y confirme la invitación, el contacto y las políticas de privacidad. La plantilla recomienda empezar por AutoMod y onboarding nativos; no requiere instalar bots de terceros.

## Antes de publicar

Revisa y sustituye la información pendiente directamente en `index.html`:

- dirección y versión del servidor
- capacidad real del servidor y origen fiable para su estado
- invitación de Discord
- canales oficiales de contacto
- aprobación de las normas, sanciones y proceso de apelación
- estado real de apertura y plugins probados
- endpoint HTTPS y política de privacidad antes de habilitar integraciones o estadísticas

No se muestran una IP ni enlaces inventados; cuando los datos oficiales existan, pueden añadirse en las secciones ya preparadas.

## Estado del servidor e integración

La portada y el panel comparten `server-status.js`. Sin endpoint configurado, no hacen solicitudes de red ni muestran cifras inventadas. Para activar la consulta, configura `data-server-status-endpoint` en la etiqueta `<html>` de `index.html` y `panel/index.html`. El endpoint debe ser de solo lectura y devolver JSON reciente con este contrato público:

```json
{
  "schemaVersion": 1,
  "serverName": "Pentagoland",
  "online": true,
  "onlinePlayers": 0,
  "maxPlayers": 20,
  "updatedAt": "2026-09-29T13:00:00.000Z"
}
```

El ejemplo describe el formato, no el estado real del servidor. `online` debe ser booleano; `onlinePlayers`, un entero entre 0 y `maxPlayers`; `maxPlayers`, un entero entre 1 y 200; y `updatedAt`, una fecha ISO generada por la fuente. Si el servidor está desconectado, `onlinePlayers` debe ser 0. Se acepta también el casing PascalCase emitido por .NET. Los timestamps ausentes, inválidos, futuros o con más de 120 segundos se rechazan o se muestran como desactualizados, nunca se sustituyen por la hora del navegador.

El navegador publica únicamente el estado, el contador y la capacidad: no muestra nombres, UserId/SteamID ni estadísticas. La integración debe generar esos datos en el servidor y exponer solo este DTO; **no** publiques `estadisticas.json`, snapshots con datos de jugadores, claves ni configuración interna.

El sondeo se realiza cada 30 segundos por defecto. `data-server-status-refresh-seconds` permite ajustar el intervalo (15–300) y `data-server-status-max-age-seconds` la antigüedad máxima (15–3600). Si el endpoint está en otro origen, necesita HTTPS y CORS limitado al origen de esta web. Es preferible publicar la ruta mediante un proxy inverso del servidor; no abras un puerto de administración ni incluyas credenciales en JavaScript.

Las integraciones también pueden actualizar la interfaz desde código confiable con `window.PentagolandServerStatus.update(snapshot)` en la portada o `window.PentagolandPanelServerStatus.update(snapshot)` en el panel. Ambos exponen `refresh()` para solicitar el endpoint configurado. No se debe invocar `update` con datos de ejemplo en producción.

## Estructura

- `index.html`: portada, navegación y estructura semántica
- `normas.html` / `normas.css`: apartado separado de normativa
- `plugins.html` / `plugins.css` / `plugins.js`: catálogo con filtros y estado verificado en el último arranque revisado
- `styles.css`: tema, componentes y breakpoints responsive
- `script.js`: menú móvil, animaciones discretas y estado explícito del formulario de muestra
- `server-status.js`: validación compartida, sondeo configurable y renderizado del estado para portada y panel
- `panel/index.html`, `panel/panel.css`, `panel/panel.js`: panel de jugador, navegación y borrador local de perfil
- `../Discord/servidor-template.md`: plantilla manual de Discord; no contiene secretos ni representa una configuración ya aplicada
- `assets/`: marca, favicon e ilustraciones SVG originales

La galería usa ilustraciones SVG originales, no capturas del juego. Añade aquí imágenes propias del servidor cuando estén disponibles y actualiza los textos alternativos y pies de foto; no las presentes como partidas reales si son material promocional o ilustraciones.

## Personalización y ampliaciones

Para cambiar textos o añadir una sección, edita `index.html` y enlaza la nueva sección desde la navegación. Los estilos compartidos viven en `styles.css`; las interacciones, en `script.js`.

Ideas de secciones futuras:

- autenticación real con Steam mediante backend seguro (el SteamID local actual no autentica)
- conexión de estadísticas a los eventos registrados por el servidor y definición/versionado del esquema de datos
- cuentas verificadas con Google u otro proveedor y futura integración con el foro
- perfiles públicos y directorio real del equipo con consentimiento
- publicación de las normas revisadas y proceso de apelación
- Discord y canales oficiales
- listado de plugins probados, con versiones y notas de compatibilidad
- novedades o changelog del servidor
- página completa de normas y apelaciones
- contenido multimedia del servidor, cuando haya material propio listo para publicar
- conectar `data-server-status-endpoint` a un proxy/backend real que publique el contrato de estado seguro
- formulario conectado a un destino con protección contra spam y aviso de privacidad

## Publicación

Como es un sitio estático, puede publicarse en cualquier proveedor de páginas estáticas que permita servir HTML, CSS, JavaScript y SVG. Usa HTTPS en producción y comprueba los enlaces y la experiencia en móvil antes de anunciarlo.

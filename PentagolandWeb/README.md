# Pentagoland Web

Web de presentación del proyecto Pentagoland para SCP: Secret Laboratory. Está hecha con HTML, CSS y JavaScript sin framework ni proceso de compilación, para que sea fácil de abrir, editar y subir a un hosting estático.

## Abrir la web

Abre `index.html` en un navegador moderno. No hace falta instalar dependencias ni arrancar un servidor.

## Qué incluye

- Navegación adaptable a móvil, teclado y ratón
- Enlace para saltar al contenido principal y facilitar la navegación con teclado
- Guía de entrada, principios de comunidad y una página propia para las normas oficiales
- Mejoras previstas sin fechas ni promesas inventadas
- Preguntas frecuentes desplegables
- Sección “Estado Servidores” con vista previa, contador y lista de jugadores
- Vista “Servidor” dentro del panel, con estado, jugadores y hora de actualización
- Panel de jugador independiente, con vistas de estadísticas, equipo y perfil
- Vistas enlazables con fragmentos (`panel/index.html#stats`, `#server`, `#account`, etc.) y accesos al sitio principal desde cada sección
- Stat tracker preparado para kills, muertes, KDR, horas jugadas, SCPs asesinados, rondas y victorias
- Desglose de kills por Caos, MTF, SCP, Guardia, Clase D y Científico
- Apartado de cuenta planificado para acceso Google/correo y futuras funciones de comunidad; sin credenciales ni autenticación simulada
- Apartado de normas separado en `normas.html`, listo para el texto oficial
- Accesos rápidos a cada categoría de normas, todavía marcada como pendiente
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

Los indicadores del tracker están vacíos hasta que exista telemetría real del servidor. KDR se plantea como kills totales divididas por muertes totales; “SCPs asesinados” contará objetivos SCP eliminados por el jugador, mientras que “Kills como SCP” cuenta eliminaciones realizadas desde el rol SCP. El desglose por equipo usa el rol del atacante al registrar cada eliminación. El tiempo jugado debe acumular sesiones reales del servidor, no el tiempo que la web permanezca abierta.

La pantalla de cuenta solo presenta opciones futuras desactivadas. No pide correo, contraseñas ni tokens y no crea cuentas locales inseguras. Para habilitar Google se necesita un flujo OAuth/OIDC verificado en backend; para correo/contraseña, almacenamiento de contraseñas con hash resistente, verificación de correo, recuperación segura, límites contra abuso, gestión de sesión y políticas de privacidad. El acceso a las estadísticas privadas debe autorizarse en el servidor y no confiar en un SteamID escrito por el usuario. Un futuro foro podrá reutilizar esa cuenta mediante una sesión compartida segura.

Los datos aún no disponibles se muestran con guiones o como pendientes: no se presentan nombres de staff, estados de actividad, estadísticas, rangos ni normas como si fueran reales.

## Normas

`normas.html` es el apartado separado para las reglas oficiales. Hasta que se publiquen y aprueben, la página muestra su estado pendiente y no inventa sanciones ni un proceso de apelación.

## Antes de publicar

Revisa y sustituye la información pendiente directamente en `index.html`:

- dirección y versión del servidor
- capacidad real del servidor y origen fiable para su estado
- invitación de Discord
- canales oficiales de contacto
- normas completas, sanciones y proceso de apelación
- estado real de apertura y plugins probados

No se muestran una IP ni enlaces inventados; cuando los datos oficiales existan, pueden añadirse en las secciones ya preparadas.

## Estado Servidores: integración futura

La sección es una vista previa y no consulta el juego ni muestra jugadores reales. Cuando exista un servicio de estado fiable, una integración puede actualizarla cargando `script.js` y llamando a la API pública de la página:

```js
window.PentagolandServerStatus.update({
  online: true,
  maxPlayers: 30,
  players: ["Jugador de ejemplo"],
  updatedAt: new Date().toISOString()
});
```

`online` debe ser booleano, `maxPlayers` un entero de 1 a 200, `players` una lista de nombres de 1 a 32 caracteres, y la cantidad de nombres no puede superar la capacidad. Si `online` es `false`, la lista debe estar vacía. `updatedAt` es opcional y acepta una fecha válida. Los nombres se insertan como texto, no como HTML.

La página no incluye un endpoint, polling ni conexión desde el navegador. Para producción, conecta esta API desde el código de la web a un endpoint público de solo lectura servido por un backend/proxy controlado por el servidor; no publiques secretos del servidor o claves en JavaScript. Confirma que los nombres de jugadores pueden mostrarse públicamente y ofrece una alternativa de solo contador si la privacidad o las reglas del juego lo requieren. Muestra una hora de actualización real y evita presentar datos antiguos como actuales.

La vista equivalente del panel expone `window.PentagolandPanelServerStatus.update(snapshot)` con el mismo formato y validaciones. Debe recibir el mismo snapshot desde una fuente fiable; no llames a la función con datos de ejemplo en producción. La sección indica “Estado no disponible” y no inventa capacidad ni jugadores hasta que se actualice:

```js
window.PentagolandPanelServerStatus.update({
  online: true,
  maxPlayers: 30,
  players: ["Nombre visible"],
  updatedAt: new Date().toISOString()
});
```

El panel valida tipos, capacidad y nombres, y crea las filas con `textContent`. La página estática no consulta el plugin ni expone una conexión por sí sola: una integración futura debe obtener el snapshot de un endpoint propio, con acceso de solo lectura y datos recientes.

## Estructura

- `index.html`: portada, navegación y estructura semántica
- `normas.html` / `normas.css`: apartado separado de normativa
- `styles.css`: tema, componentes y breakpoints responsive
- `script.js`: menú móvil, animaciones discretas, estado del formulario de muestra y API de actualización futura del estado
- `panel/index.html`, `panel/panel.css`, `panel/panel.js`: panel de jugador, estado futuro, navegación y borrador local de perfil
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
- conexión real de “Estado Servidores” a una fuente de datos fiable
- formulario conectado a un destino con protección contra spam y aviso de privacidad

## Publicación

Como es un sitio estático, puede publicarse en cualquier proveedor de páginas estáticas que permita servir HTML, CSS, JavaScript y SVG. Usa HTTPS en producción y comprueba los enlaces y la experiencia en móvil antes de anunciarlo.

# Pentagoland Web

Web de presentación del proyecto Pentagoland para SCP: Secret Laboratory. Está hecha con HTML, CSS y JavaScript sin framework ni proceso de compilación, para que sea fácil de abrir, editar y subir a un hosting estático.

## Abrir la web

Abre `index.html` en un navegador moderno. No hace falta instalar dependencias ni arrancar un servidor.

## Qué incluye

- Navegación adaptable a móvil, teclado y ratón
- Principios de comunidad, guía de entrada y normas básicas
- Mejoras previstas sin fechas ni promesas inventadas
- Preguntas frecuentes desplegables
- Sección “Estado Servidores” con vista previa, contador y lista de jugadores
- Ilustraciones SVG originales locales, sin hotlinks a imágenes de terceros
- Tarjeta editorial para un futuro vídeo, marcada como próxima en lugar de simular reproducción
- Navegación simplificada con indicador de sección actual
- Animaciones discretas con alternativa para usuarios que prefieren movimiento reducido
- Formulario de contacto de muestra con validación HTML
- Diseño adaptable, foco visible y respeto a la preferencia de movimiento reducido
- Metadatos básicos para compartir el sitio y favicon SVG
- Sin analítica, backend, cookies ni dependencias de fuentes externas

## Contacto: estado actual

El formulario **no envía ni almacena información**. Comprueba los campos obligatorios en el navegador y muestra un aviso de que todavía no tiene un destino. Para ponerlo en producción, hay que conectarlo a un servicio de formularios o a un backend propio y actualizar el aviso y la política de privacidad. No se debe presentar como funcional hasta completar esa conexión.

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

## Estructura

- `index.html`: contenido, navegación y estructura semántica
- `styles.css`: tema, componentes y breakpoints responsive
- `script.js`: menú móvil, animaciones discretas, estado del formulario de muestra y API de actualización futura del estado
- `assets/`: marca, favicon e ilustraciones SVG originales

## Personalización y ampliaciones

Para cambiar textos o añadir una sección, edita `index.html` y enlaza la nueva sección desde la navegación. Los estilos compartidos viven en `styles.css`; las interacciones, en `script.js`.

Ideas de secciones futuras:

- Discord y canales oficiales
- listado de plugins probados, con versiones y notas de compatibilidad
- novedades o changelog del servidor
- página completa de normas y apelaciones
- tráiler real para sustituir la tarjeta editorial de “Vídeo próximamente”
- conexión real de “Estado Servidores” a una fuente de datos fiable
- formulario conectado a un destino con protección contra spam y aviso de privacidad

## Publicación

Como es un sitio estático, puede publicarse en cualquier proveedor de páginas estáticas que permita servir HTML, CSS, JavaScript y SVG. Usa HTTPS en producción y comprueba los enlaces y la experiencia en móvil antes de anunciarlo.

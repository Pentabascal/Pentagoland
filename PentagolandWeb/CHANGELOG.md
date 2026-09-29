# Changelog

## [1.6.0] - 2026-09-29

### Added
- Vista de estado del servidor dentro del panel, con capacidad, lista de jugadores y última actualización sin inventar valores iniciales.
- API `window.PentagolandPanelServerStatus.update(snapshot)` para una futura integración de estado en el panel.
- Accesos persistentes entre vistas del panel y enlaces al resto de la web desde su navegación y pie.
- Galería de ilustraciones locales, claramente identificadas como ambientación y no como capturas de juego.
- Vistas del panel enlazables directamente con fragmentos como `panel/index.html#stats` y `#server`.

### Changed
- Aclaradas las ilustraciones de la portada y eliminadas explicaciones repetidas del panel.
- Ajustados los acentos de las ilustraciones a la paleta negra, rosa, morada y azul claro.
- Cambiada la etiqueta de estado de la portada para no sugerir que los datos ya son en directo.
- Añadido un atajo de teclado para saltar al contenido principal de la web.
- Reparada la regla de movimiento reducido de la hoja principal.

## [1.5.0] - 2026-09-29

### Added
- Ampliado el Stat tracker con KDR total, horas jugadas, SCPs asesinados, rondas, victorias y racha.
- Añadido un desglose de eliminaciones por Caos, MTF, SCP, Guardia, Clase D y Científico.
- Añadida una vista de cuenta futura con opciones desactivadas de Google y correo, sin recoger credenciales ni simular autenticación.

### Changed
- Mejorados textos de portada para distinguir las funciones preparadas de las que aún no están activas.
- Añadido un enlace accesible para saltar directamente al contenido de la portada.
- Documentadas las definiciones previstas de KDR, rol atacante, tiempo jugado y los requisitos de seguridad para cuentas y foro.

## [1.4.0] - 2026-09-29

### Changed
- Actualizado el texto principal a “SCP: Secret Laboratory · Pentagoland” y una presentación más directa del servidor hispanohablante.
- Ajustado el titular de portada a “SCP:SL en español. Esto es Pentagoland.”, sin una afirmación promocional no verificable.
- Sustituida la antigua sección narrativa por un enlace sencillo al panel de jugador.
- Cambiada la identidad visual a negro con acentos rosa claro, morado y azul claro.
- Retirados los estilos y animaciones de la antigua tarjeta de vídeo, ya eliminada de la portada.

### Added
- Panel separado con vistas de Stat tracker, equipo y perfil de jugador.
- Campo para guardar, recordar, cambiar y quitar un SteamID64 en el navegador actual.
- Enlace directo al perfil público correspondiente de Steam; el panel aclara que el ID no verifica identidad ni carga estadísticas.
- Vista previa local y editable del perfil, guardada solo en el navegador.
- La autenticación real, la API de estadísticas y la persistencia entre dispositivos quedan pendientes de backend.
- Página independiente de normas que deja claro que las reglas definitivas aún no están publicadas.
- Navegación rápida entre las categorías previstas para las normas.

## [1.3.0] - 2026-09-29

### Changed
- Eliminadas por completo las secciones “Proyecto” y “Compatibilidad” y sus enlaces de navegación.
- Menú reorganizado alrededor de servidores, guía, normas, novedades, comunidad y contacto.
- La navegación indica la sección visible y conserva los controles móviles existentes.

### Added
- Ilustraciones SVG originales para portada y vista editorial del futuro tráiler.
- Microanimaciones ambientales y detalles visuales, respetando `prefers-reduced-motion`.
- Tarjeta para el futuro vídeo claramente marcada como próximo, sin simular una reproducción funcional.

## [1.2.0] - 2026-09-29

### Added
- Enlace de navegación y panel dedicado a “Estado Servidores”.
- Vista previa con contador, estado, lista vacía y aviso explícito de que no son datos reales.
- API `window.PentagolandServerStatus.update()` para una futura integración de lectura, con validación y renderizado seguro de nombres.
- Documentación del formato de datos y consideraciones de privacidad/seguridad.

### Changed
- Se eliminó el identificador duplicado de contacto en el pie de página.

## [1.1.0] - 2026-09-29

### Added
- Guía inicial, acuerdos de comunidad y preguntas frecuentes.
- Formulario de contacto de muestra con validación y aviso de que no transmite datos.
- Mejoras de accesibilidad para teclado, menú móvil y preferencias de movimiento reducido.
- Metadatos para compartir el sitio y navegación móvil mejorada.

### Changed
- La portada describe el estado del proyecto con claridad y evita mostrar una IP, invitación o estado de servidor no confirmado.
- Se retiró la dependencia de Google Fonts para que el sitio no solicite recursos a terceros.
- README actualizado con pasos reales antes de publicar y el estado del formulario.

## [1.0.0] - 2026-09-29

### Added
- Landing page inicial de Pentagoland.
- Navegación simple y escalable.
- Secciones de proyecto, compatibilidad, roadmap y comunidad.
- Diseño humano, moderno y fácil de personalizar.
- estructura preparada para futuras ampliaciones.

### Changed
- Se priorizó un estilo elegante y natural sobre un aspecto excesivamente automatizado.

### Notes
- El proyecto está preparado para crecer con nuevas secciones sin romper el diseño base.

# Plantilla de Discord — Pentagoland

Documento operativo para preparar el servidor de Discord. No contiene IDs, invitaciones, tokens ni datos de staff. No se ha aplicado a una cuenta de Discord: antes de abrirlo, el propietario debe revisar y aprobar las normas, elegir el contacto de soporte y confirmar cada integración.

## Estado y datos pendientes

| Dato | Estado |
|---|---|
| Invitación permanente y propietario del servidor de Discord | Pendiente |
| Banner proporcionado por el propietario | Recibido y copiado a `../PentagolandWeb/assets/pentagoland-banner.jpg`; revisar el recorte en Discord antes de aplicarlo |
| Icono cuadrado para Discord | Pendiente; no recortar el banner automáticamente porque podría cortar el logotipo |
| Reglas y procedimiento de apelación | Borrador en `../PentagolandWeb/normas.html`; requiere aprobación |
| Dirección, región, versión y capacidad del servidor SCP:SL | Pendiente de publicación |
| Canal, horario y responsables de soporte | Pendiente |
| Política de privacidad, edad mínima aplicable y retención de tickets/logs | Pendiente de revisión |
| IDs de roles/canales y aplicaciones instaladas | No crear placeholders como si fueran IDs reales |
| Webhook de estado | No creado; el secreto debe guardarse solo en el servidor |
| SCPDiscord | No habilitado; compatibilidad y seguridad sin verificar |

## Estructura de categorías y canales

Usa permisos de categoría como base y excepciones por canal solo cuando hagan falta. `@everyone` no debe poder mencionar a todo el servidor, administrar roles/canales/webhooks ni crear invitaciones sin control. Los canales de anuncios y reglas son de solo lectura para miembros. El banner recibido puede usarse como portada si el servidor cumple los requisitos de Discord; no reutilices el banner como icono cuadrado sin una versión específica.

| Categoría | Canal | Acceso recomendado | Uso |
|---|---|---|---|
| **EMPIEZA AQUÍ** | `#bienvenida` | Lectura: todos; escritura: bot de bienvenida, si se instala | Mensaje breve, sin IDs personales ni mención masiva. |
| | `#empieza-aqui` | Lectura: todos; escritura: staff | Guía de canales, idioma, juego y soporte. |
| | `#normas` | Lectura: todos; escritura: staff | Copia del texto aprobado; hasta entonces, fijar el aviso de borrador. |
| | `#anuncios` | Lectura: todos; escritura: administración autorizada | Anuncios confirmados, con fecha. |
| | `#elige-avisos` | Lectura y selección: miembros | Roles opt-in para eventos y novedades no esenciales. |
| | `#preguntas-frecuentes` | Lectura: todos; escritura: staff | Respuestas confirmadas y enlaces vigentes. |
| **COMUNIDAD** | `#general` | Miembros | Conversación general. |
| | `#scp-secret-laboratory` | Miembros | Conversación del juego. |
| | `#busca-partida` | Miembros | Coordinar partidas sin publicar datos privados. |
| | `#clips-y-capturas` | Miembros | Material propio/autorizado; no presentar ilustraciones como capturas reales. |
| | `#sugerencias` | Miembros; tipo Foro recomendado | Etiquetas: `Idea`, `Servidor`, `Web`, `Panel`, `Discord`. No prometer que una idea se implementará. |
| | `#eventos` | Lectura: todos; participación: miembros | Eventos confirmados; usar eventos nativos de Discord si bastan. |
| **SERVIDOR** | `#estado-servidor` | Lectura: todos; escritura: integración limitada al canal | Solo estado reciente y contador. Si no hay fuente reciente, mostrar “sin datos recientes”. |
| | `#novedades-del-servidor` | Lectura: todos; escritura: staff autorizado | Cambios de servidor con versión/fecha verificadas. |
| | `#plugins-y-cambios` | Lectura: todos; escritura: responsables técnicos | Plugins cargados, versión, fecha y compatibilidad comprobada. |
| | `#guia-para-jugar` | Lectura: todos; escritura: staff | Requisitos y pasos reales, sin direcciones de ejemplo. |
| **AYUDA PRIVADA** | `#abrir-soporte` | Lectura: todos; creación limitada | Instrucciones para contactar al equipo. Si no hay sistema privado, no aceptar datos sensibles aquí. |
| | Canales privados de tickets | Solo quien abre el ticket y el equipo asignado | Reportes, soporte técnico y apelaciones; separar tickets de juego y Discord. |
| **VOZ** | `Sala social` | Miembros | Voz casual. |
| | `Grupo SCP:SL 1` / `Grupo SCP:SL 2` | Miembros | Añadir solo las salas que la actividad justifique. |
| | `AFK` | Miembros | Opcional; configurar inactividad nativa si conviene. |
| **STAFF PRIVADO** | `#staff-chat` | Staff autorizado | Coordinación general; no usar para secretos o contraseñas. |
| | `#mod-queue` | Moderadores asignados | Casos abiertos con referencias mínimas; no duplicar datos sensibles. |
| | `#reportes-internos` | Head Staff y personal autorizado | Incidentes delicados, acceso restringido. |
| | `#logs` | Responsables estrictamente necesarios | Acciones de moderación; nunca tokens, IPs públicas, contraseñas o listas de jugadores. |
| | `#bot-commands` | Staff autorizado y bots concretos | Comandos de bots permitidos, no control remoto del servidor. |
| | `#operaciones` | Owner, administración y developers autorizados | Mantenimiento e incidencias técnicas sin secretos. |

Para crear tickets sin bot, un moderador autorizado puede crear un canal privado por caso y asignar explícitamente a la persona afectada y al staff responsable. Si el volumen lo hace inviable, evaluar **un solo** bot de tickets después de revisar su política de privacidad, permisos efectivos, retención, exportación y borrado. No habilitar tickets públicos para reportes o apelaciones.

## Roles y permisos

Orden sugerido de mayor a menor. Mantén los roles de bots debajo del personal que los controla. Un bot no debe poder administrar roles superiores a su propio rol.

| Rol | Propósito | Permisos recomendados | Acceso |
|---|---|---|---|
| **Owner** | Responsable real del servidor | `Administrator` solo para el propietario y, si fuera imprescindible, una persona de respaldo. Activar 2FA. | Todo |
| **Administración** | Configuración y organización | Solo permisos concretos que requiera la tarea; no heredar `Administrator` por comodidad. | Categorías públicas y administración necesaria |
| **Developer** | Plugins e integraciones | Sin acceso automático a moderación privada, tickets o datos personales. `Manage Webhooks` solo si mantiene una integración concreta. | `#operaciones`, `#plugins-y-cambios`; otros canales solo por necesidad |
| **Head Staff** | Coordinación de moderación | Timeout, revisión de casos y permisos de kick/ban solo si la función los exige. Sin gestión general de webhooks. | Staff y casos asignados |
| **Moderator** | Moderación cotidiana | Timeout y gestión limitada de mensajes; kick/ban solo si se delega expresamente. | Comunidad y cola de moderación |
| **Trial Moderator** | Formación supervisada | Herramientas limitadas; sin ban, gestión de roles ni permisos de servidor. | Comunidad y casos asignados |
| **Support** | Ayuda al usuario | Sin permisos globales de moderación; acceso solo a tickets asignados. | Ticket asignado |
| **Miembro** | Participación habitual | Permisos de conversación por canal; sin menciones masivas ni gestión. | Comunidad |
| **Jugador SCP:SL** | Identificador informativo opt-in | Ningún privilegio adicional. | Canales públicos |
| **Avisos: eventos / novedades** | Notificaciones voluntarias | Sin permisos adicionales; no hacer ping por defecto. | Canales públicos |
| **Bot: [nombre de app]** | Una función concreta | Solo permisos imprescindibles y canales asignados; nunca `Administrator` por conveniencia. | Canales necesarios para esa función |

Discord `Administrator` ignora los overwrites de canal. No es posible ocultar un canal privado a quien conserve ese permiso.

## Bots, aplicaciones y webhooks

| Prioridad | Herramienta | Recomendación y límites |
|---|---|---|
| 1 | AutoMod, reglas, onboarding, eventos y audit log nativos de Discord | Recomendado al abrir: no requiere una app adicional. Probar filtros para spam/raid en modo prudente, con revisión humana y forma de corregir falsos positivos. |
| 2 | Webhook entrante de estado | Un webhook dedicado a `#estado-servidor`, creado desde el servidor. Publicar solo disponibilidad, contador y hora real. La URL completa es un secreto: no ponerla en HTML, JavaScript, Git, mensajes ni capturas. Revocarla si se expone. |
| 3 | Uptime Kuma autoalojado (opcional) | Útil para alertas de disponibilidad fuera del juego; no sustituye a la telemetría del servidor ni debe recibir datos de jugadores. Enviar alertas a un canal privado restringido o webhook dedicado. |
| 4 | Sistema de tickets | Empezar con tickets manuales privados. `Ticket Tool` puede evaluarse más adelante, no está seleccionado ni instalado: su documentación indica que los transcripts se envían a Discord y que configurar el panel requiere ser owner, tener un rol del panel o permiso `Administrator`; no conceder `Administrator` al bot por ello. Revisar antes su política, permisos de invitación y borrado. |
| 5 | OAuth de Discord para la web (futuro) | Es una aplicación OAuth, no un bot de servidor. Pedir solo el scope `identify` si basta; completar primero la política de privacidad y el backend. Discord no demuestra la titularidad de una cuenta Steam. |
| Bloqueado | SCPDiscord del directorio local | No activar todavía: la compilación contra LabAPI 1.1.7 no está validada; el código/dll tienen versiones distintas según la auditoría previa; el puente TCP observado no autentica. Mantener solo en loopback/red privada y no exponerlo a Internet. |

### Apps que no hacen falta al inicio

- **Lista recomendada de inicio:** herramientas nativas de Discord (AutoMod, onboarding, eventos y audit log). Opcional para el operador: [Uptime Kuma](https://github.com/louislam/uptime-kuma) autoalojado para vigilar disponibilidad y emitir avisos de infraestructura a un webhook privado.
- **Ticket Tool:** candidato para una prueba futura, no recomendación de instalación inmediata. Los transcripts que el bot genera se publican en Discord y requieren una política de acceso/retención; configurar su panel no debe llevar a conceder `Administrator` a la aplicación.
- No instalar varios bots de moderación que dupliquen AutoMod y se contradigan.
- No instalar bots de niveles, XP o perfiles antes de definir privacidad, datos y reglas.
- No dar `Administrator` a bots de tickets, estadísticas, bienvenida o logs.
- No conceder a un bot permisos para ejecutar comandos arbitrarios del servidor, gestionar roles superiores, ver tickets no asignados o leer todos los mensajes si la función no lo necesita.
- No usar webhooks como autenticación. Un webhook puede publicar en su destino y la URL debe tratarse como credencial.
- No se recomienda instalar el repositorio público [ServerMod/SCPDiscord](https://github.com/ServerMod/SCPDiscord) sin verificar el origen del código local: la revisión encontró ese repositorio archivado y no oficial. No se ha confirmado que sea idéntico al proyecto local de `Plugins SCP`.

### Permisos mínimos para integraciones

- **Publicador de estado:** enviar mensajes/embeds solo en `#estado-servidor`; sin lectura general, moderación, miembros ni administración.
- **Bot de tickets:** crear/cerrar canales o hilos privados según su diseño; ver solo la categoría de soporte y los tickets asignados; no acceder a `#logs` ni a todos los mensajes.
- **SCPDiscord:** si la auditoría futura lo aprueba, roles y comandos limitados a una lista explícita; servicio y puerto en loopback/red privada; firewall local; sin exposición pública del socket ni del RCON.
- **App OAuth web:** callback HTTPS de backend; scopes mínimos; validar `state`, nonce/sesión y respuesta en servidor; desconectar vínculo; no poner client secret en el navegador.

## Onboarding y mensajes listos

### Mensaje de bienvenida

> ¡Bienvenido/a a **Pentagoland**! Esta comunidad es para jugar SCP:SL y charlar en español. Empieza por `#empieza-aqui`, revisa `#normas` y elige solo los avisos que quieras recibir en `#elige-avisos`.  
> Servidor de juego: **datos por confirmar** · Ayuda: **canal por confirmar**.  
> No compartas contraseñas, códigos, tokens ni datos personales.

Reemplazar los textos en negrita únicamente cuando el propietario confirme los datos.

### Aviso de normas para `#normas`

> **Borrador pendiente de aprobación.** Estas normas todavía no están vigentes. La propuesta web está en `PentagolandWeb/normas.html`. No se aplicarán sanciones basadas en una versión no aprobada.  
> Cuando el propietario apruebe el texto, publica aquí la versión y enlaza la página oficial. No inventes una invitación, contacto, apelación o sanción.

### Texto de reglas para `#normas` (publicar solo después de aprobación)

> **Pentagoland · normas de convivencia**  
> Comunidad independiente en español para SCP: Secret Laboratory. No estamos afiliados ni respaldados por Northwood Studios ni por SCP Wiki.
>
> 1. **Trato:** se permiten bromas y desacuerdos respetuosos. Si alguien pide que pares, para. No se permite acoso dirigido, amenazas, odio, doxxing ni hostigamiento repetido.
> 2. **Juego limpio:** no uses trampas, software no permitido ni exploits para conseguir ventaja o arruinar una partida. Reporta errores por el canal privado aprobado.
> 3. **Uso de canales y voz:** evita spam, audio deliberadamente molesto, raids, phishing, malware y menciones masivas. No grabes ni transmitas conversaciones de voz sin consentimiento explícito.
> 4. **Privacidad:** no publiques datos personales, IPs, contraseñas, códigos, tokens, mensajes privados ni logs internos. El equipo nunca te pedirá credenciales.
> 5. **Moderación:** las reglas del juego y las de Discord se aplican en sus espacios respectivos; una medida no se traslada automáticamente al otro. La respuesta será proporcional y podrá escalar en casos graves o repetidos.
> 6. **Revisión:** para pedir revisión usa **[canal aprobado]** y aporta solo lo necesario. No discutas reportes ni apelaciones en público.
>
> **Estado:** publicar este bloque solo cuando el propietario apruebe las reglas y confirme el canal de revisión, los procedimientos y la información requerida por Northwood para el Server Info del juego.

### Estado del servidor sin integración

> Aún no hay una fuente fiable conectada. No se puede confirmar si el servidor está disponible ni cuántas personas juegan. No uses capturas ni cifras de ejemplo como estado actual.

### Anuncio o mantenimiento

> **[Título confirmado]**  
> Fecha y hora: **[zona horaria y fecha confirmadas]**  
> Qué cambia: **[detalle verificado]**  
> Impacto esperado: **[confirmado o “por determinar”]**  
> Próxima actualización: **[solo si existe un compromiso real]**

### Sugerencia

> **Qué propones:**  
> **Qué problema resuelve:**  
> **Área:** servidor / web / panel / Discord  
> **¿Es opcional o bloquea algo?:**  
> No incluyas claves, datos personales ni información de otros jugadores.

### Ticket privado

> Describe el problema y cuándo ocurrió. Si es del juego, incluye solo la información necesaria para localizar el incidente. No envíes contraseñas, códigos de Steam, tokens, IPs privadas ni datos de terceros. El equipo usará este canal solo para revisar el caso; el tiempo de respuesta no está garantizado.

## Conexión con la web y el panel

| Función | Diseño previsto | Estado |
|---|---|---|
| Enlace Discord en la web | Invitación permanente verificada | Pendiente; no se publica URL ficticia |
| Normas web/Discord | Mismo texto aprobado en ambos sitios | El sitio muestra un borrador; no está aplicado en Discord |
| Estado de juego | Plugin escribe DTO público; backend/proxy puede publicar un resumen | `estado.json` local existe; endpoint HTTPS y webhook no configurados |
| Estadísticas | Eventos del servidor, autorización por Steam verificada y API privada | `StatsSystem`/`MvpSystem` están instalados, pero no hay API web autenticada |
| Inicio de sesión | Steam OpenID en backend; vínculo Discord opcional tras verificar Steam | No implementado; el SteamID escrito en el navegador no verifica identidad |
| Anuncios web/Discord | Publicar solo cambios confirmados y sincronizados por responsables | No automatizar hasta disponer de canal y backend |

El navegador nunca debe contener bot tokens, client secrets, webhooks, credenciales de LocalAdmin/RCON o archivos de estadísticas privadas. El estado público no debe enumerar jugadores. Antes de mostrar estadísticas, comprobar además la política vigente del juego sobre privacidad y la opción Do Not Track.

## Lista de pruebas antes de abrir

- [ ] Revisar permisos de `@everyone` en escritorio y móvil.
- [ ] Probar cuenta nueva, aceptación de reglas, onboarding y selección opt-in.
- [ ] Confirmar que `#normas`, `#anuncios` y `#plugins-y-cambios` son de solo lectura para miembros.
- [ ] Confirmar que Trial Moderator, Support, Developer y bots no ven canales que no necesitan.
- [ ] Verificar que no hay bots con `Administrator` ni roles sobre el staff que los administra.
- [ ] Crear ticket de prueba: solo la persona afectada y el personal asignado pueden leerlo; cerrar y borrar conforme a la política aprobada.
- [ ] Probar AutoMod con casos normales y falsos positivos; revisar apelación antes de activar respuestas automáticas.
- [ ] Rotar/eliminar cualquier webhook expuesto y comprobar que no aparece en archivos web o Git.
- [ ] Probar el endpoint de estado con JSON válido, caído, antiguo e inválido; no publicar datos individuales.
- [ ] Revisar mensaje de Discord y página web desde una cuenta sin privilegios.
- [ ] Confirmar reglas en la información del servidor SCP:SL y declarar el idioma/ámbito de forma correcta conforme a las directrices vigentes de Northwood.

## Fuentes oficiales consultadas

- [Northwood Community Server Guidelines](https://www.scpslgame.com/CSG.pdf): reglas del servidor, idioma/Server Info, privacidad, Do Not Track, voz y seguridad. Revisar la versión vigente antes de publicar.
- [EULA de SCP: Secret Laboratory](https://store.steampowered.com/eula/700330_eula_0): obligaciones y restricciones del juego.
- [Discord Community Guidelines](https://discord.com/guidelines), [Developer Terms](https://discord.com/developers/docs/policies-and-agreements/developer-terms-of-service) y [Developer Policy](https://discord.com/developers/docs/policies-and-agreements/developer-policy).
- [Permisos de Discord](https://docs.discord.com/developers/topics/permissions), [OAuth2](https://docs.discord.com/developers/topics/oauth2) y [webhooks](https://docs.discord.com/developers/resources/webhook).
- [Guía de permisos de Remote Admin de Northwood](https://techwiki.scpslgame.com/books/server-guides/page/remote-admin-permissions-guide).

La consulta de fuentes y el borrador son una guía operativa, no asesoría legal ni una declaración de elegibilidad para la lista verificada de Northwood, monetización de Discord u otros programas.

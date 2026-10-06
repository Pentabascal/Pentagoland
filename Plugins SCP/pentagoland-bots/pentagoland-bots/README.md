# Bots de Pentagoland

Tres bots independientes (cada uno con su token, su base de datos SQLite y su proceso) que comparten un núcleo común.

| Bot | Comando | Para qué sirve |
|---|---|---|
| **Web** | `npm run web` | Todo lo relacionado con la web: novedades (borrador → publicación), galería aprobada, enlaces oficiales, monitor de la web y JSON públicos de solo lectura. |
| **Vínculo** | `npm run link` | Puente Discord ↔ web ↔ servidor SCP:SL: API firmada, estado en vivo, vinculación de cuentas verificada desde el juego, estadísticas. |
| **Sentinel** | `npm run scp` | Auxiliar: moderación con casos, tickets privados con transcripción y utilidades de SCP. Base para el futuro módulo SCP. |

Todos incluyen `/ayuda`, `/ping` y `/config` (configuración por comandos, guardada en la base de datos).

> **Estado real:** el código está revisado por sintaxis, **pero aún no se ha ejecutado contra Discord**. Pruébalo primero en un servidor de pruebas siguiendo la checklist del final.

## Instalación

Requisitos: Node 20+ (better-sqlite3 usa binarios precompilados; si tu plataforma no los tiene, hacen falta herramientas de compilación).

1. `npm install`
2. En discord.com/developers/applications crea **3 aplicaciones** (Web, Vínculo, Sentinel). En cada una: Bot → copia el token. No hace falta ningún intent privilegiado.
3. Copia `.env.example` a `.env` y rellénalo. Genera los secretos con `node -e "console.log(require('crypto').randomBytes(32).toString('hex'))"` (uno distinto para `API_SECRET_SERVER` y otro para `API_SECRET_WEB`).
4. `npm run check` valida la configuración.
5. `npm run invite -- web` (y `link`, `scp`) imprime el enlace de invitación con los **permisos mínimos** de cada bot. Ninguno pide Administrador.
6. Sube el rol de cada bot por encima de los roles que deba gestionar (Vínculo: `Jugador SCP:SL`; Sentinel: los usuarios que modere).
7. Arranca: `npm run web`, `npm run link`, `npm run scp` (o `pm2 start ecosystem.config.cjs`).
8. En Discord, con un administrador, ejecuta **`/config autodetectar`** en cada bot: asigna canales y roles por el nombre que crea `pentagoland-setup.js`. Revisa con `/config ver` y completa lo que falte con `/config canal|rol|texto`.

### Permisos por bot
- **Web:** ver canales, enviar mensajes, insertar enlaces, leer historial.
- **Vínculo:** lo anterior + Gestionar roles (solo para dar/quitar `Jugador SCP:SL`).
- **Sentinel:** lo anterior + adjuntar archivos, gestionar mensajes (`/purge`), gestionar canales (tickets), aislar, expulsar y banear.

## Niveles de permiso (los define el staff, no Discord)
`support < trial < mod < head < admin`, más `dev` aparte. Se asignan con `/config rol`. El propietario del servidor y quien tenga el permiso Administrador cuentan como `admin`. Cada comando comprueba el nivel al ejecutarse.

## Comandos

**Web:** `/web estado|enlaces|enlace-poner|enlace-quitar`, `/novedad crear|editar|publicar|archivar|lista`, `/galeria proponer|aprobar|rechazar|lista`.
**Vínculo:** `/vincular`, `/desvincular`, `/perfil ver|publico`, `/top`, `/servidor estado|panel|actualizar`, `/vinculo info`.
**Sentinel:** `/warn`, `/timeout`, `/untimeout`, `/kick`, `/ban`, `/unban`, `/nota`, `/casos`, `/caso`, `/purge`, `/ticket panel|cerrar|anadir|quitar`, `/scp servidor`.

## API

Pon un proxy inverso con HTTPS delante (p. ej. Caddy: `api.tudominio.com { reverse_proxy 127.0.0.1:3102 }`) y `TRUST_PROXY=1`. Por defecto las APIs solo escuchan en `127.0.0.1`.

**Firma (endpoints `/v1/*`):** cabeceras `X-Pentagoland-Timestamp` (segundos unix) y `X-Pentagoland-Signature` = hex de `HMAC-SHA256(secreto, "{timestamp}.{cuerpo_json}")`. Ventana de 60 s; una firma no se puede reutilizar. Ver `docs/ExampleSignedClient.cs`.

| Endpoint (bot Vínculo) | Auth | Función |
|---|---|---|
| `POST /v1/server/status` | servidor | El plugin envía `online`, `players`, `maxPlayers`, `version`, `name`, `round`. Actualiza el panel de estado. |
| `POST /v1/server/stats` | servidor | Instantánea absoluta por jugador (1–200 por petición): `steamId`, `kills`, `deaths`, `secondsPlayed`, `scpKills`, `breakdown`. |
| `POST /v1/link/code` | servidor | El juego pide un código de 8 caracteres (10 min) para un jugador. |
| `POST /v1/web/notify` | web | Tu backend publica un embed en `anuncios`, `novedades` u `operaciones`. |
| `GET /public/status.json` | — | Estado saneado. Sin lista de jugadores ni IP. Indica `stale` si no hay datos recientes. |
| `GET /public/leaderboard.json` | — | Solo jugadores que activaron `/perfil publico`. |

| Endpoint (bot Web) | Función |
|---|---|
| `GET /public/news.json` | Solo novedades publicadas. |
| `GET /public/gallery.json` | Solo elementos aprobados, con crédito. |
| `GET /public/links.json` | Solo enlaces oficiales fijados. |

Los JSON públicos solo llevan CORS si defines `WEB_ORIGIN`. **En la web usa siempre `textContent`, nunca `innerHTML`**, con estos datos.

## Cómo se vincula una cuenta (verificado)
1. El jugador pide el código **dentro del juego** (tu plugin llama a `/v1/link/code` con el UserId que el servidor conoce).
2. Escribe `/vincular codigo:XXXXXXXX` en Discord.
3. El bot enlaza ambos. Escribir un SteamID a mano **no** autentica a nadie, por eso no existe esa vía.

## Seguridad y privacidad
- Tokens y secretos solo en `.env` (fuera de git). Los logs ocultan tokens.
- Sin webhooks en la web pública: la web llama a tu backend y este a `/v1/web/notify`.
- Las estadísticas solo entran por la API firmada del servidor; Discord no las inventa ni las edita.
- Perfil privado por defecto; `/desvincular` elimina el vínculo y el nombre público.
- Los tickets guardan la transcripción solo en el canal de logs (staff). **La política de retención la decides tú** (bórralas a mano o define un plazo). Sentinel no borra el canal si no puede guardar la transcripción.
- No hay acciones remotas sobre el servidor de juego: no existen todavía.

## Qué funciona ya y qué es futuro
- **Ya (tras probarlo):** todos los comandos y APIs de arriba.
- **Futuro / no implementado:** comandos de administración remota del servidor SCP, cuentas Google/correo y foro (necesitan backend), Discord OAuth en la web, formulario web → sugerencias/tickets.

## Datos que debes confirmar tú
- Qué significa exactamente “bajas por Caos/MTF/SCP/Guardia/Clase D/Científico”: el bot guarda `breakdown` con esas seis claves y lo muestra como “Desglose por facción”, sin interpretar si son bajas causadas o recibidas. Ajusta el texto según tu plugin.
- Formato real de los UserId que envía tu plugin (el bot acepta `letras/números@plataforma`).
- IP/puerto públicos (se muestran solo si los fijas con `/config texto`).

## Checklist de pruebas (servidor de pruebas)
- [ ] `npm run check` sin errores; los 3 bots conectan y registran comandos.
- [ ] `/config autodetectar` y `/config ver` en cada bot.
- [ ] Un usuario sin rol no puede usar comandos de staff.
- [ ] `/novedad crear` → `publicar` (aparece en Discord y en `/public/news.json`).
- [ ] `/servidor panel`; enviar `POST /v1/server/status` firmado; parar 3 min y ver “Sin datos recientes”.
- [ ] Petición con firma incorrecta, timestamp viejo y firma repetida → 401.
- [ ] `/v1/link/code` → `/vincular` → `/perfil ver` → `/desvincular`.
- [ ] Abrir ticket (móvil y escritorio), visibilidad solo para el autor y el equipo, cerrar con transcripción.
- [ ] `/timeout`, `/kick` y `/ban` respetan jerarquías de roles.
- [ ] Reinicio con pm2: los datos persisten.

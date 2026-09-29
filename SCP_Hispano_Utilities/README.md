# Pentagoland v3.1.0

Proyecto base y utilidades para EXILED pensado para un servidor de SCP: Secret Laboratory con identidad propia: Pentagoland.

## Visión general

Esta versión representa una base más robusta, más limpia y más útil para un servidor real. Está diseñada para funcionar como núcleo de utilidades del servidor, manteniendo una estructura clara tanto para usuarios como para desarrolladores. La idea principal es ofrecer estabilidad, compatibilidad y facilidad de expansión sin romper el ecosistema del servidor.

## Funcionalidades principales

- bienvenida automática al entrar
- anuncios de inicio y fin de ronda
- protección anti-spam y anti-enlaces externos
- sistema de reportes para staff
- mensajes de muerte y avisos del servidor
- sistema de puntos por actividad y recompensas
- comandos útiles para usuarios y comunidad
- compatibilidad con aliases y protección de namespace
- diseño pensado para convivir con plugins custom sin colisiones graves

## Nombres oficiales del proyecto

El nombre del servidor es Pentagoland y la identidad del proyecto se mantiene en una línea moderna y profesional:

- Pentagoland Utilities
- Pentagoland Core
- Pentagoland Stats
- Pentagoland Staff Reports
- Pentagoland Event Manager

Se mantienen aliases legacy como HispanoConfig, HispanoServerUtilities, HispanoCommandRegistry y otros para asegurar compatibilidad con código previo.

## Estructura del proyecto

- `HispanoServerUtilities.cs`: núcleo principal del plugin
- `HispanoConfig.cs`: configuración base del servidor
- `HispanoCompatibilityProfile.cs`: compatibilidad y control de comandos
- `HispanoCommandRegistry.cs`: registro de comandos del servidor
- `HispanoStatsStore.cs`: almacenamiento de puntos por usuario
- `Plugins/`: módulos adicionales para staff, stats y eventos

## Instalación rápida

1. Compila el proyecto con .NET.
2. Añade la referencia real de EXILED en tu servidor.
3. Copia el DLL generado a la carpeta de plugins del servidor.
4. Ajusta la configuración según la necesidad del servidor.
5. Personaliza mensajes, reglas, prefijo y nombre del servidor en la configuración.

## Compatibilidad y estabilidad

La v3.0.0 está pensada para minimizar conflictos:

- prefijos configurables
- aliases controlados
- protección de comandos reservados
- validación básica de entradas
- estructura desacoplada y más fácil de mantener
- compatibilidad con nombres antiguos para no romper proyectos actuales

## Comandos activos

- `!help` / `!ayuda`
- `!info`
- `!report` / `!reporte` / `!denuncia`
- `!puntos` / `!points` / `!score`
- `!rules` / `!reglas`
- `!staff` / `!moderacion`
- `!warns` / `!advertencias`
- `!server` / `!status`

## Estado de desarrollo

La versión v3.1.0 queda enfocada a una base profesional y lista para ser usada en un servidor real de SCP:SL. La arquitectura es más limpia, el branding es coherente y la capa de compatibilidad está preparada para crecer sin romper la funcionalidad existente.

## Próximos pasos recomendados

- ampliar el sistema de puntos y ranking
- añadir logs de moderación y auditoría
- crear eventos programados por horario
- añadir roles o perks por comunidad
- preparar un sistema de ranking o recompensa más avanzado

## Resumen final

La v3.1.0 es una versión estable, más profesional, más completa y mejor preparada para Pentagoland. Está lista como base funcional para el servidor, manteniendo buena compatibilidad, claridad y facilidad de expansión.

## Continuación de la evolución del proyecto

La base ya está preparada para convertirse en el núcleo operativo del servidor, pero aún queda consolidar la capa de utilidad real que hará que Pentagoland se sienta vivo para la comunidad. La siguiente fase no es solo estética ni de branding, sino de estabilidad funcional: reforzar la compatibilidad con EXILED, evitar conflictos de comandos y dejar un sistema de estadísticas y reportes que pueda escalar sin romper el resto del plugin.

En esta etapa se prioriza lo siguiente:

- cerrar la compatibilidad con alias históricos para no romper instalaciones previas
- proteger los nombres de comandos principales frente a colisiones con plugins externos
- mantener un registro limpio y reutilizable de reportes, advertencias y puntos
- generar snapshots JSON para integración con sistemas externos o dashboards simples
- dejar un diseño modular para que cada funcionalidad pueda añadirse sin tocar la lógica base

## Fase de consolidación recomendada

La siguiente iteración del proyecto debería centrarse en tres bloques principales:

1. Core y configuración
   - definir claramente qué valores deben mantenerse persistentes y cuáles son temporales
   - separar la lógica del plugin de los textos del servidor para facilitar traducciones
   - permitir activar o desactivar cada sistema sin tocar el código base

2. Compatibilidad y comandos
   - mantener `!help`, `!info`, `!report`, `!puntos`, `!rules` y equivalentes bajo una misma política de alias
   - reservar nombres críticos para evitar conflictos con plugins futuros
   - normalizar entradas y filtrado de prefijos para evitar errores de uso por parte de usuarios

3. Estadísticas y mantenimiento
   - guardar estadísticas por jugador y por ronda para medir actividad del servidor
   - registrar muertes, kills, puntos, advertencias y comportamiento general
   - exportar un snapshot útil para backend, web o herramientas de monitoring internas

## Objetivo de la siguiente versión

La idea es que Pentagoland pase de ser una base sólida a convertirse en una capa operativa completa: útil para jugadores, cómoda para staff y segura para la expansión del servidor. No se trata de añadir complejidad artificial, sino de dejar una estructura preparada para crecer sin perder claridad ni compatibilidad.

En otras palabras, la próxima fase debe hacer que el proyecto se sienta profesional no solo por nombre, sino por diseño: cada módulo con una responsabilidad clara, cada comando con un propósito útil y cada ajuste de configuración con impacto visible en la experiencia general del servidor.

## Cierre del avance actual

Hasta este punto, el proyecto ya tiene una identidad definida, una base técnica razonable y una roadmap clara. La siguiente evolución no debe perder de vista la esencia del servidor: comunidad hispana, estabilidad, orden y una experiencia de juego más fluida para todos.

La continuidad natural del proyecto es avanzar de la base conceptual a la capa funcional, dejando preparado el sistema para que Pentagoland pueda crecer como un núcleo serio y profesional dentro del ecosistema de EXILED.

## Requisitos para despliegue real en servidor

Antes de subir este proyecto a un repositorio o asignarlo a un servidor real, conviene dejarlo preparado para que:

- solo los administradores puedan ejecutar comandos sensibles
- la compatibilidad con aliases, prefijos y nombres antiguos se mantenga intacta
- la configuración de identidad del servidor sea totalmente personalizable
- la base del plugin no altere funciones críticas del juego ni del núcleo de EXILED
- la estructura del proyecto sea limpia y fácil de mantener en futuras versiones

Esto se consigue con un diseño modular, restricciones claras de permisos, alias protegidos y una configuración centralizada que no obliga a tocar la lógica principal del plugin.

# Guía de orden y publicación para maximizar productividad

## Objetivo

Publicar cada plugin por separado sin mezclar contenido del servidor, documentación interna ni trabajo experimental. La estrategia correcta es mantener un repositorio de trabajo local claro y crear repositorios de publicación limpios y pequeños.

## 1. Estructura del repositorio de trabajo

Mantén esta estructura base:

EXILED/
├── README.md
├── .gitignore
├── Configs/
│   ├── permissions.yml
│   ├── AdminTools/
│   ├── CreditTags/
│   ├── Plugins/
│   ├── Translations/
│   ├── UncomplicatedCustomItems/
│   └── XPSystem/
├── Plugins/
│   ├── Activos/
│   │   ├── Exiled.CustomItems/
│   │   ├── VVUP.CustomItems/
│   │   ├── SubclassFramework/
│   │   └── ...
│   ├── Documentacion/
│   ├── 7777/
│   ├── dependencies/
│   ├── backups/
│   └── staging/
├── Instrucciones Proyecto IA/
├── Pendientes Por Configurar e Instalar/
├── zCreacionPlugins/
├── DisabledPlugins/
├── Dedicated Server/
├── Logs/
└── _reset_20260923_2018/

## 2. Regla general de publicación

Publica solo lo útil:

- plugins activos
- DLLs estables
- configuración real
- README claro
- licencia si aplica

No publiques:

- logs
- backups
- staging
- server runtime
- build folders
- dependencies de servidor local
- trabajo experimental
- documentación interna mezclada con el plugin

## 3. Orden recomendado por prioridad

Publica por prioridad de utilidad y estabilidad:

### Prioridad alta

- Exiled.CustomItems
- Exiled.CustomRoles
- Exiled.Events
- Exiled.Permissions
- SubclassFramework
- VVUP.Base
- VVUP.CustomItems
- VVUP.SubclassCompatibility
- XPSystem

### Prioridad media

- VVUP.CustomEscapes
- VVUP.MicroDamageReduction
- VVUP.PostNukeEnhancements
- VVUP.RoundStart
- VVUP.ScpChanges
- VVUP.ServerEvents
- VVUP.Votes
- VVUP.WeaponEvaporate
- RemoteKeycard
- ScpUtils

### Prioridad baja o solo como complemento

- CreditTags
- AdminTools
- FacilityManagement
- AntiSuicidalZombies
- LateSpawn
- VPNGuard
- custom extras o plugins de pruebas

## 4. Cómo preparar cada repositorio individual

Cada repositorio publicado debe quedar así:

PluginName/
├── README.md
├── LICENSE
├── PluginName.dll
├── Configs/
│   └── plugin.yml
├── Dependencies/
│   └── dlls necesarios
├── changelog.md
└── screenshots/

## 5. Qué incluir en el README de cada plugin

Cada README debe contener:

- nombre del plugin
- versión
- compatibilidad con EXILED / LabAPI
- requisitos
- instalación paso a paso
- configuración mínima
- permisos o comandos si aplica
- notas de compatibilidad
- licencia

## 6. Qué no incluir bajo ninguna circunstancia

- DLLs de otros plugins
- archivos de logs del servidor
- config del servidor completo
- plugins de HUD mezclados con el mismo repo
- contenido del entorno local
- backups o pruebas antiguas
- archivos de compilación bin/ obj/ Build/

## 7. Estrategia para máxima productividad

### Opción recomendada

Mantén este flujo:

1. Trabajo local: repositorio grande con toda la base del servidor
2. Validación: revisa que la carpeta del plugin sea correcta
3. Separación: cada plugin se mueve a su propio repositorio limpio
4. Publicación: sube solo la versión estable del plugin
5. Respaldo: conserva el trabajo local solo como referencia

### Resultado

Esto te da:

- menos caos
- más velocidad al publicar
- menos riesgo de romper el servidor
- repositorios limpios y profesionales
- mejor trazabilidad por plugin

## 8. Recomendación final de orden

Si quieres ser realmente eficiente, haz este orden de publicación:

1. Exiled.CustomItems
2. Exiled.CustomRoles
3. Exiled.Events
4. Exiled.Permissions
5. SubclassFramework
6. VVUP.Base
7. VVUP.CustomItems
8. VVUP.SubclassCompatibility
9. XPSystem
10. VVUP.CustomEscapes
11. VVUP.ServerEvents
12. VVUP.RoundStart
13. VVUP.MicroDamageReduction
14. VVUP.PostNukeEnhancements
15. VVUP.ScpChanges
16. VVUP.Votes
17. VVUP.WeaponEvaporate
18. RemoteKeycard
19. ScpUtils

## 9. Regla práctica

Si un plugin no tiene README claro, no tiene DLL limpia, no tiene compatibilidad confirmada o está mezclado con otro plugin, no lo publiques todavía. Mejora primero la carpeta y luego sácala a GitHub.

## 10. En resumen

La mayor eficiencia viene de esta regla:

- trabajo local ordenado
- publicación por plugin individual
- documentación clara
- configuración limitada
- repositorios pequeños y específicos

Es la forma más limpia y profesional de mantener un proyecto de este tipo.

# Guía rápida para subir el proyecto a GitHub

## 1. Regla principal

Haz repositorios pequeños y específicos. No publiques 5 HUD en un mismo repositorio.
Lo ideal es:

- un repositorio por plugin
- una carpeta por plugin con su DLL, README y configuración si aplica
- un repositorio separado solo para documentación o para el servidor, si quieres mantenerlo como base de trabajo

## 2. Estructura recomendada para publicar

Cada plugin debe quedar así:

PluginName/
├── README.md
├── LICENSE (si aplica)
├── PluginName.dll
├── Configs/
│   └── plugin-name.yml
├── Dependencies/
│   └── dlls necesarios
└── changelog.txt

No publiques:

- logs del servidor
- bin/ y obj/
- backups
- carpetas de runtime del servidor
- archivos de build local
- folders de pruebas o staging
- documentación interna mezclada con el plugin

## 3. Qué sí conviene subir

Para GitHub, lo útil es:

- [README.md](README.md)
- [Configs](Configs)
- [Plugins/Activos](Plugins/Activos)
- [Instrucciones Proyecto IA](Instrucciones%20Proyecto%20IA)
- documentación técnica clara

## 4. Cómo dejar cada plugin listo

1. Abre la carpeta del plugin en [Plugins/Activos](Plugins/Activos)
2. Comprueba que el nombre sea limpio y real: por ejemplo:
   - Exiled.CustomItems
   - VVUP.CustomItems
   - SubclassFramework
   - RemoteKeycard
3. Asegúrate de que la DLL esté dentro de su carpeta propia
4. Si hay configuración relacionada, deja un archivo claro y con nombre legible
5. Añade un README del plugin con:
   - qué hace
   - requisitos
   - archivos necesarios
   - cómo instalarlo
   - versión y compatibilidad

## 5. Cómo crear el repositorio en GitHub

1. En GitHub, crea un repositorio nuevo
2. Dale un nombre limpio:
   - VVUP.CustomItems
   - Exiled.CustomItems
   - SubclassFramework
   - RemoteKeycard
3. Crea la rama principal como main
4. Haz git init en la carpeta del plugin
5. Añade el remoto:

```bash
git remote add origin https://github.com/TU_USUARIO/NOMBRE_REPO.git
```

6. Haz commit:

```bash
git add .
git commit -m "Initial plugin release"
```

7. Publica:

```bash
git push -u origin main
```

## 6. Buenas prácticas para mantenerlo profesional

- Usa tags de versión: v1.0.0, v1.1.0
- No subas archivos de prueba ni configuración del servidor local
- Mantén el README con tono serio y útil
- Si un plugin depende de otro, indícalo en el README
- Si el plugin es de terceros, deja la licencia y la fuente original clara

## 7. Recomendación final

El mejor enfoque es:

- un repositorio por plugin para la parte pública
- una copia local del servidor para desarrollo y pruebas
- una documentación técnica central para mantenimiento

Esto evita ruido, aumenta la claridad y hace que el proyecto se vea profesional desde el primer vistazo.

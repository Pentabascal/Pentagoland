# Pentagoland

Workspace portable del servidor SCP:SL y su desarrollo. La raíz mantiene las dos carpetas principales solicitadas:

- [`EXILED/`](./EXILED/): snapshot de plugins y configuración de EXILED y LabAPI. Revisa rutas y configuración local antes de instalarlo en otro equipo; las credenciales y los registros privados se excluyen.
- [`Plugins SCP/`](./Plugins%20SCP/): fuentes de plugins, subclases, herramientas del servidor, documentación, web y desarrollo del mapa.

## Mapa, Unity y Blender

- Toolkit del mapa: [`Plugins SCP/Map Development/PentagoLand/`](./Plugins%20SCP/Map%20Development/PentagoLand/README.md).
- Blender, texturas, modelos exportados y esquemáticos ProjectMER: [`AssetLibrary/`](./Plugins%20SCP/Map%20Development/PentagoLand/AssetLibrary/README.md).
- Proyecto Unity para revisar materiales: [`UnityMaterialWorkbench/`](./Plugins%20SCP/Map%20Development/PentagoLand/AssetLibrary/UnityMaterialWorkbench/README.md).
- Proyecto de galería Unity de los modelos: [`PentagolandAssetReview/`](./Plugins%20SCP/Map%20Development/PentagoLand/AssetLibrary/PentagolandAssetReview/README.md).
- Paquete actual de 87 esquemáticos nativos: [`Samples/Schematics/`](./Plugins%20SCP/Map%20Development/PentagoLand/AssetLibrary/Samples/README.md).

Instala por separado Unity **6000.6.4f1**, Blender **5.2.2**, el .NET SDK indicado por cada proyecto y las versiones compatibles del servidor SCP:SL y sus frameworks. Para validar el toolkit desde la raíz:

```powershell
& ".\Plugins SCP\Map Development\PentagoLand\verify.cmd"
```

Los esquemáticos deben probarse en un servidor de staging. Los modelos FBX/GLB no son meshes cargables directamente por ProjectMER stock. ProjectMER y sus binarios no se redistribuyen en este repositorio; obtén una versión compatible desde el [proyecto oficial](https://github.com/Michal78900/ProjectMER). Los assemblies del juego y las dependencias de frameworks no se reconstruyen a partir de esta copia; sigue las instrucciones de cada plugin.

## Publicación y datos locales

El nombre de esta carpeta de trabajo es `Pentagoland1`; el repositorio remoto sigue siendo `Pentabascal/Pentagoland`, rama `main`. Revisa `git status` y el diff antes de publicar. `.gitignore` conserva los dos directorios principales, pero excluye cachés, builds, credenciales, datos de jugadores, copias de respaldo y notas locales.

El snapshot `EXILED/LabAPI` documenta la instalación de esta máquina; no es un instalador universal. No subas claves de CedMod, registros, cachés de jugadores ni configuraciones privadas. Instala y valida cada plugin en staging con las dependencias correctas.

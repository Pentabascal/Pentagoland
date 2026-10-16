# Pentagoland SCP:SL Server Toolkit

Una colección de herramientas independientes para servidores de SCP: Secret Laboratory: Pentagoland Server Doctor, el validador offline de esquemáticos ProjectMER y una biblioteca de props originales con fuentes Blender y formatos de intercambio.

## Contenido

- [`ServerDoctor/`](./ServerDoctor/README.md): plugin administrativo LabAPI con inspección explícita, inventario, backups y edición limitada y protegida de configuración. No altera la partida al iniciar.
- [`AssetLibrary/`](./AssetLibrary/README.md): herramientas .NET para validar JSON de esquemáticos, 81 props estáticos Blender (77 de utilidad y 4 previos), FBX/GLB, texturas, previews, proxies opcionales y esquemáticos de ejemplo hechos con primitivas nativas de MER. [`AssetLibrary/PentagolandAssetReview/`](./AssetLibrary/PentagolandAssetReview/README.md) contiene un proyecto de inspección visual Unity separado.

Los proyectos se pueden compilar o validar por separado. Las mallas de Blender no son assets listos para SCP:SL; ProjectMER stock no carga estas mallas como bloques. Los esquemáticos MER son aproximaciones visuales estáticas. Ningún modelo ofrece física dinámica ni comportamiento de juego.

## Empezar desde un clon limpio

Instala Git y el **.NET 10 SDK**. Blender 5.2 es necesario para regenerar o volver a validar los modelos; Unity **6000.6.4f1** solo se necesita para el proyecto de inspección visual. Ninguna de esas herramientas se instala automáticamente ni se incluye en el repositorio.

```powershell
git clone <URL-del-repositorio>
cd PentagolandSCPServerToolkit
.\verify.cmd
```

`verify.cmd` ejecuta las pruebas aisladas de Server Doctor, compila y prueba el validador MER y revisa los 82 esquemáticos. Si encuentra Blender en la ubicación habitual o como `blender.exe` en `PATH`, valida además el paquete de modelos; para otra ubicación se puede indicar la ruta con `set BLENDER_EXE=C:\ruta\blender.exe`. Para instalar Blender y Unity en otro PC, usa sus instaladores oficiales. El proyecto Unity está fijado a 6000.6.4f1 y se abre desde Unity Hub tras instalar esa versión.

Para compilar e instalar Server Doctor hace falta, además, un servidor dedicado SCP:SL con LabAPI instalado. Sus assemblies no se distribuyen:

Desde la raíz:

```powershell
.\ServerDoctor\build.cmd "D:\SCP Server\SCPSL_Data\Managed"
```

El DLL se compila para la versión del servidor cuyas referencias indicaste. Sigue la [guía de instalación de Server Doctor](./ServerDoctor/README.md) para copiarlo a la carpeta de plugins LabAPI correspondiente. Esto no modifica por sí solo una instalación del servidor. El flujo de GitHub Actions y `verify.cmd` no reemplazan una compilación contra los assemblies reales ni una prueba en staging.

## Publicar este repositorio en GitHub

El repositorio local está preparado con un commit inicial. Crea un repositorio **vacío** en GitHub (sin README, licencia ni `.gitignore` inicial), añade su URL y publica la rama:

```powershell
git remote add origin <URL-del-repositorio>
git push -u origin main
```

Antes de publicar, confirma la lista con `git status`. Los recursos 3D se conservan en Git deliberadamente; las cachés de Unity, salidas de compilación, DLL del juego y secretos locales están excluidos. El archivo más grande debe permanecer por debajo del límite normal de 100 MB por archivo de GitHub. El contenido 3D y los esquemáticos propios llevan aviso CC BY 4.0 en [`AssetLibrary/CONTENT-LICENSE.md`](./AssetLibrary/CONTENT-LICENSE.md); el código utiliza MIT.

## Alcance y licencias

El código fuente conserva licencia MIT en [`LICENSE`](./LICENSE) y Server Doctor incluye su aviso local correspondiente en [`ServerDoctor/LICENSE`](./ServerDoctor/LICENSE). Los modelos, texturas, previews y ejemplos de esquemáticos originales tienen CC BY 4.0, según [`AssetLibrary/CONTENT-LICENSE.md`](./AssetLibrary/CONTENT-LICENSE.md).

La copia de trabajo local de ProjectMER, sus DLL y dependencias descargadas no se incluyen. Tampoco se incluyen ejecutables compilados, assemblies del servidor ni los demás proyectos que no se crearon para esta entrega.

## Límites de validación

La revisión Blender comprueba escenas fuente y reimporta los FBX/GLB/proxies con Blender. El proyecto PentagolandAssetReview sirve para inspeccionar importación y materiales con Unity 6000.6.4f1, que no coincide con el player local 6000.0.43f1. No certifica bundles, importación en el editor exacto del juego, colisiones, compatibilidad de red ni rendimiento en partida. Prueba los esquemáticos y el plugin en una instancia de staging con las versiones previstas antes de desplegarlos.

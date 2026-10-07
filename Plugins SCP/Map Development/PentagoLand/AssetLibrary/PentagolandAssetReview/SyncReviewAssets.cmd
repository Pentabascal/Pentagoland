@echo off
setlocal EnableExtensions

set "PROJECT_DIR=%~dp0"
for %%R in ("%PROJECT_DIR%..\Assets") do set "SOURCE_ROOT=%%~fR"
set "RENDER_ROOT=%PROJECT_DIR%Assets\SourceAssets"
set "COLLISION_ROOT=%PROJECT_DIR%Assets\CollisionProxies_Unassigned"
set "MATERIAL_REFERENCE_SOURCE=%PROJECT_DIR%..\UnityMaterialWorkbench\Assets\Habitacion"
set "MATERIAL_REFERENCE_DEST=%PROJECT_DIR%Assets\MaterialReferences\Habitacion"
set "POLYHAVEN_SOURCE=%PROJECT_DIR%..\ExternalAssets\PolyHaven\MetalPlate"
set "POLYHAVEN_DEST=%PROJECT_DIR%Assets\MaterialReferences\PolyHaven\MetalPlate"

for %%P in (PentagolandUtilityProps PentagolandFacilityProps PentagolandRelayTerminal) do call :SyncPack "%%P" || exit /b 1

if not exist "%MATERIAL_REFERENCE_SOURCE%\" (
    echo Missing Unity material reference textures: "%MATERIAL_REFERENCE_SOURCE%" 1>&2
    exit /b 1
)
if not exist "%MATERIAL_REFERENCE_DEST%\" (
    mkdir "%MATERIAL_REFERENCE_DEST%"
    if errorlevel 1 exit /b 1
)
robocopy "%MATERIAL_REFERENCE_SOURCE%" "%MATERIAL_REFERENCE_DEST%" *.png /R:1 /W:1 /NFL /NDL /NJH /NJS >nul
if errorlevel 8 (
    echo Failed copying Unity material reference textures. 1>&2
    exit /b 1
)
if not exist "%POLYHAVEN_SOURCE%\" (
    echo Missing licensed Poly Haven reference textures: "%POLYHAVEN_SOURCE%" 1>&2
    exit /b 1
)
if not exist "%POLYHAVEN_DEST%\" (
    mkdir "%POLYHAVEN_DEST%"
    if errorlevel 1 exit /b 1
)
robocopy "%POLYHAVEN_SOURCE%" "%POLYHAVEN_DEST%" *.jpg README.md /R:1 /W:1 /NFL /NDL /NJH /NJS >nul
if errorlevel 8 (
    echo Failed copying Poly Haven material textures. 1>&2
    exit /b 1
)

if exist "%SOURCE_ROOT%\PentagolandUtilityProps\asset_manifest.json" (
    copy /Y "%SOURCE_ROOT%\PentagolandUtilityProps\asset_manifest.json" "%RENDER_ROOT%\PentagolandUtilityProps\asset_manifest.json" >nul
    if errorlevel 1 (
        echo Failed copying the utility asset manifest. 1>&2
        exit /b 1
    )
)

echo Asset copies synchronized: render models, textures, collision proxies and material references.
exit /b 0

:SyncPack
set "PACK=%~1"
set "SOURCE=%SOURCE_ROOT%\%PACK%"
set "RENDER_DEST=%RENDER_ROOT%\%PACK%"
set "COLLISION_DEST=%COLLISION_ROOT%\%PACK%"
set "TEXTURE_SOURCE=%SOURCE%\Textures"
set "TEXTURE_DEST=%RENDER_DEST%\Textures"

if not exist "%SOURCE%\" (
    echo Missing asset source pack: "%SOURCE%" 1>&2
    exit /b 1
)

if not exist "%RENDER_DEST%\" (
    mkdir "%RENDER_DEST%"
    if errorlevel 1 exit /b 1
)
if not exist "%COLLISION_DEST%\" (
    mkdir "%COLLISION_DEST%"
    if errorlevel 1 exit /b 1
)

robocopy "%SOURCE%" "%RENDER_DEST%" *.fbx /XF *-collision.fbx /R:1 /W:1 /NFL /NDL /NJH /NJS >nul
if errorlevel 8 (
    echo Failed copying render models for "%PACK%". 1>&2
    exit /b 1
)

if exist "%SOURCE%\asset_manifest.json" (
    copy /Y "%SOURCE%\asset_manifest.json" "%RENDER_DEST%\asset_manifest.json" >nul
    if errorlevel 1 (
        echo Failed copying asset manifest for "%PACK%". 1>&2
        exit /b 1
    )
)

if exist "%TEXTURE_SOURCE%\" (
    if not exist "%TEXTURE_DEST%\" (
        mkdir "%TEXTURE_DEST%"
        if errorlevel 1 exit /b 1
    )
    robocopy "%TEXTURE_SOURCE%" "%TEXTURE_DEST%" *.* /E /R:1 /W:1 /NFL /NDL /NJH /NJS >nul
    if errorlevel 8 (
        echo Failed copying textures for "%PACK%". 1>&2
        exit /b 1
    )
)

robocopy "%SOURCE%" "%COLLISION_DEST%" *-collision.fbx /S /R:1 /W:1 /NFL /NDL /NJH /NJS >nul
if errorlevel 8 (
    echo Failed copying collision proxies for "%PACK%". 1>&2
    exit /b 1
)

echo Synchronized "%PACK%".
exit /b 0

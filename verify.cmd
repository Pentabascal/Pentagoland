@echo off
setlocal
set "ROOT=%~dp0"

dotnet --version >nul 2>&1
if errorlevel 1 (
    echo .NET SDK 10 is required. Install it from https://dotnet.microsoft.com/download/dotnet/10.0
    exit /b 1
)

dotnet run --project "%ROOT%ServerDoctor\tests\PentagolandServerDoctor.Tests.csproj" --configuration Release
if errorlevel 1 exit /b 1

dotnet build "%ROOT%AssetLibrary\PentagolandMERTools.csproj" --configuration Release
if errorlevel 1 exit /b 1

dotnet run --project "%ROOT%AssetLibrary\PentagolandMERTools.csproj" --configuration Release --no-build -- --self-test
if errorlevel 1 exit /b 1

dotnet run --project "%ROOT%AssetLibrary\PentagolandMERTools.csproj" --configuration Release --no-build -- "%ROOT%AssetLibrary\Samples\Schematics"
if errorlevel 1 exit /b 1

if defined BLENDER_EXE goto :blender_check
if exist "C:\Program Files\Blender Foundation\Blender 5.2\blender.exe" set "BLENDER_EXE=C:\Program Files\Blender Foundation\Blender 5.2\blender.exe"
if not defined BLENDER_EXE (
    where.exe blender.exe >nul 2>&1
    if not errorlevel 1 set "BLENDER_EXE=blender.exe"
)
if not defined BLENDER_EXE (
    echo Blender 5.2 not found; Blender source and export validation was skipped.
    exit /b 0
)

:blender_check
if not exist "%BLENDER_EXE%" (
    echo Blender executable not found: "%BLENDER_EXE%"
    exit /b 1
)
"%BLENDER_EXE%" --background --python "%ROOT%AssetLibrary\Assets\PentagolandUtilityProps\validate_asset_pack.py"
if errorlevel 1 exit /b 1

echo All available toolkit validations passed.
exit /b 0

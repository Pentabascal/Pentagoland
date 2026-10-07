@echo off
setlocal
set "PROJECT_DIR=%~dp0"
set "UNITY_VERSION=6000.6.4f1"
set "PACKAGE_PATH=%PROJECT_DIR%..\UnityAssetPackage\Pentagoland_Assets.unitypackage"

if not defined UNITY_EDITOR set "UNITY_EDITOR=C:\Program Files\Unity\Hub\Editor\%UNITY_VERSION%\Editor\Unity.exe"
if not exist "%UNITY_EDITOR%" (
    echo Unity Editor %UNITY_VERSION% was not found at "%UNITY_EDITOR%".
    echo Install that editor in Unity Hub, or set UNITY_EDITOR to its Unity.exe path.
    exit /b 1
)

pushd "%PROJECT_DIR%" || exit /b 1
if not exist "%PROJECT_DIR%Logs\" mkdir "%PROJECT_DIR%Logs"
if errorlevel 1 (
    popd
    exit /b 1
)

"%UNITY_EDITOR%" -version > "%PROJECT_DIR%Logs\editor-version.log" 2>&1
if errorlevel 1 (
    echo Could not query Unity Editor version. See "%PROJECT_DIR%Logs\editor-version.log".
    popd
    exit /b 1
)
findstr /C:"%UNITY_VERSION%" "%PROJECT_DIR%Logs\editor-version.log" >nul
if errorlevel 1 (
    echo Unity Editor %UNITY_VERSION% is required. See "%PROJECT_DIR%Logs\editor-version.log".
    popd
    exit /b 1
)

call "%PROJECT_DIR%SyncReviewAssets.cmd"
if errorlevel 1 (
    popd
    exit /b 1
)

"%UNITY_EDITOR%" -batchmode -nographics -quit -projectPath "%PROJECT_DIR:~0,-1%" -executeMethod Pentagoland.AssetReview.Editor.AssetReviewBuilder.Build -logFile "%PROJECT_DIR%Logs\asset-review.log"
if errorlevel 1 (
    echo Unity returned a failure. See "%PROJECT_DIR%Logs\asset-review.log".
    popd
    exit /b 1
)

powershell.exe -NoProfile -NonInteractive -Command "$expectedImports = 113; $report = Import-Csv -LiteralPath '%PROJECT_DIR%Assets\Reports\unity-import-report.csv'; $collisions = Import-Csv -LiteralPath '%PROJECT_DIR%Assets\Reports\unity-collision-report.csv'; $failed = @($report | Where-Object { $_.Status -ne 'OK' }); $failedCollisions = @($collisions | Where-Object { $_.Status -ne 'OK' -or $_.EnabledByDefault -ne 'false' }); $summary = Get-Content -LiteralPath '%PROJECT_DIR%Assets\Reports\unity-import-summary.txt' -Raw; $materials = @(Get-ChildItem -LiteralPath '%PROJECT_DIR%Assets\Generated\Materials' -Filter '*.mat' -File); $package = Get-Item -LiteralPath '%PACKAGE_PATH%' -ErrorAction SilentlyContinue; if ($report.Count -ne $expectedImports -or $collisions.Count -ne $expectedImports -or $failed.Count -ne 0 -or $failedCollisions.Count -ne 0 -or $materials.Count -ne 5 -or $null -eq $package -or $package.Length -eq 0 -or $summary -notmatch 'Failures: 0' -or $summary -notmatch ('Render FBX: ' + $expectedImports) -or $summary -notmatch ('Successful imports: ' + $expectedImports) -or $summary -notmatch ('Collision proxy FBX: ' + $expectedImports) -or $summary -notmatch 'URP material presets: 5') { Write-Error 'Expected 113 successful imports, 113 disabled collision prefabs, five URP materials, and a non-empty Unity package.'; exit 1 }"
if errorlevel 1 (
    echo Asset review validation failed. See "%PROJECT_DIR%Assets\Reports".
    popd
    exit /b 1
)

echo Unity %UNITY_VERSION% imported the asset library. Import "%PACKAGE_PATH%" into your Unity project; inspect Assets\Reports first.
popd
exit /b 0

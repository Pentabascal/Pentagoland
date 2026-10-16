@echo off
setlocal

if /I "%~1"=="test" goto :test

if not "%~1"=="" set "SL_REFERENCES=%~1"
if "%SL_REFERENCES%"=="" (
    echo Pass the server SCPSL_Data\Managed directory or set SL_REFERENCES.
    exit /b 1
)

for %%F in (Assembly-CSharp.dll CommandSystem.Core.dll LabApi.dll Mirror.dll UnityEngine.CoreModule.dll) do (
    if not exist "%SL_REFERENCES%\%%F" (
        echo Required server reference not found: "%SL_REFERENCES%\%%F"
        exit /b 1
    )
)

dotnet run --project "%~dp0tests\PentagolandServerDoctor.Tests.csproj"
if errorlevel 1 exit /b 1

dotnet build "%~dp0PentagolandServerDoctor.sln" --configuration Release /p:SL_REFERENCES="%SL_REFERENCES%"
exit /b %errorlevel%

:test
dotnet run --project "%~dp0tests\PentagolandServerDoctor.Tests.csproj"
if errorlevel 1 exit /b 1
exit /b 0

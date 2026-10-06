# Unity material workbench

Portable Unity project for the Pentagoland material and asset experiment.

- Unity Editor: **6000.6.4f1**
- Project assets and settings are under `Assets/` and `ProjectSettings/`.
- `Packages/manifest.json` and `Packages/packages-lock.json` pin the project dependencies.
- Unity-generated `Library/`, `Temp/`, `Logs/`, `UserSettings/` and IDE files are intentionally not included; Unity recreates them when the project is opened.

Open this directory in Unity Hub after installing the specified editor. Allow package restore and script compilation to finish before inspecting scenes. This is an authoring/review project, not an SCP:SL server plugin or a validated AssetBundle build.

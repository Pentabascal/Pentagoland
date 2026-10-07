# Pentagoland MER Tools

Offline quality checks for ProjectMER schematic folders. This utility does not install SCP:SL, start a server, modify schematic files, or load Unity/ProjectMER assemblies.

## Validate schematics

Point it at the `LabAPI/configs/ProjectMER/Schematics` directory:

```powershell
dotnet run --project .\PentagolandMERTools.csproj -- "D:\SCP Server\AppData\SCP Secret Laboratory\LabAPI\configs\ProjectMER\Schematics"
```

The validator checks the expected `<name>\<name>.json` layout, JSON syntax and duplicate keys, root and block IDs, parent references, hierarchy cycles, vector shapes, block-property requirements, optional rigidbody JSON, and local animator references. It returns exit code `0` when there are no errors, `1` when the files have errors, and `2` for invocation or I/O failures. Warnings do not change the exit code.

Run its built-in fixtures with:

```powershell
dotnet run --project .\PentagolandMERTools.csproj -- --self-test
```

The included native schematic can be validated with:

```powershell
dotnet run --project .\PentagolandMERTools.csproj -- .\Samples\Schematics
```

The block IDs and block types were checked against the local ProjectMER source. In that source, `SchematicBlockData.Create` handles Empty, Primitive, Light, Pickup, Workstation, Text, Interactable, and Waypoint. The declared Schematic, Teleport, and Locker block types currently fall through to an empty-object fallback; the validator reports those as warnings instead of claiming that they work.

This is a static preflight, not a game test. It cannot verify SCP:SL version compatibility, Unity serialization, AssetBundle contents, network visibility, colliders, lighting, performance, or the final in-game appearance. Keep backups and test assets on a staging server before using them in production.

## Blender and Unity handoff

The current ProjectMER schematic format builds objects from the game’s supported networked prefabs and primitive toys. A Blender mesh export is not directly a ProjectMER schematic block. MER can load animator controllers from Unity AssetBundles, but this does not make arbitrary Blender meshes load as schematic blocks.

An original low-poly Pentagoland relay terminal is provided under `Assets/PentagolandRelayTerminal`, alongside three facility props (wall alarm, containment cabinet and security bollard) under `Assets/PentagolandFacilityProps` and 109 utility props under `Assets/PentagolandUtilityProps`. The latest eight utility additions cover spill response, PPE dispensing, specimen scanning, air sampling, floor decontamination, lab centrifugation, fire blankets and contamination monitoring. The utility library includes static access, security, medical, maintenance, containment and infrastructure dressing. Each asset pack includes editable Blender source, FBX/GLB interchange exports and preview renders. The Blender utility source retains individual editable mesh pieces; each FBX/GLB prop is exported as one joined render mesh with UVs, 256px tileable base-color, tangent-space normal and roughness maps, and subdued industrial PBR values. Optional compound low-poly box collision proxies are separate FBX files and require review and explicit configuration in Unity. Coordinated primitive-only ProjectMER approximations are under `Samples/Schematics`; those native schematics are the part that can be copied to a staging server. The `.blend`, `.fbx`, and `.glb` files are not ready-to-load SCP:SL assets. Converting their meshes into in-game custom objects would require a Unity-side integration that can load and network those assets; stock MER's schematic block loader does not do this.

These are original static industrial props, not copies of SCP:SL assets. A reader, camera, sign, extinguisher, bulkhead, sensor, server rack or other prop is visual dressing only; it does not implement interaction, animation, access control, detection, recording, or gameplay. The beacon has an actual steady MER light block, but no flashing or event behavior. MER schematic approximations use multiple networked primitive toys per asset; each approximation is limited to a silhouette-oriented subset. Schematics have no custom collision meshes or runtime rigid-body physics; optional Blender compound-box collision proxies are separate FBX files. Benchmark repeated placements on staging. Blender re-import checks verify the interchange files only; validate them in the target Unity/SCP:SL version and staging server before deployment.

Blender generation scripts and rendered previews are included with the asset packs. Test native schematics on a staging server first: this validator cannot verify server-version compatibility, network visibility, collision, lighting, performance, or in-game appearance. Do not deploy them to production without that check.

For offline FBX/material/collision inspection and bulk packaging, run `PentagolandAssetReview\BuildReview.cmd`. It synchronizes the source exports and material-reference textures, creates a review gallery, material study, per-prop prefabs, separate collision prefabs with `MeshCollider`s disabled, and import/collision reports. It exports [`UnityAssetPackage/Pentagoland_Assets.unitypackage`](./UnityAssetPackage/README.md); import that single package into a Unity project using **Assets > Import Package > Custom Package...**. Its URP/Lit material presets require a compatible URP package. This workflow uses Unity 6000.6.4f1, not the local game player's 6000.0.43f1, and does not build AssetBundles or certify in-game compatibility.

The portable [UnityMaterialWorkbench project](./UnityMaterialWorkbench/README.md) contains the separate material and asset experiment copied into this library; its base-color and roughness images are carried into the AssetReview material library and assigned according to their actual PBR roles. Both Unity projects omit generated `Library`, `Temp`, logs and per-user settings; Unity regenerates those after opening the project.

References checked:
- [ProjectMER README](https://github.com/Michal78900/ProjectMER#readme)
- [ProjectMER schematic block creation](https://github.com/Michal78900/ProjectMER/blob/master/Features/Serializable/Schematics/SchematicBlockData.cs)
- [ProjectMER schematic loading and animation-controller handling](https://github.com/Michal78900/ProjectMER/blob/master/Features/Objects/SchematicObject.cs)

# Pentagoland model library

This delivery contains original static assets for an industrial SCP:SL facility setting.

## Contents

- `Assets/PentagolandRelayTerminal/`: one editable Blender source asset and interchange exports.
- `Assets/PentagolandFacilityProps/`: three editable facility props and exports.
- `Assets/PentagolandUtilityProps/`: 109 editable utility props, 109 FBX and GLB render exports, compound collision proxies, 256px PBR texture sets, previews, manifest and generation/validation scripts.
- `Assets/asset_pipeline.py`: shared PBR, UV, joined-mesh export and collision-proxy helper used by the terminal/facility generators.
- `Samples/Schematics/`: 119 native MER examples, including prop approximations and the research annex, decontamination suite, security checkpoint and containment observation bay.
- `ExternalAssets/PolyHaven/`: a 1K CC0 metal-plate PBR texture set with source and license information.
- `UnityAssetPackage/Pentagoland_Assets.unitypackage`: one importable Unity package containing the render library, review prefabs, disabled compound-collider prefabs, URP material presets, reference textures and review scenes.
- `PentagolandAssetReview/`: automated Unity import/material/collision review and reproducible `.unitypackage` exporter.
- `PentagolandMERTools.csproj` and `Program.cs`: offline schematic validator source.

## Performance-oriented defaults

Each Blender source keeps its detailed pieces separately editable. Render FBX/GLB exports combine a prop into one joined render-mesh object while preserving material assignments and UVs. PBR colors use restrained graphite, steel, enamel, and safety/status accents; tileable 256px base-color, tangent-space normal, and roughness textures are generated per material. Separate collision FBX files contain up to six low-poly axis-aligned boxes selected from major prop forms. Unity produces separate prefabs with disabled compound `MeshCollider` components for manual review; they are not assigned to render prefabs or certified player colliders.

MER schematics use a small selection of built-in visible-only primitives per prop to limit networked objects. They are decorative and do not provide interactions, physics, animations or custom Blender meshes. Validate quantity, placement and appearance on a staging server.

The final ten utility additions are the elevator call panel, ceiling emergency light, wall hydrant cabinet, floor service hatch, chemical storage locker, lab centrifuge, portable generator, archive terminal, decontamination sink, and secure storage safe.

The latest 24 additions cover hazmat storage, air scrubbing, specimen refrigeration, laboratory ventilation, eyewash, access control, tool storage, containment pressure monitoring, emergency shower systems, a biometric keypad, radio charging, cable routing, a reactor coolant pump, fire alarms, sterilization, isolation controls, air sampling, battery storage, reinforced containment shipping, specimen dispensing, cryogenic transport, drain service, and secure equipment storage.

Build the importable Unity package with `PentagolandAssetReview\BuildReview.cmd`. Then import `UnityAssetPackage\Pentagoland_Assets.unitypackage` in Unity using **Assets > Import Package > Custom Package...**. The package materials use URP/Lit; the target project needs a compatible URP installation. This workflow validates Unity import in 6000.6.4f1, not SCP:SL's Unity 6000.0.43f1, AssetBundles, runtime physics, or server integration.

## Validate

Run Blender's asset round-trip validation from this folder:

```powershell
& "C:\Program Files\Blender Foundation\Blender 5.2\blender.exe" --background --python .\Assets\PentagolandUtilityProps\validate_asset_pack.py
```

Run the offline MER validation:

```powershell
dotnet run --project .\PentagolandMERTools.csproj -- --self-test
dotnet run --project .\PentagolandMERTools.csproj -- .\Samples\Schematics
```

The Blender checks validate the three source scenes and round-trip all 226 render FBX/GLB files plus 113 collision proxies; they do not validate Unity import settings, collider behavior, network cost or in-game compatibility. Test with the intended Unity, SCP:SL, LabAPI and ProjectMER versions before deployment.

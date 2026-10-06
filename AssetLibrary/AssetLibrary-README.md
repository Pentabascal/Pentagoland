# Pentagoland model library

This delivery contains original static assets for an industrial SCP:SL facility setting.

## Contents

- `Assets/PentagolandRelayTerminal/`: one editable Blender source asset and interchange exports.
- `Assets/PentagolandFacilityProps/`: three editable facility props and exports.
- `Assets/PentagolandUtilityProps/`: 77 editable utility props, 77 FBX and GLB render exports, 77 optional collision proxies, previews, textures, manifest and generation/validation scripts.
- `Assets/asset_pipeline.py`: shared PBR, UV, joined-mesh export and collision-proxy helper used by the terminal/facility generators.
- `Samples/Schematics/`: native MER examples, including all 77 utility props and the earlier facility examples.
- `PentagolandMERTools.csproj` and `Program.cs`: offline schematic validator source.

## Performance-oriented defaults

Each Blender source keeps its detailed pieces separately editable. Render FBX/GLB exports combine a prop into one joined render-mesh object while preserving material assignments and UVs. PBR colors use restrained graphite, steel, enamel, and safety/status accents; base-color and tangent-space normal textures are packed at modest resolution. The separate collision FBX is only a simple axis-aligned bounds proxy: inspect it and decide whether/how to use it in Unity. It is not a certified player collider, dynamic physics body, or gameplay feature.

MER schematics use a small selection of built-in visible-only primitives per prop to limit networked objects. They are decorative and do not provide interactions, physics, animations or custom Blender meshes. Validate quantity, placement and appearance on a staging server.

The final ten utility additions are the elevator call panel, ceiling emergency light, wall hydrant cabinet, floor service hatch, chemical storage locker, lab centrifuge, portable generator, archive terminal, decontamination sink, and secure storage safe.

The newest ten utility additions are the gas detector, wall patch panel, portable decontamination unit, ceiling dome camera, emergency shutoff valve, specimen transport case, wall charging station, mobile medical monitor, industrial water pump, and mobile evidence cart.

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

The Blender checks validate the three source scenes and round-trip all 162 render FBX/GLB files plus 81 collision proxies; they do not validate Unity import settings, collider behavior, network cost or in-game compatibility. Test with the intended Unity, SCP:SL, LabAPI and ProjectMER versions before deployment.

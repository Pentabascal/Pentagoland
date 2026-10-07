# Pentagoland Facility Props

Three original static props designed for a SCP:SL facility:

- **Security alarm:** wall-mounted alarm panel with beacon, display, controls and speaker grille.
- **Containment cabinet:** reinforced cabinet with inspection window, status strip, handle and vents.
- **Security bollard:** floor-mounted steel post with colored safety bands and anchor bolts.

## Files

- `PentagolandFacilityProps.blend`: editable Blender scene containing all three models and a lightweight studio setup.
- `<AssetName>.fbx` and `<AssetName>.glb`: individual Unity/interchange exports.
- `<AssetName>-collision.fbx`: optional compound low-poly box collider proxy, separate from the render mesh.
- `Textures/`: packed PBR base-color, tangent-space normal and roughness textures.
- `<AssetName>-preview.png`: individual preview renders.
- `create_props.py`: reproducible Blender generation/export script.
- `../../Samples/Schematics/`: coordinated, primitive-only ProjectMER approximations.

The meshes and exports are not directly loadable by stock ProjectMER. Each schematic approximation uses built-in MER primitives and remains visible-only; validate placement and appearance on a staging server. Exports use one joined render mesh per prop with UVs and packed PBR maps. Unity review prefabs keep separate compound collision `MeshCollider`s disabled by default; inspect and explicitly configure the proxies before use. They are not tested player colliders, and no dynamic physics is configured. Unity import, server integration and in-game performance are not verified here.

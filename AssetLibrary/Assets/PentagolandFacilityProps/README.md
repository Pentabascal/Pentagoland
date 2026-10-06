# Pentagoland Facility Props

Three original static props designed for a SCP:SL facility:

- **Security alarm:** wall-mounted alarm panel with beacon, display, controls and speaker grille.
- **Containment cabinet:** reinforced cabinet with inspection window, status strip, handle and vents.
- **Security bollard:** floor-mounted steel post with colored safety bands and anchor bolts.

## Files

- `PentagolandFacilityProps.blend`: editable Blender scene containing all three models and a lightweight studio setup.
- `<AssetName>.fbx` and `<AssetName>.glb`: individual Unity/interchange exports.
- `<AssetName>-collision.fbx`: optional conservative bounds-only collider proxy.
- `Textures/`: packed PBR base-color and tangent-space normal textures.
- `<AssetName>-preview.png`: individual preview renders.
- `create_props.py`: reproducible Blender generation/export script.
- `../../Samples/Schematics/`: coordinated, primitive-only ProjectMER approximations.

The meshes and exports are not directly loadable by stock ProjectMER. Each schematic approximation uses built-in MER primitives and remains visible-only; validate placement and appearance on a staging server. Exports use one joined render mesh per prop with UVs and packed PBR maps. Review and explicitly assign the separate bounds proxy in Unity; it is not a tested player collider and no dynamic physics is configured. Unity import, server integration and in-game performance are not verified here.

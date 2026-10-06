# Pentagoland Relay Terminal

Original static prop for an industrial facility/server-room setting. The subdued graphite, enamel, steel and restrained safety/status accents are designed to sit more naturally in a neutral SCP:SL environment.

## Files

- `PentagolandRelayTerminal.blend`: editable Blender scene, source geometry, materials, studio lighting and camera.
- `PentagolandRelayTerminal.fbx`: mesh/material handoff for a Unity project.
- `PentagolandRelayTerminal.glb`: portable preview/interchange export.
- `PentagolandRelayTerminal-collision.fbx`: conservative bounds-only proxy for optional review in Unity.
- `Textures/`: packed base-color and tangent-space normal map images.
- `PentagolandRelayTerminal-preview.png`: rendered preview.
- `create_asset.py`: reproducible Blender Python build script.
- `../asset_pipeline.py`: shared UV, PBR material, joined-mesh export and optional collision-proxy helpers.
- `../../Samples/Schematics/PentagolandRelayTerminal/`: native ProjectMER primitive approximation.

Model proportions are approximately 1.16 m wide and 1.9 m high. The custom-mesh exports are not directly loadable by stock ProjectMER. Use them in Unity only if a separate, compatible plugin or game-side asset pipeline is available. The native MER approximation uses supported primitive toys; it is decorative, deliberately non-collidable, and still needs in-game staging validation.

The source was generated with Blender 5.2.2 LTS. FBX axis conversion is set for a Y-up Unity import; interchange files contain one joined render mesh, packed PBR textures and a shared origin at the floor center. Review the separate bounds proxy before explicitly assigning it as a Unity collider; it is not a tested player collider and no rigid-body simulation is configured. Unity shader import, batching, occlusion, networking and SCP:SL compatibility still need target-project and staging validation.

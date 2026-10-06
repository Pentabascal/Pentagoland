import json
import math
import os

import bpy


ROOT = os.path.dirname(os.path.abspath(__file__))
MANIFEST_PATH = os.path.join(ROOT, "asset_manifest.json")
BLEND_PATH = os.path.join(ROOT, "PentagolandUtilityProps.blend")
TEXTURES_PATH = os.path.join(ROOT, "Textures")
EXPECTED_ASSET_COUNT = 77


def validate_render_file(filepath, name, extension):
    if not os.path.isfile(filepath) or os.path.getsize(filepath) == 0:
        raise RuntimeError(f"Missing or empty interchange file: {filepath}")

    bpy.ops.wm.read_factory_settings(use_empty=True)
    if extension == ".fbx":
        bpy.ops.import_scene.fbx(filepath=filepath)
    else:
        bpy.ops.import_scene.gltf(filepath=filepath)

    meshes = [obj for obj in bpy.data.objects if obj.type == "MESH"]
    if len(meshes) != 1 or len(meshes[0].data.polygons) == 0:
        raise RuntimeError(
            f"{name}{extension} should import as one non-empty joined mesh; "
            f"found {len(meshes)} mesh objects."
        )
    mesh = meshes[0]
    if any(not math.isfinite(component) or component <= 0 for component in mesh.dimensions):
        raise RuntimeError(f"{name}{extension} has invalid or degenerate dimensions.")
    if not mesh.data.uv_layers or not mesh.material_slots:
        raise RuntimeError(f"{name}{extension} is missing UVs or material assignments.")
    if not all(slot.material for slot in mesh.material_slots):
        raise RuntimeError(f"{name}{extension} contains an empty material slot.")
    textured_materials = [
        slot.material
        for slot in mesh.material_slots
        if slot.material.node_tree is not None
        and any(
            node.type == "TEX_IMAGE" and node.image is not None
            for node in slot.material.node_tree.nodes
        )
    ]
    if not textured_materials:
        raise RuntimeError(f"{name}{extension} has no imported base-color image texture.")
    if extension == ".glb" and not any(
        node.type == "NORMAL_MAP"
        for slot in mesh.material_slots
        if slot.material and slot.material.node_tree is not None
        for node in slot.material.node_tree.nodes
    ):
        raise RuntimeError(f"{name}{extension} has no imported tangent-space normal map.")


def validate_collision_file(filepath, name):
    if not os.path.isfile(filepath) or os.path.getsize(filepath) == 0:
        raise RuntimeError(f"Missing or empty optional collision proxy: {filepath}")
    bpy.ops.wm.read_factory_settings(use_empty=True)
    bpy.ops.import_scene.fbx(filepath=filepath)
    collision_meshes = [obj for obj in bpy.data.objects if obj.type == "MESH"]
    if len(collision_meshes) != 1 or len(collision_meshes[0].data.polygons) == 0:
        raise RuntimeError(f"{name} collision proxy should import as one non-empty mesh.")


def main():
    with open(MANIFEST_PATH, encoding="utf-8") as manifest_file:
        manifest = json.load(manifest_file)
    assets = manifest.get("assets")
    if not isinstance(assets, list) or len(assets) != EXPECTED_ASSET_COUNT:
        raise RuntimeError(f"asset_manifest.json must list {EXPECTED_ASSET_COUNT} utility models.")
    texture_files = [
        os.path.join(TEXTURES_PATH, name)
        for name in os.listdir(TEXTURES_PATH)
        if name.lower().endswith(".png")
    ] if os.path.isdir(TEXTURES_PATH) else []
    if len(texture_files) < 10 or any(os.path.getsize(path) == 0 for path in texture_files):
        raise RuntimeError("Expected ten non-empty packed base-color and normal textures.")

    if not os.path.isfile(BLEND_PATH):
        raise RuntimeError(f"Missing editable Blender source: {BLEND_PATH}")
    bpy.ops.wm.open_mainfile(filepath=BLEND_PATH)
    missing_collections = [
        asset["name"]
        for asset in assets
        if bpy.data.collections.get(asset["name"]) is None
    ]
    if missing_collections:
        raise RuntimeError(f"Blender source is missing asset collections: {missing_collections}")

    checked = 0
    collisions = 0
    for asset in assets:
        name = asset["name"]
        preview_path = os.path.join(ROOT, name + "-preview.png")
        if not os.path.isfile(preview_path) or os.path.getsize(preview_path) == 0:
            raise RuntimeError(f"Missing or empty preview render: {preview_path}")
        for extension in (".fbx", ".glb"):
            filepath = os.path.join(ROOT, name + extension)
            validate_render_file(filepath, name, extension)
            checked += 1

        collision_path = os.path.join(ROOT, name + "-collision.fbx")
        validate_collision_file(collision_path, name)
        collisions += 1

    additional_assets = {
        "PentagolandRelayTerminal": ("PentagolandRelayTerminal",),
        "PentagolandFacilityProps": ("SecurityAlarm", "ContainmentCabinet", "SecurityBollard"),
    }
    for pack_name, names in additional_assets.items():
        pack_root = os.path.join(os.path.dirname(ROOT), pack_name)
        source_name = (
            "PentagolandRelayTerminal.blend"
            if pack_name == "PentagolandRelayTerminal"
            else "PentagolandFacilityProps.blend"
        )
        source_path = os.path.join(pack_root, source_name)
        if not os.path.isfile(source_path):
            raise RuntimeError(f"Missing editable Blender source: {source_path}")
        bpy.ops.wm.open_mainfile(filepath=source_path)
        if pack_name == "PentagolandRelayTerminal":
            if not any(obj.get("asset_export") for obj in bpy.data.objects):
                raise RuntimeError(f"{source_name} has no editable relay-terminal asset objects.")
        else:
            missing = [name for name in names if bpy.data.collections.get(name) is None]
            if missing:
                raise RuntimeError(f"{source_name} is missing asset collections: {missing}")

        for name in names:
            preview_path = os.path.join(pack_root, name + "-preview.png")
            if not os.path.isfile(preview_path) or os.path.getsize(preview_path) == 0:
                raise RuntimeError(f"Missing or empty preview render: {preview_path}")
            for extension in (".fbx", ".glb"):
                validate_render_file(os.path.join(pack_root, name + extension), name, extension)
                checked += 1
            validate_collision_file(os.path.join(pack_root, name + "-collision.fbx"), name)
            collisions += 1

    print(
        f"ASSET_PACK_VALIDATION_OK models={len(assets) + 4} "
        f"interchange_files={checked} collision_proxies={collisions} textures={len(texture_files)}"
    )


if __name__ == "__main__":
    main()

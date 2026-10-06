import math
import os
import sys

import bpy
from mathutils import Vector


ROOT = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, os.path.dirname(ROOT))
from asset_pipeline import export_asset, finish_mesh, make_material


BLEND_PATH = os.path.join(ROOT, "PentagolandFacilityProps.blend")
ASSET_NAMES = ("SecurityAlarm", "ContainmentCabinet", "SecurityBollard")


def material(name, color, metallic=0.0, roughness=0.45, emission=0.0):
    return make_material(ROOT, name, color, metallic, roughness, emission)


def finish(obj, name, mat, asset_id):
    finish_mesh(obj, name, mat, asset_id)
    return obj


def box(name, location, dimensions, mat, asset_id, bevel=0.02):
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=location)
    obj = bpy.context.object
    obj.dimensions = dimensions
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    if bevel:
        modifier = obj.modifiers.new("Manufactured edge", "BEVEL")
        modifier.width = bevel
        modifier.segments = 3
        bpy.context.view_layer.objects.active = obj
        bpy.ops.object.modifier_apply(modifier=modifier.name)
    return finish(obj, name, mat, asset_id)


def cylinder(name, location, radius, depth, mat, asset_id, rotation=(0.0, 0.0, 0.0), vertices=20):
    bpy.ops.mesh.primitive_cylinder_add(
        vertices=vertices,
        radius=radius,
        depth=depth,
        location=location,
        rotation=rotation,
    )
    obj = bpy.context.object
    modifier = obj.modifiers.new("Machined edge", "BEVEL")
    modifier.width = min(radius * 0.08, depth * 0.15)
    modifier.segments = 3
    bpy.context.view_layer.objects.active = obj
    bpy.ops.object.modifier_apply(modifier=modifier.name)
    for polygon in obj.data.polygons:
        polygon.use_smooth = len(polygon.vertices) <= 4
    return finish(obj, name, mat, asset_id)


def text_mesh(text, name, location, size, mat, asset_id):
    bpy.ops.object.text_add(location=location, rotation=(math.radians(90), 0.0, 0.0))
    obj = bpy.context.object
    obj.data.body = text
    obj.data.size = size
    obj.data.extrude = 0.001
    bpy.ops.object.convert(target="MESH")
    return finish(bpy.context.object, name, mat, asset_id)


def point_at(obj, target):
    obj.rotation_euler = (Vector(target) - obj.location).to_track_quat("-Z", "Y").to_euler()


def add_security_alarm(mats):
    asset = "SecurityAlarm"
    graphite, shell, trim, pink, violet, cyan, white, green = mats
    box("Alarm | wall housing", (0, 0, 0.54), (0.68, 0.16, 0.88), graphite, asset, 0.045)
    box("Alarm | raised face", (0, -0.09, 0.54), (0.58, 0.035, 0.77), shell, asset, 0.025)
    box("Alarm | top rose beacon", (0, -0.12, 0.81), (0.36, 0.018, 0.06), pink, asset, 0.008)
    box("Alarm | display", (0, -0.12, 0.66), (0.43, 0.018, 0.19), graphite, asset, 0.01)
    box("Alarm | display text line", (-0.08, -0.134, 0.68), (0.22, 0.008, 0.018), cyan, asset, 0.003)
    for index, (x, color) in enumerate(((-0.16, green), (0, pink), (0.16, trim)), start=1):
        cylinder(
            f"Alarm | button {index}",
            (x, -0.15, 0.39),
            0.062,
            0.035,
            color,
            asset,
            rotation=(math.radians(90), 0, 0),
        )
    for index in range(4):
        box(
            f"Alarm | speaker slot {index + 1:02d}",
            (0, -0.119, 0.18 + index * 0.038),
            (0.24, 0.012, 0.012),
            trim,
            asset,
            0.003,
        )
    text_mesh("FACILITY ALARM", "Alarm | label", (-0.18, -0.137, 0.755), 0.034, white, asset)


def add_containment_cabinet(mats):
    asset = "ContainmentCabinet"
    graphite, shell, trim, pink, violet, cyan, white, _ = mats
    box("Cabinet | main shell", (0, 0, 0.86), (0.82, 0.42, 1.72), graphite, asset, 0.045)
    box("Cabinet | inset door", (0, -0.225, 0.88), (0.70, 0.035, 1.48), shell, asset, 0.025)
    box("Cabinet | crown plate", (0, -0.25, 1.56), (0.62, 0.018, 0.13), pink, asset, 0.008)
    box("Cabinet | inspection window frame", (0, -0.253, 1.18), (0.46, 0.025, 0.34), trim, asset, 0.012)
    box("Cabinet | inspection glass", (0, -0.27, 1.18), (0.39, 0.012, 0.27), graphite, asset, 0.006)
    box("Cabinet | status stripe", (0, -0.253, 0.65), (0.50, 0.018, 0.028), violet, asset, 0.004)
    box("Cabinet | handle mount", (0.24, -0.255, 0.91), (0.055, 0.025, 0.24), trim, asset, 0.009)
    box("Cabinet | handle", (0.24, -0.28, 0.91), (0.025, 0.025, 0.17), cyan, asset, 0.009)
    for index in range(5):
        box(
            f"Cabinet | vent {index + 1:02d}",
            (0, -0.253, 0.42 + index * 0.043),
            (0.38, 0.012, 0.012),
            trim,
            asset,
            0.003,
        )
    text_mesh("CONTAINMENT", "Cabinet | label", (-0.24, -0.267, 1.525), 0.043, white, asset)


def add_security_bollard(mats):
    asset = "SecurityBollard"
    graphite, shell, trim, pink, violet, cyan, _, _ = mats
    box("Bollard | anchor plate", (0, 0, 0.10), (0.62, 0.62, 0.20), graphite, asset, 0.045)
    cylinder("Bollard | steel post", (0, 0, 0.79), 0.17, 1.25, shell, asset, vertices=24)
    cylinder("Bollard | foot collar", (0, 0, 0.25), 0.205, 0.14, trim, asset, vertices=24)
    cylinder("Bollard | top cap", (0, 0, 1.43), 0.178, 0.10, graphite, asset, vertices=24)
    for index, (z, color) in enumerate(((0.48, pink), (0.71, violet), (0.94, pink), (1.17, violet)), start=1):
        cylinder(
            f"Bollard | reflective band {index}",
            (0, 0, z),
            0.174,
            0.11,
            color,
            asset,
            vertices=24,
        )
    for index, (x, y) in enumerate(((-0.22, -0.22), (0.22, -0.22), (-0.22, 0.22), (0.22, 0.22)), start=1):
        cylinder(
            f"Bollard | anchor bolt {index}",
            (x, y, 0.207),
            0.035,
            0.012,
            cyan,
            asset,
            vertices=12,
        )


def main():
    bpy.ops.wm.read_factory_settings(use_empty=True)
    bpy.context.preferences.filepaths.save_version = 0
    scene = bpy.context.scene
    scene.unit_settings.system = "METRIC"
    scene.render.engine = "BLENDER_WORKBENCH"
    scene.render.resolution_x = 640
    scene.render.resolution_y = 640
    scene.render.resolution_percentage = 75
    scene.render.image_settings.file_format = "PNG"
    scene.render.image_settings.color_mode = "RGBA"
    scene.render.film_transparent = False
    scene.display.shading.light = "STUDIO"
    scene.display.shading.color_type = "MATERIAL"
    scene.display.shading.show_shadows = True
    scene.display.shading.show_cavity = True
    scene.display.shading.background_type = "WORLD"
    scene.world = bpy.data.worlds.new("Studio | World")
    scene.world.color = (0.035, 0.048, 0.068)

    graphite = material("01 | Graphite polymer", (0.035, 0.045, 0.052), 0.28, 0.42)
    shell = material("02 | Gunmetal shell", (0.10, 0.13, 0.145), 0.42, 0.38)
    trim = material("03 | Brushed steel", (0.32, 0.36, 0.37), 0.55, 0.36)
    pink = material("04 | Safety amber", (0.62, 0.29, 0.11), 0.12, 0.42, 0.04)
    violet = material("05 | Signal blue gray", (0.20, 0.30, 0.34), 0.30, 0.38, 0.025)
    cyan = material("06 | Muted status glass", (0.38, 0.57, 0.59), 0.10, 0.30, 0.06)
    white = material("07 | Enamel marking", (0.54, 0.58, 0.57), 0.08, 0.38, 0.025)
    green = material("08 | Ready indicator", (0.27, 0.38, 0.30), 0.02, 0.40, 0.05)
    backdrop = material("Studio | Midnight", (0.035, 0.048, 0.068), 0, 0.82)
    mats = (graphite, shell, trim, pink, violet, cyan, white, green)

    collections = {}
    for name in ASSET_NAMES:
        collection = bpy.data.collections.new(name)
        scene.collection.children.link(collection)
        collections[name] = collection
        bpy.context.view_layer.active_layer_collection = next(
            layer for layer in bpy.context.view_layer.layer_collection.children if layer.collection == collection
        )
        if name == "SecurityAlarm":
            add_security_alarm(mats)
        elif name == "ContainmentCabinet":
            add_containment_cabinet(mats)
        else:
            add_security_bollard(mats)

    studio = bpy.data.collections.new("STUDIO | Preview only")
    scene.collection.children.link(studio)
    bpy.context.view_layer.active_layer_collection = next(
        layer for layer in bpy.context.view_layer.layer_collection.children if layer.collection == studio
    )
    floor = box("Studio | floor", (0, 0, -0.08), (2000, 2000, 0.1), backdrop, "STUDIO", 0)
    floor["asset_id"] = "STUDIO"

    bpy.ops.object.camera_add(location=(2.1, -3.5, 2.2))
    camera = bpy.context.object
    camera.name = "STUDIO | Camera"
    scene.camera = camera
    camera.data.type = "ORTHO"
    view_direction = Vector((2.1, -3.5, 1.5)).normalized()

    framing = {
        "SecurityAlarm": (0.55, 1.25),
        "ContainmentCabinet": (0.88, 2.25),
        "SecurityBollard": (0.72, 2.0),
    }
    for name, (target_z, ortho_scale) in framing.items():
        target = (0, 0, target_z)
        camera.location = Vector(target) + view_direction * 4.4
        camera.data.ortho_scale = ortho_scale
        point_at(camera, target)
        for collection_name, collection in collections.items():
            collection.hide_render = collection_name != name

        bpy.ops.object.select_all(action="DESELECT")
        selected = [obj for obj in scene.objects if obj.get("asset_id") == name]
        for obj in selected:
            obj.select_set(True)
        bpy.context.view_layer.objects.active = selected[0]

        base = os.path.join(ROOT, name)
        export_asset(selected, base)
        scene.render.filepath = base + "-preview.png"
        bpy.ops.render.render(write_still=True)

    for collection in collections.values():
        collection.hide_render = False
    bpy.ops.object.select_all(action="DESELECT")
    bpy.ops.wm.save_as_mainfile(filepath=BLEND_PATH)
    print(f"FACILITY_PROPS_OK assets={','.join(ASSET_NAMES)}")


if __name__ == "__main__":
    main()

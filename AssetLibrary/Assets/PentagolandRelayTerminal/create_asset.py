import math
import os
import sys

import bpy
from mathutils import Vector


ROOT = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, os.path.dirname(ROOT))
from asset_pipeline import export_asset, finish_mesh, make_material


BLEND_PATH = os.path.join(ROOT, "PentagolandRelayTerminal.blend")
PREVIEW_PATH = os.path.join(ROOT, "PentagolandRelayTerminal-preview.png")


def material(name, color, metallic=0.0, roughness=0.45, emission=0.0):
    return make_material(ROOT, name, color, metallic, roughness, emission)


def finish_asset(obj, name, mat):
    finish_mesh(obj, name, mat, "PentagolandRelayTerminal")
    obj["asset_export"] = True
    return obj


def box(name, location, dimensions, mat, bevel=0.025):
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=location)
    obj = bpy.context.object
    obj.dimensions = dimensions
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    if bevel:
        modifier = obj.modifiers.new("Soft manufactured edges", "BEVEL")
        modifier.width = bevel
        modifier.segments = 3
        bpy.context.view_layer.objects.active = obj
        bpy.ops.object.modifier_apply(modifier=modifier.name)
    return finish_asset(obj, name, mat)


def cylinder(name, location, radius, depth, mat, rotation=(0.0, 0.0, 0.0), vertices=24):
    bpy.ops.mesh.primitive_cylinder_add(
        vertices=vertices,
        radius=radius,
        depth=depth,
        location=location,
        rotation=rotation,
    )
    obj = bpy.context.object
    bevel = obj.modifiers.new("Machined edge", "BEVEL")
    bevel.width = min(radius * 0.12, depth * 0.18)
    bevel.segments = 3
    bpy.context.view_layer.objects.active = obj
    bpy.ops.object.modifier_apply(modifier=bevel.name)
    for polygon in obj.data.polygons:
        polygon.use_smooth = len(polygon.vertices) <= 4
    return finish_asset(obj, name, mat)


def text_mesh(body, name, location, size, mat):
    bpy.ops.object.text_add(location=location, rotation=(math.radians(90), 0.0, 0.0))
    obj = bpy.context.object
    obj.data.body = body
    obj.data.size = size
    obj.data.extrude = 0.001
    obj.data.bevel_depth = 0.0005
    obj.data.align_x = "LEFT"
    bpy.ops.object.convert(target="MESH")
    return finish_asset(bpy.context.object, name, mat)


def point_camera(obj, target):
    obj.rotation_euler = (Vector(target) - obj.location).to_track_quat("-Z", "Y").to_euler()


def main():
    bpy.ops.wm.read_factory_settings(use_empty=True)
    bpy.context.preferences.filepaths.save_version = 0
    scene = bpy.context.scene
    scene.unit_settings.system = "METRIC"
    available_engines = {
        item.identifier for item in scene.render.bl_rna.properties["engine"].enum_items
    }
    scene.render.engine = "BLENDER_EEVEE_NEXT" if "BLENDER_EEVEE_NEXT" in available_engines else "BLENDER_EEVEE"
    scene.render.resolution_x = 760
    scene.render.resolution_y = 760
    scene.render.resolution_percentage = 75
    scene.render.image_settings.file_format = "PNG"
    scene.render.film_transparent = False
    scene.render.filepath = PREVIEW_PATH
    scene.view_settings.view_transform = "AgX"
    scene.render.image_settings.color_mode = "RGBA"

    graphite = material("01 | Graphite polymer", (0.035, 0.045, 0.052), metallic=0.28, roughness=0.42)
    shell = material("02 | Gunmetal shell", (0.10, 0.13, 0.145), metallic=0.42, roughness=0.38)
    trim = material("03 | Edge trim", (0.32, 0.36, 0.37), metallic=0.55, roughness=0.36)
    screen = material("04 | Dark display glass", (0.009, 0.025, 0.04), metallic=0.2, roughness=0.22)
    pink = material("05 | Safety amber", (0.62, 0.29, 0.11), metallic=0.12, roughness=0.42, emission=0.04)
    violet = material("06 | Signal blue gray", (0.20, 0.30, 0.34), metallic=0.30, roughness=0.38, emission=0.025)
    cyan = material("07 | Muted status glass", (0.38, 0.57, 0.59), metallic=0.10, roughness=0.30, emission=0.06)
    white = material("08 | Interface lettering", (0.54, 0.58, 0.57), metallic=0.08, roughness=0.38, emission=0.025)
    green = material("09 | Online indicator", (0.27, 0.38, 0.30), metallic=0.02, roughness=0.40, emission=0.05)
    backdrop = material("Studio | Midnight", (0.025, 0.035, 0.052), metallic=0.0, roughness=0.82)

    # Main housing, freestanding support, and stable base.
    box("Base | lower plinth", (0.0, 0.0, 0.22), (1.02, 0.72, 0.44), graphite, 0.055)
    box("Base | front fascia", (0.0, -0.367, 0.28), (0.88, 0.035, 0.19), shell, 0.018)
    box("Base | rose status strip", (0.0, -0.389, 0.35), (0.58, 0.012, 0.018), pink, 0.006)
    box("Base | top deck", (0.0, 0.0, 0.48), (1.14, 0.82, 0.12), shell, 0.04)
    box("Support | central riser", (0.0, 0.10, 0.91), (0.34, 0.42, 0.78), graphite, 0.035)
    box("Support | front accent", (0.0, -0.116, 0.94), (0.20, 0.018, 0.42), violet, 0.008)

    # Wide, slightly recessed display with restrained UI detail.
    box("Display | outer housing", (0.0, 0.02, 1.48), (1.16, 0.22, 0.82), graphite, 0.055)
    box("Display | perimeter trim", (0.0, -0.101, 1.48), (1.04, 0.035, 0.72), trim, 0.025)
    box("Display | glass", (0.0, -0.123, 1.48), (0.97, 0.018, 0.65), screen, 0.018)
    box("Display UI | top rose line", (-0.245, -0.136, 1.755), (0.40, 0.008, 0.018), pink, 0.005)
    box("Display UI | top violet line", (0.105, -0.136, 1.755), (0.22, 0.008, 0.018), violet, 0.005)
    box("Display UI | top cyan line", (0.337, -0.136, 1.755), (0.16, 0.008, 0.018), cyan, 0.005)
    text_mesh("PENTA RELAY  /  04", "Display UI | title", (-0.405, -0.141, 1.635), 0.061, white)
    text_mesh("FACILITY LINK   ONLINE", "Display UI | subtitle", (-0.405, -0.141, 1.575), 0.034, cyan)
    box("Display UI | data panel", (-0.235, -0.138, 1.455), (0.39, 0.008, 0.012), trim, 0.003)
    box("Display UI | data bar 01", (-0.32, -0.139, 1.405), (0.22, 0.008, 0.022), cyan, 0.004)
    box("Display UI | data bar 02", (-0.245, -0.139, 1.365), (0.37, 0.008, 0.016), violet, 0.004)
    box("Display UI | data bar 03", (-0.285, -0.139, 1.325), (0.29, 0.008, 0.016), pink, 0.004)
    for index, height in enumerate((0.11, 0.17, 0.14, 0.23, 0.19, 0.27)):
        x = 0.12 + index * 0.055
        box(
            f"Display UI | signal graph {index + 1:02d}",
            (x, -0.139, 1.365 + height * 0.5),
            (0.027, 0.009, height),
            cyan if index % 2 else violet,
            0.004,
        )
    cylinder("Display | online lens", (0.505, -0.116, 1.79), 0.025, 0.018, green, rotation=(math.radians(90), 0, 0))

    # Controls sit below the screen and remain simple to replace in Unity.
    for index, (x, mat) in enumerate(((-0.32, cyan), (0.0, violet), (0.32, pink)), start=1):
        cylinder(
            f"Control {index} | bezel",
            (x, -0.403, 0.555),
            0.072,
            0.025,
            trim,
            rotation=(math.radians(90), 0, 0),
        )
        cylinder(
            f"Control {index} | key",
            (x, -0.422, 0.555),
            0.048,
            0.025,
            mat,
            rotation=(math.radians(90), 0, 0),
        )
    for index in range(6):
        z = 0.16 + index * 0.045
        box(f"Left side | cooling slot {index + 1:02d}", (-0.514, -0.05, z), (0.012, 0.30, 0.012), trim, 0.003)
    for x in (-0.43, 0.43):
        cylinder(
            "Base | captive fastener",
            (x, -0.39, 0.25),
            0.018,
            0.012,
            trim,
            rotation=(math.radians(90), 0, 0),
            vertices=16,
        )

    # Studio elements are deliberately not included in the interchange exports.
    studio_floor = box("Studio | floor", (0.0, 0.0, -0.055), (2000.0, 2000.0, 0.10), backdrop, 0.0)
    studio_floor["asset_export"] = False
    bpy.context.scene.camera = None
    bpy.ops.object.camera_add(location=(2.65, -4.25, 2.45))
    camera = bpy.context.object
    camera.name = "Studio | camera"
    camera.data.lens = 52
    point_camera(camera, (0.0, -0.02, 0.92))
    scene.camera = camera

    def area_light(name, location, energy, color, size):
        bpy.ops.object.light_add(type="AREA", location=location)
        light = bpy.context.object
        light.name = name
        light.data.energy = energy
        light.data.color = color
        light.data.shape = "DISK"
        light.data.size = size
        point_camera(light, (0.0, 0.0, 0.95))

    area_light("Studio | key", (-3.0, -3.0, 4.5), 420, (0.74, 0.84, 1.0), 4.0)
    area_light("Studio | rose fill", (2.7, -1.4, 2.7), 190, (1.0, 0.43, 0.68), 3.0)
    area_light("Studio | rim", (0.5, 2.5, 3.2), 360, (0.34, 0.54, 1.0), 2.5)
    scene.world = bpy.data.worlds.new("Studio | World")
    scene.world.color = (0.025, 0.025, 0.025)
    scene.render.resolution_percentage = 75

    asset_objects = [obj for obj in scene.objects if obj.get("asset_export")]
    bpy.ops.object.select_all(action="DESELECT")
    for obj in asset_objects:
        obj.select_set(True)
    bpy.context.view_layer.objects.active = asset_objects[0]

    export_asset(asset_objects, os.path.join(ROOT, "PentagolandRelayTerminal"))

    bpy.ops.object.select_all(action="DESELECT")
    for obj in scene.objects:
        obj.select_set(True)
    bpy.context.view_layer.objects.active = camera
    scene.render.filepath = PREVIEW_PATH
    bpy.ops.render.render(write_still=True)
    bpy.ops.wm.save_as_mainfile(filepath=BLEND_PATH)
    print(f"ASSET_EXPORT_OK objects={len(asset_objects)} blend={BLEND_PATH} preview={PREVIEW_PATH}")


if __name__ == "__main__":
    main()

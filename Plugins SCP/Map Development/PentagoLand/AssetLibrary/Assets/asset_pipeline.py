import math
import os
import re

import bpy
from mathutils import Vector


NORMAL_MAPS = {}


def make_material(root, name, color, metallic=0.0, roughness=0.45, emission=0.0):
    texture_root = os.path.join(root, "Textures")
    os.makedirs(texture_root, exist_ok=True)
    safe_name = re.sub(r"[^A-Za-z0-9_-]+", "_", name).strip("_")

    material = bpy.data.materials.new(name)
    material.diffuse_color = (*color, 1.0)
    shader = material.node_tree.nodes.get("Principled BSDF")
    shader.inputs["Base Color"].default_value = (*color, 1.0)
    shader.inputs["Metallic"].default_value = metallic
    shader.inputs["Roughness"].default_value = roughness
    if "Coat Weight" in shader.inputs:
        shader.inputs["Coat Weight"].default_value = 0.06
        shader.inputs["Coat Roughness"].default_value = 0.34

    color_image = bpy.data.images.new(safe_name + "_BaseColor", width=128, height=128, alpha=True)
    pixels = []
    for y in range(128):
        v = y / 128
        for x in range(128):
            u = x / 128
            grain = (
                0.55 * math.sin(2 * math.pi * (u * 13 + v * 19))
                + 0.30 * math.sin(2 * math.pi * (u * 37 - v * 29))
                + 0.15 * math.sin(2 * math.pi * (u * 59 + v * 47))
            )
            shade = 1.0 + grain * 0.035
            pixels.extend((color[0] * shade, color[1] * shade, color[2] * shade, 1.0))
    color_image.pixels.foreach_set(pixels)
    color_image.file_format = "PNG"
    color_path = os.path.join(texture_root, safe_name + "_BaseColor.png")
    color_image.filepath_raw = color_path
    color_image.save()
    color_image.pack()

    nodes = material.node_tree.nodes
    links = material.node_tree.links
    coordinates = nodes.new("ShaderNodeTexCoord")
    color_texture = nodes.new("ShaderNodeTexImage")
    color_texture.image = color_image
    color_texture.extension = "REPEAT"
    links.new(coordinates.outputs["UV"], color_texture.inputs["Vector"])
    links.new(color_texture.outputs["Color"], shader.inputs["Base Color"])

    normal_path = os.path.join(texture_root, "Shared_MicroNormal.png")
    normal_image = NORMAL_MAPS.get(texture_root)
    if normal_image is None:
        normal_image = bpy.data.images.new("Shared | Micro-surface normal", width=128, height=128, alpha=False)
        pixels = []
        for y in range(128):
            v = y / 128
            for x in range(128):
                u = x / 128
                phase_a = 2 * math.pi * (u * 8 + v * 5)
                phase_b = 2 * math.pi * (u * 17 - v * 13)
                dx = 0.08 * 2 * math.pi * 8 * math.cos(phase_a) - 0.04 * 2 * math.pi * 17 * math.sin(phase_b)
                dy = 0.08 * 2 * math.pi * 5 * math.cos(phase_a) + 0.04 * 2 * math.pi * 13 * math.sin(phase_b)
                nx, ny, nz = -dx * 0.006, -dy * 0.006, 1.0
                length = math.sqrt(nx * nx + ny * ny + nz * nz)
                pixels.extend((nx / length * 0.5 + 0.5, ny / length * 0.5 + 0.5, nz / length * 0.5 + 0.5, 1.0))
        normal_image.pixels.foreach_set(pixels)
        normal_image.file_format = "PNG"
        normal_image.colorspace_settings.name = "Non-Color"
        normal_image.filepath_raw = normal_path
        normal_image.save()
        normal_image.pack()
        NORMAL_MAPS[texture_root] = normal_image

    normal_texture = nodes.new("ShaderNodeTexImage")
    normal_texture.image = normal_image
    normal_map = nodes.new("ShaderNodeNormalMap")
    normal_map.inputs["Strength"].default_value = 0.18
    links.new(coordinates.outputs["UV"], normal_texture.inputs["Vector"])
    links.new(normal_texture.outputs["Color"], normal_map.inputs["Color"])
    links.new(normal_map.outputs["Normal"], shader.inputs["Normal"])

    if emission:
        shader.inputs["Emission Color"].default_value = (*color, 1.0)
        shader.inputs["Emission Strength"].default_value = emission
    material["pbr_texture_set"] = "Packed base-color and tangent-space micro-normal"
    return material


def unwrap_mesh(mesh, scale=4.0):
    uv_layer = mesh.uv_layers.active or mesh.uv_layers.new(name="UVMap")
    for polygon in mesh.polygons:
        dominant_axis = max(range(3), key=lambda axis: abs(polygon.normal[axis]))
        axes = ((1, 2), (0, 2), (0, 1))[dominant_axis]
        for loop_index in polygon.loop_indices:
            vertex = mesh.vertices[mesh.loops[loop_index].vertex_index].co
            uv_layer.data[loop_index].uv = (vertex[axes[0]] * scale, vertex[axes[1]] * scale)


def finish_mesh(obj, name, material, asset_id):
    obj.name = name
    obj.data.name = name + "Mesh"
    unwrap_mesh(obj.data)
    obj.data.materials.append(material)
    obj["asset_id"] = asset_id
    return obj


def _export(operator, path, options):
    supported = {prop.identifier for prop in operator.get_rna_type().properties}
    operator(filepath=path, **{key: value for key, value in options.items() if key in supported})


def export_asset(objects, base_path):
    objects = list(objects)
    if not objects:
        raise ValueError("At least one mesh object is required for asset export.")

    bpy.ops.object.select_all(action="DESELECT")
    for obj in objects:
        obj.select_set(True)
    bpy.context.view_layer.objects.active = objects[0]
    bpy.ops.object.duplicate()
    exported = bpy.context.selected_objects
    joined = bpy.context.view_layer.objects.active
    if len(exported) > 1:
        bpy.ops.object.join()
        joined = bpy.context.view_layer.objects.active
    joined.name = os.path.basename(base_path) + "_Mesh"
    joined.data.name = joined.name
    bpy.context.scene.cursor.location = (0, 0, 0)
    bpy.ops.object.origin_set(type="ORIGIN_CURSOR")

    try:
        _export(
            bpy.ops.export_scene.fbx,
            base_path + ".fbx",
            {
                "use_selection": True,
                "object_types": {"MESH"},
                "apply_unit_scale": True,
                "axis_forward": "-Z",
                "axis_up": "Y",
                "add_leaf_bones": False,
                "bake_anim": False,
                "path_mode": "COPY",
                "embed_textures": True,
            },
        )
        _export(
            bpy.ops.export_scene.gltf,
            base_path + ".glb",
            {"export_format": "GLB", "use_selection": True, "export_apply": True, "export_yup": True},
        )
    finally:
        bpy.data.objects.remove(joined, do_unlink=True)

    points = [obj.matrix_world @ Vector(corner) for obj in objects for corner in obj.bound_box]
    lower = [min(point[axis] for point in points) for axis in range(3)]
    upper = [max(point[axis] for point in points) for axis in range(3)]
    dimensions = tuple(upper[axis] - lower[axis] for axis in range(3))
    center = tuple((lower[axis] + upper[axis]) * 0.5 for axis in range(3))
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=center)
    collision = bpy.context.object
    collision.name = os.path.basename(base_path) + "_CollisionProxy"
    collision.dimensions = dimensions
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    collision["asset_kind"] = "optional_conservative_box_collision"
    bpy.context.scene.cursor.location = (0, 0, 0)
    bpy.ops.object.origin_set(type="ORIGIN_CURSOR")
    bpy.ops.object.select_all(action="DESELECT")
    collision.select_set(True)
    bpy.context.view_layer.objects.active = collision
    try:
        _export(
            bpy.ops.export_scene.fbx,
            base_path + "-collision.fbx",
            {
                "use_selection": True,
                "object_types": {"MESH"},
                "apply_unit_scale": True,
                "axis_forward": "-Z",
                "axis_up": "Y",
                "add_leaf_bones": False,
                "bake_anim": False,
                "embed_textures": False,
            },
        )
    finally:
        bpy.data.objects.remove(collision, do_unlink=True)

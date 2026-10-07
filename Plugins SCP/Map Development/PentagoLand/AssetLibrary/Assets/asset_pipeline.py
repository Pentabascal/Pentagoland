import math
import os
import re

import bpy
from mathutils import Vector


TEXTURE_SIZE = 256


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

    color_pixels = []
    normal_pixels = []
    roughness_pixels = []
    phase = sum(ord(character) for character in safe_name) * 0.017
    tau = 2.0 * math.pi
    for y in range(TEXTURE_SIZE):
        v = y / TEXTURE_SIZE
        for x in range(TEXTURE_SIZE):
            u = x / TEXTURE_SIZE
            first = tau * (u * 7 + v * 11) + phase
            second = tau * (u * 19 - v * 13) + phase * 1.7
            third = tau * (u * 37 + v * 29) + phase * 0.7
            grain = 0.5 * math.sin(first) + 0.32 * math.sin(second) + 0.18 * math.sin(third)
            scuff = max(0.0, math.sin(tau * (u * 5 - v * 3) + phase * 2.3) - 0.91)
            shade = max(0.72, min(1.16, 1.0 + grain * 0.055 - scuff * 0.34))
            for channel in color:
                linear = max(0.0, min(1.0, channel * shade))
                srgb = linear * 12.92 if linear <= 0.0031308 else 1.055 * linear ** (1.0 / 2.4) - 0.055
                color_pixels.append(srgb)
            color_pixels.append(1.0)

            roughness_value = max(0.12, min(0.96, roughness + grain * 0.045 + scuff * 0.18))
            roughness_pixels.extend((roughness_value, roughness_value, roughness_value, 1.0))

            derivative_u = (
                0.5 * 7 * math.cos(first)
                + 0.32 * 19 * math.cos(second)
                + 0.18 * 37 * math.cos(third)
            )
            derivative_v = (
                0.5 * 11 * math.cos(first)
                - 0.32 * 13 * math.cos(second)
                + 0.18 * 29 * math.cos(third)
            )
            nx, ny, nz = -derivative_u * 0.003, -derivative_v * 0.003, 1.0
            normal_length = math.sqrt(nx * nx + ny * ny + nz * nz)
            normal_pixels.extend((
                nx / normal_length * 0.5 + 0.5,
                ny / normal_length * 0.5 + 0.5,
                nz / normal_length * 0.5 + 0.5,
                1.0,
            ))

    def save_texture(image_name, suffix, pixels, non_color=False):
        image = bpy.data.images.new(
            image_name,
            width=TEXTURE_SIZE,
            height=TEXTURE_SIZE,
            alpha=True,
        )
        image.pixels.foreach_set(pixels)
        if non_color:
            image.colorspace_settings.name = "Non-Color"
        image.file_format = "PNG"
        image_path = os.path.join(texture_root, safe_name + suffix + ".png")
        image.filepath_raw = image_path
        image.save()
        image.pack()
        return image, os.path.relpath(image_path, root)

    color_image, color_path = save_texture(safe_name + "_BaseColor", "_BaseColor", color_pixels)
    normal_image, normal_path = save_texture(
        safe_name + "_Normal", "_Normal", normal_pixels, non_color=True
    )
    roughness_image, roughness_path = save_texture(
        safe_name + "_Roughness", "_Roughness", roughness_pixels, non_color=True
    )

    nodes = material.node_tree.nodes
    links = material.node_tree.links
    coordinates = nodes.new("ShaderNodeTexCoord")
    color_texture = nodes.new("ShaderNodeTexImage")
    color_texture.image = color_image
    color_texture.extension = "REPEAT"
    links.new(coordinates.outputs["UV"], color_texture.inputs["Vector"])
    links.new(color_texture.outputs["Color"], shader.inputs["Base Color"])

    normal_texture = nodes.new("ShaderNodeTexImage")
    normal_texture.image = normal_image
    normal_texture.image.colorspace_settings.name = "Non-Color"
    normal_map = nodes.new("ShaderNodeNormalMap")
    normal_map.inputs["Strength"].default_value = 0.24
    links.new(coordinates.outputs["UV"], normal_texture.inputs["Vector"])
    links.new(normal_texture.outputs["Color"], normal_map.inputs["Color"])
    links.new(normal_map.outputs["Normal"], shader.inputs["Normal"])

    roughness_texture = nodes.new("ShaderNodeTexImage")
    roughness_texture.image = roughness_image
    roughness_texture.image.colorspace_settings.name = "Non-Color"
    links.new(coordinates.outputs["UV"], roughness_texture.inputs["Vector"])
    links.new(roughness_texture.outputs["Color"], shader.inputs["Roughness"])

    if emission:
        shader.inputs["Emission Color"].default_value = (*color, 1.0)
        shader.inputs["Emission Strength"].default_value = emission
    material["pbr_texture_set"] = "256px tileable base-color, tangent-space normal, and roughness maps"
    material["base_color_texture"] = color_path
    material["normal_texture"] = normal_path
    material["roughness_texture"] = roughness_path
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


def collision_source_objects(source_objects, max_parts=6):
    candidates = []
    for obj in source_objects:
        if obj.type != "MESH":
            continue
        corners = [obj.matrix_world @ Vector(corner) for corner in obj.bound_box]
        minimum = tuple(min(point[axis] for point in corners) for axis in range(3))
        maximum = tuple(max(point[axis] for point in corners) for axis in range(3))
        dimensions = tuple(maximum[axis] - minimum[axis] for axis in range(3))
        volume = math.prod(dimensions)
        if volume > 0.0:
            candidates.append((volume, minimum, maximum, dimensions))

    if not candidates:
        raise ValueError("At least one non-degenerate source mesh is required for collision proxies.")

    largest_volume = max(candidate[0] for candidate in candidates)
    chosen = []
    for candidate in sorted(candidates, key=lambda item: item[0], reverse=True):
        volume, minimum, maximum, dimensions = candidate
        if volume < largest_volume * 0.035 or any(dimension < 0.025 for dimension in dimensions):
            continue
        fully_covered = any(
            all(
                minimum[axis] >= outer[1][axis] - 0.015
                and maximum[axis] <= outer[2][axis] + 0.015
                for axis in range(3)
            )
            for outer in chosen
        )
        if fully_covered:
            continue
        chosen.append(candidate)
        if len(chosen) == max_parts:
            break

    if not chosen:
        raise ValueError("No source geometry met the minimum compound-collider size.")
    return chosen


def create_collision_proxy_parts(name, source_objects, max_parts=6):
    proxies = []
    for index, (_, minimum, maximum, dimensions) in enumerate(
        collision_source_objects(source_objects, max_parts),
        start=1,
    ):
        center = tuple((minimum[axis] + maximum[axis]) * 0.5 for axis in range(3))
        bpy.ops.mesh.primitive_cube_add(size=1.0, location=center)
        proxy = bpy.context.object
        proxy.name = f"{name}_CollisionPart_{index:02d}"
        proxy.data.name = proxy.name + "Mesh"
        proxy.dimensions = dimensions
        bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
        proxy["asset_id"] = name
        proxy["asset_kind"] = "unassigned_compound_box_collision"
        proxy["collision_part_index"] = index
        proxy["collision_note"] = "Separate low-poly box component; review and configure before gameplay use."
        bpy.context.scene.cursor.location = (0, 0, 0)
        bpy.ops.object.origin_set(type="ORIGIN_CURSOR")
        proxies.append(proxy)
    return proxies


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

    collision_objects = create_collision_proxy_parts(os.path.basename(base_path), objects)
    bpy.ops.object.select_all(action="DESELECT")
    for collision in collision_objects:
        collision.select_set(True)
    bpy.context.view_layer.objects.active = collision_objects[0]
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
        for collision in collision_objects:
            bpy.data.objects.remove(collision, do_unlink=True)

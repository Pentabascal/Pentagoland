# Pentagoland Utility Props

Reusable static pieces for corridors, entrances and control rooms:

- **Access reader:** wall-mounted card/key reader with display and status lights.
- **Security camera:** compact CCTV body, wall arm, lens and recording indicator.
- **Ceiling light:** low-profile modular diffuser fixture.
- **Wall vent:** recessed industrial grille with a seven-fin face.
- **First-aid case:** wall-mounted emergency supply cabinet.
- **Power junction:** breaker enclosure with status lamps and vents.
- **Cargo crate:** reinforced floor prop with braces, label and tamper seal.
- **Pipe segment:** modular steel riser with flanges and couplings.
- **Sliding bulkhead:** detailed static door, guide rails and status marker.
- **Wall sign:** layered directional/signage panel with raised graphic bars.
- **Fire extinguisher:** vessel, valve, gauge, hose and safety pin.
- **Ceiling sensor:** low-profile detector with lens and status marker.
- **Floor drain:** framed inset grate with individual fins.
- **Cable tray:** mounting spine, side rails and three color-coded cable runs.
- **Warning beacon:** industrial signal lamp with a corresponding MER light toy.
- **Server rack:** cabinet, vent panels, rails, eight drive bays and feet.
- **Airlock control panel:** authorization controls, status display and guarded release.
- **Wall fire-hose cabinet:** recessed reel, nozzle cradle and access hardware.
- **Biohazard waste bin:** lidded segregation bin with pedal, handles and ID plate.
- **Specimen canister:** sealed containment vessel with lock pins and label window.
- **Decontamination shower:** overhead head, supports, manifold and pull handle.
- **Floor cable cover:** low-profile crossing ramp with removable access lid.
- **Ceiling speaker:** recessed paging speaker and mounting plate.
- **Wall clock:** housing, bezel, dial and static hands.
- **Security pillar:** armored sensor column with optical lens and service hatch.
- **Workbench:** reinforced bench with tool mat, drawer and power module.
- **Tool cart:** two trays, uprights, handle, wheels and toolbox.
- **Industrial fan:** wall mount, casing, grille and static rotor silhouette.
- **Pipe valve:** flanged pipe assembly, valve body and handwheel.
- **Pressure gauge:** casing, dial, static needle and pipe connector.
- **Cryogenic pod:** insulated capsule, observation panel and control module.
- **Observation window:** armored frame, inset pane and braces.
- **Blast-door control:** guarded actuator, display and key switch.
- **Handrail section:** top/lower rails, posts and mounting feet.
- **Utility locker:** twin doors, vents, handles and base plinth.
- **Hazard barrel:** ribbed drum, lid plugs and hazard-label panel.
- **Chemical safety shower:** overhead spray head, pipework, activation lever and foot plate.
- **Biometric scanner:** access terminal with screen, glass pad and status indicator.
- **Wall intercom:** speaker grille, call control and indicator display.
- **Medical supply trolley:** wheeled cart with trays and a removable-looking case.
- **Air scrubber:** cabinet filtration unit with louvers, exhaust and status panel.
- **Portable work light:** battery base, yoke, carry handle and diffuser.
- **Floor warning beacon:** weighted portable signal light.
- **First-aid dispenser:** wall cabinet with medical marking and hinges.
- **Pipe elbow:** right-angle industrial pipe junction with flanges.
- **Wall cable reel:** wall mount, drum, flanges, handle and cable guide.
- **Wall monitor:** framed display, status strip, control and wall mount.
- **Emergency phone:** alarm-colored enclosure, handset, speaker and call button.
- **Ceiling sprinkler:** escutcheon, drop pipe, thermal bulb and deflector.
- **Wall junction box:** armored enclosure with vents, latch and indicator.
- **Storage drum:** ribbed industrial barrel with lid, hoop and inventory plate.
- **Wall bollard:** steel guard post, base flange, anchor details and reflective band.
- **Hazard placard:** framed, color-coded warning panel with mounting hardware.
- **Ventilation duct:** modular rectangular duct section with flanges and access hatch.
- **Radiation meter:** portable survey meter with display, selector and sensor grille.
- **Ceiling access panel:** framed service hatch with hinge and latch details.
- **Decontamination canister:** sealed vessel with cap, hazard label and pins.
- **Elevator call panel:** compact floor display, call button and flush enclosure.
- **Ceiling emergency light:** low-profile fixture with diffused indicator window.
- **Wall hydrant cabinet:** hose reel, nozzle cradle and armored cabinet front.
- **Floor service hatch:** framed inset access cover with handle and fasteners.
- **Chemical storage locker:** reinforced double-door cabinet with vents and warning header.
- **Lab centrifuge:** benchtop machine with rotor lid and operator controls.
- **Portable generator:** framed power unit with vent, outlet and warning plate.
- **Archive terminal:** compact records workstation with display and keyboard deck.
- **Decontamination sink:** industrial basin, backsplash, faucet and pedestal.
- **Secure storage safe:** reinforced body, dial, locking bar and plinth.

`PentagolandUtilityProps.blend` contains 77 editable utility models. Each model has an individual `.fbx`, `.glb`, rendered preview and optional axis-aligned bounds collision proxy (`-collision.fbx`). `create_utility_props.py` regenerates the scene and exports a single joined render mesh object per interchange asset to reduce renderer/object overhead while preserving material assignments. The Blender source keeps the detailed pieces separately editable. Every render mesh has UVs and packed base-color and tangent-space normal textures with muted industrial PBR values. `asset_manifest.json` reports dimensions, polygon/material counts, approximate material batches and collision-proxy guidance.

The latest ten additions are a gas detector, wall patch panel, portable decontamination unit, ceiling dome camera, emergency shutoff valve, specimen transport case, wall charging station, mobile medical monitor, industrial water pump and mobile evidence cart.

Check all three saved Blender sources and re-import the 162 render FBX/GLB files plus 81 optional collision proxies across the complete asset pack with Blender:

```powershell
& "C:\Program Files\Blender Foundation\Blender 5.2\blender.exe" --background --python .\Assets\PentagolandUtilityProps\validate_asset_pack.py
```

The interchange meshes are not loadable by stock ProjectMER. The corresponding schematic approximations in `../../Samples/Schematics/` use built-in MER primitive objects; confirm placement, visibility, orientation and performance in staging before use. All 59 modular approximations intentionally use only a few silhouette-defining parts per prop to limit network toys; they are less detailed than the Blender exports. All props are static scenery: signs are not readable text, and panels, scanners, sensors, extinguishers, alarms, fans and monitors are not gameplay systems.

The models are detailed, stylized industrial props rather than photorealistic scans. The MER approximations are made from multiple networked primitive toys per prop and are intended for selective placement, not mass duplication. No dynamic rigid-body simulation or gameplay scripts are included. The separate collision FBX is a simple bounding box, not a tested player collider; inspect it and choose suitable Unity collider settings for each placement. Benchmark representative scenes on the target server before deployment. FBX/GLB materials, UVs, normal maps and collision proxies are checked by Blender re-import; Unity materials, collider behavior and SCP:SL compatibility still require validation in the target project/server.

The utility pack uses a subdued graphite, enamel and steel palette with restrained safety/status colors to better fit neutral industrial environments. Models have a consistent meter-scale coordinate convention and a ground/attachment-oriented origin at `(0, 0, 0)`; inspect orientation for ceiling and wall-mounted props before use.

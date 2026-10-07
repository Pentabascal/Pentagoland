# Unity asset review

This project imports the authored FBX render models and their textures, creates
one review prefab per model, builds a static gallery scene, and writes import and
collision reports. It also creates separate optional collision prefabs with
compound mesh colliders disabled by default, converts the supplied roughness
references into URP metallic/smoothness maps, and exports one importable Unity
package for the complete library.

## Editor version and scope

The project is pinned to Unity **6000.6.4f1**, the editor available on the
workstation when this review project was prepared. The local SCP:SL player is
Unity **6000.0.43f1**. This version mismatch means the project is for offline
visual/import inspection only: it does not establish AssetBundle, game, or
server compatibility. No AssetBundles are built.

## Rebuild the review scene and report

Install Unity Editor **6000.6.4f1** through Unity Hub. From this directory,
run the checked-in batch script (or set `UNITY_EDITOR` to that editor's
`Unity.exe` path if Unity is installed outside the default location):

```cmd
BuildReview.cmd
```

The generated gallery is `Assets/Generated/AssetReviewGallery.unity`; the
material reference scene is `Assets/Generated/MaterialReferenceStudy.unity`.
Render prefabs and disabled collision prefabs are in separate subfolders of
`Assets/Generated`; import and collision CSV reports are in `Assets/Reports`.
Render FBX files, texture maps, collision FBX files, the utility manifest, and
the three material-study source images and the Poly Haven CC0 metal-plate PBR
texture set are synchronized into separate source folders before Unity runs.
The project manifest includes URP **17.6.0**, matching this Unity 6000.6 editor,
so the material builder can create and validate actual URP/Lit presets.

After every successful build, import
`../UnityAssetPackage/Pentagoland_Assets.unitypackage` into another Unity
project through **Assets > Import Package > Custom Package...**. It contains
the render models, per-prop prefabs, separate disabled compound-collider
prefabs, five generated URP material presets and masks, source reference images,
review scenes, and reports. The Poly Haven material includes its diffuse,
OpenGL normal and roughness maps; its source and CC0 license note are included.
The target project must have a compatible URP
package for the material presets. The package output is reproducible and is
intended to be checked in with the adjacent `UnityAssetPackage/README.md`.

The sync script overwrites changed or new exports and does not delete files;
if an export is removed from its source pack, remove that specific obsolete
copy from the corresponding review folder. The build script validates the
render and collision CSV reports, disabled collider state, package presence,
and Unity's exit code. The project is pinned to 6000.6.4f1; do not open it in
a different editor version without first making a separate copy.

This is not a physics, performance, network-replication, or in-game test.
Review each generated collider manually before enabling it.

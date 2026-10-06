# Unity asset review

This project imports the authored FBX render models and their textures, creates
one review prefab per model, and builds a static gallery scene with an import
report. Collision-proxy FBX files are kept in a separate folder and are never
placed in the scene or assigned to colliders by the generator.

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

The generated gallery is `Assets/Generated/AssetReviewGallery.unity`; review
prefabs are under `Assets/Generated/Prefabs`; the CSV and summary are under
`Assets/Reports`. Failed model imports are written to the report and cause the
batch invocation to fail. Render FBX files and textures are copied into
`Assets/SourceAssets`; optional bounds-only collision FBX files are copied to
`Assets/CollisionProxies_Unassigned`. The sync script overwrites changed or
new exports and does not delete files; if an export is removed from its source
pack, remove that specific obsolete copy from the corresponding review folder.
The build script validates the generated CSV and summary as well as Unity's
exit code. The project is pinned to 6000.6.4f1; do not open it in a different
editor version without first making a separate copy.

This is not a physics, performance, network-replication, or in-game test.
Review each optional bounds-only collision proxy manually before using one.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pentagoland.AssetReview.Editor
{
    public static class AssetReviewBuilder
    {
        private const string RenderSourceRoot = "Assets/SourceAssets";
        private const string CollisionSourceRoot = "Assets/CollisionProxies_Unassigned";
        private const string GeneratedRoot = "Assets/Generated";
        private const string PrefabRoot = GeneratedRoot + "/Prefabs";
        private const string CollisionPrefabRoot = GeneratedRoot + "/CollisionPrefabs_Unassigned";
        private const string MaterialRoot = GeneratedRoot + "/Materials";
        private const string MaterialStudyPath = GeneratedRoot + "/MaterialReferenceStudy.unity";
        private const string MaterialReferenceRoot = "Assets/MaterialReferences/Habitacion";
        private const string PolyHavenMaterialRoot = "Assets/MaterialReferences/PolyHaven/MetalPlate";
        private const string ScenePath = GeneratedRoot + "/AssetReviewGallery.unity";
        private const string ReportRoot = "Assets/Reports";
        private const int GridColumns = 9;
        private const float GridSpacing = 4f;
        private const int MaxRenderVerticesPerAsset = 30000;
        private const int MaxRenderMaterialsPerAsset = 9;
        private const int MaxRenderMeshesPerAsset = 1;
        private const float MaxRenderBoundsDimension = 10f;
        private const int MaxCollisionPartsPerAsset = 6;
        private const int MaxCollisionTrianglesPerAsset = 96;

        private sealed class AssetResult
        {
            public string AssetPath;
            public string Status;
            public int MeshCount;
            public int VertexCount;
            public int MaterialCount;
            public int TexturedMaterialCount;
            public Vector3 Bounds;
            public string Error;
        }

        private sealed class CollisionResult
        {
            public string AssetPath;
            public string Status;
            public int ColliderCount;
            public int TriangleCount;
            public string Error;
        }

        [MenuItem("Pentagoland/Build Asset Review")]
        public static void Build()
        {
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            EnsureFolder(GeneratedRoot);
            EnsureFolder(PrefabRoot);
            EnsureFolder(CollisionPrefabRoot);
            EnsureFolder(MaterialRoot);
            EnsureFolder(ReportRoot);

            string[] modelPaths = AssetDatabase.FindAssets("t:Model", new[] { RenderSourceRoot })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => string.Equals(Path.GetExtension(path), ".fbx", StringComparison.OrdinalIgnoreCase))
                .OrderBy(path => path, StringComparer.Ordinal)
                .ToArray();
            string[] collisionPaths = AssetDatabase.FindAssets("t:Model", new[] { CollisionSourceRoot })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => string.Equals(Path.GetExtension(path), ".fbx", StringComparison.OrdinalIgnoreCase))
                .OrderBy(path => path, StringComparer.Ordinal)
                .ToArray();

            if (modelPaths.Length == 0)
            {
                throw new InvalidOperationException("No render FBX models were imported under " + RenderSourceRoot + ".");
            }

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            AddGalleryLighting(scene, modelPaths.Length);

            List<AssetResult> results = new List<AssetResult>(modelPaths.Length);
            List<string> failures = new List<string>();
            if (collisionPaths.Length != modelPaths.Length)
            {
                failures.Add("Expected one collision proxy per render FBX; found "
                    + collisionPaths.Length + " collision FBX for " + modelPaths.Length + " render FBX.");
            }

            for (int index = 0; index < modelPaths.Length; index++)
            {
                string modelPath = modelPaths[index];
                AssetResult result = new AssetResult
                {
                    AssetPath = modelPath,
                    Status = "ERROR",
                };

                try
                {
                    GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
                    if (model == null)
                    {
                        throw new InvalidDataException("Unity did not load this FBX as a model.");
                    }

                    InspectModel(model, result);
                    if (result.MeshCount == 0)
                    {
                        throw new InvalidDataException("The imported model contains no meshes.");
                    }

                    ValidateRenderBudget(result);

                    string prefabPath = PrefabRoot + "/" + model.name + ".prefab";
                    GameObject prefab = PrefabUtility.SaveAsPrefabAsset(model, prefabPath);
                    if (prefab == null)
                    {
                        throw new InvalidOperationException("Unity failed to save the review prefab at " + prefabPath + ".");
                    }

                    GameObject instance = PrefabUtility.InstantiatePrefab(prefab, scene) as GameObject;
                    if (instance == null)
                    {
                        throw new InvalidOperationException("Unity failed to instantiate the review prefab.");
                    }

                    instance.name = model.name;
                    PlaceOnGalleryGrid(instance, index);
                    if (instance.GetComponentsInChildren<Collider>(true).Length != 0)
                    {
                        throw new InvalidDataException("The review instance unexpectedly contains a collider.");
                    }

                    result.Status = "OK";
                }
                catch (Exception exception)
                {
                    result.Error = exception.ToString();
                    failures.Add(modelPath + ": " + exception.Message);
                    Debug.LogError("Asset review failed for " + modelPath + ": " + exception);
                }

                results.Add(result);
            }

            EditorSceneManager.SaveScene(scene, ScenePath);
            List<CollisionResult> collisionResults = BuildCollisionPrefabs(collisionPaths, failures);
            Dictionary<string, Material> materials = BuildMaterialLibrary();
            BuildMaterialStudy(materials);
            AssetDatabase.SaveAssets();
            WriteReports(results, collisionResults, materials.Count, failures);

            string summary = "Unity asset review used " + Application.unityVersion
                + ": " + results.Count + " render FBX, "
                + results.Count(result => result.Status == "OK") + " successful, "
                + collisionResults.Count(result => result.Status == "OK") + " compound collision prefabs built with colliders disabled, "
                + materials.Count + " URP material presets, "
                + failures.Count + " failures.";
            if (failures.Count > 0)
            {
                Debug.LogError(summary + Environment.NewLine + string.Join(Environment.NewLine, failures));
                throw new InvalidOperationException(summary);
            }

            string packagePath = ExportAssetPackage();
            Debug.Log("Import-ready Unity package exported to " + packagePath);
            Debug.Log(summary);
        }

        private static List<CollisionResult> BuildCollisionPrefabs(string[] collisionPaths, List<string> failures)
        {
            List<CollisionResult> results = new List<CollisionResult>(collisionPaths.Length);
            foreach (string collisionPath in collisionPaths)
            {
                CollisionResult result = new CollisionResult
                {
                    AssetPath = collisionPath,
                    Status = "ERROR",
                };
                GameObject instance = null;

                try
                {
                    GameObject source = AssetDatabase.LoadAssetAtPath<GameObject>(collisionPath);
                    if (source == null)
                    {
                        throw new InvalidDataException("Unity did not load this collision FBX as a model.");
                    }

                    instance = UnityEngine.Object.Instantiate(source);
                    instance.name = Path.GetFileNameWithoutExtension(collisionPath)
                        .Replace("-collision", string.Empty) + "_Collision_UNASSIGNED";
                    foreach (MeshFilter filter in instance.GetComponentsInChildren<MeshFilter>(true))
                    {
                        if (filter.sharedMesh == null)
                        {
                            throw new InvalidDataException("Collision mesh filter has no mesh: " + filter.name);
                        }

                        MeshCollider collider = filter.GetComponent<MeshCollider>();
                        if (collider == null)
                        {
                            collider = filter.gameObject.AddComponent<MeshCollider>();
                        }

                        collider.sharedMesh = filter.sharedMesh;
                        collider.convex = false;
                        collider.enabled = false;
                        result.ColliderCount++;
                        result.TriangleCount += filter.sharedMesh.triangles.Length / 3;

                        MeshRenderer renderer = filter.GetComponent<MeshRenderer>();
                        if (renderer != null)
                        {
                            UnityEngine.Object.DestroyImmediate(renderer);
                        }
                    }

                    if (result.ColliderCount == 0)
                    {
                        throw new InvalidDataException("The imported collision proxy contains no mesh colliders.");
                    }

                    ValidateCollisionBudget(result);

                    string prefabPath = CollisionPrefabRoot + "/" + instance.name + ".prefab";
                    if (PrefabUtility.SaveAsPrefabAsset(instance, prefabPath) == null)
                    {
                        throw new InvalidOperationException("Unity failed to save collision prefab at " + prefabPath + ".");
                    }

                    result.Status = "OK";
                }
                catch (Exception exception)
                {
                    result.Error = exception.ToString();
                    failures.Add(collisionPath + ": " + exception.Message);
                    Debug.LogError("Collision proxy review failed for " + collisionPath + ": " + exception);
                }
                finally
                {
                    if (instance != null)
                    {
                        UnityEngine.Object.DestroyImmediate(instance);
                    }
                }

                results.Add(result);
            }

            return results;
        }

        private static Dictionary<string, Material> BuildMaterialLibrary()
        {
            EnsureFolder(GeneratedRoot + "/Textures");
            string slatePath = MaterialReferenceRoot + "/Pentago_DeepSlatePanel_Roughness.png";
            string oxidizedPath = MaterialReferenceRoot + "/Pentago_OxidizedMetal_Roughness.png";
            string ceramicPath = MaterialReferenceRoot + "/Pentago_SterileCeramic_BaseColor.png";
            Texture2D slateRoughness = LoadReferenceTexture(slatePath, false, true);
            Texture2D oxidizedRoughness = LoadReferenceTexture(oxidizedPath, false, true);
            Texture2D ceramicBaseColor = LoadReferenceTexture(ceramicPath, true, false);
            Texture2D polyHavenBaseColor = LoadReferenceTexture(
                PolyHavenMaterialRoot + "/metal_plate_diff_1k.jpg", true, false);
            Texture2D polyHavenNormal = LoadNormalReferenceTexture(
                PolyHavenMaterialRoot + "/metal_plate_nor_gl_1k.jpg");
            Texture2D polyHavenRoughness = LoadReferenceTexture(
                PolyHavenMaterialRoot + "/metal_plate_rough_1k.jpg", false, true);

            Texture2D slateMask = CreateMetallicSmoothnessMap(
                "Pentago_DeepSlatePanel_MetallicSmoothness.png", slateRoughness, 0.28f);
            Texture2D oxidizedMask = CreateMetallicSmoothnessMap(
                "Pentago_OxidizedMetal_MetallicSmoothness.png", oxidizedRoughness, 0.38f);
            Texture2D polyHavenMask = CreateMetallicSmoothnessMap(
                "PolyHaven_MetalPlate_MetallicSmoothness.png", polyHavenRoughness, 0.72f);
            SetTextureReadable(slateRoughness, false);
            SetTextureReadable(oxidizedRoughness, false);
            SetTextureReadable(polyHavenRoughness, false);

            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null || !shader.isSupported)
            {
                throw new InvalidOperationException(
                    "The Universal Render Pipeline Lit shader is unavailable. Restore the project's URP package before building materials.");
            }

            Dictionary<string, Material> materials = new Dictionary<string, Material>(StringComparer.Ordinal)
            {
                ["Pentago_DeepSlatePanel"] = CreateMaterialPreset(
                    shader, "Pentago_DeepSlatePanel", new Color(0.19f, 0.23f, 0.26f), null, slateMask, 1f, 1f),
                ["Pentago_OxidizedMetal"] = CreateMaterialPreset(
                    shader, "Pentago_OxidizedMetal", new Color(0.34f, 0.24f, 0.17f), null, oxidizedMask, 1f, 1f),
                ["Pentago_SterileCeramic"] = CreateMaterialPreset(
                    shader, "Pentago_SterileCeramic", Color.white, ceramicBaseColor, null, 0.03f, 0.27f),
                ["Pentago_GunmetalShell"] = CreateMaterialPreset(
                    shader, "Pentago_GunmetalShell", new Color(0.16f, 0.20f, 0.22f), null, slateMask, 1f, 1f),
                ["PolyHaven_MetalPlate"] = CreateMaterialPreset(
                    shader, "PolyHaven_MetalPlate", Color.white, polyHavenBaseColor, polyHavenMask,
                    1f, 1f, polyHavenNormal),
            };

            return materials;
        }

        private static Texture2D LoadNormalReferenceTexture(string assetPath)
        {
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null)
            {
                throw new FileNotFoundException("Required normal-map reference texture is missing.", assetPath);
            }

            bool changed = importer.textureType != TextureImporterType.NormalMap
                || importer.isReadable
                || importer.wrapMode != TextureWrapMode.Repeat;
            if (changed)
            {
                importer.textureType = TextureImporterType.NormalMap;
                importer.isReadable = false;
                importer.wrapMode = TextureWrapMode.Repeat;
                importer.SaveAndReimport();
            }

            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            if (texture == null || importer.textureType != TextureImporterType.NormalMap)
            {
                throw new InvalidDataException("Unity did not import the texture as a normal map: " + assetPath);
            }

            return texture;
        }

        private static Texture2D LoadReferenceTexture(string assetPath, bool sRgb, bool readable)
        {
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null)
            {
                throw new FileNotFoundException("Required material reference texture is missing.", assetPath);
            }

            bool changed = importer.sRGBTexture != sRgb
                || importer.isReadable != readable
                || importer.textureType != TextureImporterType.Default
                || importer.wrapMode != TextureWrapMode.Repeat;
            if (changed)
            {
                importer.sRGBTexture = sRgb;
                importer.isReadable = readable;
                importer.textureType = TextureImporterType.Default;
                importer.wrapMode = TextureWrapMode.Repeat;
                importer.SaveAndReimport();
            }

            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            if (texture == null || (readable && !texture.isReadable))
            {
                throw new InvalidDataException("Unity could not load the material reference texture: " + assetPath);
            }

            return texture;
        }

        private static void SetTextureReadable(Texture2D texture, bool readable)
        {
            TextureImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture)) as TextureImporter;
            if (importer != null && importer.isReadable != readable)
            {
                importer.isReadable = readable;
                importer.SaveAndReimport();
            }
        }

        private static Texture2D CreateMetallicSmoothnessMap(string fileName, Texture2D roughness, float metallic)
        {
            Color[] sourcePixels = roughness.GetPixels();
            Color[] packedPixels = new Color[sourcePixels.Length];
            for (int index = 0; index < sourcePixels.Length; index++)
            {
                float smoothness = 1f - sourcePixels[index].grayscale;
                packedPixels[index] = new Color(metallic, 0f, 0f, Mathf.Clamp01(smoothness));
            }

            Texture2D packed = new Texture2D(roughness.width, roughness.height, TextureFormat.RGBA32, false, true);
            packed.SetPixels(packedPixels);
            packed.Apply(false, false);
            string assetPath = GeneratedRoot + "/Textures/" + fileName;
            string absolutePath = Path.Combine(Application.dataPath, assetPath.Substring("Assets/".Length));
            File.WriteAllBytes(absolutePath, packed.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(packed);
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport);

            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null)
            {
                throw new InvalidDataException("Unity did not import the generated surface mask: " + assetPath);
            }

            importer.sRGBTexture = false;
            importer.alphaIsTransparency = false;
            importer.textureType = TextureImporterType.Default;
            importer.wrapMode = TextureWrapMode.Repeat;
            importer.filterMode = FilterMode.Bilinear;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
        }

        private static Material CreateMaterialPreset(
            Shader shader,
            string name,
            Color baseColor,
            Texture2D baseMap,
            Texture2D metallicSmoothnessMap,
            float metallic,
            float smoothness,
            Texture2D normalMap = null)
        {
            string assetPath = MaterialRoot + "/" + name + ".mat";
            Material material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
            if (material == null)
            {
                material = new Material(shader);
                AssetDatabase.CreateAsset(material, assetPath);
            }
            else
            {
                material.shader = shader;
            }

            material.name = name;
            material.SetColor("_BaseColor", baseColor);
            material.SetTexture("_BaseMap", baseMap);
            material.SetFloat("_Metallic", metallic);
            material.SetFloat("_Smoothness", smoothness);
            material.SetTexture("_MetallicGlossMap", metallicSmoothnessMap);
            material.SetTexture("_BumpMap", normalMap);
            material.SetFloat("_BumpScale", normalMap != null ? 0.35f : 1f);
            if (normalMap != null)
            {
                material.EnableKeyword("_NORMALMAP");
            }
            else
            {
                material.DisableKeyword("_NORMALMAP");
            }
            if (metallicSmoothnessMap != null)
            {
                material.EnableKeyword("_METALLICSPECGLOSSMAP");
                material.SetFloat("_Metallic", 1f);
                material.SetFloat("_Smoothness", 1f);
            }
            else
            {
                material.DisableKeyword("_METALLICSPECGLOSSMAP");
            }

            material.enableInstancing = true;
            EditorUtility.SetDirty(material);
            return material;
        }

        private static void BuildMaterialStudy(Dictionary<string, Material> materials)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            GameObject lightObject = new GameObject("Material Study Directional Light");
            SceneManager.MoveGameObjectToScene(lightObject, scene);
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;
            lightObject.transform.rotation = Quaternion.Euler(42f, -28f, 0f);

            GameObject cameraObject = new GameObject("Material Study Camera");
            SceneManager.MoveGameObjectToScene(cameraObject, scene);
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.fieldOfView = 42f;
            cameraObject.transform.position = new Vector3(0f, 3.3f, -8.5f);
            cameraObject.transform.LookAt(new Vector3(0f, 0.7f, 0f));

            int index = 0;
            foreach (KeyValuePair<string, Material> entry in materials)
            {
                float x = (index - (materials.Count - 1) * 0.5f) * 2.2f;
                GameObject sample = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                SceneManager.MoveGameObjectToScene(sample, scene);
                sample.name = entry.Key + " Sample";
                sample.transform.position = new Vector3(x, 0.75f, 0f);
                sample.GetComponent<MeshRenderer>().sharedMaterial = entry.Value;
                UnityEngine.Object.DestroyImmediate(sample.GetComponent<Collider>());

                GameObject labelObject = new GameObject(entry.Key + " Label");
                SceneManager.MoveGameObjectToScene(labelObject, scene);
                labelObject.transform.position = new Vector3(x, 0.12f, 0f);
                labelObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                TextMesh label = labelObject.AddComponent<TextMesh>();
                label.text = entry.Key.Replace("Pentago_", string.Empty).Replace("_", " ");
                label.anchor = TextAnchor.MiddleCenter;
                label.alignment = TextAlignment.Center;
                label.characterSize = 0.12f;
                label.fontSize = 48;
                index++;
            }

            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            SceneManager.MoveGameObjectToScene(floor, scene);
            floor.name = "Material Study Floor";
            floor.transform.position = new Vector3(0f, -0.05f, 0f);
            floor.transform.localScale = new Vector3(1.2f, 1f, 0.7f);
            UnityEngine.Object.DestroyImmediate(floor.GetComponent<Collider>());
            EditorSceneManager.SaveScene(scene, MaterialStudyPath);
        }

        private static string ExportAssetPackage()
        {
            string[] paths =
            {
                RenderSourceRoot,
                CollisionSourceRoot,
                "Assets/MaterialReferences",
                GeneratedRoot,
                ReportRoot,
            };
            string outputRoot = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "..", "UnityAssetPackage"));
            Directory.CreateDirectory(outputRoot);
            string packagePath = Path.Combine(outputRoot, "Pentagoland_Assets.unitypackage");
            AssetDatabase.ExportPackage(
                paths,
                packagePath,
                ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);
            FileInfo package = new FileInfo(packagePath);
            if (!package.Exists || package.Length == 0)
            {
                throw new IOException("Unity did not create the importable package at " + packagePath + ".");
            }

            return packagePath;
        }

        private static void InspectModel(GameObject model, AssetResult result)
        {
            HashSet<Mesh> meshes = new HashSet<Mesh>();
            HashSet<Material> materials = new HashSet<Material>();
            HashSet<Material> texturedMaterials = new HashSet<Material>();
            Renderer[] renderers = model.GetComponentsInChildren<Renderer>(true);

            foreach (MeshFilter filter in model.GetComponentsInChildren<MeshFilter>(true))
            {
                if (filter.sharedMesh != null)
                {
                    meshes.Add(filter.sharedMesh);
                }
            }

            foreach (SkinnedMeshRenderer renderer in model.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            {
                if (renderer.sharedMesh != null)
                {
                    meshes.Add(renderer.sharedMesh);
                }
            }

            foreach (Renderer renderer in renderers)
            {
                foreach (Material material in renderer.sharedMaterials)
                {
                    if (material == null)
                    {
                        continue;
                    }

                    materials.Add(material);
                    if (HasTexture(material))
                    {
                        texturedMaterials.Add(material);
                    }

                    if (material.shader == null || !material.shader.isSupported)
                    {
                        throw new InvalidDataException("Material has a missing or unsupported shader: " + material.name);
                    }
                }
            }

            result.MeshCount = meshes.Count;
            result.VertexCount = meshes.Sum(mesh => mesh.vertexCount);
            result.MaterialCount = materials.Count;
            result.TexturedMaterialCount = texturedMaterials.Count;

            bool hasBounds = false;
            Bounds bounds = new Bounds();
            foreach (Renderer renderer in renderers)
            {
                if (!hasBounds)
                {
                    bounds = renderer.bounds;
                    hasBounds = true;
                }
                else
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            result.Bounds = hasBounds ? bounds.size : Vector3.zero;
        }

        private static void ValidateRenderBudget(AssetResult result)
        {
            if (result.MeshCount > MaxRenderMeshesPerAsset)
            {
                throw new InvalidDataException(
                    "Render mesh budget exceeded: " + result.MeshCount + " meshes; maximum is "
                    + MaxRenderMeshesPerAsset + ".");
            }

            if (result.VertexCount > MaxRenderVerticesPerAsset)
            {
                throw new InvalidDataException(
                    "Render vertex budget exceeded: " + result.VertexCount + " vertices; maximum is "
                    + MaxRenderVerticesPerAsset + ".");
            }

            if (result.MaterialCount > MaxRenderMaterialsPerAsset)
            {
                throw new InvalidDataException(
                    "Render material-slot budget exceeded: " + result.MaterialCount + " slots; maximum is "
                    + MaxRenderMaterialsPerAsset + ".");
            }

            if (result.TexturedMaterialCount == 0)
            {
                throw new InvalidDataException("The imported model has no textured materials.");
            }

            if (!IsValidBoundsDimension(result.Bounds.x)
                || !IsValidBoundsDimension(result.Bounds.y)
                || !IsValidBoundsDimension(result.Bounds.z))
            {
                throw new InvalidDataException(
                    "Render bounds must be finite, positive, and no larger than "
                    + MaxRenderBoundsDimension + " metres per axis.");
            }
        }

        private static void ValidateCollisionBudget(CollisionResult result)
        {
            if (result.ColliderCount > MaxCollisionPartsPerAsset)
            {
                throw new InvalidDataException(
                    "Collision-part budget exceeded: " + result.ColliderCount + " parts; maximum is "
                    + MaxCollisionPartsPerAsset + ".");
            }

            if (result.TriangleCount > MaxCollisionTrianglesPerAsset)
            {
                throw new InvalidDataException(
                    "Collision-triangle budget exceeded: " + result.TriangleCount + " triangles; maximum is "
                    + MaxCollisionTrianglesPerAsset + ".");
            }
        }

        private static bool IsValidBoundsDimension(float dimension)
        {
            return !float.IsNaN(dimension)
                && !float.IsInfinity(dimension)
                && dimension > 0f
                && dimension <= MaxRenderBoundsDimension;
        }

        private static bool HasTexture(Material material)
        {
            return HasTexture(material, "_BaseMap")
                || HasTexture(material, "_MainTex")
                || HasTexture(material, "_BumpMap");
        }

        private static bool HasTexture(Material material, string property)
        {
            return material.HasProperty(property) && material.GetTexture(property) != null;
        }

        private static void PlaceOnGalleryGrid(GameObject instance, int index)
        {
            int row = index / GridColumns;
            int column = index % GridColumns;
            Vector3 target = new Vector3(column * GridSpacing, 0f, row * GridSpacing);
            Renderer[] renderers = instance.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0)
            {
                instance.transform.position = target;
                return;
            }

            Bounds bounds = renderers[0].bounds;
            for (int rendererIndex = 1; rendererIndex < renderers.Length; rendererIndex++)
            {
                bounds.Encapsulate(renderers[rendererIndex].bounds);
            }

            instance.transform.position += new Vector3(
                target.x - bounds.center.x,
                -bounds.min.y,
                target.z - bounds.center.z);
        }

        private static void AddGalleryLighting(Scene scene, int assetCount)
        {
            GameObject lightObject = new GameObject("Review Directional Light");
            SceneManager.MoveGameObjectToScene(lightObject, scene);
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;
            lightObject.transform.rotation = Quaternion.Euler(48f, -28f, 0f);

            int rows = (assetCount + GridColumns - 1) / GridColumns;
            float centerX = (GridColumns - 1) * GridSpacing * 0.5f;
            float centerZ = (rows - 1) * GridSpacing * 0.5f;

            GameObject cameraObject = new GameObject("Review Camera");
            SceneManager.MoveGameObjectToScene(cameraObject, scene);
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.fieldOfView = 50f;
            cameraObject.transform.position = new Vector3(centerX, 28f, centerZ - 42f);
            cameraObject.transform.LookAt(new Vector3(centerX, 0f, centerZ));

            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            SceneManager.MoveGameObjectToScene(floor, scene);
            floor.name = "Review Floor";
            floor.transform.position = new Vector3(centerX, -0.025f, centerZ);
            floor.transform.localScale = new Vector3(GridColumns * GridSpacing / 10f, 1f, rows * GridSpacing / 10f);
            Collider floorCollider = floor.GetComponent<Collider>();
            if (floorCollider != null)
            {
                UnityEngine.Object.DestroyImmediate(floorCollider);
            }
        }

        private static void WriteReports(
            List<AssetResult> results,
            List<CollisionResult> collisionResults,
            int materialCount,
            List<string> failures)
        {
            StringBuilder csv = new StringBuilder();
            csv.AppendLine("AssetPath,Status,MeshCount,VertexCount,MaterialCount,TexturedMaterialCount,BoundsX,BoundsY,BoundsZ,Error");
            foreach (AssetResult result in results)
            {
                csv.Append(Csv(result.AssetPath)).Append(',')
                    .Append(Csv(result.Status)).Append(',')
                    .Append(result.MeshCount).Append(',')
                    .Append(result.VertexCount).Append(',')
                    .Append(result.MaterialCount).Append(',')
                    .Append(result.TexturedMaterialCount).Append(',')
                    .Append(result.Bounds.x.ToString("R", System.Globalization.CultureInfo.InvariantCulture)).Append(',')
                    .Append(result.Bounds.y.ToString("R", System.Globalization.CultureInfo.InvariantCulture)).Append(',')
                    .Append(result.Bounds.z.ToString("R", System.Globalization.CultureInfo.InvariantCulture)).Append(',')
                    .Append(Csv(result.Error))
                    .AppendLine();
            }

            StringBuilder collisionCsv = new StringBuilder();
            collisionCsv.AppendLine("AssetPath,Status,CompoundColliderCount,TriangleCount,EnabledByDefault,Error");
            foreach (CollisionResult result in collisionResults)
            {
                collisionCsv.Append(Csv(result.AssetPath)).Append(',')
                    .Append(Csv(result.Status)).Append(',')
                    .Append(result.ColliderCount).Append(',')
                    .Append(result.TriangleCount).Append(',')
                    .Append("false").Append(',')
                    .Append(Csv(result.Error))
                    .AppendLine();
            }

            string summary = "Editor version: " + Application.unityVersion + Environment.NewLine
                + "Render FBX: " + results.Count + Environment.NewLine
                + "Successful imports: " + results.Count(result => result.Status == "OK") + Environment.NewLine
                + "Maximum render vertices per asset: "
                + results.Max(result => result.VertexCount) + " / " + MaxRenderVerticesPerAsset + Environment.NewLine
                + "Maximum render material slots per asset: "
                + results.Max(result => result.MaterialCount) + " / " + MaxRenderMaterialsPerAsset + Environment.NewLine
                + "Collision proxy FBX: " + collisionResults.Count + Environment.NewLine
                + "Collision prefabs built with disabled compound MeshColliders: "
                + collisionResults.Count(result => result.Status == "OK") + Environment.NewLine
                + "Maximum collision parts per asset: "
                + collisionResults.Max(result => result.ColliderCount) + " / " + MaxCollisionPartsPerAsset + Environment.NewLine
                + "Maximum collision triangles per asset: "
                + collisionResults.Max(result => result.TriangleCount) + " / " + MaxCollisionTrianglesPerAsset + Environment.NewLine
                + "Performance budgets: " + (failures.Count == 0 ? "PASS" : "FAIL") + Environment.NewLine
                + "URP material presets: " + materialCount + Environment.NewLine
                + "Failures: " + failures.Count + Environment.NewLine
                + "Collision meshes remain unassigned and disabled; review dimensions and placement before use." + Environment.NewLine
                + "This is an offline import/visual review, not a game compatibility, physics, or performance test."
                + Environment.NewLine;

            File.WriteAllText(ReportRoot + "/unity-import-report.csv", csv.ToString(), new UTF8Encoding(false));
            File.WriteAllText(ReportRoot + "/unity-collision-report.csv", collisionCsv.ToString(), new UTF8Encoding(false));
            File.WriteAllText(ReportRoot + "/unity-import-summary.txt", summary, new UTF8Encoding(false));
            AssetDatabase.Refresh();
        }

        private static string Csv(string value)
        {
            return "\"" + (value ?? string.Empty).Replace("\"", "\"\"") + "\"";
        }

        private static void EnsureFolder(string assetPath)
        {
            string[] segments = assetPath.Split('/');
            string current = segments[0];
            for (int index = 1; index < segments.Length; index++)
            {
                string next = current + "/" + segments[index];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, segments[index]);
                }

                current = next;
            }
        }
    }
}

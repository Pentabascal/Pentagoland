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
        private const string ScenePath = GeneratedRoot + "/AssetReviewGallery.unity";
        private const string ReportRoot = "Assets/Reports";
        private const int GridColumns = 9;
        private const float GridSpacing = 4f;

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

        [MenuItem("Pentagoland/Build Asset Review")]
        public static void Build()
        {
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            EnsureFolder(GeneratedRoot);
            EnsureFolder(PrefabRoot);
            EnsureFolder(ReportRoot);

            string[] modelPaths = AssetDatabase.FindAssets("t:Model", new[] { RenderSourceRoot })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => string.Equals(Path.GetExtension(path), ".fbx", StringComparison.OrdinalIgnoreCase))
                .OrderBy(path => path, StringComparer.Ordinal)
                .ToArray();
            string[] collisionPaths = AssetDatabase.FindAssets("t:Model", new[] { CollisionSourceRoot })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => string.Equals(Path.GetExtension(path), ".fbx", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (modelPaths.Length == 0)
            {
                throw new InvalidOperationException("No render FBX models were imported under " + RenderSourceRoot + ".");
            }

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            AddGalleryLighting(scene, modelPaths.Length);

            List<AssetResult> results = new List<AssetResult>(modelPaths.Length);
            List<string> failures = new List<string>();

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
            AssetDatabase.SaveAssets();
            WriteReports(results, collisionPaths.Length, failures);

            string summary = "Unity asset review used " + Application.unityVersion
                + ": " + results.Count + " render FBX, "
                + results.Count(result => result.Status == "OK") + " successful, "
                + collisionPaths.Length + " collision proxies kept separate and unassigned, "
                + failures.Count + " failures.";
            if (failures.Count > 0)
            {
                Debug.LogError(summary + Environment.NewLine + string.Join(Environment.NewLine, failures));
                throw new InvalidOperationException(summary);
            }

            Debug.Log(summary);
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

        private static void WriteReports(List<AssetResult> results, int collisionCount, List<string> failures)
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

            string summary = "Editor version: " + Application.unityVersion + Environment.NewLine
                + "Render FBX: " + results.Count + Environment.NewLine
                + "Successful imports: " + results.Count(result => result.Status == "OK") + Environment.NewLine
                + "Collision proxy FBX, kept separate and unassigned: " + collisionCount + Environment.NewLine
                + "Failures: " + failures.Count + Environment.NewLine
                + "This is an offline import/visual review, not a game compatibility, physics, or performance test."
                + Environment.NewLine;

            File.WriteAllText(ReportRoot + "/unity-import-report.csv", csv.ToString(), new UTF8Encoding(false));
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

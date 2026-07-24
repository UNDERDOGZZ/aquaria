using System.IO;
using Acuaria.Environment;
using Acuaria.Fish;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Acuaria.Editor
{
    public static class FirstFishSetup
    {
        private const string AquariumScenePath = "Assets/_Acuaria/Scenes/Aquarium.unity";
        private const string SpeciesPath = "Assets/_Acuaria/Data/Fish/PrototypeFish.asset";
        private const string FishPrefabPath = "Assets/_Acuaria/Prefabs/Fish/FishPrototype.prefab";
        private const string BodyMaterialPath =
            "Assets/_Acuaria/Art/Materials/FishPrototypeBody.mat";
        private const string EyeMaterialPath =
            "Assets/_Acuaria/Art/Materials/FishPrototypeEye.mat";

        [MenuItem("Acuaria/Setup First Fish")]
        public static void Configure()
        {
            FishSpecies species = GetOrCreateSpecies();
            Material bodyMaterial = GetOrCreateMaterial(
                BodyMaterialPath,
                species.PrototypeColor,
                0.35f);
            Material eyeMaterial = GetOrCreateMaterial(
                EyeMaterialPath,
                new Color(0.01f, 0.015f, 0.02f, 1f),
                0.1f);
            CreateFishPrefab(bodyMaterial, eyeMaterial);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            species = AssetDatabase.LoadAssetAtPath<FishSpecies>(SpeciesPath);
            FishMovement fishPrefab =
                AssetDatabase.LoadAssetAtPath<GameObject>(FishPrefabPath).GetComponent<FishMovement>();
            ConfigureAquariumScene(species, fishPrefab);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Acuaria first fish configured.");
        }

        public static void ConfigureFromCommandLine()
        {
            Configure();
        }

        private static FishSpecies GetOrCreateSpecies()
        {
            FishSpecies species = AssetDatabase.LoadAssetAtPath<FishSpecies>(SpeciesPath);
            if (species != null)
            {
                return species;
            }

            species = ScriptableObject.CreateInstance<FishSpecies>();
            AssetDatabase.CreateAsset(species, SpeciesPath);
            return species;
        }

        private static void CreateFishPrefab(Material bodyMaterial, Material eyeMaterial)
        {
            GameObject root = new GameObject("FishPrototype");
            FishMovement movement = root.AddComponent<FishMovement>();

            CreatePrimitive(
                PrimitiveType.Sphere,
                "Body",
                root.transform,
                Vector3.zero,
                new Vector3(0.72f, 0.48f, 1.5f),
                Quaternion.identity,
                bodyMaterial);
            CreatePrimitive(
                PrimitiveType.Cube,
                "Tail",
                root.transform,
                new Vector3(0f, 0f, -0.9f),
                new Vector3(0.08f, 0.72f, 0.62f),
                Quaternion.Euler(0f, 0f, 45f),
                bodyMaterial);
            CreatePrimitive(
                PrimitiveType.Sphere,
                "LeftEye",
                root.transform,
                new Vector3(-0.29f, 0.12f, 0.52f),
                Vector3.one * 0.13f,
                Quaternion.identity,
                eyeMaterial);
            CreatePrimitive(
                PrimitiveType.Sphere,
                "RightEye",
                root.transform,
                new Vector3(0.29f, 0.12f, 0.52f),
                Vector3.one * 0.13f,
                Quaternion.identity,
                eyeMaterial);

            PrefabUtility.SaveAsPrefabAsset(root, FishPrefabPath);
            Object.DestroyImmediate(root);
        }

        private static void ConfigureAquariumScene(FishSpecies species, FishMovement fishPrefab)
        {
            Scene scene = EditorSceneManager.OpenScene(AquariumScenePath, OpenSceneMode.Single);
            Transform systems = FindTransform(scene, "AquariumScene/Systems");
            AquariumVolume aquariumVolume = FindComponent<AquariumVolume>(scene);

            if (systems == null || aquariumVolume == null)
            {
                throw new InvalidDataException(
                    "Aquarium scene requires AquariumScene/Systems and an AquariumVolume.");
            }

            FishSpawner spawner = systems.GetComponentInChildren<FishSpawner>(true);
            if (spawner == null)
            {
                GameObject spawnerObject = new GameObject("FishSpawner");
                spawnerObject.transform.SetParent(systems, false);
                spawner = spawnerObject.AddComponent<FishSpawner>();
            }

            spawner.Configure(species, fishPrefab, aquariumVolume);
            EditorUtility.SetDirty(spawner);
            EditorSceneManager.MarkSceneDirty(scene);

            SerializedObject validation = new SerializedObject(spawner);
            if (validation.FindProperty("species").objectReferenceValue == null
                || validation.FindProperty("fishPrefab").objectReferenceValue == null
                || validation.FindProperty("aquariumVolume").objectReferenceValue == null)
            {
                throw new InvalidDataException("FishSpawner references could not be serialized.");
            }

            if (!EditorSceneManager.SaveScene(scene, AquariumScenePath))
            {
                throw new IOException($"Could not save scene at {AquariumScenePath}.");
            }
        }

        private static Transform FindTransform(Scene scene, string path)
        {
            string[] segments = path.Split('/');
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                if (root.name != segments[0])
                {
                    continue;
                }

                Transform current = root.transform;
                for (int i = 1; i < segments.Length && current != null; i++)
                {
                    current = current.Find(segments[i]);
                }

                return current;
            }

            return null;
        }

        private static T FindComponent<T>(Scene scene) where T : Component
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                T component = root.GetComponentInChildren<T>(true);
                if (component != null)
                {
                    return component;
                }
            }

            return null;
        }

        private static GameObject CreatePrimitive(
            PrimitiveType primitiveType,
            string name,
            Transform parent,
            Vector3 localPosition,
            Vector3 localScale,
            Quaternion localRotation,
            Material material)
        {
            GameObject primitive = GameObject.CreatePrimitive(primitiveType);
            primitive.name = name;
            primitive.transform.SetParent(parent, false);
            primitive.transform.localPosition = localPosition;
            primitive.transform.localScale = localScale;
            primitive.transform.localRotation = localRotation;
            Object.DestroyImmediate(primitive.GetComponent<Collider>());
            primitive.GetComponent<MeshRenderer>().sharedMaterial = material;
            return primitive;
        }

        private static Material GetOrCreateMaterial(
            string path,
            Color color,
            float smoothness)
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit");
                material = new Material(shader);
                AssetDatabase.CreateAsset(material, path);
            }

            material.color = color;
            material.SetFloat("_Smoothness", smoothness);
            EditorUtility.SetDirty(material);
            return material;
        }
    }
}

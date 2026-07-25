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
        private const string ScenePath = "Assets/_Acuaria/Scenes/Aquarium.unity";
        private const string PrefabPath = "Assets/_Acuaria/Prefabs/Fish/FishPrototype.prefab";
        private const string BodyMaterialPath = "Assets/_Acuaria/Art/Materials/FishPrototypeBody.mat";
        private const string EyeMaterialPath = "Assets/_Acuaria/Art/Materials/FishPrototypeEye.mat";

        [MenuItem("Acuaria/Setup Multi-Fish Prototype")]
        public static void Configure()
        {
            Material body = GetOrCreateMaterial(BodyMaterialPath, Color.white, 0.35f);
            Material eye = GetOrCreateMaterial(EyeMaterialPath, new Color(0.01f, 0.015f, 0.02f), 0.1f);
            CreateFishPrefab(body, eye);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            FishMovement prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath)
                .GetComponent<FishMovement>();

            FishSpecies neon = ConfigureSpecies("Assets/_Acuaria/Data/Fish/NeonTetraPrototype.asset",
                "neon-tetra-prototype", "Neon Tetra Prototype", prefab,
                0.38f, 0.5f, 0.75f, 1.05f, 0.6f, 0.8f, 110f,
                0.25f, 0.65f, 0.45f, 16f, 14f, new Color(0.05f, 0.75f, 0.95f), 8);
            FishSpecies guppy = ConfigureSpecies("Assets/_Acuaria/Data/Fish/GuppyPrototype.asset",
                "guppy-prototype", "Guppy Prototype", prefab,
                0.52f, 0.68f, 0.52f, 0.82f, 0.4f, 0.55f, 80f,
                0.42f, 0.82f, 0.55f, 18f, 16f, new Color(0.95f, 0.58f, 0.12f), 4);
            FishSpecies angel = ConfigureSpecies("Assets/_Acuaria/Data/Fish/AngelfishPrototype.asset",
                "angelfish-prototype", "Angelfish Prototype", prefab,
                0.82f, 1.02f, 0.32f, 0.55f, 0.22f, 0.35f, 55f,
                0.2f, 0.7f, 0.35f, 12f, 10f, new Color(0.78f, 0.68f, 0.92f), 2);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            neon = AssetDatabase.LoadAssetAtPath<FishSpecies>(
                "Assets/_Acuaria/Data/Fish/NeonTetraPrototype.asset");
            guppy = AssetDatabase.LoadAssetAtPath<FishSpecies>(
                "Assets/_Acuaria/Data/Fish/GuppyPrototype.asset");
            angel = AssetDatabase.LoadAssetAtPath<FishSpecies>(
                "Assets/_Acuaria/Data/Fish/AngelfishPrototype.asset");
            ConfigureScene(neon, guppy, angel);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Acuaria multi-fish prototype configured.");
        }

        public static void ConfigureFromCommandLine() => Configure();

        private static FishSpecies ConfigureSpecies(string path, string id, string name,
            FishMovement prefab, float minScale, float maxScale, float minSpeed, float maxSpeed,
            float acceleration, float deceleration, float turnSpeed, float depthMin, float depthMax,
            float verticalVariation, float ascent, float descent, Color color, int count)
        {
            FishSpecies species = AssetDatabase.LoadAssetAtPath<FishSpecies>(path);
            if (species == null)
            {
                species = ScriptableObject.CreateInstance<FishSpecies>();
                AssetDatabase.CreateAsset(species, path);
            }
            SerializedObject data = new SerializedObject(species);
            Set(data, "speciesId", id); Set(data, "displayName", name);
            Set(data, "visualPrefab", prefab); Set(data, "minimumScale", minScale);
            Set(data, "maximumScale", maxScale); Set(data, "minimumSpeed", minSpeed);
            Set(data, "maximumSpeed", maxSpeed); Set(data, "acceleration", acceleration);
            Set(data, "deceleration", deceleration); Set(data, "turningSpeed", turnSpeed);
            Set(data, "minimumDirectionTime", 1.6f); Set(data, "maximumDirectionTime", 4.5f);
            Set(data, "wallSafetyDistance", Mathf.Max(0.45f, maxScale * 0.7f));
            Set(data, "preferredDepthMinimum", depthMin); Set(data, "preferredDepthMaximum", depthMax);
            Set(data, "maximumVerticalVariation", verticalVariation);
            Set(data, "maximumAscentAngle", ascent); Set(data, "maximumDescentAngle", descent);
            Set(data, "separationRadius", maxScale * 1.15f); Set(data, "separationStrength", 0.55f);
            Set(data, "maximumSeparation", 0.35f); Set(data, "maximumVisualBank", 4f);
            Set(data, "swimOscillation", 1.1f); Set(data, "prototypeColor", color);
            Set(data, "suggestedPrototypeCount", count);
            data.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(species);
            return species;
        }

        private static void ConfigureScene(params FishSpecies[] species)
        {
            Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            Transform root = Find(scene, "AquariumScene");
            Transform systems = Find(scene, "AquariumScene/Systems");
            AquariumVolume volume = FindComponent<AquariumVolume>(scene);
            if (root == null || systems == null || volume == null)
                throw new InvalidDataException("Aquarium scene foundation is incomplete.");

            FishRegistry registry = systems.GetComponentInChildren<FishRegistry>(true);
            if (registry == null)
            {
                GameObject go = new GameObject("FishRegistry");
                go.transform.SetParent(systems, false);
                registry = go.AddComponent<FishRegistry>();
            }
            FishSpawner spawner = systems.GetComponentInChildren<FishSpawner>(true);
            if (spawner == null)
            {
                GameObject go = new GameObject("FishSpawner");
                go.transform.SetParent(systems, false);
                spawner = go.AddComponent<FishSpawner>();
            }
            Transform runtime = root.Find("RuntimeFish");
            if (runtime == null)
            {
                runtime = new GameObject("RuntimeFish").transform;
                runtime.SetParent(root, false);
            }

            FishSpawnGroup[] configuredGroups = new FishSpawnGroup[species.Length];
            for (int i = 0; i < species.Length; i++)
            {
                configuredGroups[i] = new FishSpawnGroup(
                    species[i],
                    species[i].SuggestedPrototypeCount,
                    0.42f,
                    1001 + i * 7919);
            }
            spawner.Configure(volume, registry, runtime, configuredGroups);
            EditorUtility.SetDirty(spawner);
            EditorSceneManager.MarkSceneDirty(scene);
            SerializedObject validation = new SerializedObject(spawner);
            SerializedProperty groups = validation.FindProperty("groups");
            for (int i = 0; i < groups.arraySize; i++)
            {
                if (groups.GetArrayElementAtIndex(i).FindPropertyRelative("species")
                    .objectReferenceValue == null)
                {
                    throw new InvalidDataException("A FishSpecies reference could not be serialized.");
                }
            }
            if (!EditorSceneManager.SaveScene(scene, ScenePath))
                throw new IOException($"Could not save {ScenePath}.");
        }

        private static void CreateFishPrefab(Material body, Material eye)
        {
            GameObject fish = new GameObject("FishPrototype");
            FishMovement movement = fish.AddComponent<FishMovement>();
            Transform visual = new GameObject("Visual").transform;
            visual.SetParent(fish.transform, false);
            Renderer bodyRenderer = CreatePrimitive(PrimitiveType.Sphere, "Body", visual,
                Vector3.zero, new Vector3(0.72f, 0.48f, 1.5f), Quaternion.identity, body);
            Renderer tailRenderer = CreatePrimitive(PrimitiveType.Cube, "Tail", visual,
                new Vector3(0f, 0f, -0.9f), new Vector3(0.08f, 0.72f, 0.62f),
                Quaternion.Euler(0f, 0f, 45f), body);
            CreatePrimitive(PrimitiveType.Sphere, "LeftEye", visual,
                new Vector3(-0.29f, 0.12f, 0.52f), Vector3.one * 0.13f, Quaternion.identity, eye);
            CreatePrimitive(PrimitiveType.Sphere, "RightEye", visual,
                new Vector3(0.29f, 0.12f, 0.52f), Vector3.one * 0.13f, Quaternion.identity, eye);
            movement.ConfigureVisual(visual, new[] { bodyRenderer, tailRenderer });
            PrefabUtility.SaveAsPrefabAsset(fish, PrefabPath);
            Object.DestroyImmediate(fish);
        }

        private static Renderer CreatePrimitive(PrimitiveType type, string name, Transform parent,
            Vector3 position, Vector3 scale, Quaternion rotation, Material material)
        {
            GameObject go = GameObject.CreatePrimitive(type);
            go.name = name; go.transform.SetParent(parent, false);
            go.transform.localPosition = position; go.transform.localScale = scale;
            go.transform.localRotation = rotation;
            Object.DestroyImmediate(go.GetComponent<Collider>());
            Renderer renderer = go.GetComponent<Renderer>(); renderer.sharedMaterial = material;
            return renderer;
        }

        private static Material GetOrCreateMaterial(string path, Color color, float smoothness)
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                AssetDatabase.CreateAsset(material, path);
            }
            material.color = color; material.SetFloat("_Smoothness", smoothness);
            EditorUtility.SetDirty(material); return material;
        }

        private static Transform Find(Scene scene, string path)
        {
            string[] parts = path.Split('/');
            foreach (GameObject go in scene.GetRootGameObjects())
            {
                if (go.name != parts[0]) continue;
                Transform result = go.transform;
                for (int i = 1; i < parts.Length && result != null; i++) result = result.Find(parts[i]);
                return result;
            }
            return null;
        }

        private static T FindComponent<T>(Scene scene) where T : Component
        {
            foreach (GameObject go in scene.GetRootGameObjects())
            {
                T value = go.GetComponentInChildren<T>(true);
                if (value != null) return value;
            }
            return null;
        }

        private static void Set(SerializedObject target, string name, string value) =>
            target.FindProperty(name).stringValue = value;
        private static void Set(SerializedObject target, string name, float value) =>
            target.FindProperty(name).floatValue = value;
        private static void Set(SerializedObject target, string name, int value) =>
            target.FindProperty(name).intValue = value;
        private static void Set(SerializedObject target, string name, Color value) =>
            target.FindProperty(name).colorValue = value;
        private static void Set(SerializedObject target, string name, Object value) =>
            target.FindProperty(name).objectReferenceValue = value;
    }
}

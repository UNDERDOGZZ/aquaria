using System.IO;
using System.Linq;
using Acuaria.Environment;
using Acuaria.Fish;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Acuaria.Editor
{
    public static class FishAliveIntegrationSetup
    {
        private const string ScenePath = "Assets/_Acuaria/Scenes/Aquarium.unity";
        private const string GuppySource =
            "Assets/DenysAlmaral/FishAlive/Prefabs/FishFreshwater/freshWater_guppy.prefab";
        private const string ClownfishSource =
            "Assets/DenysAlmaral/FishAlive/Prefabs/FishMarine/marine_clownfish.prefab";
        private const string GuppyAdapter = "Assets/_Acuaria/Prefabs/Fish/Fish_Guppy.prefab";
        private const string ClownfishAdapter = "Assets/_Acuaria/Prefabs/Fish/Fish_Clownfish.prefab";
        private const string GuppySpecies = "Assets/_Acuaria/Data/Fish/GuppyPrototype.asset";
        private const string ClownfishSpecies = "Assets/_Acuaria/Data/Fish/ClownfishPrototype.asset";
        private const string NeonSpecies = "Assets/_Acuaria/Data/Fish/NeonTetraPrototype.asset";

        [MenuItem("Acuaria/Integrate Fish Alive Samples")]
        public static void Configure()
        {
            CreateAdapter(GuppySource, GuppyAdapter, "Fish_Guppy", 1f);
            CreateAdapter(ClownfishSource, ClownfishAdapter, "Fish_Clownfish", 1f);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            FishSpecies guppy = ConfigureSpecies(GuppySpecies, "guppy", "Guppy",
                LoadMovement(GuppyAdapter), 0.8f, 1f, 0.58f, 0.92f, 0.48f, 0.62f,
                95f, 0.28f, 0.78f, 0.5f, 17f, 15f, 6);
            FishSpecies clownfish = ConfigureSpecies(ClownfishSpecies, "clownfish", "Clownfish",
                LoadMovement(ClownfishAdapter), 0.9f, 1.12f, 0.42f, 0.7f, 0.34f, 0.48f,
                72f, 0.22f, 0.7f, 0.4f, 14f, 12f, 4);
            FishSpecies neon = AssetDatabase.LoadAssetAtPath<FishSpecies>(NeonSpecies);
            if (neon == null) throw new InvalidDataException("Run the Sprint 4 setup first.");

            AssetDatabase.SaveAssets();
            ConfigureScene(guppy, clownfish, neon);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Fish Alive visual integration configured.");
        }

        public static void ConfigureFromCommandLine() => Configure();

        private static void CreateAdapter(string sourcePath, string destinationPath,
            string rootName, float visualScale)
        {
            GameObject source = AssetDatabase.LoadAssetAtPath<GameObject>(sourcePath);
            if (source == null) throw new FileNotFoundException("Fish Alive prefab missing.", sourcePath);

            GameObject root = new GameObject(rootName);
            FishMovement movement = root.AddComponent<FishMovement>();
            FishAnimationController animation = root.AddComponent<FishAnimationController>();
            Transform visual = new GameObject("Visual").transform;
            visual.SetParent(root.transform, false);
            visual.localScale = Vector3.one * visualScale;

            GameObject model = (GameObject)PrefabUtility.InstantiatePrefab(source);
            model.name = "FishAliveModel";
            model.transform.SetParent(visual, false);
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
            model.transform.localScale = Vector3.one;

            foreach (MonoBehaviour behaviour in model.GetComponentsInChildren<MonoBehaviour>(true))
                Object.DestroyImmediate(behaviour);
            foreach (Rigidbody body in model.GetComponentsInChildren<Rigidbody>(true))
                Object.DestroyImmediate(body);
            foreach (Collider collider in model.GetComponentsInChildren<Collider>(true))
                Object.DestroyImmediate(collider);

            Animator animator = model.GetComponentInChildren<Animator>(true);
            if (animator == null) throw new InvalidDataException($"{sourcePath} has no Animator.");
            animator.applyRootMotion = false;
            movement.ConfigureVisual(visual, new Renderer[0]);
            animation.Configure(movement, animator);
            PrefabUtility.SaveAsPrefabAsset(root, destinationPath);
            Object.DestroyImmediate(root);
        }

        private static FishMovement LoadMovement(string path)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            return prefab != null ? prefab.GetComponent<FishMovement>() : null;
        }

        private static FishSpecies ConfigureSpecies(string path, string id, string displayName,
            FishMovement prefab, float minScale, float maxScale, float minSpeed, float maxSpeed,
            float acceleration, float deceleration, float turnSpeed, float depthMin, float depthMax,
            float verticalVariation, float ascent, float descent, int count)
        {
            FishSpecies species = AssetDatabase.LoadAssetAtPath<FishSpecies>(path);
            if (species == null)
            {
                species = ScriptableObject.CreateInstance<FishSpecies>();
                AssetDatabase.CreateAsset(species, path);
            }
            SerializedObject data = new SerializedObject(species);
            Set(data, "speciesId", id); Set(data, "displayName", displayName);
            Set(data, "visualPrefab", prefab); Set(data, "minimumScale", minScale);
            Set(data, "maximumScale", maxScale); Set(data, "minimumSpeed", minSpeed);
            Set(data, "maximumSpeed", maxSpeed); Set(data, "acceleration", acceleration);
            Set(data, "deceleration", deceleration); Set(data, "turningSpeed", turnSpeed);
            Set(data, "minimumDirectionTime", 1.7f); Set(data, "maximumDirectionTime", 4.4f);
            Set(data, "wallSafetyDistance", Mathf.Max(0.45f, maxScale * 0.65f));
            Set(data, "preferredDepthMinimum", depthMin); Set(data, "preferredDepthMaximum", depthMax);
            Set(data, "maximumVerticalVariation", verticalVariation);
            Set(data, "maximumAscentAngle", ascent); Set(data, "maximumDescentAngle", descent);
            Set(data, "separationRadius", maxScale); Set(data, "separationStrength", 0.55f);
            Set(data, "maximumSeparation", 0.35f); Set(data, "maximumVisualBank", 4f);
            Set(data, "swimOscillation", 0f); Set(data, "prototypeColor", Color.white);
            Set(data, "suggestedPrototypeCount", count);
            data.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(species);
            return species;
        }

        private static void ConfigureScene(FishSpecies guppy, FishSpecies clownfish, FishSpecies neon)
        {
            Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            AquariumVolume volume = FindComponent<AquariumVolume>(scene);
            FishRegistry registry = FindComponent<FishRegistry>(scene);
            FishSpawner spawner = FindComponent<FishSpawner>(scene);
            Transform runtime = Find(scene, "AquariumScene/RuntimeFish");
            if (volume == null || registry == null || spawner == null || runtime == null)
                throw new InvalidDataException("Sprint 4 aquarium scene is incomplete.");
            FishSpawnGroup[] groups =
            {
                new FishSpawnGroup(guppy, 6, 0.42f, 1001),
                new FishSpawnGroup(clownfish, 4, 0.42f, 8920),
                new FishSpawnGroup(neon, 2, 0.42f, 16839)
            };
            spawner.Configure(volume, registry, runtime, groups);
            EditorUtility.SetDirty(spawner);
            EditorSceneManager.MarkSceneDirty(scene);
            if (!EditorSceneManager.SaveScene(scene, ScenePath))
                throw new IOException($"Could not save {ScenePath}.");
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

        private static T FindComponent<T>(Scene scene) where T : Component =>
            scene.GetRootGameObjects().Select(go => go.GetComponentInChildren<T>(true))
                .FirstOrDefault(value => value != null);

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

using System.IO;
using Acuaria.Environment;
using Acuaria.Input;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Acuaria.Editor
{
    public static class AquariumPrototypeSetup
    {
        private const string AquariumScenePath = "Assets/_Acuaria/Scenes/Aquarium.unity";
        private const string TankPrefabPath =
            "Assets/_Acuaria/Prefabs/Environment/AquariumTankPrototype.prefab";
        private const string CameraPrefabPath = "Assets/_Acuaria/Prefabs/Core/CameraRig.prefab";
        private const string MaterialsDirectory = "Assets/_Acuaria/Art/Materials";

        [InitializeOnLoadMethod]
        private static void ConfigureMissingPrototype()
        {
            if (File.Exists(Path.GetFullPath(AquariumScenePath))
                && !SceneContainsPrototypeMarker())
            {
                EditorApplication.delayCall += Configure;
            }
        }

        [MenuItem("Acuaria/Setup Aquarium Prototype")]
        public static void Configure()
        {
            Material frameMaterial = GetOrCreateMaterial(
                "AquariumFramePrototype", new Color(0.035f, 0.07f, 0.09f, 1f), 0.2f);
            Material interiorMaterial = GetOrCreateMaterial(
                "AquariumInteriorPrototype", new Color(0.12f, 0.22f, 0.24f, 1f), 0.05f);
            Material backgroundMaterial = GetOrCreateMaterial(
                "AquariumBackgroundPrototype", new Color(0.025f, 0.08f, 0.11f, 1f), 0f);

            CreateTankPrefab(frameMaterial, interiorMaterial, backgroundMaterial);
            CreateCameraRigPrefab();
            CreateAquariumScene();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Acuaria aquarium prototype configured.");
        }

        public static void ConfigureFromCommandLine()
        {
            Configure();
        }

        private static bool SceneContainsPrototypeMarker()
        {
            Scene scene = SceneManager.GetSceneByPath(AquariumScenePath);
            bool wasLoaded = scene.IsValid() && scene.isLoaded;
            if (!wasLoaded)
            {
                scene = EditorSceneManager.OpenScene(AquariumScenePath, OpenSceneMode.Additive);
            }

            bool containsMarker = false;
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                if (root.transform.Find("Environment/AquariumTank") != null)
                {
                    containsMarker = true;
                    break;
                }
            }

            if (!wasLoaded)
            {
                EditorSceneManager.CloseScene(scene, true);
            }

            return containsMarker;
        }

        private static void CreateTankPrefab(
            Material frameMaterial,
            Material interiorMaterial,
            Material backgroundMaterial)
        {
            GameObject root = new GameObject("AquariumTank");
            root.AddComponent<AquariumVolume>();

            Transform interior = new GameObject("AquariumInterior").transform;
            interior.SetParent(root.transform, false);

            CreateCube("Base", root.transform, new Vector3(0f, -0.2f, 0f),
                new Vector3(8.8f, 0.4f, 3.8f), frameMaterial);
            CreateCube("InteriorFloor", interior, new Vector3(0f, 0.05f, 0f),
                new Vector3(8f, 0.1f, 3f), interiorMaterial);
            CreateCube("Background", root.transform, new Vector3(0f, 1.6f, 1.6f),
                new Vector3(8.8f, 3.6f, 0.18f), backgroundMaterial);
            CreateCube("LeftFrame", root.transform, new Vector3(-4.3f, 1.6f, 0f),
                new Vector3(0.22f, 3.6f, 3.4f), frameMaterial);
            CreateCube("RightFrame", root.transform, new Vector3(4.3f, 1.6f, 0f),
                new Vector3(0.22f, 3.6f, 3.4f), frameMaterial);
            CreateCube("TopFrame", root.transform, new Vector3(0f, 3.35f, 0f),
                new Vector3(8.8f, 0.18f, 3.4f), frameMaterial);

            PrefabUtility.SaveAsPrefabAsset(root, TankPrefabPath);
            Object.DestroyImmediate(root);
        }

        private static void CreateCameraRigPrefab()
        {
            GameObject root = new GameObject("CameraRig");
            PointerInputReader inputReader = root.AddComponent<PointerInputReader>();
            AquariumCameraController controller = root.AddComponent<AquariumCameraController>();

            Transform pivot = new GameObject("CameraPivot").transform;
            pivot.SetParent(root.transform, false);

            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.SetParent(pivot, false);
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.015f, 0.035f, 0.045f, 1f);
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 100f;
            cameraObject.AddComponent<AudioListener>();

            SerializedObject serializedController = new SerializedObject(controller);
            serializedController.FindProperty("inputReader").objectReferenceValue = inputReader;
            serializedController.FindProperty("cameraPivot").objectReferenceValue = pivot;
            serializedController.FindProperty("controlledCamera").objectReferenceValue = camera;
            serializedController.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(root, CameraPrefabPath);
            Object.DestroyImmediate(root);
        }

        private static void CreateAquariumScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            Transform sceneRoot = new GameObject("AquariumScene").transform;

            Transform environment = CreateGroup("Environment", sceneRoot);
            GameObject tankPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(TankPrefabPath);
            GameObject tank = (GameObject)PrefabUtility.InstantiatePrefab(tankPrefab, scene);
            tank.name = "AquariumTank";
            tank.transform.SetParent(environment, false);

            GameObject cameraPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(CameraPrefabPath);
            GameObject cameraRig = (GameObject)PrefabUtility.InstantiatePrefab(cameraPrefab, scene);
            cameraRig.transform.SetParent(sceneRoot, false);
            AquariumCameraController controller = cameraRig.GetComponent<AquariumCameraController>();
            AquariumVolume volume = tank.GetComponent<AquariumVolume>();
            SerializedObject serializedController = new SerializedObject(controller);
            serializedController.FindProperty("aquariumVolume").objectReferenceValue = volume;
            serializedController.ApplyModifiedPropertiesWithoutUndo();

            Transform lighting = CreateGroup("Lighting", sceneRoot);
            GameObject lightObject = new GameObject("Main Light");
            lightObject.transform.SetParent(lighting, false);
            lightObject.transform.rotation = Quaternion.Euler(45f, -30f, 0f);
            Light mainLight = lightObject.AddComponent<Light>();
            mainLight.type = LightType.Directional;
            mainLight.color = new Color(0.92f, 0.97f, 1f);
            mainLight.intensity = 1.1f;
            mainLight.shadows = LightShadows.None;

            CreateGroup("Systems", sceneRoot);
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.22f, 0.27f, 0.3f);

            if (!EditorSceneManager.SaveScene(scene, AquariumScenePath))
            {
                throw new IOException($"Could not save scene at {AquariumScenePath}.");
            }
        }

        private static Transform CreateGroup(string name, Transform parent)
        {
            Transform group = new GameObject(name).transform;
            group.SetParent(parent, false);
            return group;
        }

        private static GameObject CreateCube(
            string name,
            Transform parent,
            Vector3 localPosition,
            Vector3 localScale,
            Material material)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.SetParent(parent, false);
            cube.transform.localPosition = localPosition;
            cube.transform.localScale = localScale;
            Object.DestroyImmediate(cube.GetComponent<Collider>());
            cube.GetComponent<MeshRenderer>().sharedMaterial = material;
            return cube;
        }

        private static Material GetOrCreateMaterial(string name, Color color, float smoothness)
        {
            string path = $"{MaterialsDirectory}/{name}.mat";
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit");
                material = new Material(shader) { name = name };
                AssetDatabase.CreateAsset(material, path);
            }

            material.color = color;
            material.SetFloat("_Smoothness", smoothness);
            EditorUtility.SetDirty(material);
            return material;
        }
    }
}

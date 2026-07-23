using System;
using System.IO;
using System.Linq;
using Acuaria.Core;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Acuaria.Editor
{
    public static class ProjectFoundationSetup
    {
        private const string ScenesDirectory = "Assets/_Acuaria/Scenes";

        [InitializeOnLoadMethod]
        private static void ConfigureMissingFoundation()
        {
            string[] requiredScenePaths =
            {
                GetScenePath("Bootstrap"),
                GetScenePath("MainMenu"),
                GetScenePath("Aquarium")
            };

            if (requiredScenePaths.Any(path => !File.Exists(Path.GetFullPath(path))))
            {
                EditorApplication.delayCall += Configure;
            }
        }

        [MenuItem("Acuaria/Setup Project Foundation")]
        public static void Configure()
        {
            EnsureDirectory(ScenesDirectory);
            CreateBootstrapScene();
            CreateCameraScene("MainMenu", "MainMenuScene");
            CreateCameraScene("Aquarium", "AquariumScene");
            ConfigureBuildScenes();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Acuaria project foundation configured.");
        }

        public static void ConfigureFromCommandLine()
        {
            Configure();
        }

        private static void CreateBootstrapScene()
        {
            string path = GetScenePath("Bootstrap");
            if (File.Exists(Path.GetFullPath(path)))
            {
                return;
            }

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            GameObject application = new GameObject("Application");
            application.AddComponent<ApplicationBootstrap>();
            SaveScene(scene, path);
        }

        private static void CreateCameraScene(string sceneName, string rootName)
        {
            string path = GetScenePath(sceneName);
            if (File.Exists(Path.GetFullPath(path)))
            {
                return;
            }

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            new GameObject(rootName);

            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();

            SaveScene(scene, path);
        }

        private static void ConfigureBuildScenes()
        {
            string[] acuariaScenePaths =
            {
                GetScenePath("Bootstrap"),
                GetScenePath("MainMenu"),
                GetScenePath("Aquarium")
            };

            EditorBuildSettingsScene[] preservedScenes = EditorBuildSettings.scenes
                .Where(scene => !acuariaScenePaths.Contains(scene.path, StringComparer.Ordinal))
                .Select(scene => new EditorBuildSettingsScene(scene.path, false))
                .ToArray();

            EditorBuildSettings.scenes = acuariaScenePaths
                .Select(path => new EditorBuildSettingsScene(path, true))
                .Concat(preservedScenes)
                .ToArray();
        }

        private static string GetScenePath(string sceneName)
        {
            return $"{ScenesDirectory}/{sceneName}.unity";
        }

        private static void SaveScene(Scene scene, string path)
        {
            if (!EditorSceneManager.SaveScene(scene, path))
            {
                throw new InvalidOperationException($"Could not save scene at {path}.");
            }
        }

        private static void EnsureDirectory(string assetPath)
        {
            string fullPath = Path.GetFullPath(assetPath);
            Directory.CreateDirectory(fullPath);
        }
    }
}

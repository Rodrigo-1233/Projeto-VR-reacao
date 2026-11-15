using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ReflexLabSetup
{
    public static void SetupProject()
    {
        Debug.Log("Starting ReflexLab Setup...");

        string[] directories = new string[]
        {
            "Assets/_Project",
            "Assets/_Project/Scenes",
            "Assets/_Project/Scripts",
            "Assets/_Project/Scripts/Core",
            "Assets/_Project/Scripts/Data",
            "Assets/_Project/Scripts/UI",
            "Assets/_Project/Scripts/Activities",
            "Assets/_Project/Scripts/Activities/FallingBars",
            "Assets/_Project/Scripts/Activities/TargetShooting",
            "Assets/_Project/Scripts/Activities/ObjectSelection",
            "Assets/_Project/Prefabs",
            "Assets/_Project/Materials",
            "Assets/_Project/Models",
            "Assets/_Project/Textures",
            "Assets/_Project/Audio",
            "Assets/_Project/Animations",
            "Assets/_Project/UI",
            "Assets/_Project/Fonts"
        };

        foreach (var dir in directories)
        {
            if (!AssetDatabase.IsValidFolder(dir))
            {
                string parent = Path.GetDirectoryName(dir).Replace("\\", "/");
                string newFolder = Path.GetFileName(dir);
                AssetDatabase.CreateFolder(parent, newFolder);
                Debug.Log($"Created folder: {dir}");
            }
        }

        string[] scenes = new string[]
        {
            "MainMenu",
            "HospitalRoom",
            "FallingBars",
            "TargetShooting",
            "ObjectSelection",
            "Results"
        };

        var buildScenes = new EditorBuildSettingsScene[scenes.Length];
        for (int i = 0; i < scenes.Length; i++)
        {
            string sceneName = scenes[i];
            string scenePath = $"Assets/_Project/Scenes/{sceneName}.unity";

            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            
            // Add a title object in the scene
            GameObject go = new GameObject($"{sceneName}_Root");
            EditorSceneManager.SaveScene(newScene, scenePath);
            Debug.Log($"Created scene: {scenePath}");

            buildScenes[i] = new EditorBuildSettingsScene(scenePath, true);
        }

        EditorBuildSettings.scenes = buildScenes;
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("ReflexLab Setup Finished Successfully!");
    }
}

using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class ReflexLabBuildTest
{
    public static void TestBuild()
    {
        Debug.Log("Starting Windows Standalone build test...");
        string buildDir = "Builds/Windows";
        if (!Directory.Exists(buildDir))
        {
            Directory.CreateDirectory(buildDir);
        }

        string[] scenes = new string[]
        {
            "Assets/_Project/Scenes/MainMenu.unity"
        };

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = scenes;
        buildPlayerOptions.locationPathName = Path.Combine(buildDir, "ReflexLab_Test.exe");
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.Development;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"[BUILD TEST SUCCESS] Total size: {summary.totalSize} bytes, time: {summary.totalTime}");
        }
        else
        {
            Debug.LogError($"[BUILD TEST FAILED] Result: {summary.result}, errors: {summary.totalErrors}");
        }
    }
}

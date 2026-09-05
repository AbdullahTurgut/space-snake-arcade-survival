#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class AndroidBuildHelper
{
    [MenuItem("Build/Check Android Support")]
    public static bool CheckAndroidSupport()
    {
        bool supported = BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Android, BuildTarget.Android);
        Debug.Log($"[AndroidBuildHelper] Android Build Target Supported: {supported}");
        return supported;
    }

    [MenuItem("Build/Build Android APK (Development)")]
    public static void BuildAndroidApk()
    {
        if (!CheckAndroidSupport())
        {
            Debug.LogError("[AndroidBuildHelper] Cannot build Android APK: Android Build Support module is not installed in Unity Editor. Please install 'Android Build Support' and 'Android SDK & NDK Tools' via Unity Hub.");
            return;
        }

        string outputDir = Path.Combine(Directory.GetCurrentDirectory(), "Builds", "Android");
        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        string apkPath = Path.Combine(outputDir, "space-snake-arcade-survival.apk");

        string[] scenes = new string[]
        {
            "Assets/Scenes/MainMenu.unity",
            "Assets/Scenes/PlayScene.unity"
        };

        BuildPlayerOptions buildOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = apkPath,
            target = BuildTarget.Android,
            targetGroup = BuildTargetGroup.Android,
            options = BuildOptions.Development
        };

        Debug.Log($"[AndroidBuildHelper] Starting Android build to: {apkPath}");
        BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"[AndroidBuildHelper] Build SUCCEEDED: {summary.totalSize} bytes at {apkPath}");
        }
        else if (summary.result == BuildResult.Failed)
        {
            Debug.LogError($"[AndroidBuildHelper] Build FAILED with {summary.totalErrors} errors.");
        }
    }
}
#endif

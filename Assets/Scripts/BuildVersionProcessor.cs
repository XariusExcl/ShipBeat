using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class BuildVersionProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        string version = PlayerSettings.bundleVersion;
        string buildNumber = DateTime.Now.ToString("yyyyMMdd.HHmm");
        string fullVersion = $"{version}.{buildNumber}";

        PlayerSettings.bundleVersion = fullVersion;
        Debug.Log($"Build version set to: {fullVersion}");
    }

    [MenuItem("Build/Set Version")]
    public static void SetVersion()
    {
        string version = PlayerSettings.bundleVersion;
        string buildNumber = DateTime.Now.ToString("yyyyMMdd.HHmm");
        string fullVersion = $"{version}.{buildNumber}";

        PlayerSettings.bundleVersion = fullVersion;
        Debug.Log($"Build version set to: {fullVersion}");
    }
}
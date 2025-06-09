using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class BuildVersionProcessor : IPreprocessBuildWithReport
{
    const string VERSION = "0.2";
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        string buildNumber = DateTime.Now.ToString("yyyyMMdd.HHmm");
        string fullVersion = $"{VERSION}.{buildNumber}";

        PlayerSettings.bundleVersion = fullVersion;
    }

    [MenuItem("Build/Set Version")]
    public static void SetVersion()
    {
        string buildNumber = DateTime.Now.ToString("yyyyMMdd.HHmm");
        string fullVersion = $"{VERSION}.{buildNumber}";

        PlayerSettings.bundleVersion = fullVersion;
    }
}
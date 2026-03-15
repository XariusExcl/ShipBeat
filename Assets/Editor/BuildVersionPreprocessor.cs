using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class BuildVersionPreprocessor : IPreprocessBuildWithReport
{
    const string VERSION = "1.0";
    public int callbackOrder => 0;
    
    public void OnPreprocessBuild(BuildReport report)
    {
        string buildNumber = DateTime.Now.ToString("yyyyMMdd.HHmm");
        string fullVersion = $"{VERSION}.{buildNumber}";

        UnityEditor.PlayerSettings.bundleVersion = fullVersion;
    }

    [MenuItem("Build/Set Version")]
    public static void SetVersion()
    {
        string buildNumber = DateTime.Now.ToString("yyyyMMdd.HHmm");
        string fullVersion = $"{VERSION}.{buildNumber}";

        UnityEditor.PlayerSettings.bundleVersion = fullVersion;
    }
}
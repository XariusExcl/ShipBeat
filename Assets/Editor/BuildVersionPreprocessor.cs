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
        SetVersion();
    }

    [MenuItem("Build/Set Version")]
    public static void SetVersion()
    {
        string buildDate = DateTime.Now.ToString("yyMMdd");
        int buildNumber = int.Parse(UnityEditor.PlayerSettings.bundleVersion.Split("_")[1]) + 1;
        string fullVersion = $"{VERSION}.{buildDate}_{buildNumber}";

        UnityEditor.PlayerSettings.bundleVersion = fullVersion;
    }
}
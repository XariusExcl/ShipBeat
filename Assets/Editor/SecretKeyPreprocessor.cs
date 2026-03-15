using System.IO;
using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class SecretKeyPreprocessor : IPreprocessBuildWithReport
{
    public int callbackOrder => 1;

    public void OnPreprocessBuild(BuildReport report)
    {
        string secretFilePath = "Assets/secret";
        if (File.Exists(secretFilePath))
        {
            Secret.Key = File.ReadAllText(secretFilePath).Trim();
        }
        else
        {
            Debug.LogError($"Secret file not found at path: {secretFilePath}");
            Secret.Key = string.Empty;
        }
    }
}
// Cleans up all leftover data from not using domain reload.

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EditorJanitor : MonoBehaviour
{
    #if UNITY_EDITOR
    void Start()
    {
        EditorApplication.playModeStateChanged += OnExitPlayMode;
    }

    void OnExitPlayMode(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.ExitingPlayMode) return;
        OnlineDataManager.ClearData();
        EditorApplication.playModeStateChanged -= OnExitPlayMode;
    }
    #endif
}
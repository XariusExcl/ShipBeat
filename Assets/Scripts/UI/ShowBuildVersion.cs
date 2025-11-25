using UnityEngine;
using TMPro;

public class ShowBuildVersion : MonoBehaviour
{
    void Awake()
    {
        if (TryGetComponent(out TextMeshProUGUI versionText))
#if UNITY_EDITOR
            versionText.text = $"v{Application.version.Split(".")[0]}.{Application.version.Split(".")[1]}(Editor Build)";
#else
            versionText.text = $"v{Application.version}";
#endif
    }
}
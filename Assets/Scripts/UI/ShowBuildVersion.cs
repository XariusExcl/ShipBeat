using UnityEngine;
using TMPro;

public class ShowBuildVersion : MonoBehaviour
{
    void Awake()
    {
        if (TryGetComponent(out TextMeshProUGUI versionText))
            versionText.text = $"v{Application.version}";
    }
}
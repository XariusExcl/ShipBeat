using UnityEngine;

public class TargetFrameRate : MonoBehaviour
{
    void Start()
    {
        # if UNITY_EDITOR
        Application.targetFrameRate = 60;
        # endif
        // Frame rate will be capped at 60 on the arcade machine anyway, this is just for testing in the editor.
    }
}

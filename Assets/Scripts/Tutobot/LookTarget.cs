using UnityEngine;

public class LookTarget : MonoBehaviour
{
    Transform trackedObject;
    [SerializeField] Vector3 offset = Vector3.zero;
    public void Track(string name = "camera")
    {
        if (string.IsNullOrEmpty(name))
        {
            // Reset tracking if name is empty
            trackedObject = null;
            return;
        }

        if (name == "camera")
        {
            trackedObject = Camera.main?.transform;
            if (trackedObject == null)
                Debug.LogWarning("LookTarget: Main camera not found.");
            return;
        }

        trackedObject = GameObject.Find(name)?.transform;
        if (trackedObject == null)
            Debug.LogWarning($"LookTarget: Object '{name}' not found.");
    }

    public void Track(Transform target)
    {
        trackedObject = target;
    }

    public void Update()
    {
        if (trackedObject == null)
            return;

        transform.position = trackedObject.position + offset;
    }
}

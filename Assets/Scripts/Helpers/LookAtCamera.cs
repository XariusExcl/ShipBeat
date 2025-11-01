using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public enum Axis { X, Y, Z }

    [SerializeField]
    [Tooltip("Which axis should be aligned to face the camera (only that Euler axis will be changed).")]
    private Axis axis = Axis.Y;

    [SerializeField]
    [Tooltip("Apply look once when the object is enabled.")]
    private bool lookAwake = true;

    [SerializeField]
    [Tooltip("Apply look every frame.")]
    private bool lookOnUpdate = false;

    private void Awake()
    {
        if (lookAwake)
            ApplyLook();
    }

    private void Update()
    {
        if (lookOnUpdate)
            ApplyLook();
    }

    [ContextMenu("Apply Look")]
    private void ApplyLook()
    {
        Camera cam = Camera.main;
        if (cam == null)
            return;

        transform.LookAt(cam.transform, Vector3.up);

        switch (axis)
        {
            case Axis.X:
                transform.Rotate(0f, -90f, 0f);
                // Rotate around Y by -90 degrees, assuming y is up
                break;
            case Axis.Y:
                transform.Rotate(90f, 0f, 0f);
                // Rotate around X by 90 degrees, still assuming Y is up
                break;
        }

        /*

        Vector3 dir = cam.transform.position - transform.position;
        if (invert) dir = -dir;
        if (dir.sqrMagnitude < Mathf.Epsilon)
            return;

        // ok, a bit busted, we can fix that

        Quaternion lookRotation = Quaternion.LookRotation(dir, Vector3.right);
        Vector3 lookEuler = lookRotation.eulerAngles;
        Vector3 current = useLocalRotation ? transform.localEulerAngles : transform.eulerAngles;

        switch (axis)
        {
            case Axis.X:
                current.x = lookEuler.x;
                break;
            case Axis.Y:
                current.y = lookEuler.y;
                break;
            case Axis.Z:
                current.z = lookEuler.z;
                break;
        }

        if (useLocalRotation)
            transform.localEulerAngles = current;
        else
            transform.eulerAngles = current;

        */
    }
}

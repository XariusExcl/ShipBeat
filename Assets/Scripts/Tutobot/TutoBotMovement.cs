using UnityEngine;

public class TutoBotMovement : MonoBehaviour, ILookAt
{
    Quaternion targetRotation;
    public bool disableSlerp = false;
    void Start()
    {
        targetRotation = transform.rotation;
    }

    void Update()
    {
        if (!disableSlerp) transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    public void LookAt(Vector3 targetPosition = default)
    {
        if (targetPosition == default)
            targetPosition = Camera.main.transform.position;

        // look at the target position only by rotating around the Y axis
        Transform clonedTransform = Instantiate(transform, transform);
        clonedTransform.localPosition = Vector3.zero; // reset local position to avoid offset
        clonedTransform.LookAt(targetPosition);
        clonedTransform.rotation = Quaternion.Euler(0f, clonedTransform.rotation.eulerAngles.y, 0f);
        targetRotation = clonedTransform.rotation;
        Destroy(clonedTransform.gameObject);

    }
}
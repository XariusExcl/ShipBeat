using UnityEngine;

public class TutoBotMovement : MonoBehaviour
{
    Quaternion targetRotation;
    [SerializeField] LookTarget lookTarget;
    [SerializeField] public bool IsLookingAtTarget;
    [SerializeField] public bool disableSlerp = false;

    void Start()
    {
        targetRotation = transform.rotation;
    }

    void Update()
    {
        transform.rotation = disableSlerp ? targetRotation : Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        if (IsLookingAtTarget) LookAtTarget();
    }

    Vector3 cachedTargetPosition;
    void LookAtTarget()
    {
        if (cachedTargetPosition == lookTarget.transform.position)
            return;

        cachedTargetPosition = lookTarget.transform.position;

        // look at the target position, only around Y axis
        Transform clonedTransform = Instantiate(transform, transform);
        clonedTransform.localPosition = Vector3.zero; // reset local position to avoid offset
        clonedTransform.LookAt(lookTarget.transform.position);
        clonedTransform.rotation = Quaternion.Euler(0f, clonedTransform.rotation.eulerAngles.y, 0f);
        targetRotation = clonedTransform.rotation;
        Destroy(clonedTransform.gameObject);
    }
}
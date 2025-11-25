using UnityEngine;

public class TutoBotHead : MonoBehaviour
{
    Quaternion targetRotation;
    Vector3 initialPosition;
    [SerializeField] LookTarget lookTarget;
    [SerializeField] public bool IsLookingAtTarget;
    [SerializeField] public bool disableSlerp = false;
    void Start()
    {
        initialPosition = transform.localPosition;
        targetRotation = transform.rotation;
    }

    void Update()
    {
        // bob the head up and down
        float bobSpeed = 2f;
        float bobAmount = 0.01f;
        float bob = Mathf.Sin(Time.time * bobSpeed) * bobAmount;
        transform.localPosition = new Vector3(transform.localPosition.x, bob + initialPosition.y, transform.localPosition.z);
        transform.rotation = disableSlerp ? targetRotation : Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        if (IsLookingAtTarget) LookAtTarget();
    }

    Vector3 cachedTargetPosition;
    public void LookAtTarget()
    {
        if (cachedTargetPosition == lookTarget.transform.position)
            return;

        cachedTargetPosition = lookTarget.transform.position;

        Transform clonedTransform = Instantiate(transform, transform);
        clonedTransform.localPosition = Vector3.zero; // reset local position to avoid offset
        clonedTransform.LookAt(lookTarget.transform.position);
        targetRotation = clonedTransform.rotation;
        Destroy(clonedTransform.gameObject);
    }
}

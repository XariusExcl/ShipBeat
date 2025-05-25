using UnityEngine;

public class TutoBotHead : MonoBehaviour
{
    Quaternion targetRotation;
    Vector3 initialPosition;

    [SerializeField] Transform lookAtTarget;
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
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    public void LookAt(Vector3 targetPosition = default)
    {
        Debug.Log("TutoBotHead: LookAt called with targetPosition: " + targetPosition);

        if (targetPosition == default)
            targetPosition = Camera.main.transform.position;

        // targetPosition.y = transform.position.y;
        Transform clonedTransform = Instantiate(transform, transform);
        clonedTransform.LookAt(targetPosition);
        targetRotation = clonedTransform.rotation;
        Destroy(clonedTransform.gameObject);
    }
}

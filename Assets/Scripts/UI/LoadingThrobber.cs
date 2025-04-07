using UnityEngine;

public class LoadingThrobber : MonoBehaviour
{
    [SerializeField] float rotationSpeed;
    [SerializeField] float angleOffset;

    void Update()
    {
        float t = 1f - Mathf.Cos(Mathf.Repeat(Time.time * rotationSpeed, Mathf.PI));
        transform.rotation = Quaternion.Euler(0, 0, t * 180f + angleOffset);
    }
}
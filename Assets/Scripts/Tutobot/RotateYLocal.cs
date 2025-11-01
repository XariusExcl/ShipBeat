using UnityEngine;

public class TailAltRotate : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 10f;
    
    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
    }
}

using UnityEngine;

public class ShipLaserBehaviour : MonoBehaviour
{
    [SerializeField] float velocity = 500;
    void Update()
    {
        transform.position += new Vector3(0f, 0f, velocity * Time.deltaTime);
        if (transform.position.z > 500f) Destroy(gameObject);
    }
}

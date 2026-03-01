using UnityEngine;

public class ShipLaser : MonoBehaviour
{
    [SerializeField] GameObject laserPrefab;

    public void ShootLaser()
    {
        Instantiate(laserPrefab, transform);
    }
}

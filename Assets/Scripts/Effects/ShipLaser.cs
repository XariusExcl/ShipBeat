using UnityEngine;

public class ShipLaser : MonoBehaviour
{
    [SerializeField] GameObject laserPrefab;

    bool isShooting = false;
    float interval = .5f;
    void ShootLaser()
    {
        interval = 30f / Maestro.CurrentTimingPoint.BPM;
        Instantiate(laserPrefab, transform);
    }

    public void StartShooting()
    {
        isShooting = true;
        ShootLaser();
    }

    public void StopShooting()
    {
        isShooting = false;
    }

    void Update()
    {
        if (isShooting) interval -= Time.deltaTime;
        if (interval < 0f) ShootLaser();
    }
}

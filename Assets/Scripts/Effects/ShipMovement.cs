using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float decisionTime = .1f;
    [SerializeField] float distance;
    Vector3 origin;
    Vector3 target;
    Vector3 secondaryTarget;
    void Start()
    {
        origin = transform.position;
        timeToNextMove = decisionTime * Random.value;
        secondaryTarget = origin;
        target = GetRandomPoint(origin, distance);
    }

    float timeToNextMove;
    void Update()
    {
        if (timeToNextMove <= 0) {
            target = GetRandomPoint(origin, distance);
            timeToNextMove = decisionTime;
        }
        else {
            timeToNextMove -= Time.deltaTime;
        }

        // Move towards the target (2 iterations)
        secondaryTarget = Vector3.Lerp(secondaryTarget, target, speed * 2f * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, secondaryTarget, speed * Time.deltaTime);
    }

    Vector3 GetRandomPoint(Vector3 origin, float distance)
    {
        return origin + new Vector3(Random.Range(-distance, distance), Random.Range(-distance, distance), 0);
    }
}

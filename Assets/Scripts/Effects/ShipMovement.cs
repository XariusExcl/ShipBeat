using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float decisionTime = .1f;
    [SerializeField] float distance;
    [SerializeField] float jostleAmount = 0.5f;
    Vector3 origin;
    Vector3 target;
    Vector3 secondaryTarget;
    Vector3 position;
    Vector3 jostle;
    void Start()
    {
        origin = transform.localPosition;
        timeToNextMove = decisionTime * Random.value;
        secondaryTarget = origin;
        position = origin;
        target = GetRandomPoint(origin, distance);
    }

    float timeToNextMove;
    void Update()
    {
        if (decisionTime >= 0) {
            if (timeToNextMove <= 0) {
                target = GetRandomPoint(origin, distance);
                timeToNextMove = decisionTime;
            }
            else {
                timeToNextMove -= Time.deltaTime;
            }
        }

        // Move towards the target (2 iterations)
        secondaryTarget = Vector3.Lerp(secondaryTarget, target, speed * 2f * Time.deltaTime);
        position = Vector3.Lerp(position, secondaryTarget, speed * Time.deltaTime);
        // Apply jostle effect
        transform.localPosition = Vector3.Lerp(transform.localPosition, position + jostle, .5f);
        jostle = Vector3.Lerp(jostle, new Vector3(0, 0, 0), 4f * Time.deltaTime);
    }

    public void Jostle(ButtonState direction)
    {
        jostle = new Vector3(0, 0, 0);
        switch (direction) {
            case ButtonState.Right:
                jostle.x = -jostleAmount;
                break;
            case ButtonState.Left:
                jostle.x = jostleAmount;
                break;
        }
    }

    Vector3 GetRandomPoint(Vector3 origin, float distance)
    {
        return origin + new Vector3(Random.Range(-distance, distance), Random.Range(-distance, distance), 0);
    }
}

using System.Collections;
using UnityEngine;

public class ShakeEffect : MonoBehaviour
{
    [System.Flags]
    enum TransformParameters
    {
        None = 0,
        Position = 1 << 0,
        Rotation = 1 << 1,
        Scale = 1 << 2
    }

    [SerializeField] TransformParameters affectedParameters = TransformParameters.Position;
    [SerializeField] float speed = 5f;
    [SerializeField] float decisionTime = .1f;
    [SerializeField] float amplitude;
    Vector3 origin;
    Vector3 targetPosition;
    Vector3 secondaryTarget;
    Vector3 position;
    Quaternion originalRotation;
    Quaternion targetRotation;
    Quaternion currentRotation;
    Quaternion newRotation;

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void SetDecisionTime(float newDecisionTime)
    {
        decisionTime = newDecisionTime;
        timeToNextMove = decisionTime * Random.value;
    }

    public void SetDistance(float newDistance)
    {
        amplitude = newDistance;
        targetPosition = GetRandomPoint(origin, amplitude);
        secondaryTarget = origin;
        position = origin;
    }

    public void SetAmountOverTime(float incrementValue)
    {
        StopAllCoroutines();
        StartCoroutine(SetAmountOverTimeCoroutine(incrementValue));
    }

    private IEnumerator SetAmountOverTimeCoroutine(float incrementValue)
    {
        float elapsed = 0f;
        while (amplitude > 0f)
        {
            elapsed += Time.deltaTime;
            amplitude += incrementValue * Time.deltaTime;
            yield return null;
        }
        amplitude = 0f; // Ensure distance does not go below zero
        yield return null;
    }

    void Start()
    {
        origin = transform.localPosition;
        originalRotation = transform.localRotation;
        timeToNextMove = decisionTime * Random.value;
        secondaryTarget = origin;
        position = origin;
        targetPosition = GetRandomPoint(origin, amplitude);
        targetRotation = GetRandomRotation(originalRotation, amplitude);
    }

    float timeToNextMove;
    void Update()
    {
        if (decisionTime >= 0)
        {
            if (timeToNextMove <= 0)
            {
                targetPosition = GetRandomPoint(origin, amplitude);
                targetRotation = GetRandomRotation(originalRotation, amplitude);
                timeToNextMove = decisionTime;
            }
            else
            {
                timeToNextMove -= Time.deltaTime;
            }
        }

        if (affectedParameters.HasFlag(TransformParameters.Position))
        {
            // Move towards the target (2 iterations)
            secondaryTarget = Vector3.Lerp(secondaryTarget, targetPosition, speed * 2f * Time.deltaTime);
            position = Vector3.Lerp(position, secondaryTarget, speed * Time.deltaTime);
            transform.localPosition = position;
        }

        if (affectedParameters.HasFlag(TransformParameters.Rotation))
        {
            // Rotate towards the target
            currentRotation = transform.localRotation;
            newRotation = Quaternion.Lerp(currentRotation, targetRotation, speed * Time.deltaTime);
            transform.localRotation = newRotation;
        }
    }

    Vector3 GetRandomPoint(Vector3 origin, float distance)
    {
        return origin + new Vector3(Random.Range(-distance, distance), Random.Range(-distance, distance), 0);
    }
    
    Quaternion GetRandomRotation(Quaternion originalRotation, float maxAngle)
    {
        return originalRotation * Quaternion.Euler(Random.Range(-maxAngle, maxAngle), Random.Range(-maxAngle, maxAngle), Random.Range(-maxAngle, maxAngle));
    }

}

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
    [SerializeField] public float Speed = 5f;
    [SerializeField] public float DecisionTime = .1f;
    [SerializeField] public float Amplitude;
    [SerializeField] bool ChangesOnKiai;
    float normalSpeed;
    [SerializeField] float kiaiSpeed;
    float normalDecisionTime;
    [SerializeField] float kiaiDecisionTime;
    float normalAmplitude;
    [SerializeField] float kiaiAmplitude;

    Vector3 origin;
    Vector3 targetPosition;
    Vector3 secondaryTarget;
    Vector3 position;
    Quaternion originalRotation;
    Quaternion targetRotation;
    Quaternion currentRotation;
    Quaternion newRotation;

    // Used in Animation
    public void SetSpeed(float newSpeed)
    {
        Speed = newSpeed;
    }

    public void SetDecisionTime(float newDecisionTime)
    {
        DecisionTime = newDecisionTime;
        timeToNextMove = DecisionTime * Random.value;
    }

    public void SetDistance(float newDistance)
    {
        Amplitude = newDistance;
        targetPosition = GetRandomPoint(origin, Amplitude);
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
        while (Amplitude > 0f)
        {
            elapsed += Time.deltaTime;
            Amplitude += incrementValue * Time.deltaTime;
            yield return null;
        }
        Amplitude = 0f; // Ensure distance does not go below zero
        yield return null;
    }

    void Start()
    {
        origin = transform.localPosition;
        originalRotation = transform.localRotation;
        timeToNextMove = DecisionTime * Random.value;
        secondaryTarget = origin;
        position = origin;
        targetPosition = GetRandomPoint(origin, Amplitude);
        targetRotation = GetRandomRotation(originalRotation, Amplitude);
        if (ChangesOnKiai)
        {
            normalSpeed = Speed;
            normalDecisionTime = DecisionTime;
            normalAmplitude = Amplitude;
            Maestro.OnKiaiStart.AddListener(SetKiaiShake);
            Maestro.OnKiaiEnd.AddListener(SetNormalShake);
        }
    }

    void SetNormalShake()
    {
        SetSpeed(normalSpeed);
        SetDecisionTime(normalDecisionTime);
        SetDistance(normalAmplitude);
    }

    void SetKiaiShake()
    {
        SetSpeed(kiaiSpeed);
        SetDecisionTime(kiaiDecisionTime);
        SetDistance(kiaiAmplitude);
    }

    void OnDestroy()
    {
        Maestro.OnKiaiStart.RemoveListener(SetKiaiShake);
        Maestro.OnKiaiEnd.RemoveListener(SetNormalShake);
    }

    float timeToNextMove;
    void Update()
    {
        if (DecisionTime >= 0)
        {
            if (timeToNextMove <= 0)
            {
                targetPosition = GetRandomPoint(origin, Amplitude);
                targetRotation = GetRandomRotation(originalRotation, Amplitude);
                timeToNextMove = DecisionTime;
            }
            else
            {
                timeToNextMove -= Time.deltaTime;
            }
        }

        if (affectedParameters.HasFlag(TransformParameters.Position))
        {
            // Move towards the target (2 iterations)
            secondaryTarget = Vector3.Lerp(secondaryTarget, targetPosition, Speed * 2f * Time.deltaTime);
            position = Vector3.Lerp(position, secondaryTarget, Speed * Time.deltaTime);
            transform.localPosition = position;
        }

        if (affectedParameters.HasFlag(TransformParameters.Rotation))
        {
            // Rotate towards the target
            currentRotation = transform.localRotation;
            newRotation = Quaternion.Lerp(currentRotation, targetRotation, Speed * Time.deltaTime);
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
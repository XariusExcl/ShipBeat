using UnityEngine;
using UnityEngine.Splines;

public class Lane : MonoBehaviour
{
    [SerializeField] Receptor receptor;
    [SerializeField] int laneResolution = 20;
    [HideInInspector] public SplineContainer SplineContainer;
    [HideInInspector] public Vector3 ExtrapolationVector;

    [HideInInspector] LineRenderer lineRenderer;

    void Awake()
    {
        SplineContainer = GetComponent<SplineContainer>();
        lineRenderer = GetComponent<LineRenderer>();
        receptor = GetComponentInChildren<Receptor>();
        Debug.Assert(receptor != null, "Receptor not found in Lane object.", gameObject);
        UpdateLane();
    }

    public void UpdateLane()
    {
        ExtrapolationVector = (-(Vector3)SplineContainer.EvaluateTangent(0.01f)).normalized * SplineContainer.CalculateLength();

        if (lineRenderer.enabled)
        {
            lineRenderer.positionCount = laneResolution;
            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                lineRenderer.SetPosition(i, (Vector3)SplineContainer.EvaluatePosition(i / (float)(lineRenderer.positionCount - 1) ));
            }
        }

        // Debug.DrawRay(transform.position, ExtrapolationVector, Color.red, 10f);
    }

    public void SuccessfulHit(JudgeType judge, bool mirror = false)
    {
        if (receptor != null)
            receptor.SuccessfulHit(judge, mirror);
    }

    public void Hold(JudgeType judge)
    {
        if (receptor != null)
            receptor.SuccessfulHold();
    }

    public void SuccessfulRelease()
    {
        if (receptor != null)
            receptor.SuccessfulRelease();
    }
    
    public void Release() {
        if (receptor != null)
            receptor.Release();
    }
}

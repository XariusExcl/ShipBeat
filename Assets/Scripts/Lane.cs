using UnityEngine;
using UnityEngine.Splines;

public class Lane : MonoBehaviour {
    [HideInInspector] public SplineContainer SplineContainer;
    [HideInInspector] public Vector3 ExtrapolationVector;

    [HideInInspector] LineRenderer lineRenderer;

    void Awake()
    {
        SplineContainer = GetComponent<SplineContainer>();
        lineRenderer = GetComponent<LineRenderer>();
        UpdateLane();
    }

    public void UpdateLane()
    {
        ExtrapolationVector = (-(Vector3)SplineContainer.EvaluateTangent(0.01f)).normalized * SplineContainer.CalculateLength();

        if (lineRenderer.enabled) {
            lineRenderer.positionCount = 20;
            for (int i = 0; i < lineRenderer.positionCount; i++) {
                lineRenderer.SetPosition(i, (Vector3)SplineContainer.EvaluatePosition(i / (float)lineRenderer.positionCount)); // TODO, can be paralellized with a job?
            }
        }

        // Debug.DrawRay(transform.position, ExtrapolationVector, Color.red, 10f);
    }
}

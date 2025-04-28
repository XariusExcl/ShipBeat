using UnityEngine;
using UnityEngine.Splines;

public class Lane : MonoBehaviour {
    [SerializeField] Receptor receptor;
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

        if (lineRenderer.enabled) {
            lineRenderer.positionCount = 20;
            for (int i = 0; i < lineRenderer.positionCount; i++) {
                lineRenderer.SetPosition(i, (Vector3)SplineContainer.EvaluatePosition(i / (float)lineRenderer.positionCount));
            }
        }

        // Debug.DrawRay(transform.position, ExtrapolationVector, Color.red, 10f);
    }

    public void SuccessfulHit() {
        if (receptor != null) {
            receptor.SuccessfulHit();
        }
    }

    public void SuccessfulHold() {
        if (receptor != null) {
            receptor.SuccessfulHold();
        }
    }

    public void SuccessfulRelease() {
        if (receptor != null) {
            receptor.SuccessfulRelease();
        }
    }
}

using UnityEngine;
using UnityEngine.Splines;

public class BeatMarkerBehaviour : MonoBehaviour
{
    public SplineAnimate splineAnimate;
    [SerializeField] GameObject mesh;
    public float ThisBeatTime;
    public bool IsBar;
    public bool InUse;

    void OnEnable()
    {
        if (IsBar)
            mesh.transform.localScale = new Vector3(2.45f, 0.05f, 0.05f);
        else
            mesh.transform.localScale = new Vector3(2.45f, 0.025f, 0.025f);

        InUse = true;
    }

    void Update()
    {
        float normalizedTime = (ThisBeatTime - Maestro.SongTime) / (10f / Maestro.LaneSpeed);
        if (normalizedTime > 1f)
        {
            mesh.SetActive(false);
            return;
        }

        if (normalizedTime < 0f)
        {
            gameObject.SetActive(false);
            InUse = false;
            return;
        }

        if (!mesh.activeSelf) mesh.SetActive(true);
        splineAnimate.NormalizedTime = normalizedTime;
    }

    void OnDisable()
    {
        InUse = false;
    }
}
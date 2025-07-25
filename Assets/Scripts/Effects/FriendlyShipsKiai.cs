using System.Collections.Generic;
using UnityEngine;

public class FriendlyShipsKiai : MonoBehaviour
{
    [SerializeField] Vector3 kiaiPosition;
    Vector3 normalPosition;
    [SerializeField] Vector3 kiaiScale;
    Vector3 normalScale;
    [SerializeField] List<TrailRenderer> shipTrails;

    Vector3 targetPosition;
    Vector3 targetScale;

    void Start()
    {
        normalPosition = transform.localPosition;
        normalScale = transform.localScale;
        Maestro.OnKiaiStart.AddListener(SetKiai);
        Maestro.OnKiaiEnd.AddListener(SetNormal);

        SetNormal();
    }

    void SetNormal()
    {
        targetPosition = normalPosition;
        targetScale = normalScale;
        shipTrails.ForEach(t => t.emitting = false);
    }

    void SetKiai()
    {
        targetPosition = kiaiPosition;
        targetScale = kiaiScale;
        shipTrails.ForEach(t => t.emitting = true);
    }

    void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, 2f * Time.deltaTime);
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, 2f * Time.deltaTime);
    }


    void OnDestroy()
    {
        Maestro.OnKiaiStart.RemoveListener(SetNormal);
        Maestro.OnKiaiEnd.RemoveListener(SetKiai);
    }
}
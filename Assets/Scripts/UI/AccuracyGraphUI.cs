using UnityEngine;
public class AccuracyGraphUI : MonoBehaviour
{
    [SerializeField] GameObject accuracyGraphPointPrefab;
    
    RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void AddPoint(float diff)
    {
        float normalized = Mathf.Clamp(diff / Judge.BadHitWindow, -1f, 1f);
        float halfWidth = rectTransform != null ? rectTransform.rect.width * 0.5f : 0f;
        Vector2 anchoredPosition = new Vector2(-normalized * halfWidth, 0f);

        GameObject point = Instantiate(accuracyGraphPointPrefab, transform);
        RectTransform pointRect = point.GetComponent<RectTransform>();
        if (pointRect != null)
            pointRect.anchoredPosition = anchoredPosition;
        else
            point.transform.localPosition = new Vector3(anchoredPosition.x, anchoredPosition.y, 0f);

        Destroy(point, 2f);
    }   
}
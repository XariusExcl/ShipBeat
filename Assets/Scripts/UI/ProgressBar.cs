using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private float fillSpeed = 1f;
    [Range(0f, 1f)] [SerializeField] float fillAmount = 0f;
    private float currentFillAmount = 0f;

    void Update()
    {
        currentFillAmount = Mathf.Lerp(currentFillAmount, fillAmount, fillSpeed * Time.deltaTime);
        float width = backgroundImage.rectTransform.rect.width * currentFillAmount;
        fillImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Max(width, backgroundImage.rectTransform.rect.height - 2f));
    }

    public void SetProgress(float progress)
    {
        fillAmount = Mathf.Clamp01(progress);
    }
}
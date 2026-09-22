using UnityEngine;
using TMPro;

public class EarlyLateUI : MonoBehaviour
{
    static int _hitAccuracyOption = 0;
    public static int HitAccuracyOption
    {
        get => _hitAccuracyOption;
        set
        {
            if (value < 0) value = 2;
            if (value > 2) value = 0;
            _hitAccuracyOption = value;
        }
    }
    
    TMP_Text text;
    Animation animation;
    [SerializeField] string earlyText;
    [SerializeField] Color earlyColor;
    [SerializeField] string lateText;
    [SerializeField] Color lateColor;

    void Start()
    {
        animation = GetComponent<Animation>();
        text = GetComponent<TMP_Text>();
        Hide();
    }

    public void ShowEarly()
    {
        gameObject.SetActive(true);
        text.text = earlyText;
        text.color = earlyColor;
        animation.Play();
    }

    public void ShowLate()
    {
        gameObject.SetActive(true);
        text.text = lateText;
        text.color = lateColor;
        animation.Play();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
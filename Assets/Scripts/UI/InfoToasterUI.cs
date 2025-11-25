using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InfoToasterUI : MonoBehaviour
{
    public static InfoToasterUI Instance;
    [SerializeField] TMP_Text tmp;
    [SerializeField] Image bg;
    static float timer;
    const float timerDuration = 3f;
    float targetOpacity;
    float opacity;

    void Awake()
    {
        Instance = this;
    }

    public static void ShowToaster(string text) {
        Instance.tmp.text = text;
        timer = timerDuration;
    }

    void Update()
    {
        if (timer > 0) {
            timer -= Time.deltaTime; 
            targetOpacity = 1f;
        }
        else
            targetOpacity = 0f;

        float diff = targetOpacity - opacity;
        if (diff != 0) {
            opacity += Mathf.Sign(diff) * Mathf.Max(diff, Time.deltaTime * 2f);
            tmp.alpha = opacity;
            bg.color = new Color(bg.color.r, bg.color.g, bg.color.b, opacity * .6f);
        }
    }
}

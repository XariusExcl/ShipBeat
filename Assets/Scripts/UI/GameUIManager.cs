using UnityEngine;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    static GameUIManager Instance;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI percentageText;
    [SerializeField] TextMeshProUGUI comboText;
    [SerializeField] Ticker ticker;


    void Start()
    {
        Instance = this;
    }

    void Update()
    {
       // ici ça update 
    }

    public static void ShowTicker(TickerType type)
    {
        Instance.ticker.ShowTicker(type);
    }

    public static void UpdateScore(int score)
    {
        Instance.scoreText.text = score.ToString();
    }

    public static void UpdateCombo(int combo)
    {
        Instance.comboText.text = combo.ToString();
    }

    public static void ResetCombo()
    {
        Instance.comboText.text = "0";
    }

    public static void UpdatePercentage(float percentage)
    {
        Instance.percentageText.text = percentage.ToString("F2") + "%";
    }
}

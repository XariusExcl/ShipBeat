using UnityEngine;
using TMPro;
using System;

public class GameUIManager : MonoBehaviour
{
    static GameUIManager Instance;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI percentageText;
    [SerializeField] TextMeshProUGUI comboText;
    [SerializeField] Ticker ticker;
    [SerializeField] ResultScreenUI resultsScreen;
    [SerializeField] TransitionDoors transitionDoors;


    void Awake()
    {
        uiScore = 0;
        uiPercentage = 100f;
        Instance = this;
    }

    void Update()
    {
        int scoreDiff = Scoring.Score - uiScore;
        if (scoreDiff > 0)
        {
            uiScore += (int)Mathf.Ceil(scoreDiff / 10f);
            scoreText.text = uiScore.ToString("D7");
        }

        float percentageDiff = Scoring.Percentage - uiPercentage;
        if (Math.Abs(percentageDiff) > 0.005f)
        {
            uiPercentage += percentageDiff / 5f;
            percentageText.text = uiPercentage.ToString("F2") + "%";
        }
    }

    public static void ShowTicker(TickerType type)
    {
        Instance.ticker.ShowTicker(type);
    }
    static int uiScore = 0;
    static float uiPercentage = 0;
    public static void UpdateScore(int score)
    {
        // Maybe something here?
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
        // Maybe something here?
    }

    public static void ShowResults()
    {
        Instance.resultsScreen.gameObject.SetActive(true);
        Instance.resultsScreen.ShowResults(Scoring.Perfects, Scoring.Goods, Scoring.Bads, Scoring.Misses, Scoring.Percentage, Scoring.GetRank());
    }

    public static void ReturnToSongSelect()
    {
        Instance.transitionDoors.CloseDoor();
        Instance.Invoke("LoadSongSelect", 1f);
    }

    void LoadSongSelect()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SongSelect");
    }
}

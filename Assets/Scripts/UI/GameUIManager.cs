using UnityEngine;
using TMPro;
using System;

public class GameUIManager : MonoBehaviour
{
    static GameUIManager Instance;
    public static bool IsHighscore;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI percentageText;
    [SerializeField] TextMeshProUGUI comboText;
    [SerializeField] Color PfcColor;
    [SerializeField] Color FcColor;
    [SerializeField] Color NormalColor;
    [SerializeField] Ticker ticker;
    [SerializeField] ResultScreenUI resultsScreen;
    [SerializeField] TransitionDoors transitionDoors;
    [SerializeField] ProgressBar progressBar;
    [SerializeField] TextMeshProUGUI songTime;
    [SerializeField] GameObject skipButton;
    [SerializeField] PlayerInfoScoreUI playerInfoScoreUI;

    void Awake()
    {
        uiScore = 0;
        uiPercentage = 100f;
        Instance = this;
        comboText.text = "";
    }

    static int uiScore = 0;
    static float uiPercentage = 0;
    void Update()
    {
        if (!Maestro.IsTutorial && Maestro.SongTime > 2 && Maestro.SongTime < SongLoader.LoadedSong.Info.SongStart - 2)
            skipButton.SetActive(true);
        else
            skipButton.SetActive(false);

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

        if (Maestro.SongTime > 30f && Scoring.Bads == 0 && Scoring.Misses == 0)
        {
            Instance.comboText.alpha =  Instance.NormalColor.a + Mathf.Sin(Maestro.SongTime * 1.5f) * .1f;
        }

        if (!Maestro.SongEnded)
        {
            progressBar.SetProgress(Maestro.SongTime / (SongLoader.LoadedSong.Info.Length + SongLoader.LoadedSong.Info.SongStart));       
            songTime.text = ((Maestro.SongTime < 0) ? "-" : "") + TimeSpan.FromSeconds(Maestro.SongTime).ToString(@"m\:ss");
        }
    }

    public static void ShowTicker(JudgeType type)
    {
        Instance.ticker.ShowTicker(type);
    }

    public static void UpdateCombo(int combo)
    {
        if (Maestro.SongTime > 30f)
            if(Scoring.Bads == 0 && Scoring.Misses == 0)
            {
                if (Scoring.Goods == 0)
                    Instance.comboText.color = Instance.PfcColor;
                else
                    Instance.comboText.color = Instance.FcColor;
                Instance.comboText.alpha =  Instance.NormalColor.a + Mathf.Sin(Maestro.SongTime * 1.5f) * .1f;
            }
            else
                Instance.comboText.color = Instance.NormalColor;

        Instance.comboText.text = combo.ToString();
    }

    public static void ResetCombo()
    {
        Instance.comboText.text = "";
    }

    public static void ShowResults()
    {
        Instance.resultsScreen.gameObject.SetActive(true);
        Instance.resultsScreen.ShowResults();
    }

    public static void ReturnToSongSelect()
    {
        Instance.transitionDoors.CloseDoor();
        Instance.Invoke("LoadSongSelect", 1f);
    }

    public static void UpdateTotalScore()
    {
        Debug.Log("Test");
        Instance.playerInfoScoreUI.UpdateTotalScore();
    }

    void LoadSongSelect()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SongSelect");
    }
}

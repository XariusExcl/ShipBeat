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
    [SerializeField] Ticker ticker;
    [SerializeField] ResultScreenUI resultsScreen;
    [SerializeField] TransitionDoors transitionDoors;
    [SerializeField] ProgressBar progressBar;
    [SerializeField] TextMeshProUGUI songTime;
    [SerializeField] GameObject skipButton;


    void Awake()
    {
        uiScore = 0;
        uiPercentage = 100f;
        Instance = this;
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

        progressBar.SetProgress(Maestro.SongTime / (SongLoader.LoadedSong.Info.Length + SongLoader.LoadedSong.Info.SongStart));

        songTime.text = ((Maestro.SongTime < 0) ? "-" : "") + TimeSpan.FromSeconds(Maestro.SongTime).ToString(@"m\:ss");
    }

    public static void ShowTicker(JudgeType type)
    {
        Instance.ticker.ShowTicker(type);
    }

    public static void UpdateCombo(int combo)
    {
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

    void LoadSongSelect()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SongSelect");
    }
}

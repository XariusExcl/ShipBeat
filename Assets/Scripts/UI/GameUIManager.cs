using UnityEngine;
using TMPro;

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
        Instance = this;
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

    public static void DontShowTransitionDoors()
    {
        Debug.Log("Transition doors will not be shown.");
        Instance.transitionDoors.OpenInstantly();
    }
}

using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ResultScreenUI : MonoBehaviour
{
    bool buttonsEnabled = false;
    [SerializeField] TMP_Text tmp_PerfectCount;
    [SerializeField] TMP_Text tmp_GoodCount;
    [SerializeField] TMP_Text tmp_BadCount;
    [SerializeField] TMP_Text tmp_MissCount;
    [SerializeField] TMP_Text tmp_Accuracy;
    [SerializeField] TMP_Text tmp_Rank;
    [SerializeField] GameObject returnButton;


    public void ShowResults(int perfectCount, int goodCount, int badCount, int missCount, float accuracy, string rank) {
        tmp_PerfectCount.text = perfectCount.ToString();
        tmp_GoodCount.text = goodCount.ToString();
        tmp_BadCount.text = badCount.ToString();
        tmp_MissCount.text = missCount.ToString();
        tmp_Accuracy.text = accuracy.ToString("F2") + "%";
        tmp_Rank.text = rank;
    }

    void OnEnable() {
        Invoke("EnableButtons", 1.5f);
    }

    void OnDisable() {
        buttonsEnabled = false;
    }

    void EnableButtons() {
        buttonsEnabled = true;
    }

    void Update(){
        if (buttonsEnabled)
            CheckInput();
    }

    void CheckInput() {
        if (Input.GetButtonDown("P1_B1")) {
            SceneManager.LoadScene("SongSelect");
        }
    }
}
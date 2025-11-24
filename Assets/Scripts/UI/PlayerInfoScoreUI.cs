using UnityEngine;
using TMPro;
using System;
using UnityEditor;

public class PlayerInfoScoreUI : MonoBehaviour
{
    [SerializeField] TMP_Text agentScore;
    [SerializeField] TMP_Text scoreUpdate;
    int totalScore;

    void OnEnable()
    {
        totalScore = Scoring.TotalScore;
        UpdateScore(totalScore);
    }

    public void UpdateTotalScore()
    {
        Debug.Log("Ah oiuais");
        int diff = Scoring.TotalScore - totalScore;
        totalScore = Scoring.TotalScore;
        if (Mathf.Abs(diff) < 1000f) {
            scoreUpdate.gameObject.SetActive(false);
            return;
        }
        Debug.Log("iuais");
        scoreUpdate.text = $"{((Math.Sign(diff) == 1) ? "" : "-")}{(int)(Math.Abs(diff) / 1000000f)}<size=60%>.{(int)(Math.Abs(diff) % 1000000f / 1000):D3}";
        scoreUpdate.color = (Math.Sign(diff) == 1) ? Color.green : Color.red;
        scoreUpdate.gameObject.SetActive(true);
        UpdateScore(totalScore);
    }

    void UpdateScore(int score)
    {
        agentScore.text = $"{(int)(score / 1000000f)}<size=60%>.{(int)(score % 1000000f / 1000):D3} <size=100%>M<size=60%>";
    }

    public void TestUpdateTotalScore(int diff)
    {
        totalScore -= diff;
        UpdateTotalScore();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PlayerInfoScoreUI))]
public class PlayerInfoScoreUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        PlayerInfoScoreUI script = (PlayerInfoScoreUI)target;
        if (GUILayout.Button("TestScoreUpdate"))
        {
            script.TestUpdateTotalScore(1000000);
        }
    }
}
#endif
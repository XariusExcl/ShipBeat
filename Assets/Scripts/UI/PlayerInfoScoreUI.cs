using UnityEngine;
using TMPro;

public class PlayerInfoScoreUI : MonoBehaviour
{
    [SerializeField] TMP_Text agentScore;

    void OnEnable()
    {
        agentScore.text = $"{(int)(Scoring.TotalScore / 1000000f)}<size=60%>.{(int)(Scoring.TotalScore % 1000000f / 1000):D3} <size=100%>M<size=60%>";
    }
}

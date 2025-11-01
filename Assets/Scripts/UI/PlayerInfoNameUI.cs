using UnityEngine;
using TMPro;
using Anatidae;

public class PlayerInfoNameUI : MonoBehaviour
{
    [SerializeField] TMP_Text agentName;

    void OnEnable()
    {
        agentName.text = HighscoreManager.PlayerName;
    }
}

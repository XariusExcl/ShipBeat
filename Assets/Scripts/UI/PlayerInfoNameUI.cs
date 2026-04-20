using UnityEngine;
using TMPro;
using Anatidae;

public class PlayerInfoNameUI : MonoBehaviour
{
    [SerializeField] TMP_Text agentName;

    void OnEnable()
    {
        if (OnlineDataManager.Online)
            agentName.text = OnlineDataManager.Data.PlayerInfo.Name;
        else
            agentName.text = HighscoreManager.PlayerName;
    }
}

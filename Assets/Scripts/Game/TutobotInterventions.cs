using Anatidae;
using UnityEngine;

public class TutobotInterventions : MonoBehaviour
{
    [SerializeField] GameObject tutoBotHologram;
    static TutobotInterventions instance;

    void Start()
    {
        instance = this;
        DialogueTriggers.TurnOffHolo.AddListener(TurnOffHolo);
    }

    public static void CheckforIntervention()
    {
        // Unlock hard songs    
        if (ExtradataManager.GetDataWithKey($"Player/{HighscoreManager.PlayerName}/UnlockHard") is null && SongUnlockConditions.HardUnlockConditionsMet())
        {
            // Showup and start dialogue
            instance.tutoBotHologram.SetActive(true);
            instance.Invoke("StartUnlockHardDialogue", .75f);
            instance.StartCoroutine(ExtradataManager.SetExtraData($"Player/{HighscoreManager.PlayerName}/UnlockHard", "1"));

        }
        // Unlock expert songs
        else if (ExtradataManager.GetDataWithKey($"Player/{HighscoreManager.PlayerName}/UnlockExpert") is null && SongUnlockConditions.ExpertUnlockConditionsMet())
        {
            // Showup and start dialogue
            instance.tutoBotHologram.SetActive(true);
            instance.Invoke("StartUnlockExpertDialogue", .75f);
            instance.StartCoroutine(ExtradataManager.SetExtraData($"Player/{HighscoreManager.PlayerName}/UnlockExpert", "1"));
        }
        else
        {
            GameUIManager.EnableResultScreenButtons();
        }
    }

    void StartUnlockHardDialogue()
    {
        TextboxSystem.StartDialogue("unlock_hard");
    }

    void StartUnlockExpertDialogue()
    {
        TextboxSystem.StartDialogue("unlock_expert");
    }

    void TurnOffHolo()
    {
        tutoBotHologram.GetComponent<Animation>().Play("TutobotFadeOut");
    }

    void OnDestroy()
    {
        instance = null;
        DialogueTriggers.TurnOffHolo.RemoveListener(TurnOffHolo);    
    }
}
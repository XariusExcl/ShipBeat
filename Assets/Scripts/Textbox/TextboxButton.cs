using UnityEngine;
using TMPro;

public class TextboxButton : MonoBehaviour
{
    [HideInInspector] public string nextDialogueID;
    [HideInInspector] public TMP_Text Text;
    void Awake()
    {
        Text = GetComponentInChildren<TMP_Text>();
    }

    public void SetData(DialogueChoice choice)
    {
        Text.text = choice.Text;
        nextDialogueID = choice.NextId;
    }
    
    public void TriggerNextDialogue()
    {
        TextboxSystem.StartDialogue(nextDialogueID);
    }
}

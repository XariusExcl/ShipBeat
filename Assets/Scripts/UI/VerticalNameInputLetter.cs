using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class VerticalNameInputLetter : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] VerticalNameInput verticalNameInput;
    [SerializeField] TMP_Text letterText;
    public bool IsSelected { get; private set; }
    public char Letter { get => letterText.text[0]; set => letterText.text = value.ToString(); }
    public int Index { get; set; }

    public void OnSelect(BaseEventData eventData)
    {
        IsSelected = true;
        verticalNameInput.LetterSelected(Index);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        IsSelected = false;
        verticalNameInput.LetterDeselected(Index);
    }
}
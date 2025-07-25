using UnityEngine;
using UnityEngine.EventSystems;

public class PlaySoundOnSelect : MonoBehaviour, ISelectHandler
{
    public void OnSelect(BaseEventData eventData)
    {
        SFXManager.PlayVerticalBlipSound();
    }
}
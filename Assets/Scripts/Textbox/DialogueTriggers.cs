using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueTriggers : MonoBehaviour
{
    public static UnityEvent StartGame = new UnityEvent();
    public static UnityEvent StartTutorial = new UnityEvent();
    public static UnityEvent EndSong = new UnityEvent();
    public static UnityEvent RetrySelectingName = new UnityEvent();
    public static UnityEvent TurnOffHolo = new UnityEvent();
    public static UnityEvent EnableResultScreenButtons = new UnityEvent();
    public static Dictionary<string, UnityEvent> events = new Dictionary<string, UnityEvent>
    {
        { "StartGame", StartGame },
        { "StartTutorial", StartTutorial },
        { "EndSong", EndSong },
        { "RetrySelectingName", RetrySelectingName },
        { "TurnOffHolo", TurnOffHolo },
        { "EnableResultScreenButtons", EnableResultScreenButtons }
    };

    public static void TriggerEvent(string eventName)
    {
        Debug.Log($"TriggerEvent: '{eventName}'");
        if (events.TryGetValue(eventName, out UnityEvent unityEvent))
            unityEvent.Invoke();
        else
            Debug.LogWarning($"TriggerEvent: No event found with name '{eventName}'");
    }
}

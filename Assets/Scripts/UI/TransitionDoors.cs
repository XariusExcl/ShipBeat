using UnityEngine;

[RequireComponent(typeof(Animation))]
public class TransitionDoors : MonoBehaviour
{
    enum DoorState
    {
        Closed,
        Open
    }
    static DoorState initialState = DoorState.Open;

    [SerializeField] GameObject doorL;
    [SerializeField] GameObject doorR;

    private Animation doorAnimation;

    void Awake()
    {
        doorAnimation = GetComponent<Animation>();
        OnSceneLoaded();
    }

    void Start()
    {
        if (initialState == DoorState.Closed)
        {
            doorAnimation.Play("TransitionDoorOpen");
            initialState = DoorState.Open;
        }
    }

    void OnSceneLoaded()
    {
        switch (initialState)
        {
            case DoorState.Closed:
                doorAnimation.Play("DoorClosed");
                break;
            case DoorState.Open:
                doorAnimation.Play("DoorOpened");
                break;
        }
    }

    public void OpenDoor()
    {
        if (doorAnimation.isPlaying)
            return;

        doorAnimation.Play("TransitionDoorOpen");
        initialState = DoorState.Open;
    }

    public void CloseDoor()
    {
        if (doorAnimation.isPlaying)
            return;

        doorAnimation.Play("TransitionDoorClose");
        initialState = DoorState.Closed;
    }

    public void OpenInstantly()
    {
        doorAnimation.Stop("TransitionDoorOpen");
        doorAnimation.Stop("TransitionDoorClose");
        doorAnimation.Play("DoorOpened");
        initialState = DoorState.Open;
    }
}

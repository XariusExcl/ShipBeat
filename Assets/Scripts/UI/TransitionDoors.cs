using UnityEngine;

[RequireComponent(typeof(Animation))]
public class TransitionDoors : MonoBehaviour
{
    enum DoorState
    {
        Closed,
        Open
    }

    [SerializeField] GameObject doorL;
    [SerializeField] GameObject doorR;

    private Animation doorAnimation;
    [SerializeField] private DoorState initialState = DoorState.Closed;
    [SerializeField] public bool PlayOnAwake = true;

    void Awake()
    {
        doorAnimation = GetComponent<Animation>();

        if (PlayOnAwake)
            if (initialState == DoorState.Closed)
                doorAnimation.Play("TransitionDoorOpen");
            else
                doorAnimation.Play("TransitionDoorOpen");
    }

    public void OpenDoor()
    {
        if (doorAnimation.isPlaying)
            return;

        doorAnimation.Play("TransitionDoorOpen");
    }

    public void CloseDoor()
    {
        if (doorAnimation.isPlaying)
            return;

        doorAnimation.Play("TransitionDoorClose");
    }

    public void OpenInstantly()
    {
        doorAnimation.Stop("TransitionDoorOpen");
        doorAnimation.Stop("TransitionDoorClose");
        doorL.SetActive(false);
        doorR.SetActive(false);
    }
}

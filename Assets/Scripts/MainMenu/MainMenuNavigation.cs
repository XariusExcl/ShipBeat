using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuNavigation : MonoBehaviour
{
    [SerializeField] private CameraDolly cameraDolly;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject mainMenuQuickPlayButton;
    [SerializeField] private GameObject quickPlayMenu;
    [SerializeField] private GameObject quickPlayMenuFirstField;
    [SerializeField] private AudioClip introMusic;
    [SerializeField] private AudioClip introMusicLoop;

    void Start()
    {
        Jukebox.QueueSong(introMusic);
        Jukebox.QueueSong(introMusicLoop, true);
    }

    public void ShowMainMenu()
    {
        cameraDolly.MoveToWaypoint(1);
        quickPlayMenu.SetActive(false);
        mainMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(mainMenuQuickPlayButton);
    }

    public void QuickPlayPressed()
    {
        cameraDolly.MoveToNextWaypoint();
        quickPlayMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(quickPlayMenuFirstField);
    }

    public void LoginPressed()
    {
        // tbd
    }

    public void QuickPlayBackPressed()
    {
        cameraDolly.MoveToPreviousWaypoint();
        quickPlayMenu.SetActive(false);
        mainMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(mainMenuQuickPlayButton);
    }

    public void GuestAuth()
    {
        eventSystem.SetSelectedGameObject(null);
        TextboxSystem.StartDialogue("debug_test");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
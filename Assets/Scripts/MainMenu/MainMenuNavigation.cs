using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuNavigation : MonoBehaviour
{
    [SerializeField] private CameraDolly cameraDolly;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject quickPlayButton;
    [SerializeField] private GameObject loginMenu;
    [SerializeField] private GameObject loginFirstField;
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
        loginMenu.SetActive(false);
        mainMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(quickPlayButton);
    }

    public void QuickPlayPressed()
    {
        cameraDolly.MoveToNextWaypoint();
        loginMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(loginFirstField);
    }

    public void LoginPressed()
    {
        // tbd
    }

    public void QuickPlayBackPressed()
    {
        cameraDolly.MoveToPreviousWaypoint();
        loginMenu.SetActive(false);
        mainMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(quickPlayButton);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
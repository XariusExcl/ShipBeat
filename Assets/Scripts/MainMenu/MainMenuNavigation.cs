using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuNavigation : MonoBehaviour
{
    [SerializeField] private CameraDolly cameraDolly;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject mainMenuQuickPlayButton;
    [SerializeField] private GameObject quickPlayMenu;
    [SerializeField] private GameObject quickPlayMenuFirstField;
    [SerializeField] private TransitionDoors transitionDoors;
    [SerializeField] private AudioClip introMusic;
    [SerializeField] private AudioClip introMusicLoop;

    void Start()
    {
        Jukebox.QueueSong(introMusic);
        Jukebox.QueueSong(introMusicLoop, true);
        DialogueTriggers.StartTutorial.AddListener(StartTutorial);
        DialogueTriggers.StartGame.AddListener(StartGame);
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
        TextboxSystem.StartDialogue("guest_tutoask");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void StartTutorial()
    {
        transitionDoors.CloseDoor();
        Invoke("LoadTutorial", 1f);
    }

    public void StartGame()
    {
        transitionDoors.CloseDoor();
        Invoke("LoadGame", 1f);
    }

    void LoadTutorial()
    {
        Jukebox.ClearQueue();
        SceneManager.LoadScene("Game");
    }

    void LoadGame()
    {
        Jukebox.ClearQueue();
        SceneManager.LoadScene("SongSelect");
    }
}
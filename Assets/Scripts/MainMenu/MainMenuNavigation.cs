using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Networking;
using Anatidae;
using UnityEngine.Events;

public class MainMenuNavigation : MonoBehaviour
{
    [SerializeField] CameraDolly cameraDolly;
    [SerializeField] EventSystem eventSystem;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject mainMenuQuickPlayButton;
    [SerializeField] GameObject quickPlayMenu;
    [SerializeField] GameObject quickPlayMenuFirstField;
    [SerializeField] GameObject firstLetter;
    [SerializeField] TransitionDoors transitionDoors;
    [SerializeField] VerticalNameInput nameInput;

    [SerializeField] string TutorialSongPath;
    [SerializeField] string TutorialAudioPath;

    void Start()
    {
        StartCoroutine(ExtradataManager.FetchExtraData());
        DialogueTriggers.StartTutorial.AddListener(StartTutorial);
        DialogueTriggers.StartGame.AddListener(StartGame);
        DialogueTriggers.RetrySelectingName.AddListener(SelectFirstLetter);
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
        SFXManager.PlayHorizontalBlipSound();
        cameraDolly.MoveToNextWaypoint();
        quickPlayMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(quickPlayMenuFirstField);
    }

    public void SelectFirstLetter()
    {
        eventSystem.SetSelectedGameObject(firstLetter);
    }

    public void QuickPlayBackPressed()
    {
        SFXManager.PlayHorizontalBlipSound();
        cameraDolly.MoveToPreviousWaypoint();
        quickPlayMenu.SetActive(false);
        mainMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(mainMenuQuickPlayButton);
    }

    public void GuestAuth()
    {
        SFXManager.PlayHorizontalBlipSound();
        eventSystem.SetSelectedGameObject(null);
        nameInput.UpdateName();
        // Check censor
        StartCoroutine(CheckIfNameValidCoroutine((valid) =>
        {
            if (valid)
                if (ExtradataManager.GetDataWithKey($"Player/{HighscoreManager.PlayerName}/TotalScore") is not null)
                    TextboxSystem.StartDialogue("guest_welcomeback");
                else
                    TextboxSystem.StartDialogue("guest_tutoask");
            else
                TextboxSystem.StartDialogue("guest_badname");
        }));
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void StartTutorial()
    {
        transitionDoors.CloseDoor();
        LoadTutorial();
    }

    public void StartGame()
    {
        transitionDoors.CloseDoor();
        Invoke("LoadGame", 1f);
    }

    void LoadTutorial()
    {
        Jukebox.ClearQueue();
        GameObject songLoader = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);
        songLoader.name = "SongLoader";
        SongLoader songLoaderComponent = songLoader.AddComponent<SongLoader>();
#if UNITY_EDITOR
        songLoaderComponent.Init(new SongInfo
        {
            ChartFile = Application.streamingAssetsPath + TutorialSongPath,
            AudioFile = Application.streamingAssetsPath + TutorialAudioPath
        });
#else
        songLoaderComponent.Init(new SongInfo
        {
            ChartFile = "/ShipBeat/StreamingAssets" + TutorialSongPath,
            AudioFile = "/ShipBeat/StreamingAssets" + TutorialAudioPath
        });
#endif

        StartCoroutine(WaitForFileLoad());
    }

    IEnumerator WaitForFileLoad()
    {
        while (!SongLoader.IsFileLoaded && !SongLoader.IsAudioLoaded)
            yield return null;

        Invoke("LoadGameScene", 1f);
    }

    void LoadGameScene()
    {
        Jukebox.ClearQueue();
        SceneManager.LoadScene("Game");
    }

    void LoadGame()
    {
        Jukebox.ClearQueue();
        SceneManager.LoadScene("SongSelect");
    }

    struct ValidNameResponse
    {
        public bool valid;
    }
    IEnumerator CheckIfNameValidCoroutine(UnityAction<bool> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Post("http://localhost:3000/api/nameValid", $"{{\"name\": \"{HighscoreManager.PlayerName}\"}}", "application/json"))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
                callback.Invoke(true);
            }
            else
            {
                string data = request.downloadHandler.text;
                ValidNameResponse validNameResponse = JsonUtility.FromJson<ValidNameResponse>(data);
                callback.Invoke(validNameResponse.valid);
            }
        }
    }

    void OnDestroy()
    {
        DialogueTriggers.StartTutorial.RemoveListener(StartTutorial);
        DialogueTriggers.StartGame.RemoveListener(StartGame);
        DialogueTriggers.RetrySelectingName.RemoveListener(SelectFirstLetter);        
    }
}
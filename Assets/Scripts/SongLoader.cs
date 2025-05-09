// Carries data between Song Select and Game scene
// and spawns notes in the Game scene

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Linq;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SongLoader : MonoBehaviour
{
    [SerializeField] bool UseDebugSong = false;
    [SerializeField] string DebugSongPath = "\\Songs\\Crazy Shuffle\\debug.osu";
    [SerializeField] string DebugAudioPath = "\\Songs\\Crazy Shuffle\\audio.mp3";


    const float LookAhead = 2f;
    public static bool IsFileLoaded = false;
    public static bool IsAudioLoaded = false;
    public static SongData LoadedSong = new();
    public static SongLoader Instance;
    SongInfo preloadedSongInfo;

    void Awake()
    {
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += OnExitPlayMode;
#endif    

        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (UseDebugSong) {
            SongFolderReader.ClearCache();
            Init(new SongInfo { ChartFile = Application.streamingAssetsPath + DebugSongPath, AudioFile = Application.streamingAssetsPath + DebugAudioPath });
        }
    }

    public void Init(SongInfo songInfo) {
        preloadedSongInfo = songInfo;
        StartCoroutine(SongFolderReader.FetchFile(songInfo.ChartFile, OnFileFetched));
        StartCoroutine(SongFolderReader.FetchAudioFile(songInfo.AudioFile, OnAudioFetched));
    }

    void OnFileFetched(FetchResult fetchResult) {
        if (fetchResult.result != UnityWebRequest.Result.Success)
            return;

        TextAsset file = fetchResult.fetchedObject as TextAsset;
        SongValidationResult result = OsuFileParser.ParseFile(file);
        if (result.Valid == false) {
            Debug.LogError($"Error parsing file: {result.Message}");
            SceneManager.LoadScene("Menu");
            return;
        }
        LoadedSong = result.Data;
        if (!UseDebugSong) {
            LoadedSong.Info = preloadedSongInfo;
            LoadedSong.Info.AudioFile = preloadedSongInfo.AudioFile;
        }

        IsFileLoaded = true;
    }

    void OnAudioFetched(FetchResult fetchResult) {
        if (fetchResult.result != UnityWebRequest.Result.Success) {
            Debug.LogError($"Error fetching audio file");
            return;
        }
        AudioClip audioClip = fetchResult.fetchedObject as AudioClip;
        if (audioClip == null) {
            Debug.LogError("AudioClip is null");
            SceneManager.LoadScene("Menu");
            return;
        }
        Jukebox.LoadSong(audioClip);
        IsAudioLoaded = true;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        Debug.Log($"Scene loaded: {scene.name}");
        if (scene.name == "Game") {
            if (preloadedSongInfo.ChartFile == null) {
                Debug.LogError("No file path set for song loader");
                SceneManager.LoadScene("Menu");
                return;
            }
            StartCoroutine(WaitForFileLoad());
        } else {
            Destroy(gameObject);
        }
    }

    IEnumerator WaitForFileLoad() {
        while (!IsFileLoaded || !IsAudioLoaded) {
            yield return null;
        }
        GameStart();
    }

    void GameStart() {
        Debug.Log("Game Start");
        Scoring.Reset();
        Scoring.NoteCount = LoadedSong.Notes.Length;
        LaneManager.SetLaneCount(LoadedSong.Info.LaneCount);
        Maestro.TimingPoints = LoadedSong.TimingPoints.ToList();
        Maestro.StartSong();
    }

    float lastNoteTime = 0;
    int lastNoteIndex = 0;
    bool endOfNotesReached = false;
    void Update() {
        if (!Maestro.SongStarted)
            return;

        while (!endOfNotesReached && Maestro.SongTime > lastNoteTime - LookAhead) {
            if (lastNoteIndex >= LoadedSong.Notes.Length) {
                endOfNotesReached = true;
                break;
            }

            Note note = LoadedSong.Notes[lastNoteIndex];
            NoteBehaviourManager.SpawnNote(note);
            Judge.AddNote(note);
            lastNoteTime = note.HitTime;
            lastNoteIndex++;
        }
    }
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

#if UNITY_EDITOR
    private static void OnExitPlayMode(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.ExitingPlayMode) return;
        IsFileLoaded = false;
        IsAudioLoaded = false;
        LoadedSong = new();
        Instance = null;
        EditorApplication.playModeStateChanged -= OnExitPlayMode;
    }
# endif
}

// Carries data between Song Select and Game scene
// and loads the song when the game scene is loaded

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Unity.Serialization.Json;
using NUnit.Framework;

public class SongLoader : MonoBehaviour
{
    const float LookAhead = 2f;
    public static bool IsFileLoaded = false;
    public static bool IsAudioLoaded = false;
    public static SongData LoadedSong = new();
    public static string Folder;
    SongInfo preloadedSongInfo;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void Init(SongInfo songInfo) {
        preloadedSongInfo = songInfo;
        StartCoroutine(SongFolderReader.FetchFile(songInfo.ChartFile, OnFileFetched));
        StartCoroutine(SongFolderReader.FetchAudioFile(songInfo.AudioFile, OnAudioFetched));
    }

    void OnFileFetched(UnityWebRequest request) {
        if (request.result != UnityWebRequest.Result.Success) {
            Debug.LogError($"Error fetching file: {request.error}");
            return;
        }
        TextAsset file = new TextAsset(request.downloadHandler.text);
        if (file == null) {
            Debug.LogError("File is null");
            SceneManager.LoadScene("Menu");
            return;
        }
        SongValidationResult result = OsuFileParser.ParseFile(file);
        if (result.Valid == false) {
            Debug.LogError($"Error parsing file: {result.Message}");
            SceneManager.LoadScene("Menu");
            return;
        }
        LoadedSong = result.Data;
        LoadedSong.Info = preloadedSongInfo;
        LoadedSong.Info.AudioFile = preloadedSongInfo.AudioFile;
        IsFileLoaded = true;
        if(IsAudioLoaded)
            SceneManager.LoadScene("Game");
    }

    void OnAudioFetched(UnityWebRequest request) {
        if (request.result != UnityWebRequest.Result.Success) {
            Debug.LogError($"Error fetching audio file: {request.error}");
            return;
        }
        AudioClip audioClip = DownloadHandlerAudioClip.GetContent(request);
        if (audioClip == null) {
            Debug.LogError("AudioClip is null");
            SceneManager.LoadScene("Menu");
            return;
        }
        Jukebox.LoadSong(audioClip);
        IsAudioLoaded = true;
        if(IsFileLoaded)
            SceneManager.LoadScene("Game");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        Debug.Log($"Scene loaded: {scene.name}");
        if (scene.name == "Game") {
            if (preloadedSongInfo.ChartFile == null) {
                Debug.LogError("No file path set for song loader");
                SceneManager.LoadScene("Menu");
                return;
            }
            GameStart();
        } else {
            Destroy(gameObject);
        }
    }

    void GameStart() {
        Scoring.Reset();
        Scoring.NoteCount = LoadedSong.Notes.Length;
        Maestro.PlaySong(LoadedSong.Info.AudioFile); // TODO: Fix this
    }

    float lastNoteTime = 0;
    int lastNoteIndex = 0;
    bool endOfNotesReached = false;
    void Update() {
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
}

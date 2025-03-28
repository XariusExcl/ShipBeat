using UnityEngine;
using UnityEngine.SceneManagement;

public class SongLoader : MonoBehaviour
{
    const float LookAhead = 2f;
    public static SongData LoadedSong = new();
    public static string Folder;
    public string FilePath;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name == "Game") {
            if (FilePath == null) {
                Debug.LogError("No file path set for song loader");
                SceneManager.LoadScene("Menu");
                return;
            }
            LoadSong();
        } else {
            Destroy(gameObject);
        }
    }

    void LoadSong() {
        /*
        SongValidationResult parsedSong = OsuFileParser.ParseFile(songFile);
        if (!parsedSong.Valid) {
            Debug.LogError(parsedSong.Message);
            return;
        } else { 
            Debug.Log("Song Loaded!");
            LoadedSong = parsedSong.Data;
        }
        */
        
        Scoring.Reset();
        Scoring.NoteCount = LoadedSong.Notes.Length;
        Debug.Log(FilePath);
        Maestro.PlaySong(FilePath.Split('/')[0] + "/" + FilePath.Split('/')[1] + "/" + LoadedSong.Info.AudioFile); // TODO: Fix this
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
}

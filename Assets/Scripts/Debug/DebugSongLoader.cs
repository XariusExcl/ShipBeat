using UnityEngine;

public class DebugSongLoader : MonoBehaviour {
    const float LookAhead = 2f;

    public static SongData LoadedSong = new();

    void Start() {
        TextAsset songFile = Resources.Load<TextAsset>("Naked Glow/chart");
        SongValidationResult parsedSong = OsuFileParser.ParseFile(songFile);
        if (!parsedSong.Valid) {
            Debug.LogError(parsedSong.Message);
            return;
        } else { 
            Debug.Log("Song Loaded!");
            LoadedSong = parsedSong.Data;
        }
        Scoring.Reset();
        Scoring.NoteCount = LoadedSong.Notes.Length;
        Maestro.PlaySong("Naked Glow/" + LoadedSong.Info.File);
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

            // Spawn note
            Note note = LoadedSong.Notes[lastNoteIndex];
            // DebugNoteSpawner.SpawnNote(note);
            NoteBehaviourManager.SpawnNote(note);
            Judge.AddNote(note);
            lastNoteTime = note.HitTime;
            lastNoteIndex++;
        }
    }
}
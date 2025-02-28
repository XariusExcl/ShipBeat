using UnityEngine;

public class DebugSongLoader : MonoBehaviour {
    const float LookAhead = 2f;

    public SongData loadedSong = new();

    void Start() {
        TextAsset songFile = Resources.Load<TextAsset>("Naked Glow/chart");
        SongValidationResult parsedSong = OsuFileParser.ParseFile(songFile);
        if (!parsedSong.Valid) {
            Debug.LogError(parsedSong.Message);
            return;
        } else { 
            Debug.Log("Song Loaded!");
            loadedSong = parsedSong.Data;
        }
        Scoring.Reset();
        Scoring.NoteCount = loadedSong.Notes.Length;
        Maestro.PlaySong("Naked Glow/" + loadedSong.Info.File);
    }

    float lastNoteTime = 0;
    int lastNoteIndex = 0;
    bool endOfNotesReached = false;
    void Update() {
        while (!endOfNotesReached && Maestro.SongTime > lastNoteTime - LookAhead) {
            if (lastNoteIndex >= loadedSong.Notes.Length) {
                endOfNotesReached = true;
                break;
            }

            // Spawn note
            Note note = loadedSong.Notes[lastNoteIndex];
            // DebugNoteSpawner.SpawnNote(note);
            NoteBehaviourManager.SpawnNote(note);
            Judge.AddNote(note);
            lastNoteTime = note.HitTime;
            lastNoteIndex++;
        }
    }
}
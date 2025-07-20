using UnityEngine;
using System.Collections.Generic;

public class Judge : MonoBehaviour {
    static Queue<Note>[] notes = new Queue<Note>[8];
    // Hit windows in seconds and in both directions.
    public const float PerfectHitWindow = 0.05f;
    public const float GreatHitWindow = 0.1f;
    public const float BadHitWindow = 0.15f;
    public const float MissHitWindow = 0.25f;
    static List<Note> heldNotes;  

    public static void JudgePlayerInput(int id, ButtonState state) {

        if (state == ButtonState.Down || state == ButtonState.Left || state == ButtonState.Right) {
            if (notes[id].Count == 0) return;
            
            Note note = notes[id].Peek();
            float diff = Mathf.Abs(note.HitTime - Maestro.SongTime);
            if (diff > MissHitWindow) return;
            
            JudgeNoteHit(note, state, diff);
            if (note.Type == NoteType.Hold)
                heldNotes.Add(note);

            notes[id].Dequeue();

        } else if (state == ButtonState.Up) {
            if (heldNotes.Count == 0) return;
            Note note = heldNotes.Find(n => n.Lane == id);
            if (note.Equals(default(Note))) return; // If no note was found. TODO: Does this work?
            
            float diff = Mathf.Abs(note.ReleaseTime - Maestro.SongTime);

            heldNotes.Remove(note);
            JudgeNoteHit(note, state, diff);
        }
    }

    static void JudgeNoteHit(Note note, ButtonState state, float diff) {
        if (state == ButtonState.Down || state == ButtonState.Left || state == ButtonState.Right) {
            if (note.Lane != 0) { // Normal note
                if (diff < PerfectHitWindow)
                    Scoring.AddPerfect();
                else if (diff < GreatHitWindow)
                    Scoring.AddGood();
                else if (diff < BadHitWindow)
                    Scoring.AddBad();
                else
                    Scoring.AddMiss();
            } else { // Slam note
                if (diff < BadHitWindow)
                    Scoring.AddPerfect();
                else
                    Scoring.AddMiss();
            }

            if (note.Type == NoteType.Note) {
                LaneManager.GetLane(note.Lane).SuccessfulHit(state == ButtonState.Right);
                NoteBehaviourManager.ReturnToPool(note);
            }
            else if (note.Type == NoteType.Hold) {
                LaneManager.GetLane(note.Lane).Hold();
                NoteBehaviourManager.HideHead(note);
            }

        } else if (state == ButtonState.Up) {
            if (diff < BadHitWindow)
                Scoring.AddPerfect();
            else
                Scoring.AddMiss();
            
            LaneManager.GetLane(note.Lane).SuccessfulRelease();
            NoteBehaviourManager.ReturnToPool(note);
        }
    }

    public static void AddNote(Note note) {
        notes[note.Lane].Enqueue(note);
    }

    void Start()
    {
        notes = new Queue<Note>[8];
        for (int i = 0; i < 8; i++) {
            notes[i] = new Queue<Note>();
        }
        heldNotes = new();
    } 

    void Update()
    {
        foreach (Queue<Note> lane in notes) {
            if (lane.Count == 0) continue;
            Note note = lane.Peek();
            if (note.HitTime < Maestro.SongTime - BadHitWindow) {
                lane.Dequeue();
                Scoring.AddMiss();
                if (note.Type == NoteType.Note)
                    NoteBehaviourManager.ReturnToPool(note);
                else if (note.Type == NoteType.Hold) {
                    NoteBehaviourManager.HideHead(note);
                    heldNotes.Add(note);
                }
            }
        }

        for (int i = heldNotes.Count - 1; i >= 0; i--) {
            Note heldNote = heldNotes[i];
            if (heldNote.ReleaseTime < Maestro.SongTime - BadHitWindow) {
                heldNotes.RemoveAt(i);
                Scoring.AddMiss();
                LaneManager.GetLane(heldNote.Lane).Release();
                NoteBehaviourManager.ReturnToPool(heldNote);
            }
        }
    }
}
using UnityEngine;
using System.Collections.Generic;

public class Judge : MonoBehaviour {
    public static Queue<Note>[] Notes = new Queue<Note>[8];
    // Hit windows in seconds and in both directions.
    public static float PerfectHitWindow = 0.05f;
    public static float GreatHitWindow = 0.1f;
    public static float BadHitWindow = 0.15f;
    public static float MissHitWindow = 0.25f;

    static void JudgeNoteHit(Note note) {
        float diff = Mathf.Abs(Maestro.SongTime - note.HitTime);

        if (note.Lane >= 2) { // Normal note
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
        
        NoteBehaviourManager.ReturnToPool(note.Id);
    }

    public static void JudgePlayerInput(int id) {
        if (Notes[id].Count == 0) return;
        Note note = Notes[id].Peek();
        float diff = Mathf.Abs(note.HitTime - Maestro.SongTime);
        if (diff > MissHitWindow) return;
        
        JudgeNoteHit(note);
        Notes[id].Dequeue();
    }

    public static void AddNote(Note note) {
        Notes[note.Lane].Enqueue(note);
    }

    void Start()
    {
        Notes = new Queue<Note>[8];
        for (int i = 0; i < 8; i++) {
            Notes[i] = new Queue<Note>();
        }
    } 

    void Update()
    {
        foreach (Queue<Note> lane in Notes) {
            if (lane.Count == 0) continue;
            Note note = lane.Peek();
            if (note.HitTime < Maestro.SongTime - BadHitWindow) {
                lane.Dequeue();
                Scoring.AddMiss();
                NoteBehaviourManager.ReturnToPool(note.Id);
            }
        }    
    }
}
// Manages notes on the lanes (spawning, object pooling, and visual representation of current state of notes)
using System.Collections.Generic;
using UnityEngine;

public class NoteBehaviourManager : MonoBehaviour
{
    List<NoteBehaviour> noteBehaviours = new List<NoteBehaviour>();
    public static NoteBehaviourManager Instance;
    public GameObject notePrefab;


    void Awake(){
        Instance = this;
    }

    void Start() {
        // TODO: Pre-warm models of notes

        // Create a pool of notes
        for (int i = 0; i < 10; i++)
        {
            GameObject noteObject = Instantiate(notePrefab, transform);
            noteObject.transform.position = new Vector3(0, -10, 0); 
            NoteBehaviour noteBehaviour = noteObject.GetComponent<NoteBehaviour>();
            noteBehaviour.ReturnToPool();
            noteBehaviours.Add(noteBehaviour);
        }
    }

    NoteBehaviour FetchNoteFromPool()
    {
        NoteBehaviour noteBehaviour = noteBehaviours.Find(note => note.PoolState == NotePoolState.InPool);
        // If none are available, create new ones
        if (noteBehaviour == null)
        {
            for (int i = 0; i < 5; i++)
            {
                GameObject noteObject = Instantiate(notePrefab, transform);
                noteObject.transform.position = new Vector3(0, -10, 0); 
                NoteBehaviour newNoteBehaviour = noteObject.GetComponent<NoteBehaviour>();
                newNoteBehaviour.ReturnToPool();
                noteBehaviours.Add(newNoteBehaviour);
            }
            noteBehaviour = noteBehaviours.Find(note => note.PoolState == NotePoolState.InPool);
        }
        return noteBehaviour;
    }

    /// <summary>
    /// Spawns a random note on a random lane
    /// </summary>
    /// <param name="maxChord"></param>
    public void SpawnRandomNote(float maxChord = 1) {
        for (int i = 0; i < Random.Range(1, maxChord); i++)
        {
            NoteBehaviour noteBehaviour = FetchNoteFromPool();
            noteBehaviour.Init(new Note { Type = NoteType.Note, Lane = Random.Range(0, 6), HitTime = Maestro.SongTime + 2f, ReleaseTime = Maestro.SongTime + 2f });
        }
    }

    /// <summary>
    /// Spawns a note on a lane
    /// </summary>
    /// <param name="note"></param>
    public static void SpawnNote(Note note) {
        NoteBehaviour noteBehaviour = Instance.noteBehaviours.Find(note => note.PoolState == NotePoolState.InPool);
        noteBehaviour.Init(note);
    }

    /// <summary>
    /// Returns a note to the pool
    /// </summary>
    /// <param name="note"></param>
    public static void ReturnToPool(Note note) {
        NoteBehaviour noteBehaviour = Instance.noteBehaviours.Find(noteb => noteb.Note.Id == note.Id);
        if (noteBehaviour == null) {
            Debug.LogError($"Note {note.Id} not found in pool!");
            return;
        }       
        noteBehaviour.ReturnToPool();
    }

    /// <summary>
    /// Returns a note to the pool
    /// </summary>
    /// <param name="note"></param>
    public static void ReturnToPool(int id) {
        NoteBehaviour noteBehaviour = Instance.noteBehaviours.Find(noteb => noteb.Note.Id == id);
        if (noteBehaviour == null) {
            Debug.LogError($"Note {id} not found in pool!");
            return;
        }
        noteBehaviour.ReturnToPool();
    }
}

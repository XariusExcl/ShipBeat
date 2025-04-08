using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

public struct Note {
    public int Id;
    public NoteType Type;
    public int Lane;
    public float HitTime;
    public float ReleaseTime;
}

public enum NoteType
{
    Note,
    Hold
}

public enum NotePoolState
{
    InUse,
    InPool
}

public class NoteBehaviour : MonoBehaviour
{
    // Models
    [SerializeField] GameObject noteSLModel;
    [SerializeField] GameObject noteSRModel;
    [SerializeField] GameObject noteGndModel;
    [SerializeField] GameObject noteSkyModel;
    // Data
    public Note Note;
    public NotePoolState PoolState { get; private set; } = NotePoolState.InPool;
    // Components
    SplineAnimate splineAnimate;
    Lane lane;


    /// <summary>
    /// Initializes the note with the given data. Sets the note to be in use (pool).
    /// </summary>
    /// <param name="note"></param>
    public void Init(Note note)
    {
        noteGndModel.SetActive(false);
        noteSkyModel.SetActive(false);
        noteSLModel.SetActive(false);
        noteSRModel.SetActive(false);
        splineAnimate = GetComponent<SplineAnimate>();
        Note = note;
        switch (note.Lane) {
            case 0: // Slam Left
                noteSLModel.SetActive(true);
                break;
            case 1: // Slam Right
                noteSRModel.SetActive(true);
                break;
            case 2: case 3: case 4: // Block
                noteGndModel.SetActive(true);
                break;
            case 5: case 6: case 7: // Sky
                noteSkyModel.SetActive(true);
                break;
        }
        lane = LaneManager.GetLane(note.Lane);
        splineAnimate.Container = lane.SplineContainer;

        transform.position = new Vector3(0, -10, 0);
        PoolState = NotePoolState.InUse;
    }

    public void ReturnToPool()
    {
        PoolState = NotePoolState.InPool;
        noteGndModel.SetActive(false);
        noteSkyModel.SetActive(false);
        noteSLModel.SetActive(false);
        noteSRModel.SetActive(false);
        transform.position = new Vector3(0, -10, 0);
    }
    

    void Update()
    {
        if (PoolState == NotePoolState.InPool) return;
        float normalizedTime = (Note.HitTime - Maestro.SongTime) / (10f / Maestro.LaneSpeed);

        if (normalizedTime < 1){       
            if (normalizedTime > 0) // Move along the spline
                splineAnimate.NormalizedTime = normalizedTime;
            else // Move along the extrapolation vector
                transform.position += lane.ExtrapolationVector * Time.deltaTime * (Maestro.LaneSpeed / 10f);
            
        }
    }

    /// <summary>
    ///  Cycles through the models of the note. Used for preloading the models at gameplay start.
    /// </summary>
    public IEnumerator CycleModels()
    {
        yield return new WaitForEndOfFrame();
        noteGndModel.SetActive(true);
        yield return new WaitForEndOfFrame();
        noteGndModel.SetActive(false);
        noteSkyModel.SetActive(true);
        yield return new WaitForEndOfFrame();
        noteSkyModel.SetActive(false);
        noteSLModel.SetActive(true);
        yield return new WaitForEndOfFrame();
        noteSLModel.SetActive(false);
        noteSRModel.SetActive(true);
        yield return new WaitForEndOfFrame();
        noteSRModel.SetActive(false);
        Destroy(this);
        yield return null;
    }
}

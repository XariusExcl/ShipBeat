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
    [SerializeField] GameObject noteDodgeModel;
    [SerializeField] GameObject noteGndModel;
    [SerializeField] GameObject noteSkyModel;
    // Data
    public Note Note;
    public NotePoolState PoolState { get; private set; } = NotePoolState.InPool;
    // Components
    SplineAnimate splineAnimate;
    LineRenderer tailLineRenderer;
    Lane lane;


    void Awake()
    {        
        splineAnimate = GetComponent<SplineAnimate>();
        tailLineRenderer = GetComponent<LineRenderer>();
    }

    /// <summary>
    /// Initializes the note with the given data. Sets the note to be in use (pool).
    /// </summary>
    /// <param name="note"></param>
    public void Init(Note note)
    {
        noteGndModel.SetActive(false);
        noteSkyModel.SetActive(false);
        noteDodgeModel.SetActive(false);
        tailLineRenderer.positionCount = 2;
        tailLineRenderer.enabled = false;

        Note = note;
        switch (note.Lane) {
            case 0: // Slam
                noteDodgeModel.SetActive(true);
                break;
            case 1: case 2: case 3: // Block
                noteGndModel.SetActive(true);
                break;
            case 4: case 5: case 6: // Sky
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
        noteDodgeModel.SetActive(false);
        tailLineRenderer.enabled = false;
        transform.position = new Vector3(0, -10, 0);
    }
    
    void Update()
    {
        if (PoolState == NotePoolState.InPool) return;
        float normalizedTime = (Note.HitTime - Maestro.SongTime) / (10f / Maestro.LaneSpeed);

        if (normalizedTime < 1){
            if (Note.Type == NoteType.Hold)
                tailLineRenderer.enabled = true;

            if (normalizedTime > 0) // Move along the spline
                splineAnimate.NormalizedTime = normalizedTime;
            else // Move along the extrapolation vector
                transform.position += lane.ExtrapolationVector * Time.deltaTime * (Maestro.LaneSpeed / 10f);
        }

        if (Note.Type == NoteType.Hold)
        {
            float normalizedReleaseTime = (Note.ReleaseTime - Maestro.SongTime) / (10f / Maestro.LaneSpeed);
            tailLineRenderer.SetPosition(0, normalizedTime > 0 ? transform.position : lane.transform.position);
            tailLineRenderer.SetPosition(1, lane.SplineContainer.EvaluatePosition(normalizedReleaseTime));
        }
    }

    /// <summary>
    /// Hides the note head for when the long note is being held.
    /// </summary>
    public void HideHead()
    {
        noteGndModel.SetActive(false);
        noteSkyModel.SetActive(false);
    }

    /// <summary>
    ///  Cycles through the models of the note. Used for preloading the models at gameplay start without causing too big of a lag spike.
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
        noteDodgeModel.SetActive(true);
        yield return new WaitForEndOfFrame();
        noteDodgeModel.SetActive(false);
        Destroy(this);
        yield return null;
    }
}

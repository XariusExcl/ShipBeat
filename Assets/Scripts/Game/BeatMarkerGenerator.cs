using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class BeatMarkerGenerator : MonoBehaviour
{
    static TimingPoint lookAheadTimingPoint;
    static TimingPoint nextLookAheadTimingPoint;
    static float lastBeatTime = 0;
    const float LookAhead = 2f;
    List<BeatMarkerBehaviour> beatMarkers = new List<BeatMarkerBehaviour>();
    [SerializeField] GameObject beatMarkerGo;
    SplineContainer splineContainer;

    static bool LastTimingPointReached = false;
    void Start()
    {
        splineContainer = GetComponent<SplineContainer>();
        CreateBeatMarkers(10);
    }
    
    public static void SongReady()
    {
        lookAheadTimingPoint = nextLookAheadTimingPoint = Maestro.TimingPoints[0];
        LastTimingPointReached = false;
        lastBeatTime = -LookAhead;
    }

    void Update()
    {
        if (Maestro.SongTime > lastBeatTime - LookAhead)
        {
            CreateNextBeat();
        }

        if (!LastTimingPointReached && nextLookAheadTimingPoint.Time < Maestro.SongTime + LookAhead)
        {
            GetNextTimingPoint();
        }
    }

    int nextBeatToCreate = -4; // This is for leading bars, always reset to 0 when a new timing point happens
    void CreateNextBeat()
    {
        BeatMarkerBehaviour bmb = GetBeatMarker();
        lastBeatTime = 60f * nextBeatToCreate / lookAheadTimingPoint.BPM + lookAheadTimingPoint.Time;
        bmb.ThisBeatTime = lastBeatTime;
        bmb.IsBar = nextBeatToCreate % (lookAheadTimingPoint.Meter == 0 ? 4 : lookAheadTimingPoint.Meter) == 0;
        bmb.gameObject.SetActive(true);
        nextBeatToCreate++;
    }

    int nextLookAheadTimingPointIndex = 0;
    void GetNextTimingPoint()
    {
        lookAheadTimingPoint = nextLookAheadTimingPoint;
        do
        {
            nextLookAheadTimingPointIndex++;
            nextBeatToCreate = 0;
        } while (nextLookAheadTimingPointIndex < Maestro.TimingPoints.Count && Maestro.TimingPoints[nextLookAheadTimingPointIndex].BPM == float.MinValue); // If bpm Is not set to MinValue, then it's a timing point
        
        if (nextLookAheadTimingPointIndex >= Maestro.TimingPoints.Count)
        {
            LastTimingPointReached = true;
            nextLookAheadTimingPointIndex = Maestro.TimingPoints.Count - 1;
        }
        nextLookAheadTimingPoint = Maestro.TimingPoints[nextLookAheadTimingPointIndex];
    }

    void CreateBeatMarkers(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject bm = Instantiate(beatMarkerGo, transform);
            BeatMarkerBehaviour bmb = bm.GetComponent<BeatMarkerBehaviour>();
            beatMarkers.Add(bmb);
            bmb.splineAnimate.Container = splineContainer;
            bmb.gameObject.SetActive(false);
        }
    }

    BeatMarkerBehaviour GetBeatMarker()
    {
        BeatMarkerBehaviour bm = beatMarkers.Find(bm => !bm.InUse);
        // If none are available, create new ones
        if (bm == null)
        {
            CreateBeatMarkers(5);
            return beatMarkers[^1];
        }
        else return bm;
    }
}
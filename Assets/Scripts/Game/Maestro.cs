// The "GameManager" for song playing. Keeps track of the song time info and when the song starts and ends. Also contains the lane speed for the notes.

using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class Maestro : MonoBehaviour
{
    [SerializeField] TMP_Text time;

    public static List<TimingPoint> TimingPoints = new List<TimingPoint>();
    static TimingPoint currentTimingPoint = new TimingPoint();
    static TimingPoint currentEffectPoint = new TimingPoint();

    // Song time and state
    public static int LaneSpeed = 8; // Note time on the lane is 10 / LaneSpeed, i.e 8 = 1250ms
    public static bool SongStarted = false;
    public static bool SongEnded = false;
    public static float SongTime;
    public static int Bar;
    public static int Beat;
    public static float Tick;
    public static float StartTime;
    const float StartDelay = 3;
    public static float GlobalOffset = -.02f; // To be adjusted by the player, in seconds
    public static bool IsKiaiTime = false;
    public static UnityEvent OnKiaiStart = new UnityEvent();
    public static UnityEvent OnKiaiEnd = new UnityEvent();

    public static void StartSong() {
        SongStarted = true;
        SongEnded = false;
        Jukebox.PlayScheduled(StartDelay);
        SongTime = 0;
        StartTime = (float)AudioSettings.dspTime + StartDelay;
        currentTimingPoint = TimingPoints[0];

        // print all timing points
        foreach (var timingPoint in TimingPoints) {
            Debug.Log($"Timing Point: {timingPoint.Time} BPM: {timingPoint.BPM} Kiai: {timingPoint.KiaiMode}");
        }
    }

    void Start() {
        SongTime = 0;
        StartTime = (float)AudioSettings.dspTime + StartDelay;
        InvokeRepeating("CheckCalibration", 3, 3);
    }

    void Update() {
        SongTime = (float)AudioSettings.dspTime - StartTime;

        if(SongStarted && !SongEnded) {
            if (SongTime > SongLoader.LoadedSong.Info.Length + SongLoader.LoadedSong.Info.SongStart + 0.3f) {
                SongEnded = true;
                Invoke("EndSong", 1.5f);
                Debug.Log("Song Ended");
            }
            UpdateTimingPoint();
            UpdateTimingInfo();
            time.text = SongTime.ToString("F2");
        }

        Shader.SetGlobalFloat("_Tick", Mathf.Abs(Tick));
    }

    int timingPointIndex = 0;
    void UpdateTimingPoint() {
        if (TimingPoints.Count == 0 || timingPointIndex >= TimingPoints.Count) return;
        if (SongTime >= TimingPoints[timingPointIndex].Time) {
            if (TimingPoints[timingPointIndex].BPM != float.MinValue) // if BPM is not set to MinValue, then it's a timing point
                currentTimingPoint = TimingPoints[timingPointIndex];

            currentEffectPoint = TimingPoints[timingPointIndex];

            timingPointIndex++;

            if (currentEffectPoint.KiaiMode && !IsKiaiTime) {
                IsKiaiTime = true;
                Debug.Log("Kiai Time!");
                OnKiaiStart.Invoke();
            } else if (!currentEffectPoint.KiaiMode && IsKiaiTime) {
                IsKiaiTime = false;
                Debug.Log("End Kiai Time");
                OnKiaiEnd.Invoke();
            }
            Debug.Log($"Timing Point {currentEffectPoint.Time}: {(IsKiaiTime?"(K) ":"")}current BPM: {currentTimingPoint.BPM} {currentTimingPoint.Meter}/4");
        }
    }

    void UpdateTimingInfo() {
        float relativeTime = SongTime - currentTimingPoint.Time;
        float beat = relativeTime * (currentTimingPoint.BPM / 60f);
        Bar = Mathf.FloorToInt(beat / currentTimingPoint.Meter);
        Beat = Mathf.FloorToInt(beat % currentTimingPoint.Meter);
        Tick = beat - (int)beat;
    }

    void EndSong() {
        SongEnded = true;
        GameUIManager.ShowResults();
    }

    void CheckCalibration() {
        if (!SongStarted || SongEnded) return;
        float delay = Jukebox.GetPlaybackPosition() - SongTime + GlobalOffset;
        if (Mathf.Abs(delay) > .020) {
            Debug.Log($"Song is {Mathf.Abs(delay) * 1000:F0} ms {(delay > 0 ? "early":"late")}.Recalibrate!");
            Jukebox.SetPlaybackPosition(SongTime - GlobalOffset);
        }
    }
}

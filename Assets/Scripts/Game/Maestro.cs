// The "GameManager" for song playing. Keeps track of the song time info and when the song starts and ends. Also contains the lane speed for the notes.

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using Anatidae;
using System.Collections;

public class Maestro : MonoBehaviour
{
    public static List<TimingPoint> TimingPoints = new List<TimingPoint>();
    static int timingPointIndex = 0;
    public static TimingPoint CurrentTimingPoint = new TimingPoint();
    public static TimingPoint CurrentEffectPoint = new TimingPoint();

    // Song time and state
    static int _laneSpeed = 7;
    public static int LaneSpeed // Note time on the lane is 10 / LaneSpeed, i.e 8 = 1250ms
    {
        get => _laneSpeed;
        set
        {
            if (value > 15) value = 15;
            if (value < 5) value = 5;
            _laneSpeed = value;
        }
    }
    public static float SpeedMultiplier;
    static float globalAverageSpeedMultiplier;
    public static bool SongStarted = false;
    public static bool SongEnded = false;
    public static bool StoryboardEnded = false;
    public static bool IsTutorial = false;
    public static float SongTime;
    public static int Bar;
    public static int Beat;
    public static float Tick;
    public static float StartTime;
    const float StartDelay = 4;
    static float _globalOffset = -.05f;
    public static float GlobalOffset
    {
        get => _globalOffset;
        set
        {
            if (value < -.25f) value = -.25f;
            if (value > .25f) value = .25f;
            _globalOffset = value;
        }
    }
    public static bool IsKiaiTime = false;
    public static UnityEvent OnKiaiStart = new UnityEvent();
    public static UnityEvent OnKiaiEnd = new UnityEvent();

    public static void StartSong() {
        SongStarted = true;
        SongEnded = false;
        IsTutorial = false;
        Jukebox.Stop();
        Jukebox.PlayScheduled(StartDelay);
        Jukebox.SetVolume(0.5f);
        SongTime = 0;
        StartTime = (float)AudioSettings.dspTime + StartDelay;
        CurrentTimingPoint = TimingPoints[0];
        timingPointIndex = 0;
        SpeedMultiplier = 1;
        ComputeAverageGlobalSpeedMultiplier();
    }

    void Start()
    {
        SongTime = 0;
        StartTime = (float)AudioSettings.dspTime + StartDelay;
        InvokeRepeating("CheckCalibration", StartDelay, 3);
    }

    void Update() {
        SongTime = (float)AudioSettings.dspTime - StartTime;

        if(SongStarted && !SongEnded) {
            if (StoryboardEnded && SongTime > SongLoader.LoadedSong.Info.Length + SongLoader.LoadedSong.Info.SongStart + 0.3f) {
                SongEnded = true;
                Debug.Log("Song Ended");
                if (!IsTutorial && Scoring.Misses == 0)
                {
                    if (Scoring.Percentage == 100) GameUIManager.ShowPerfectFullComboAnimation();
                    else GameUIManager.ShowFullComboAnimation();
                    Invoke("EndSong", 2.5f);
                } else Invoke("EndSong", 1.5f);
            }
            UpdateTimingPoint();
            UpdateTimingInfo();
        }

        Shader.SetGlobalFloat("_Tick", Mathf.Abs(Tick));
    }

    void UpdateTimingPoint() {
        if (TimingPoints.Count == 0 || timingPointIndex >= TimingPoints.Count) return;
        if (SongTime >= TimingPoints[timingPointIndex].Time) {
            if (TimingPoints[timingPointIndex].BPM != float.MinValue) // if BPM is not set to MinValue, then it's a timing point
                CurrentTimingPoint = TimingPoints[timingPointIndex];

            CurrentEffectPoint = TimingPoints[timingPointIndex];
            SpeedMultiplier = CurrentEffectPoint.SpeedMultiplier;

            timingPointIndex++;

            if (CurrentEffectPoint.KiaiMode && !IsKiaiTime) {
                IsKiaiTime = true;
                OnKiaiStart.Invoke();
            } else if (!CurrentEffectPoint.KiaiMode && IsKiaiTime) {
                IsKiaiTime = false;
                OnKiaiEnd.Invoke();
            }
            Debug.Log($"Timing Point {CurrentEffectPoint.Time}: {(IsKiaiTime?"(K) ":"")}current BPM: {CurrentTimingPoint.BPM} ({CurrentEffectPoint.SpeedMultiplier:F2}x) {CurrentTimingPoint.Meter}/4");
        }
    }

    void UpdateTimingInfo() {
        float relativeTime = SongTime - CurrentTimingPoint.Time;
        float beat = relativeTime * (CurrentTimingPoint.BPM / 60f);
        Bar = Mathf.FloorToInt(beat / CurrentTimingPoint.Meter);
        Beat = Mathf.FloorToInt(beat % CurrentTimingPoint.Meter);
        Tick = beat - (int)beat;
    }

    public static float GetNormalizedPositionAtTime(float time)
    {
        return (time - SongTime) * LaneSpeed * SpeedMultiplier * 0.1f;
    }

    public static float GetNormalizedPositionAtTimeSV(float time)
    {
        return (time - SongTime) * LaneSpeed * AverageLaneSpeedMultiplier(time) * (1f / globalAverageSpeedMultiplier) * 0.1f;
    }

    static float AverageLaneSpeedMultiplier(float endTime)
    {
        if (TimingPoints.Count == 0)
            return 1f;

        float totalWeighted = 0f;
        float totalDuration = 0f;

        float prevTime = SongTime;
        float prevMultiplier = SpeedMultiplier;

        for (int i = timingPointIndex; i < TimingPoints.Count && TimingPoints[i].Time < endTime; i++)
        {
            float nextTime = TimingPoints[i].Time;
            float duration = nextTime - prevTime;
            totalWeighted += prevMultiplier * duration;
            totalDuration += duration;
            prevTime = nextTime;
            prevMultiplier = TimingPoints[i].SpeedMultiplier;
        }

        if (prevTime < endTime)
        {
            float duration = endTime - prevTime;
            totalWeighted += prevMultiplier * duration;
            totalDuration += duration;
        }

        if (totalDuration == 0f)
            return prevMultiplier;

        return totalWeighted / totalDuration;
    }

    static void ComputeAverageGlobalSpeedMultiplier()
    {
        if (TimingPoints.Count == 0)
        {
            globalAverageSpeedMultiplier = 1f;
            return;
        }

        float totalWeighted = 0f;
        float totalDuration = 0f;
        float prevTime = TimingPoints[0].Time;
        float prevMultiplier = TimingPoints[0].SpeedMultiplier;

        for (int i = 1; i < TimingPoints.Count; i++)
        {
            float nextTime = TimingPoints[i].Time;
            float duration = nextTime - prevTime;
            totalWeighted += prevMultiplier * duration;
            totalDuration += duration;
            prevTime = nextTime;
            prevMultiplier = TimingPoints[i].SpeedMultiplier;
        }

        globalAverageSpeedMultiplier = totalDuration > 0f ? totalWeighted / totalDuration : 1f;
    }

    void EndSong() {
        SongEnded = true;
        if (SongFolderReader.SongInfos.Count != 0 || IsTutorial) // Special case to save a score only when not testing.
            StartCoroutine(UpdateScore());

        GameUIManager.ShowResults();
    }
    void CheckCalibration() {
        if (!SongStarted || SongEnded) return;
        float delay = Jukebox.GetPlaybackPosition() - SongTime + GlobalOffset;
        if (Mathf.Abs(delay) > .020) {
            Debug.Log($"Song is {Mathf.Abs(delay) * 1000:F0} ms {(delay > 0 ? "early" : "late")}. Recalibrate!");
            // Jukebox.SetPlaybackPosition(SongTime - GlobalOffset);
            StartTime -= delay;
        }
    }

    IEnumerator UpdateScore()
    {
        if (OnlineDataManager.Online)
        {
            yield return StartCoroutine(OnlineDataManager.SendScore(Scoring.CreateHighscore()));

        } else
        {
            yield return StartCoroutine(ExtradataManager.SetExtraData($"Scores/{SongLoader.LoadedSong.Info.Title}_{SongLoader.LoadedSong.Info.DifficultyName}", Scoring.SaveScore()));
            yield return StartCoroutine(ExtradataManager.SetExtraData($"Player/{HighscoreManager.PlayerName}/TotalScore", Scoring.TotalScore.ToString()));
        }

        yield return new WaitForSecondsRealtime(2f);
        GameUIManager.UpdateTotalScore();
    }
}

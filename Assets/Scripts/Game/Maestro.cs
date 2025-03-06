// The "GameManager" for song playing. Keeps track of the song time and when the song starts and ends. Also contains the hit window and lane speed for the notes.

using TMPro;
using UnityEngine;

public class Maestro : MonoBehaviour
{
    [SerializeField] TMP_Text time;
    Maestro Instance;
    public static bool SongStarted = false;
    public static bool SongEnded = false;
    public static float SongTime;

    public static int LaneSpeed = 5; // Note time on the lane is 10 / LaneSpeed, i.e 8 = 1250ms
    public static float StartTime;
    const float StartDelay = 3;
    public static float GlobalOffset = -.15f; // To be adjusted by the player, in seconds

    public static void PlaySong(string path) {
        SongStarted = true;
        SongEnded = false;
        Debug.Log(path);
        Jukebox.LoadSong(path);
        Jukebox.PlayScheduled(StartDelay);
        SongTime = 0;
        StartTime = (float)AudioSettings.dspTime + StartDelay;

    }

    void Awake() {
        Instance = this;
    }

    void Start() {
        SongTime = 0;
        StartTime = (float)AudioSettings.dspTime + StartDelay;
        InvokeRepeating("CheckCalibration", 3, 3);
    }

    void Update() {
        if(SongStarted && !SongEnded) {
            SongTime = (float)AudioSettings.dspTime - StartTime;
            if (SongTime > SongLoader.LoadedSong.Info.Length + SongLoader.LoadedSong.Info.SongStart) {
                SongEnded = true;
                Invoke("EndSong", 1.5f);
                Debug.Log("Song Ended");
            }
            time.text = SongTime.ToString("F2");
        }

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            GlobalOffset += .01f;
            Debug.Log($"Global Offset: {GlobalOffset}");
            Jukebox.SetPlaybackPosition(SongTime - GlobalOffset);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            GlobalOffset -= .01f;
            Debug.Log($"Global Offset: {GlobalOffset}");
            Jukebox.SetPlaybackPosition(SongTime - GlobalOffset);
        }
    }

    void EndSong() {
        SongEnded = true;
        Debug.Log("Song Ended");
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

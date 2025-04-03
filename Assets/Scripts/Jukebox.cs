// Plays music.

using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Jukebox : MonoBehaviour
{
    public static string NowPlaying = "";
    public static Jukebox Instance;
    AudioSource audioSource;

    /// <summary> 
    /// Loads the song into the Jukebox.
    /// </summary>
    public static void LoadSong(AudioClip clip){
        Instance.audioSource.clip = clip;
    }

    public static void LoadSongAndPlay(string path) {
        Instance.StartCoroutine(SongFolderReader.FetchAudioFile(path, (request) => {
            if (request.result != UnityWebRequest.Result.Success) {
                Debug.LogError($"Error fetching file: {request.error}");
                return;
            }
            AudioClip myClip = DownloadHandlerAudioClip.GetContent(request);
            if (myClip == null) {
                Debug.LogError("AudioClip is null");
                return;
            }
            LoadSong(myClip);
            NowPlaying = path;
            Play();
        }));
    }

    public static void Play(){
        Instance.audioSource.Play();
    }

    /// <summary> 
    /// Schedules the audio file to be played in seconds. Use this to give time for the audio file to load before playing.
    /// </summary>
    public static void PlayScheduled(float delay){
        Instance.audioSource.PlayScheduled(AudioSettings.dspTime + delay);
    }

    public static void Pause(){
        Instance.audioSource.Pause();
    }

    public static void Stop(){
        Instance.audioSource.Stop();
    }

    public static void SetVolume(float volume){
        Instance.audioSource.volume = volume;
    }

    /// <summary> 
    /// Get playback position (in seconds) from the audio system. Used to sanity check desynchronization (maybe).
    /// </summary>
    public static float GetPlaybackPosition(){
        return (float)Instance.audioSource.timeSamples / Instance.audioSource.clip.frequency;
    }

    /// <summary> 
    /// Set playback position (in seconds) for recalibration/seeking.
    /// </summary>
    public static void SetPlaybackPosition(float position){
        Instance.audioSource.timeSamples = (int)(position * Instance.audioSource.clip.frequency);
    }

    /// <summary>
    /// Set the audio clip to loop.
    /// </summary>
    /// <param name="loop">True to loop, false to not loop.</param>
    /// <param name="loopStart">Start time of the loop in seconds.</param>
    public static void SetLoop(bool loop, float loopStart = 0){
        Instance.audioSource.loop = loop;
        if (loop) {
            Instance.audioSource.time = loopStart;
        }
    }


    void Awake(){
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
        audioSource = GetComponent<AudioSource>();

    }
}

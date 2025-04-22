// Plays music.

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class Jukebox : MonoBehaviour
{
    public static string NowPlaying = "";
    public static Jukebox Instance;
    AudioSource audioSource;
    public static bool IsPlaying => Instance.audioSource.isPlaying;

    /// <summary> 
    /// Loads the song into the Jukebox.
    /// </summary>
    public static void LoadSong(AudioClip clip){
        clip.LoadAudioData();
        Instance.audioSource.clip = clip;
    }

    public static void LoadSongAndPlay(string path, float setPlaybackPosition = 0) { 
        Instance.StartCoroutine(SongFolderReader.FetchAudioFile(path, (result) => {
            if (result.result != UnityWebRequest.Result.Success)
                return;

            AudioClip myClip = result.fetchedObject as AudioClip;
            LoadSong(myClip);
            NowPlaying = path;
            Play();
            SetPlaybackPosition(setPlaybackPosition);
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
        if (position < 0)
            position = 0;

        if (Instance.audioSource.clip.loadState != AudioDataLoadState.Loaded) {
            Instance.StartCoroutine(Instance.WaitForAudioToLoad(() => SetPlaybackPosition(position)));
            return;
        }
        Instance.audioSource.timeSamples = (int)(position * Instance.audioSource.clip.frequency);
    }

    IEnumerator WaitForAudioToLoad(UnityAction callback){
        while (audioSource.clip.loadState != AudioDataLoadState.Loaded) {
            yield return null;
        }
        callback?.Invoke();
    }


    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }
}

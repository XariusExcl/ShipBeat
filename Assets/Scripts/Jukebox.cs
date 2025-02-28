// Plays music.

using UnityEngine;

public class Jukebox : MonoBehaviour
{
    public static Jukebox Instance;
    AudioSource audioSource;

    /// <summary> 
    /// Loads the song into the Jukebox (path is relative to Resources folder).
    /// </summary>
    public static void LoadSong(string path){
        if (path.EndsWith(".ogg") || path.EndsWith(".wav") || path.EndsWith(".mp3")){
            path = path.Substring(0, path.Length - 4);
        }
        AudioClip clip = Resources.Load<AudioClip>(path);
        Instance.audioSource.clip = clip;
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

    void Awake(){
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }
}

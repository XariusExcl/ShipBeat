using UnityEngine;
using UnityEngine.Playables;

public class MenuLoad : MonoBehaviour
{
    [SerializeField] PlayableDirector director;
    [SerializeField] AudioClip introMusic;
    [SerializeField] AudioClip introMusicLoop;

    void Start()
    {
        ShaderWarmUp.WarmupDone.AddListener(LoadingDone);
    }

    void LoadingDone()
    {
        director.Play();
        Jukebox.QueueSong(introMusic);
        Jukebox.QueueSong(introMusicLoop, true);
    }

    void OnDestroy()
    {
        ShaderWarmUp.WarmupDone.RemoveListener(LoadingDone);
    }
}

using UnityEngine;
using UnityEngine.Playables;

public class MenuLoad : MonoBehaviour
{
    [SerializeField] PlayableDirector director;
    [SerializeField] AudioClip introMusic;
    [SerializeField] AudioClip introMusicLoop;

    void Start()
    {
        PrewarmMaterialHelper.MaterialPrewarmDone.AddListener(LoadingDone);
    }

    void LoadingDone()
    {
        director.Play();
        Jukebox.QueueSong(introMusic);
        Jukebox.QueueSong(introMusicLoop, true);
    }

    void OnDestroy()
    {
        PrewarmMaterialHelper.MaterialPrewarmDone.RemoveListener(LoadingDone);
    }
}

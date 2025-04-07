// Logic for the song select screen

using UnityEngine;

public class SongSelect : MonoBehaviour
{
    [SerializeField] GameObject songSelectReadyMenu;

    void Start()
    {
        SongCaroussel.OnSongSelected.AddListener(OnSongSelected);
        SongCaroussel.OnCarousselUpdate.AddListener(OnCarousselUpdate);
    }

    void OnSongSelected()
    {
        // Show some kind of menu to give option to go back
        songSelectReadyMenu.SetActive(true);
    }

    public static void CreateSongLoader()
    {
        // Load the song by creating a SongLoader object
        GameObject songLoader = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);
        songLoader.name = "SongLoader";
        songLoader.AddComponent<SongLoader>();
        SongLoader songLoaderComponent = songLoader.GetComponent<SongLoader>();
        songLoaderComponent.Init(SongFolderReader.SongInfos[SongCaroussel.CurrentSongIndex]);
    }

    void OnCarousselUpdate()
    {
        // Stop preview of currently selected song, fadein new song
        SongInfo song = SongFolderReader.SongInfos[SongCaroussel.CurrentSongIndex];
        if (song.AudioFile == Jukebox.NowPlaying) return; // Don't reload the same song
        Jukebox.Stop();
        Jukebox.LoadSongAndPlay(song.AudioFile);
        Jukebox.SetPlaybackPosition(song.SongPreviewStart);
        Jukebox.SetLoop(true);
    }
}

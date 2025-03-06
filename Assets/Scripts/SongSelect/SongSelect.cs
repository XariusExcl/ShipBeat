using UnityEngine;

public class SongSelect : MonoBehaviour
{
    // Logic for the song select screen :
    // wait for SongCaroussel to call the OnSongSelected event
      // Show some kind of menu to give time to go back
    // Load the song by creating a SongLoader object and setting the FilePath to the selected song
    // Then, load the game scene
    
    // When OnCarousselUpdate is called, stop preview of currently selected song, fadein new song

    [SerializeField] GameObject songSelectReadyMenu;

    void Start()
    {
        SongCaroussel.OnSongSelected.AddListener(OnSongSelected);
        SongCaroussel.OnCarousselUpdate.AddListener(OnCarousselUpdate);
    }

    void OnSongSelected()
    {
        songSelectReadyMenu.SetActive(true);
    }

    public static void CreateSongLoader()
    {
        GameObject songLoader = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);
        songLoader.AddComponent<SongLoader>();
        SongLoader songLoaderComponent = songLoader.GetComponent<SongLoader>();
        songLoaderComponent.FilePath = SongFolderReader.Songs[SongCaroussel.CurrentSongIndex].ChartFile;
    }

    void OnCarousselUpdate()
    {
        // get the newly selected song
        SongInfo song = SongFolderReader.Songs[SongCaroussel.CurrentSongIndex];
        // stop preview of currently selected song
        Jukebox.Stop();
        // fadein new song
        Jukebox.LoadSong(song.AudioFile);
        Jukebox.Play();
    }
}

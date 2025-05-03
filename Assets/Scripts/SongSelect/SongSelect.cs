// Logic for the song select screen

using UnityEngine;

public class SongSelect : MonoBehaviour
{
    [SerializeField] GameObject songSelectReadyMenu;
    float playSongTimer;
    const float playSongDelay = .75f;
    SongInfo selectedSong;

    void Start()
    {
        SongCaroussel.OnSongSelected.AddListener(OnSongSelected);
        SongCaroussel.OnCarousselUpdate.AddListener(OnCarousselUpdate);
        Maestro.SongStarted = false;
        Maestro.SongEnded = false;
    }

    void Update()
    {
        if (playSongTimer > 0f) {
            playSongTimer -= Time.deltaTime;
            if (playSongTimer <= 0f)
                Jukebox.LoadSongAndPlay(selectedSong.AudioFile, selectedSong.SongPreviewStart);
        }
    }

    void OnSongSelected()
    {
        // Show some kind of menu to give option to go back
        songSelectReadyMenu.SetActive(true);
        SFXManager.PlaySelectSound();
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
        selectedSong = SongFolderReader.SongInfos[SongCaroussel.CurrentSongIndex];
        if (selectedSong.AudioFile == Jukebox.NowPlaying) return; // Don't reload the same song
        Jukebox.Stop();
        playSongTimer = playSongDelay;
    }
}

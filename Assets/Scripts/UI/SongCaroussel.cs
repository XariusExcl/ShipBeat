using UnityEngine;
using TMPro;

public class SongCaroussel : MonoBehaviour
{
    SongInfo[] songs;
    public GameObject songEntryPrefab;
    GameObject[] songEntries = new GameObject[9];
    int currentSongIndex = 0;

    void Start()
    {
        songs = SongFolderReader.ReadFolder("");
        foreach (SongInfo song in songs) {
            Debug.Log($"{song.Title} - {song.Artist} - {song.Creator} - {song.BPM} - {song.DifficultyName} - {song.DifficultyRating}");           
        }
        for (int i = 0; i < 9; i++) {
            songEntries[i] = Instantiate(songEntryPrefab, transform);
            SongEntryUI songEntry = songEntries[i].GetComponent<SongEntryUI>();
            songEntry.SetData(songs[i%songs.Length]);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            currentSongIndex++;
            UpdateCaroussel();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            currentSongIndex--;
            UpdateCaroussel();
        }
    }

    void UpdateCaroussel()
    {
        for (int i = 0; i < 9; i++) {
            GameObject songEntryGo = songEntries[i];
            SongEntryUI songEntry = songEntryGo.GetComponent<SongEntryUI>();
            songEntry.SetData(songs[(currentSongIndex + i)%songs.Length]);
        }
        // TODO : Update only one child per update, and call a move function to the SongEntryUI script.
    }
}
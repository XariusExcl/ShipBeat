using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class SongCaroussel : MonoBehaviour
{
    public GameObject songEntryPrefab;
    List<SongEntryUI> songEntries = new();
    public static int CurrentSongIndex { get; private set; } = 0;
    const int NumSongs = 7;

    // Events
    public static UnityEvent OnSongSelected = new UnityEvent();
    public static UnityEvent OnCarousselUpdate = new UnityEvent();


    int mathmod(int a, int b) {
        return (a % b + b) % b;
    }

    void Start()
    {
        foreach (SongInfo song in SongFolderReader.Songs) {
            Debug.Log($"{song.Title} - {song.Artist} - {song.Creator} - {song.BPM} - {song.DifficultyName} - {song.DifficultyRating}");           
        }
        for (int i = 0; i < NumSongs; i++) {
            GameObject songEntry = Instantiate(songEntryPrefab, transform);

            songEntry.transform.localPosition = new Vector3(0, (32*(NumSongs/2))-i*32, 0);

            SongEntryUI songEntryUI = songEntry.GetComponent<SongEntryUI>();
            songEntryUI.SetData(SongFolderReader.Songs[mathmod(i - NumSongs / 2, SongFolderReader.Songs.Length)]);
            songEntries.Add(songEntryUI);
        }

        UpdateCaroussel(ScrollDirection.None);
    }

    float lastHorizontal = 0;
    enum ScrollDirection
    {
        None,
        Up,
        Down
    }
    void Update()
    {
        if (SongSelectReadyMenu.IsShown) return;

        // TODO: Implement a key repeat when axis is held down after a certain time.
        if (lastHorizontal != Input.GetAxis("P1_Vertical"))
        {
            lastHorizontal = Input.GetAxis("P1_Vertical");
            if (lastHorizontal > .5) 
                UpdateCaroussel(ScrollDirection.Up);
    
            else if (lastHorizontal < -.5) 
                UpdateCaroussel(ScrollDirection.Down);
        }

        if (Input.GetButtonDown("P1_B1"))
        {
            Debug.Log("Selected song : " + SongFolderReader.Songs[CurrentSongIndex].Title + " - " + SongFolderReader.Songs[CurrentSongIndex].DifficultyName);
            OnSongSelected.Invoke();
        }
    }

    void UpdateCaroussel(ScrollDirection direction)
    {
        Debug.Log("Updating caroussel" + direction.ToString());
        if (direction == ScrollDirection.Up) {
            CurrentSongIndex = mathmod(CurrentSongIndex - 1, SongFolderReader.Songs.Length);
        }
        else if (direction == ScrollDirection.Down) {
            CurrentSongIndex = mathmod(CurrentSongIndex + 1, SongFolderReader.Songs.Length);
        }

        // TODO: only update one of them, move the others (for animation)
        for (int i = 0; i < NumSongs; i++) {
            songEntries[i].SetData(SongFolderReader.Songs[mathmod(CurrentSongIndex + i - NumSongs / 2, SongFolderReader.Songs.Length)]);
        }
        OnCarousselUpdate.Invoke();
    }
}
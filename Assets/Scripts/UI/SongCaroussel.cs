// Right now, it's kind of the "main" element of the Songselect screen. TODO, make a Manager?

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
        if (!SongFolderReader.IsDataLoaded)
            SongFolderReader.OnDataLoaded.AddListener(OnSongDataLoaded);
        else
            OnSongDataLoaded();
    }

    void OnSongDataLoaded()
    {
        for (int i = 0; i < NumSongs; i++) {
            GameObject songEntry = Instantiate(songEntryPrefab, transform);

            songEntry.transform.localPosition = new Vector3(0, (32*(NumSongs/2))-i*32, 0);

            SongEntryUI songEntryUI = songEntry.GetComponent<SongEntryUI>();
            songEntryUI.SetData(SongFolderReader.SongInfos[mathmod(i - NumSongs / 2, SongFolderReader.SongInfos.Count)]);
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
        if (SongSelectReadyMenu.IsShown || !SongFolderReader.IsDataLoaded) return;

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
            OnSongSelected.Invoke();
        }
    }

    void UpdateCaroussel(ScrollDirection direction)
    {
        if (direction == ScrollDirection.Up) {
            CurrentSongIndex = mathmod(CurrentSongIndex - 1, SongFolderReader.SongInfos.Count);
        }
        else if (direction == ScrollDirection.Down) {
            CurrentSongIndex = mathmod(CurrentSongIndex + 1, SongFolderReader.SongInfos.Count);
        }

        // TODO: only update one of them, move the others (for animation)
        for (int i = 0; i < NumSongs; i++) {
            songEntries[i].SetData(SongFolderReader.SongInfos[mathmod(CurrentSongIndex + i - NumSongs / 2, SongFolderReader.SongInfos.Count)]);
        }
        OnCarousselUpdate.Invoke();
    }
}
// Right now, it's kind of the "main" element of the Songselect screen. TODO, make a Manager?

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class SongCaroussel : MonoBehaviour
{
    public GameObject loadingThrobber;
    public GameObject songEntryPrefab;
    readonly List<SongEntryUI> songEntries = new();
    public static int CurrentSongIndex { get; private set; } = 0;
    const int NumUIs = 9;
    static int scrollPosition; 

    // Events
    public static UnityEvent OnSongSelected = new();
    public static UnityEvent OnCarousselUpdate = new();


    int Mathmod(int a, int b) {
        return (a % b + b) % b;
    }

    void Start()
    {
        loadingThrobber.SetActive(true);
        if (!SongFolderReader.IsDataLoaded)
            SongFolderReader.OnDataLoaded.AddListener(OnSongDataLoaded);
        else
            OnSongDataLoaded();
    }

    void OnSongDataLoaded()
    {
        loadingThrobber.SetActive(false);
        for (int i = 0; i < NumUIs; i++) {
            SongEntryUI songEntryUI = Instantiate(songEntryPrefab, transform).GetComponent<SongEntryUI>();
            songEntryUI.UpdatePositionInCaroussel(NumUIs/2 - i);
            songEntryUI.SetData(SongFolderReader.SongInfos[Mathmod(scrollPosition + i - NumUIs/2, SongFolderReader.Count)]);
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
        // Caroussel scroll is cursor relative: Down means select the song below current, up means above.
        const int edge = NumUIs / 2 + 1;

        if (direction == ScrollDirection.Up) {
            int updatedEntryIdx = Mathmod(-scrollPosition, NumUIs) + 1;
            songEntries[^updatedEntryIdx].SetData(SongFolderReader.SongInfos[Mathmod(scrollPosition - edge, SongFolderReader.Count)]);
            scrollPosition -= 1;
            songEntries.ForEach((entry) => entry.UpdatePositionInCaroussel(-1));
            CurrentSongIndex = Mathmod(CurrentSongIndex - 1, SongFolderReader.Count);
        }
        else if (direction == ScrollDirection.Down) {
            int updatedEntryIdx = Mathmod(scrollPosition, NumUIs);
            songEntries[updatedEntryIdx].SetData(SongFolderReader.SongInfos[Mathmod(scrollPosition + edge, SongFolderReader.Count)]);
            scrollPosition += 1;
            songEntries.ForEach((entry) => entry.UpdatePositionInCaroussel(1));
            CurrentSongIndex = Mathmod(CurrentSongIndex + 1, SongFolderReader.Count);
        }

        OnCarousselUpdate.Invoke();
    }
}
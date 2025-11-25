using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class SongCaroussel : MonoBehaviour
{
    [SerializeField] GameObject loadingThrobber;
    [SerializeField] GameObject songEntryPrefab;
    readonly List<SongEntryUI> songEntries = new();
    public static int CurrentSongIndex { get; private set; } = 0;
    public const int NumUIs = 11;
    static int scrollPosition = 0;
    int scrollOffset = 0;
    public bool HasFocus;

    // Events
    public static UnityEvent OnSongSelected = new();
    public static UnityEvent OnCarousselUpdate = new();

    int Mathmod(int a, int b)
    {
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
        scrollOffset = scrollPosition;
        for (int i = 0; i < NumUIs; i++)
        {
            SongEntryUI songEntryUI = Instantiate(songEntryPrefab, transform).GetComponent<SongEntryUI>();
            songEntryUI.UpdatePositionInCaroussel(NumUIs / 2 - i);
            songEntryUI.SetData(SongFolderReader.SongInfos[Mathmod(scrollPosition + i - NumUIs / 2, SongFolderReader.Count)]);
            songEntries.Add(songEntryUI);
        }

        UpdateCaroussel(ScrollDirection.None);
    }

    float lastVertical = 0;
    enum ScrollDirection
    {
        None,
        Up,
        Down
    }
    void Update()
    {
        if (!HasFocus || SongSelectReadyMenu.IsValidated || !SongFolderReader.IsDataLoaded) return;

        if (lastVertical != Input.GetAxis("P1_Vertical"))
        {
            lastVertical = Input.GetAxis("P1_Vertical");
            if (lastVertical > .5)
                UpdateCaroussel(ScrollDirection.Up);

            else if (lastVertical < -.5)
                UpdateCaroussel(ScrollDirection.Down);
        }

        if (Input.GetButtonDown("P1_B1") && !SongSelectReadyMenu.IsValidated)
        {
            OnSongSelected.Invoke();
        }
    }

    void UpdateCaroussel(ScrollDirection direction)
    {
        SFXManager.PlayScrollSound();
        // Caroussel scroll is cursor relative: Down means select the song below current, up means above.
        const int edge = NumUIs / 2 + 1;

        if (direction == ScrollDirection.Up)
        {
            int updatedEntryIdx = Mathmod(-scrollPosition + scrollOffset, NumUIs) + 1;
            songEntries[^updatedEntryIdx].SetData(SongFolderReader.SongInfos[Mathmod(scrollPosition - edge, SongFolderReader.Count)]);
            scrollPosition -= 1;
            songEntries.ForEach((entry) => entry.UpdatePositionInCaroussel(-1));
            CurrentSongIndex = Mathmod(scrollPosition, SongFolderReader.Count);
        }
        else if (direction == ScrollDirection.Down)
        {
            int updatedEntryIdx = Mathmod(scrollPosition - scrollOffset, NumUIs);
            songEntries[updatedEntryIdx].SetData(SongFolderReader.SongInfos[Mathmod(scrollPosition + edge, SongFolderReader.Count)]);
            scrollPosition += 1;
            songEntries.ForEach((entry) => entry.UpdatePositionInCaroussel(1));
            CurrentSongIndex = Mathmod(scrollPosition, SongFolderReader.Count);
        }

        OnCarousselUpdate.Invoke();
    }
}
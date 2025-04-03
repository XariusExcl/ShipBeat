using UnityEngine;
using TMPro;
using System;

public class SongInfoPanelUI : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI artist;
    public TextMeshProUGUI creator;
    public TextMeshProUGUI difficultyRating;
    public TextMeshProUGUI length;
    public TextMeshProUGUI bpm;
    public TextMeshProUGUI noteCount;

    void Awake()
    {
        SongCaroussel.OnCarousselUpdate.AddListener(UpdateSongInfo);
    }

    void UpdateSongInfo()
    {
        SetData(SongFolderReader.SongInfos[SongCaroussel.CurrentSongIndex]);
    }

    public void SetData(SongInfo song)
    {
        title.text = song.Title;
        artist.text = song.Artist;
        creator.text = song.Creator;
        difficultyRating.text = song.DifficultyRating.ToString();
        length.text = TimeSpan.FromSeconds(song.Length).ToString(@"mm\:ss");
        bpm.text = song.BPM.ToString();
        noteCount.text = song.NoteCount.ToString();
    }
}
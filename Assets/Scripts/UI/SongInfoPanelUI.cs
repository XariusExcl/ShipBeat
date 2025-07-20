using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SongInfoPanelUI : MonoBehaviour
{
    [SerializeField] Image bg;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI artist;
    [SerializeField] TextMeshProUGUI creator;
    [SerializeField] TextMeshProUGUI difficultyRating;
    [SerializeField] TextMeshProUGUI length;
    [SerializeField] TextMeshProUGUI bpm;
    [SerializeField] TextMeshProUGUI noteCount;
    [SerializeField] SongSelectSceneData songSelectSceneData;

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
        Color diffColor = songSelectSceneData.GetColorForRatingUI(song.DifficultyRating);
        bg.color = new Color(diffColor.r, diffColor.g, diffColor.b, 0.5f);
        title.text = song.Title;
        artist.text = song.Artist;
        creator.text = song.Creator;
        difficultyRating.text = song.DifficultyRating.ToString();
        length.text = TimeSpan.FromSeconds(song.Length).ToString(@"mm\:ss");
        bpm.text = song.BPM.ToString();
        noteCount.text = song.NoteCount.ToString();
    }
}
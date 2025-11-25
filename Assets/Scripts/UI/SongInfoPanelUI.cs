using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Networking;

public class SongInfoPanelUI : MonoBehaviour
{
    Image border;
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
        border = GetComponent<Image>();
    }

    void UpdateSongInfo()
    {
        SetData(SongFolderReader.SongInfos[SongCaroussel.CurrentSongIndex]);
    }

    public void SetData(SongInfo song)
    {
        Color diffColor = songSelectSceneData.GetColorForRatingUI(song.DifficultyRating);
        border.color = new Color(diffColor.r, diffColor.g, diffColor.b, 0.75f);
        StartCoroutine(SongFolderReader.FetchImageFile(song.BackgroundImage, (result) =>
        {
            if (result.result == UnityWebRequest.Result.Success)
                bg.sprite = result.fetchedObject as Sprite;
        }));
        title.text = song.Title;
        artist.text = $"<i>de </i>{song.Artist}";
        creator.text = $"<i>beatmap par </i>{song.Creator}";
        difficultyRating.text = song.DifficultyRating.ToString();
        length.text = TimeSpan.FromSeconds(song.Length).ToString(@"mm\:ss");
        bpm.text = song.BPM.ToString();
        noteCount.text = song.NoteCount.ToString();
    }
}
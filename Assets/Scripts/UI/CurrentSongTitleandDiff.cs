using UnityEngine;
using TMPro;

public class CurrentSongTitleandDiff : MonoBehaviour
{
    [SerializeField] TMP_Text songName;
    [SerializeField] TMP_Text diffName;

    void Start()
    {
        if (songName is not null)
            if (SongFolderReader.SongInfos.Count == 0)
                songName.text = $"{SongLoader.LoadedSong.Info.Title}";
            else 
                songName.text = $"{SongFolderReader.SongInfos[SongCaroussel.CurrentSongIndex].Title}";
        if (diffName is not null)
            if (SongFolderReader.SongInfos.Count == 0)
                diffName.text = $"{SongLoader.LoadedSong.Info.DifficultyName} {GetRatingText(SongLoader.LoadedSong.Info.DifficultyRating)}";
            else
                diffName.text = $"{SongFolderReader.SongInfos[SongCaroussel.CurrentSongIndex].DifficultyName} {GetRatingText(SongFolderReader.SongInfos[SongCaroussel.CurrentSongIndex].DifficultyRating)}";
    }

    string GetRatingText(int rating)
    {
        if (rating < 4)
            return $"(Easy {rating})";
        else if (rating < 8)
            return $"(Medium {rating})";
        else if (rating < 12)
            return $"(Hard {rating})";
        else
            return $"(Expert {rating})";
    }
}
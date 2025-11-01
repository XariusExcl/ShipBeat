using UnityEngine;
using TMPro;

public class CurrentSongTitleandDiff : MonoBehaviour
{
    [SerializeField] TMP_Text songName;
    [SerializeField] TMP_Text diffName;

    void OnEnable()
    {
        if (songName is not null)
            songName.text = $"{SongFolderReader.SongInfos[SongCaroussel.CurrentSongIndex].Title}";
        if (diffName is not null)
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
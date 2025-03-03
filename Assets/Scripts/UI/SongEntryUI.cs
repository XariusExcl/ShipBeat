using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SongEntryUI : MonoBehaviour
{
    [SerializeField] SongInfo songInfo;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI artist;
    [SerializeField] TextMeshProUGUI difficultyName;
    [SerializeField] TextMeshProUGUI difficultyRating;
    [SerializeField] Image difficultyColorL;
    [SerializeField] Image difficultyColorR;


    public void SetData(SongInfo songInfo)
    {
        this.songInfo = songInfo;
        title.text = songInfo.Title;
        artist.text = songInfo.Artist;
        difficultyName.text = songInfo.DifficultyName;
        difficultyRating.text = songInfo.DifficultyRating.ToString();

        Color diffColor = GetColorForRating(songInfo.DifficultyRating);
        difficultyColorL.color = diffColor;
        difficultyColorR.color = diffColor;
    }

    // Map rating to color gradient, 1-10 going from blue to red hue
    Color GetColorForRating(int rating)
    {
        float hue = Mathf.Lerp(240, 0, rating / 10f);
        return Color.HSVToRGB(hue / 360f, 1, 1); 
    }
}
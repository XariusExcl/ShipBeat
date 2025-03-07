using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SongEntryUI : MonoBehaviour
{
    public int PositionInCaroussel { get; private set; }
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI artist;
    [SerializeField] TextMeshProUGUI difficultyName;
    [SerializeField] Image difficultyColor;
    [SerializeField] Image backgroundImage;


    public void SetData(SongInfo songInfo)
    {
        title.text = songInfo.Title;
        artist.text = songInfo.Artist;
        difficultyName.text = songInfo.DifficultyName + " " + GetRatingText(songInfo.DifficultyRating);

        Color diffColor = GetColorForRating(songInfo.DifficultyRating);
        difficultyColor.color = diffColor;
        backgroundImage.sprite = Resources.Load<Sprite>("Songs/" + songInfo.BackgroundImage);
    }

    public void UpdatePosition(Vector3 position)
    {
        transform.localPosition = position;
    
        // TODO: update position to an intermediate value, to be used in a lerp with transform.position in the Update method.
    }

    // Map rating to color gradient, 1-10 going from blue to red hue
    Color GetColorForRating(int rating)
    {
        float hue = Mathf.Lerp(240, 0, rating / 10f);
        return Color.HSVToRGB(hue / 360f, 1, 1); 
    }

    string GetRatingText(int rating)
    {
        // 1-4 easy, 5-7 medium, 8-10 hard
        if (rating < 5)
            return "(Easy "+rating+')';
        else if (rating < 8)
            return "(Medium "+rating+')';
        else
            return "(Hard "+rating+')';
    }
}
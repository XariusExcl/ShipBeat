using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class SongEntryUI : MonoBehaviour
{
    int positionInCaroussel;
    Vector3 targetPosition;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI artist;
    [SerializeField] TextMeshProUGUI difficultyName;
    [SerializeField] Image difficultyColor;
    [SerializeField] Image backgroundImage;
    [SerializeField] SongSelectSceneData songSelectSceneData;


    public void SetData(SongInfo songInfo)
    {
        title.text = songInfo.Title;
        artist.text = songInfo.Artist;
        difficultyName.text = songInfo.DifficultyName + " " + GetRatingText(songInfo.DifficultyRating);

        Color diffColor = songSelectSceneData.GetColorForRating(songInfo.DifficultyRating);
        difficultyColor.color = diffColor;
        StartCoroutine(SongFolderReader.FetchImageFile(songInfo.BackgroundImage, (result) =>
        {
            if (result.result == UnityWebRequest.Result.Success)
                backgroundImage.sprite = result.fetchedObject as Sprite;
        }));
    }

    /// <summary>
    /// Sets the position of the song entry in the caroussel.
    /// </summary>
    /// <param name="position">Position relative to last.</param>
    public void UpdatePositionInCaroussel(int position)
    {
        positionInCaroussel += position;
        if (Math.Abs(positionInCaroussel) > SongCaroussel.NumUIs / 2) {
            positionInCaroussel = Math.Sign(-positionInCaroussel) * (Math.Abs(positionInCaroussel) - 1);
            targetPosition = CarousselToScreenPosition(positionInCaroussel);
            transform.localPosition = targetPosition;
        } else {
            targetPosition = CarousselToScreenPosition(positionInCaroussel);
        }
    }

    Vector3 CarousselToScreenPosition(int position)
    {
        // return new Vector3(-50f / (Mathf.Abs(position) + 1f), 32f * position, 0);
        return new Vector3((position == 0) ? -30f : 9f * position, 30f * position + ((position != 0) ? Math.Sign(position) * 4 : 0) , 0);

    }

    void Start()
    {
        UpdatePositionInCaroussel(0);
    }

    void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, 10f * Time.deltaTime);
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
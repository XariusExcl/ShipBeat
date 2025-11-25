using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class CurrentSongImageUI : MonoBehaviour
{
    [SerializeField] Image backgroundImage;
    [SerializeField] Image difficultyColor;
    [SerializeField] SongSelectSceneData songSelectSceneData;

    void OnEnable()
    {
        if (SongCaroussel.CurrentSongIndex > SongFolderReader.SongInfos.Count)
            return;
            
        StartCoroutine(SongFolderReader.FetchImageFile(SongFolderReader.SongInfos[SongCaroussel.CurrentSongIndex].BackgroundImage, (result) =>
        {
            if (result.result == UnityWebRequest.Result.Success) {
                backgroundImage.sprite = result.fetchedObject as Sprite;
                difficultyColor.color = songSelectSceneData.GetColorForRating(SongFolderReader.SongInfos[SongCaroussel.CurrentSongIndex].DifficultyRating);
            }
        }));
    }
}
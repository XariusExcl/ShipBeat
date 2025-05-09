using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Playables;

public class SongSelectReadyMenu : MonoBehaviour
{
    [SerializeField] Image backgroundImage;
    [SerializeField] Image difficultyColor;
    [SerializeField] TMP_Text mapName;
    [SerializeField] PlayableDirector transitionDirector;
    public static bool IsShown = false;
    public static bool IsValidated = false;
    bool enableButtons = false;
    Animation animation;

    void Start()
    {
        animation = GetComponent<Animation>();
    }
    
    void OnEnable()
    {
        Invoke("EnableButtons", .5f);
        IsShown = true;
        animation = GetComponent<Animation>();
        animation.Play("SongSelectReadyMenuIn");
        mapName.text = $"{SongFolderReader.SongInfos[SongCaroussel.CurrentSongIndex].Title} - {SongFolderReader.SongInfos[SongCaroussel.CurrentSongIndex].DifficultyName}({SongFolderReader.SongInfos[SongCaroussel.CurrentSongIndex].DifficultyRating}★)";
        // Image should be in cache so it's fine
        StartCoroutine(SongFolderReader.FetchImageFile(SongFolderReader.SongInfos[SongCaroussel.CurrentSongIndex].BackgroundImage, (result) =>
        {
            if (result.result == UnityWebRequest.Result.Success) {
                backgroundImage.sprite = result.fetchedObject as Sprite;
                difficultyColor.color = GetColorForRating(SongFolderReader.SongInfos[SongCaroussel.CurrentSongIndex].DifficultyRating);
            }
        }));
    }

    void Update()
    {
        if (Input.GetButtonDown("P1_B2"))
        {
            PlayOutAnimation();
        }
        if (enableButtons && Input.GetButtonDown("P1_B1"))
        {
            IsValidated = true;
            SFXManager.PlaySelectSound();
            SongSelect.CreateSongLoader();
            // TODO : UI transition
            transitionDirector.Play();
        }
    }

    public void PlayOutAnimation()
    {
        animation.Play("SongSelectReadyMenuOut");
        enableButtons = false;
        SFXManager.PlayReturnSound();
    }

    // Used in animation
    public void Disable()
    {
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        IsShown = false;
    }

    void OnDestroy()
    {
        IsValidated = false;
    }

    void EnableButtons()
    {
        enableButtons = true;   
    }

    Color GetColorForRating(int rating)
    {
        float hue = Mathf.Lerp(210, 0, Mathf.Clamp01((rating-3)/6f));
        return Color.HSVToRGB(hue / 360f, .8f, .9f); 
    }
}

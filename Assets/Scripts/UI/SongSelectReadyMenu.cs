using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SongSelectReadyMenu : MonoBehaviour
{
    [SerializeField] Image thumbnail;
    [SerializeField] TMP_Text mapName;
    public static bool IsShown = false;
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
        // thumbnail.sprite = SongFolderReader.Songs[SongCaroussel.CurrentSongIndex].Background;
    }

    void Update()
    {
        if (Input.GetButtonDown("P1_B2"))
        {
            animation.Play("SongSelectReadyMenuOut");
            enableButtons = false;
        }
        if (enableButtons && Input.GetButtonDown("P1_B1"))
        {
            SongSelect.CreateSongLoader();
            // TODO : wait for song to load before going to game (trigger scene transition animation, then load scene)
            SceneManager.LoadScene("Game");
        }
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

    void EnableButtons()
    {
        enableButtons = true;   
    }
}

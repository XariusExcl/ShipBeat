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
        mapName.text = $"{SongFolderReader.Songs[SongCaroussel.CurrentSongIndex].Title} - {SongFolderReader.Songs[SongCaroussel.CurrentSongIndex].DifficultyName}({SongFolderReader.Songs[SongCaroussel.CurrentSongIndex].DifficultyRating}★)";
        // thumbnail.sprite = SongFolderReader.Songs[SongCaroussel.CurrentSongIndex].Background;
    }

    void Update()
    {
        if (!enableButtons) return;
        if (Input.GetButtonDown("P1_B1"))
        {
            SongSelect.CreateSongLoader();
            SceneManager.LoadScene("Game");
        }

        if (Input.GetButtonDown("P1_B2"))
        {
            animation.Play("SongSelectReadyMenuOut");
            enableButtons = false;
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

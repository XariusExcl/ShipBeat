using UnityEngine;
using UnityEngine.Playables;

public class SongSelectReadyMenu : MonoBehaviour
{
    [SerializeField] PlayableDirector transitionDirector;
    public static bool IsShown = false;
    public static bool IsValidated = false;
    bool enableButtons = false;
    public bool HasFocus = false;
    Animation animation;

    void Start()
    {
        animation = GetComponent<Animation>();
    }
    
    void OnEnable()
    {
        Invoke("EnableButtons", .5f);
        IsShown = true;
        // animation.Play("SongSelectReadyMenuIn");
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
            transitionDirector.Play();
        }
    }

    public void PlayOutAnimation()
    {
        animation.Play("SongSelectReadyMenuOut");
        enableButtons = false;
        HasFocus = false;
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
}

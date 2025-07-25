// A Game-wide Sound Effects Manager.

using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [SerializeField] AudioClip verticalBlipSound;
    [SerializeField][Range(0f, 1f)] float verticalBlipVolume = 1f;
    [SerializeField] AudioClip horizontalBlipSound;
    [SerializeField][Range(0f, 1f)] float horizontalBlipVolume = 1f;
    [SerializeField] AudioClip flickBlipSound;
    [SerializeField][Range(0f, 1f)] float flickBlipVolume = 1f;
    [SerializeField] AudioClip deepBlipUpSound;
    [SerializeField][Range(0f, 1f)] float deepBlipUpVolume = 1f;
    [SerializeField] AudioClip deepBlipDownSound;
    [SerializeField][Range(0f, 1f)] float deepBlipDownpVolume = 1f;
    [SerializeField] AudioClip scrollSound;
    [SerializeField][Range(0f, 1f)] float scrollVolume = 1f;
    [SerializeField] AudioClip selectSound;
    [SerializeField][Range(0f, 1f)] float selectVolume = 1f;
    [SerializeField] AudioClip returnSound;
    [SerializeField][Range(0f, 1f)] float returnVolume = 1f;
    [SerializeField] AudioClip optionChangeSound;
    [SerializeField][Range(0f, 1f)] float optionChangeVolume = 1f;
    [SerializeField] AudioClip noteHitSound;
    [SerializeField][Range(0f, 1f)] float noteHitVolume = 1f;
    [SerializeField] AudioClip comboBreakSound;
    [SerializeField][Range(0f, 1f)] float comboBreakVolume = 1f;
    AudioSource audioSource;

    public static SFXManager Instance;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Create static methods to play each sound
    public static void PlayScrollSound()
    {
        Instance.audioSource.PlayOneShot(Instance.scrollSound, Instance.scrollVolume);
    }
    public static void PlaySelectSound()
    {
        Instance.audioSource.PlayOneShot(Instance.selectSound, Instance.selectVolume);
    }
    public static void PlayReturnSound()
    {
        Instance.audioSource.PlayOneShot(Instance.returnSound, Instance.returnVolume);
    }
    public static void PlayOptionChangeSound()
    {
        Instance.audioSource.PlayOneShot(Instance.optionChangeSound, Instance.optionChangeVolume);
    }
    public static void PlayVerticalBlipSound()
    {
        Instance.audioSource.PlayOneShot(Instance.verticalBlipSound, Instance.verticalBlipVolume);
    }

    public static void PlayHorizontalBlipSound()
    {
        Instance.audioSource.PlayOneShot(Instance.horizontalBlipSound, Instance.horizontalBlipVolume);
    }

    public static void PlayFlickBlipSound()
    {
        Instance.audioSource.PlayOneShot(Instance.flickBlipSound, Instance.flickBlipVolume);
    }

    public static void PlayDeepBlipUpSound()
    {
        Instance.audioSource.PlayOneShot(Instance.deepBlipUpSound, Instance.deepBlipUpVolume);
    }

    public static void PlayDeepBlipDownSound()
    {
        Instance.audioSource.PlayOneShot(Instance.deepBlipDownSound, Instance.deepBlipDownpVolume);
    }

    public static void PlayNoteHitSound()
    {
        Instance.audioSource.PlayOneShot(Instance.noteHitSound, Instance.noteHitVolume);
    }

    public static void PlayComboBreakSound()
    {
        Instance.audioSource.PlayOneShot(Instance.comboBreakSound, Instance.comboBreakVolume);
    }
}

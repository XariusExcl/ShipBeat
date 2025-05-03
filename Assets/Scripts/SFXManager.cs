// A Game-wide Sound Effects Manager.

using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [SerializeField] AudioClip scrollSound;
    [SerializeField] [Range(0f, 1f)] float scrollSoundVolume = 1f;
    [SerializeField] AudioClip selectSound;
    [SerializeField] [Range(0f, 1f)] float selectSoundVolume = 1f;
    [SerializeField] AudioClip returnSound;
    [SerializeField] [Range(0f, 1f)] float returnSoundVolume = 1f;
    [SerializeField] AudioClip optionChangeSound;
    [SerializeField] [Range(0f, 1f)] float optionChangeSoundVolume = 1f;
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
        Instance.audioSource.PlayOneShot(Instance.scrollSound, Instance.scrollSoundVolume);
    }
    public static void PlaySelectSound()
    {
        Instance.audioSource.PlayOneShot(Instance.selectSound, Instance.selectSoundVolume);
    }
    public static void PlayReturnSound()
    {
        Instance.audioSource.PlayOneShot(Instance.returnSound, Instance.returnSoundVolume);
    }
    public static void PlayOptionChangeSound()
    {
        Instance.audioSource.PlayOneShot(Instance.optionChangeSound, Instance.optionChangeSoundVolume);
    }

}

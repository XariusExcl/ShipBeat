// Attached to an object which speaks in a textbox.

using System.Collections.Generic;
using UnityEngine;
public class YapperBehaviour : MonoBehaviour
{
    static List<YapperBehaviour> Yappers = new List<YapperBehaviour>();
    public string ID;
    public bool SupportsAnimation { get { return animator is not null || TryGetComponent(out Animator _); } }
    [SerializeField] AudioClip voice;
    [SerializeField] public Color Color;
    [Range(0f, 1f)][SerializeField] float volume = 1f;
    [SerializeField] LookTarget eyesTarget;
    [SerializeField] LookTarget bodyTarget;
    AudioSource audioSource;
    Animator animator;

    void OnEnable()
    {
        if (!Yappers.Contains(this))
            Yappers.Add(this);
        else
            Debug.LogWarning($"YapperBehaviour: Duplicate ID '{ID}' found on {gameObject.name}.", gameObject);
    }

    public static YapperBehaviour FindByID(string id)
    {
        foreach (YapperBehaviour yapper in Yappers)
        {
            if (yapper.ID == id)
                return yapper;
        }
        Debug.LogWarning($"YapperBehaviour: ID '{id}' not found");
        return null;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip = voice;
        audioSource.volume = volume;
        audioSource.playOnAwake = false;
    }

    public void UpdateBodyTarget(string targetName = "camera")
    {
        if (bodyTarget == null)
        {
            Debug.LogWarning($"YapperBehaviour: Body target not set on {gameObject.name}.", gameObject);
            return;
        }
        bodyTarget.Track(targetName);
    }

    public void UpdateEyesTarget(string targetName = "camera")
    {
        if (eyesTarget == null)
        {
            Debug.LogWarning($"YapperBehaviour: Head target not set on {gameObject.name}.", gameObject);
            return;
        }
        eyesTarget.Track(targetName);
    }

    public void Speak()
    {
        if (voice != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f); // Random pitch variation
            audioSource.PlayOneShot(voice, volume);
        }
    }

    public void SetEmote(string emote)
    {
        if (SupportsAnimation)
            animator.SetTrigger(emote);
        else Debug.LogWarning($"YapperBehaviour: Animator not found on {gameObject.name}. Cannot set emote '{emote}'.", gameObject);
    }

    void OnDestroy()
    {
        if (Yappers.Contains(this))
            Yappers.Remove(this);
        else Debug.LogWarning($"YapperBehaviour: ID '{ID}' not found in Yappers list on {gameObject.name}.", gameObject);
    }
}
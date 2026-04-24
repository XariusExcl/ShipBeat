using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TutoBotSFXManager : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip wowClip;
    [SerializeField] AudioClip sweatClip;
    [SerializeField] AudioClip surprisedClip;
    [SerializeField] AudioClip confusedClip;

    public void PlayWowSFX()
    {
        audioSource.Stop();
        audioSource.loop = false;
        audioSource.clip = wowClip;
        audioSource.Play();
    }

    public void PlaySweatClip()
    {
        audioSource.Stop();
        audioSource.loop = true;
        audioSource.clip = sweatClip;
        audioSource.Play();
    }

    public void PlaySurpisedSFX()
    {
        audioSource.Stop();
        audioSource.loop = false;
        audioSource.clip = surprisedClip;
        audioSource.Play();
    }

    public void PlayConfusedSFX()
    {
        audioSource.Stop();
        audioSource.loop = false;
        audioSource.clip = confusedClip;
        audioSource.Play();
    }

    public void StopSFX()
    {
        audioSource.Stop();
    }
}
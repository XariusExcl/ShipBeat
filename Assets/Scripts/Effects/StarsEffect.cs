using UnityEngine;

public class StarsEffect : MonoBehaviour
{
    [SerializeField] ParticleSystem starsParticleSystem;
    [SerializeField] ParticleSystem farStarsParticleSystem;
    [SerializeField] Gradient normalGradient;
    [SerializeField] float normalRate;
    [SerializeField] Gradient kiaiGradient;
    [SerializeField] float kiaiRate;
    

    void Start()
    {
        Maestro.OnKiaiStart.AddListener(SetKiaiStars);
        Maestro.OnKiaiEnd.AddListener(SetNormalStars);

        SetNormalStars();
    }

    void SetNormalStars()
    {
        if (starsParticleSystem != null) {
            var main = starsParticleSystem.main;
            main.startColor = normalGradient;
            var emission = starsParticleSystem.emission;
            emission.rateOverTime = normalRate;
        }

        if (farStarsParticleSystem != null) {
            var main = farStarsParticleSystem.main;
            main.startColor = normalGradient;
            var emission = farStarsParticleSystem.emission;
            emission.rateOverTime = normalRate;
        }
    }

    void SetKiaiStars()
    {
        if (starsParticleSystem != null) {
            var main = starsParticleSystem.main;
            main.startColor = kiaiGradient;
            var emission = starsParticleSystem.emission;
            emission.rateOverTime = kiaiRate;
        }

        if (farStarsParticleSystem != null) {
            var main = farStarsParticleSystem.main;
            main.startColor = kiaiGradient;
            var emission = farStarsParticleSystem.emission;
            emission.rateOverTime = kiaiRate;
        }
    }

    void OnDestroy()
    {
        Maestro.OnKiaiStart.RemoveListener(SetKiaiStars);
        Maestro.OnKiaiEnd.RemoveListener(SetNormalStars);
    }
}

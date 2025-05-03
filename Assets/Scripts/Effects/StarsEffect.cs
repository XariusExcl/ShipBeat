using UnityEngine;

public class StarsEffect : MonoBehaviour
{
    [SerializeField] ParticleSystem starsParticleSystem;
    [SerializeField] ParticleSystem farStarsParticleSystem;
    [SerializeField] Gradient normalGradient;
    [SerializeField] float normalRate;
    [SerializeField] Gradient kiaiGradient;
    [SerializeField] float kiaiRate;
    [SerializeField] float kiaiSpeedMultiplier;
    

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
            main.simulationSpeed = 1f;
        }

        if (farStarsParticleSystem != null) {
            var main = farStarsParticleSystem.main;
            main.startColor = normalGradient;
            var emission = farStarsParticleSystem.emission;
            emission.rateOverTime = normalRate;
            main.simulationSpeed = 1f;
        }
    }

    void SetKiaiStars()
    {
        if (starsParticleSystem != null) {
            var main = starsParticleSystem.main;
            main.startColor = kiaiGradient;
            var emission = starsParticleSystem.emission;
            emission.rateOverTime = kiaiRate;
            main.simulationSpeed = kiaiSpeedMultiplier;
        }

        if (farStarsParticleSystem != null) {
            var main = farStarsParticleSystem.main;
            main.startColor = kiaiGradient;
            var emission = farStarsParticleSystem.emission;
            emission.rateOverTime = kiaiRate;
            main.simulationSpeed = kiaiSpeedMultiplier;
        }
    }

    void OnDestroy()
    {
        Maestro.OnKiaiStart.RemoveListener(SetKiaiStars);
        Maestro.OnKiaiEnd.RemoveListener(SetNormalStars);
    }
}

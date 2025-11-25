using UnityEngine;

public class HoldEffectParticleSystems : MonoBehaviour
{
    [SerializeField] ParticleSystem particles1;
    [SerializeField] ParticleSystem particles2;

    ParticleSystem.EmissionModule particles1Emission;
    ParticleSystem.EmissionModule particles2Emission;

    ParticleSystem.MinMaxCurve particles1EmissionRate;
    ParticleSystem.MinMaxCurve particles2EmissionRate;

    void Awake()
    {
        particles1Emission = particles1.emission;
        particles1EmissionRate = particles1Emission.rateOverTime;

        particles2Emission = particles2.emission;
        particles2EmissionRate = particles2Emission.rateOverTime;

        Stop();
    }

    public void Emit()
    {
        particles1Emission.rateOverTime = particles1EmissionRate;
        particles2Emission.rateOverTime = particles2EmissionRate;
        particles1.Play();
        particles2.Play();
    }

    public void Stop()
    {
        particles1Emission.rateOverTime = 0;
        particles2Emission.rateOverTime = 0;
    }
}

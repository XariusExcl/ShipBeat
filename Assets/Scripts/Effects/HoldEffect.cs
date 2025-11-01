using UnityEngine;

public class HoldEffect : MonoBehaviour {
    MeshRenderer meshRenderer;
    [SerializeField] GameObject hitEffectPrefab;
    [SerializeField] HoldEffectParticleSystems holdEffectParticles;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
    }

    public void Show()
    {
        meshRenderer.enabled = true;
        if (holdEffectParticles != null)
            holdEffectParticles.Emit();
    }

    public void Hide()
    {
        meshRenderer.enabled = false;
        if (holdEffectParticles != null)
            holdEffectParticles.Stop();
    }

    public void Release()
    {
        meshRenderer.enabled = false;
        if (holdEffectParticles != null)
            holdEffectParticles.Stop();
        if (hitEffectPrefab != null)
            Instantiate(hitEffectPrefab, transform);
    }
}
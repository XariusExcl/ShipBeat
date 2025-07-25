using UnityEngine;

public class InteriorKiai : MonoBehaviour
{
    ShakeEffect shakeEffect;
    [SerializeField] float kiaiSpeed;
    float normalSpeed;
    [SerializeField] float kiaiDecisionTime;
    float normalDecisionTime;
    [SerializeField] float kiaiAmplitude;
    float normalAmplitude;

    void Start()
    {
        shakeEffect = GetComponent<ShakeEffect>();
        normalSpeed = shakeEffect.Speed;
        normalDecisionTime = shakeEffect.DecisionTime;
        normalAmplitude = shakeEffect.Amplitude;

        Maestro.OnKiaiStart.AddListener(SetKiaiShake);
        Maestro.OnKiaiEnd.AddListener(SetNormalShake);

        SetNormalShake();
    }

    void SetNormalShake()
    {
        shakeEffect.SetSpeed(normalSpeed);
        shakeEffect.SetDecisionTime(normalDecisionTime);
        shakeEffect.SetDistance(normalAmplitude);
    }

    void SetKiaiShake()
    {
        shakeEffect.SetSpeed(kiaiSpeed);
        shakeEffect.SetDecisionTime(kiaiDecisionTime);
        shakeEffect.SetDistance(kiaiAmplitude);
    }


    void OnDestroy()
    {
        Maestro.OnKiaiStart.RemoveListener(SetKiaiShake);
        Maestro.OnKiaiEnd.RemoveListener(SetNormalShake);
    }
}

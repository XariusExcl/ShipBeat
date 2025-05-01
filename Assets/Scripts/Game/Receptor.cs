using UnityEngine;
using System.Collections;

public class Receptor : MonoBehaviour {

    [SerializeField] MeshRenderer model;
    [SerializeField] HitEffect hitEffect;
    [SerializeField] HoldEffect holdEffect;

    Coroutine beamCoroutine;
    Color? baseColor;
    [SerializeField] Color highColor = Color.white;

    void Start()
    {
        baseColor = model?.material.color;
    }

    public void HandleInput(ButtonState buttonState)
    {
        if (buttonState == ButtonState.Down)
            BeamPress();
        else
            BeamRelease();
        
    }

    void BeamPress() {
        if (beamCoroutine != null)
            StopCoroutine(beamCoroutine);

        model.material.color = highColor;
    }

    void BeamRelease() {
        if (beamCoroutine != null)
            StopCoroutine(beamCoroutine);
        
        beamCoroutine = StartCoroutine(BeamReleaseCO());
    }

    public void SuccessfulHit(bool mirror = false) {
        hitEffect.Show(mirror);
    }

    public void SuccessfulHold() {
        holdEffect.Show();
    }

    public void SuccessfulRelease() {
        holdEffect.Hide();
    }

    IEnumerator BeamReleaseCO() {
        float time = 0;
        while (time < 0.1f) {
            time += Time.deltaTime;
            model.material.color = Color.Lerp(highColor, (Color)baseColor, time / 0.1f);
            yield return null;
        }
        model.material.color = (Color)baseColor;
    }
}
using UnityEngine;
using System.Collections;

public class Receptor : MonoBehaviour {
    [SerializeField] GameObject hitEffect;
    [SerializeField] GameObject holdEffect;

    Coroutine beamCoroutine;
    MeshRenderer meshRenderer;
    Color baseColor;
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        baseColor = meshRenderer.material.color;
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

        meshRenderer.material.color = Color.white;
    }

    void BeamRelease() {
        if (beamCoroutine != null)
            StopCoroutine(beamCoroutine);
        
        beamCoroutine = StartCoroutine(BeamReleaseCO());
    }

    public void SuccessfulHit() {
        Debug.Log("Hit!");
        hitEffect.SetActive(true);
    }

    public void SuccessfulHold() {
        holdEffect.SetActive(true);
    }

    public void SuccessfulRelease() {
        holdEffect.SetActive(false);
    }

    IEnumerator BeamReleaseCO() {
        float time = 0;
        while (time < 0.1f) {
            time += Time.deltaTime;
            meshRenderer.material.color = Color.Lerp(Color.white, baseColor, time / 0.1f);
            yield return null;
        }
        meshRenderer.material.color = baseColor;
    }
}
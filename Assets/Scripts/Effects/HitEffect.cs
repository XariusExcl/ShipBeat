using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class HitEffect : MonoBehaviour {
    [SerializeField] float duration = 0.5f;
    MeshRenderer meshRenderer;
    [SerializeField] GameObject hitEffectPrefab;

    void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
            meshRenderer.enabled = false;
    }

    public void Show(bool mirror = false)
    {
        if (hitEffectPrefab != null) {
            GameObject hitEffect = Instantiate(hitEffectPrefab, transform);
            if (mirror) {
                hitEffect.transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        if (meshRenderer != null) {
            meshRenderer.enabled = true;
            StartCoroutine(ShowCO());
        }
    }
    
    IEnumerator ShowCO() {
        yield return new WaitForSeconds(duration);
        meshRenderer.enabled = false;
    }
}
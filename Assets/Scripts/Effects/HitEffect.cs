using UnityEngine;
using System.Collections;

public class HitEffect : MonoBehaviour {
    [SerializeField] float duration = 0.5f;
    MeshRenderer meshRenderer;
    [SerializeField] GameObject perfectHitEffectPrefab;
    [SerializeField] GameObject goodHitEffectPrefab;
    [SerializeField] GameObject badHitEffectPrefab;

    void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
            meshRenderer.enabled = false;
    }

    public void Show(JudgeType judge, bool mirror = false)
    {
        GameObject hitEffect;
        if (goodHitEffectPrefab != null || badHitEffectPrefab != null) switch (judge)
        {
            case JudgeType.Perfect:
                hitEffect = Instantiate(perfectHitEffectPrefab, transform);
                if (mirror) hitEffect.transform.localScale = new Vector3(-1, 1, 1);
            break;
            case JudgeType.Great:
                hitEffect = Instantiate(goodHitEffectPrefab, transform);
                if (mirror) hitEffect.transform.localScale = new Vector3(-1, 1, 1);
            break;
            case JudgeType.Bad:
                hitEffect = Instantiate(badHitEffectPrefab, transform);
                if (mirror) hitEffect.transform.localScale = new Vector3(-1, 1, 1);
            break;
        } else {
            hitEffect = Instantiate(perfectHitEffectPrefab, transform);
            if (mirror) hitEffect.transform.localScale = new Vector3(-1, 1, 1);
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
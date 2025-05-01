using UnityEngine;
using System.Collections;

public class HitEffect : MonoBehaviour {
    [SerializeField] float duration = 0.5f;
    MeshRenderer meshRenderer;

    void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
    }

    public void Show()
    {
        meshRenderer.enabled = true;
        StartCoroutine(ShowCO());
    }
    
    IEnumerator ShowCO() {
        yield return new WaitForSeconds(duration);
        meshRenderer.enabled = false;
    }
}
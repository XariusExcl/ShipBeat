using UnityEngine;
using System.Collections;

public class HitEffect : MonoBehaviour {
    [SerializeField] float duration = 0.5f;
    MeshRenderer meshRenderer;

    void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        gameObject.SetActive(false);
    }

    public void Start()
    {
        StartCoroutine(ShowCO());
    }
    
    IEnumerator ShowCO() {
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
    }
}
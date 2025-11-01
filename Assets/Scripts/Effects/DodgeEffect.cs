using UnityEngine;

public class DodgeEffect : MonoBehaviour {

    public void DisableSelf() {
        gameObject.SetActive(false);
    }

    public void DestroySelf() {
        Destroy(gameObject);
    }
}
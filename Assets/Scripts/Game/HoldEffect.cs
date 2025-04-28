using UnityEngine;

public class HoldEffect : MonoBehaviour {

    public void Show() {
        gameObject.SetActive(true);
    }
    
    public void Hide() {
        gameObject.SetActive(false);
    }
}
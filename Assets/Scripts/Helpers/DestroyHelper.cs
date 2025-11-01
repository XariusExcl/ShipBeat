using UnityEngine;

public class DestroyHelper : MonoBehaviour
{
    // Only exposes Destroy() as a public function
    public void Destroy()
    {
        Object.Destroy(gameObject);
    }
}

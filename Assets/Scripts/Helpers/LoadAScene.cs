using UnityEngine;

public class LoadAScene : MonoBehaviour
{
    [SerializeField] private string sceneName = "GameScene"; // Default scene name
    [SerializeField] private bool loadOnAwake = false; // Load scene on start

    void Awake()
    {
        if (loadOnAwake)
        {
            LoadScene(sceneName);
        }
    }

    public void LoadScene(string sceneName)
    {
        Debug.Log($"Loading scene: {sceneName}");
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}

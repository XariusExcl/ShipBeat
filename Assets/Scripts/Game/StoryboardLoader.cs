using UnityEngine;

public class StoryboardLoader : MonoBehaviour
{
    [SerializeField] StoryboardList storyboardList;

    void Start()
    {
        if (storyboardList == null || storyboardList.storyboards == null || storyboardList.storyboards.Count == 0)
        {
            Debug.LogWarning("No storyboards available to load.");
            return;
        }

        // fing the storyboard that matches the song title
        foreach (KeyValue kv in storyboardList.storyboards)
        {
            if (kv.key.Equals(SongLoader.LoadedSong.Info.Title, System.StringComparison.OrdinalIgnoreCase))
            { 
                Debug.Log($"Loading storyboard for song: {SongLoader.LoadedSong.Info.Title}");
                GameObject storyboardPrefab = kv.value;
                if (storyboardPrefab != null)
                {
                    GameObject storyboardInstance = Instantiate(storyboardPrefab);
                    storyboardInstance.transform.SetParent(transform, false);
                    Debug.Log($"Storyboard loaded: {kv.key}");
                    return;
                }
                else
                {
                    Debug.LogWarning($"Storyboard prefab for {kv.key} is null.");
                }
            }
        }
        // If no storyboard,
        Maestro.StoryboardEnded = true;
    }
}
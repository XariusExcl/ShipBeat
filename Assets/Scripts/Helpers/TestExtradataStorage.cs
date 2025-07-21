using UnityEngine;
using UnityEditor;
using Anatidae;
using System.Collections;

public class TestExtradataStorage : MonoBehaviour
{
    [SerializeField] string key;
    [SerializeField] string value;
    [SerializeField] bool fetchOnAwake;

    void Awake()
    {
        if (fetchOnAwake)
            FetchData();
    }

    public void SetData()
    {
        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
        {
            Debug.LogWarning("Key or value is empty. Please set both before saving.");
            return;
        }

        StartCoroutine(ExtradataManager.SetExtraData(key, value));
        Debug.Log($"Set data: [{key}] = \"{value}\"");
    }

    public void FetchData()
    {
        StartCoroutine(FetchExtraDataCoroutine());
    }

    public IEnumerator FetchExtraDataCoroutine()
    {
        yield return ExtradataManager.FetchExtraData();
        if (ExtradataManager.HasFetchedExtraData)
        {
            if (ExtradataManager.ExtraData.Count == 0)
                Debug.Log("No extra data found.");

            foreach (var kvp in ExtradataManager.ExtraData)
                Debug.Log($"Fetched: [{kvp.key}] = \"{kvp.value}\"");

        }
        else
            Debug.LogWarning("Extradata unavailable (server is offline).");
    }

    public void GetDataWithKey()
    {
        if (ExtradataManager.HasFetchedExtraData)
        {
            var element = ExtradataManager.ExtraData.Find(e => e.key == key);
            if (element.key != null)
                Debug.Log($"Data for key [{key}]: \"{element.value}\"");
            else
                Debug.LogWarning($"No data found for key [{key}].");
        }
        else
        {
            Debug.LogWarning("Extra data has not been fetched yet.");
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TestExtradataStorage))]
public class TestExtradataStorageEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TestExtradataStorage script = (TestExtradataStorage)target;
        if (GUILayout.Button("Set Data"))
        {
            script.SetData();
        }
        if (GUILayout.Button("Fetch Data"))
        {
            script.FetchData();
        }
        if (GUILayout.Button("Get Data With Key"))
        {
            script.GetDataWithKey();
        }
    }
}
#endif

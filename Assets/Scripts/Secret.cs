using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
public class Secret
{
    public static string Key = "";
    static bool hasFetchedKey = false;
    public static IEnumerator GetKey(UnityAction<string> callback)
    {
        if (hasFetchedKey)
        {
            callback.Invoke(Key);   
            yield return null;
        }

        # if UNITY_EDITOR
            string secretFilePath = Application.streamingAssetsPath + "/secret";
            Key = File.ReadAllText(secretFilePath).Trim();
        # else
            UnityWebRequest request = UnityWebRequest.Get("http://localhost:3000/ShipBeat/StreamingAssets/secret");
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error reading folder: {request.error}");
                Key = "";
            }
            Key = request.downloadHandler.text.Trim();
        # endif
        
        hasFetchedKey = true;
        callback.Invoke(Key);
    }
}
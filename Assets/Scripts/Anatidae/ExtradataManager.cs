using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

namespace Anatidae {

    [Serializable]
    public class ExtraData
    {
        public List<ExtraDataElement> extradata;
    }

    [Serializable]
    public struct ExtraDataElement
    {
        public string key;
        public string value;
    }
    public class ExtradataManager : MonoBehaviour
    {

        public static List<ExtraDataElement> ExtraData { get; private set; } = new List<ExtraDataElement>();
        public static bool HasFetchedExtraData { get; private set; }

        public static IEnumerator FetchExtraData()
        {
            UnityWebRequest request = UnityWebRequest.Get("http://localhost:3000/api/extradata?game=" + HighscoreManager.GameName);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                string data = request.downloadHandler.text;
                try
                {
                    ExtraData extra = JsonUtility.FromJson<ExtraData>(data);
                    ExtraData = extra.extradata ?? new List<ExtraDataElement>();
                    HasFetchedExtraData = true;
                }
                catch (Exception e)
                {
                    Debug.LogError("ExtradataManager: Error parsing extra data: " + e.Message);
                }
            }
        }

        public static IEnumerator SetExtraData(string key, string value)
        {
            ExtraDataElement test = new ExtraDataElement { key = key, value = value };
            Debug.Log($"test {test.key} = {test.value}");
            Debug.Log($"{JsonUtility.ToJson(test)}");

            UnityWebRequest request = new UnityWebRequest("http://localhost:3000/api/extradata?game=" + HighscoreManager.GameName)
            {
                method = UnityWebRequest.kHttpVerbPOST,
                uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(JsonUtility.ToJson(new ExtraDataElement { key = key, value = value })))
                {
                    contentType = "application/json"
                }
            };

            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                Debug.LogError(request.error);

            yield return FetchExtraData();
        }

        public static string GetDataWithKey(string key)
        {
            if (!HasFetchedExtraData)
            {
                Debug.LogWarning("Extradata not fetched yet. Please call FetchExtraData() first.");
                return null;
            }

            foreach (var element in ExtraData)
            {
                if (element.key == key)
                {
                    return element.value;
                }
            }

            Debug.LogWarning($"No extra data found for key: {key}");
            return null;
        }
    }
}


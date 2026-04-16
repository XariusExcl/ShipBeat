using UnityEngine;
using Anatidae;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.Events;
public struct OnlineData
{
    Dictionary<string, int> SongOnlineIDs;
    Dictionary<int, List<BeatmapHighscore>> SongHighscores;
    Dictionary<int, BeatmapHighscore> SongPersonalHighscore;
}

public class OnlineDataManager
{
    # if UNITY_EDITOR
    public const string API_ENDPOINT = "http://192.168.137.110:3443/";
    # else
    public const string API_ENDPOINT = "https://89.234.181.104/shipbeat-api/";
    # endif
    public static bool Online = false;
    public static OnlineData Data = new OnlineData();

    public static IEnumerator OnlineCheck(UnityAction<bool> callback)
    {
        UnityWebRequest request = AnatidaeProxyWebRequest.Get(API_ENDPOINT);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
            Debug.LogError("Server offline"); // TODO, reroute all requests to Extradata manager
        }
        else
        {
            Debug.Log($"Connection established with {new Uri(API_ENDPOINT).Host}!");
            Online = true;
            Secret.GetKey((key) => {});
        }
        callback.Invoke(Online);
    }

    [Serializable] struct MapDataPayload
    {
        public string secret;
        public List<SongDataInfo> songs;
    }
    public static IEnumerator SendMapDataToServer(List<SongDataInfo> songDataInfos)
    {
        // Send currently installed maps from the machine to the server through POST /songs. Get the online IDs back
        yield return Secret.GetKey((key) => {});
        MapDataPayload payload = new()
        {
            secret = Secret.Key,
            songs = songDataInfos
        };
        string bodyJson = JsonUtility.ToJson(payload);

        // Use UnityWebRequest with method POST and set the uploadHandler and content-type manually
        UnityWebRequest request = new UnityWebRequest("http://localhost:3000/proxy?url=" + API_ENDPOINT + "songs/", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(bodyJson);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError) {
            Debug.LogError($"{request.result} {request.error}");
        } else {
            Debug.Log(request.downloadHandler.text);
        }

        yield return null;
    }

    public static IEnumerator GetPlayer(string name, UnityAction callback)
    {
        // TODO: Get player data from GET /players/:name
        yield return null;
    }

    public static IEnumerator CreatePlayer(string name, UnityAction callback)
    {
        // TODO: Create a player with the name and get its data back.
        yield return null;
    }

    public static IEnumerator SendPlay(int id, UnityAction callback)
    {
        // TODO: Send a play notification to /songs/play/:id
        yield return null;
    }

    public static IEnumerator SendScore(int id, BeatmapHighscore highScore, UnityAction callback)
    {
        // TODO: Send the score to POST /songs/clear/:id
        yield return null;
    }

    public static IEnumerator GetHighscores(int id, UnityAction callback)
    {
        //TODO: Get the top 20 highscores for a song at [endpoint missing]
        yield return null; 
    }

    public static IEnumerator GetPersonalScores(int id, UnityAction callback)
    {
        //TODO: Get personal scores for every song
        yield return null; 
    }
}
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
    const string API_ENDPOINT = "http://localhost:3443/";
    # else
    const string API_ENDPOINT = "https://89.234.181.104/shipbeat-api/";
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
        }
        callback.Invoke(Online);
    }

    public static IEnumerator SendMapDataToServer()
    {
        // TODO: Send currently installed maps from the machine to the server through POST /songs. Get the online IDs back 
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
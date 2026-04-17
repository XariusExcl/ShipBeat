using UnityEngine;
using Anatidae;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

public struct OnlineData
{
    public PlayerInfo PlayerInfo;
    public Dictionary<string, int> SongOnlineIDs;
    public Dictionary<int, List<BeatmapHighscore>> SongHighscores;
    public Dictionary<int, BeatmapHighscore> SongPersonalHighscore;
}

public struct PlayerInfo
{
    public int ID;
    public string Name;
    public int TotalScore;
}

public class OnlineDataManager
{
    # if UNITY_EDITOR
    public const string API_ENDPOINT = "http://192.168.137.110:3443/";
    # else
    public const string API_ENDPOINT = "https://89.234.181.104/shipbeat-api/";
    # endif
    public static bool Online = true; // CHANGE ME AAAAAAAAA
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

    [Serializable] struct MapDataPayload
    {
        public List<SongDataInfo> songs;
    }

    [Serializable] struct MapDataResponseSong
    {
        public int id;
        public string title;
        public string artist;
        public string creator;
        public string bpm;
        public float length;
        public string difficultyName;
        public string difficultyRating;
        public int noteCount;
        public int playCount;
        public int clearCount;
        public string createdAt;
        public string updatedAt;
    }
    [Serializable] struct MapDataResponse
    {
        public List<MapDataResponseSong> songs;
    }
    public static IEnumerator SendMapDataToServer(List<SongDataInfo> songDataInfos)
    {
        // Send currently installed maps from the machine to the server through POST /songs. Get the online IDs back
        MapDataPayload payload = new()
        {
            songs = songDataInfos
        };
        string bodyJson = JsonUtility.ToJson(payload);

        UnityWebRequest request = AnatidaeProxyWebRequest.Post(API_ENDPOINT + "songs/", bodyJson, "application/json");
        request.SetRequestHeader("Authorization", $"Basic {Secret.Key}");
        
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError) {
            Debug.LogError($"{request.result} {request.error}");
        } else {
            string responseJson = request.downloadHandler.text;
            MapDataResponse response = JsonUtility.FromJson<MapDataResponse>(responseJson);
            Data.SongOnlineIDs = new Dictionary<string, int>();
            foreach (MapDataResponseSong song in response.songs)
            {
                string key = $"{song.title}_{song.difficultyName}";
                Data.SongOnlineIDs[key] = song.id;;
            }
        }

        yield return null;
    }

    public static IEnumerator GetPlayer(string name, UnityAction callback)
    {
        // Get player data from GET /players/:name
        UnityWebRequest request = AnatidaeProxyWebRequest.Get(API_ENDPOINT + "players/" + name);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"{request.result} {request.error}");
        }
        else
        {
            Debug.Log(request.downloadHandler.text);
            PlayerInfo playerInfo = JsonUtility.FromJson<PlayerInfo>(request.downloadHandler.text);
            Data.PlayerInfo = playerInfo;
            Data.PlayerInfo.Name = name;
            Debug.Log($"Player data retrieved: {playerInfo.Name}");
        }

        callback.Invoke();
        yield return null;
    }

    public static IEnumerator SendPlay(SongDataInfo songDataInfo)
    {
        string songKey = songDataInfo.Title + "_" + songDataInfo.DifficultyName;
        if (Data.SongOnlineIDs.ContainsKey(songKey))
            yield return SendPlay(Data.SongOnlineIDs[songKey]);
        else
        {
            Debug.LogError($"id of {songKey} cannot be found.");
            yield return null;
        }
    }

    public static IEnumerator SendPlay(int id)
    {
        // TODO: Send a play notification to /songs/play/:id
        UnityWebRequest request = AnatidaeProxyWebRequest.Post(API_ENDPOINT + "songs/play/" + id, "{}", "application/json");
        request.SetRequestHeader("Authorization", $"Basic {Secret.Key}");
        yield return request.SendWebRequest();
        yield return null;
    }

    public static IEnumerator SendScore(BeatmapHighscore highScore, UnityAction callback)
    {
        // TODO: Send the score to POST /songs/clear/:id
        yield return null;
    }

    public static IEnumerator GetHighscores(UnityAction callback)
    {
        //TODO: Get the top 20 highscores for a song at [endpoint missing]
        yield return null; 
    }

    public static IEnumerator GetPersonalScores(UnityAction callback)
    {
        //TODO: Get personal scores for every song
        yield return null; 
    }
}
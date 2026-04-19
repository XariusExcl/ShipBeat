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
    public long TotalScore;
    public int PlayCount;
}

public class OnlineDataManager
{
    # if UNITY_EDITOR
    public const string API_ENDPOINT = "http://localhost:3443/";
    public static bool Online = true;
    # else
    public const string API_ENDPOINT = "https://89.234.181.104/shipbeat-api/";
    public static bool Online = false; 
    # endif
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

    [SerializeField] struct PlayerInfoResponse
    {
        public int id;
        public string name;
        public long totalScore;
        public int playCount;
        public string createdAt;
        public string updatedAt;
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
            PlayerInfoResponse playerInfoResponse = JsonUtility.FromJson<PlayerInfoResponse>(request.downloadHandler.text);
            Data.PlayerInfo = new PlayerInfo
            {
                ID = playerInfoResponse.id,
                Name = playerInfoResponse.name,
                TotalScore = playerInfoResponse.totalScore,
                PlayCount = playerInfoResponse.playCount
            };
            Debug.Log($"Player data retrieved: {playerInfoResponse.name}");
        }

        callback.Invoke();
        yield return null;
    }

    static int songID = 0;
    public static IEnumerator SendPlay(SongDataInfo songDataInfo)
    {
        string songKey = songDataInfo.Title + "_" + songDataInfo.DifficultyName;
        if (Data.SongOnlineIDs.ContainsKey(songKey))
        {
            songID = Data.SongOnlineIDs[songKey];
            yield return SendPlay(songID);
        }
        else
        {
            Debug.LogError($"id of {songKey} cannot be found.");
            songID = -1;
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

    [Serializable] struct SendScoreResponse
    {
        public int totalScore;
    }
    public static IEnumerator SendScore(BeatmapHighscore highScore)
    {
        // Send the score to POST /songs/clear/:id
        if (songID == -1) yield break;
        string bodyJson = JsonUtility.ToJson(highScore);

        UnityWebRequest request = AnatidaeProxyWebRequest.Post(API_ENDPOINT + "songs/clear/" + songID, bodyJson, "application/json");
        request.SetRequestHeader("Authorization", $"Basic {Secret.Key}");
        
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError) {
            Debug.LogError($"{request.result} {request.error}");
        } else {
            string responseJson = request.downloadHandler.text;
            SendScoreResponse sendScoreResponse = JsonUtility.FromJson<SendScoreResponse>(responseJson);
            Data.PlayerInfo.TotalScore = sendScoreResponse.totalScore;
        }

        yield return null;
    }

    public static IEnumerator GetHighscores(int id, UnityAction callback)
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
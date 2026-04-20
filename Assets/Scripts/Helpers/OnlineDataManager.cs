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
    public Dictionary<int, BeatmapHighscore> SongPersonalHighscores;
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
    public static OnlineData Data = new()
    {
        PlayerInfo = new(),
        SongOnlineIDs = new(),
        SongHighscores = new(),
        SongPersonalHighscores = new()
    };

    public static IEnumerator OnlineCheck(UnityAction<bool> callback)
    {
        UnityWebRequest request = AnatidaeProxyWebRequest.Get(API_ENDPOINT);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
            Debug.LogError("Server offline");
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
                Data.SongOnlineIDs[key] = song.id;
            }
        }

        yield return null;
    }

    [Serializable] struct PlayerInfoResponse
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
            PlayerInfoResponse playerInfoResponse = JsonUtility.FromJson<PlayerInfoResponse>(request.downloadHandler.text);
            Data.PlayerInfo = new PlayerInfo
            {
                ID = playerInfoResponse.id,
                Name = playerInfoResponse.name,
                TotalScore = playerInfoResponse.totalScore,
                PlayCount = playerInfoResponse.playCount
            };
        }
        yield return GetPersonalScores(() => {});
        Debug.Log($"Player data retrieved: {Data.PlayerInfo.Name}, {Data.PlayerInfo.TotalScore}, {Data.PlayerInfo.PlayCount}");
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
        public bool isPersonalHighscore;
        public bool isCabHighscore;
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
            Scoring.IsPersonalHighscore = sendScoreResponse.isPersonalHighscore;
            Scoring.IsCabHighscore = sendScoreResponse.isCabHighscore;
        }

        yield return GetHighscores(songID, () => {}, true);
        yield return null;
    }

    [Serializable] public struct HighscoreResponse
    {
        public List<HighscoreResponseElement> scores;
    }
    [Serializable] public struct HighscoreResponseElementPlayer { public string name; };
    [Serializable] public struct HighscoreResponseElement
    {
        public int songId;
        public int playerId;
        public HighscoreResponseElementPlayer player;
        public string createdAt;
        public int score;
        public int bestCombo;
        public int perfects;
        public int goods;
        public int bads;
        public int misses;
        public float percentage;
        public char rank;
    }
    public static IEnumerator GetHighscores(int id, UnityAction callback, bool forceRefresh = false)
    {
        // Get the top 20 highscores for a song at /scores/song/:id
        if (Data.SongHighscores.ContainsKey(id) && !forceRefresh) callback.Invoke();
        else
        {            
            HighscoreResponse highscoreResponse = new();
            List<BeatmapHighscore> beatmapHighscores = new();

            UnityWebRequest request = AnatidaeProxyWebRequest.Get(API_ENDPOINT + "scores/song/" + id);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"{request.result} {request.error}");
            }
            else
            {
                highscoreResponse = JsonUtility.FromJson<HighscoreResponse>(request.downloadHandler.text);
                foreach(HighscoreResponseElement element in highscoreResponse.scores)
                {
                    beatmapHighscores.Add(new() 
                    {
                        PlayerID = element.playerId,
                        PlayerName = element.player.name,
                        Timestamp = element.createdAt,
                        Score = element.score,
                        MaxCombo = element.bestCombo,
                        Perfects = element.perfects,
                        Goods = element.goods,
                        Bads = element.bads,
                        Misses = element.misses,
                        Percentage = element.percentage,
                        Rank = element.rank
                    });
                }
            }

            Data.SongHighscores[id] = beatmapHighscores;
            callback.Invoke();
        }
        yield return null; 
    }

    public static IEnumerator GetPersonalScores(UnityAction callback)
    {
        // Get personal scores for every song at /scores/player/:id
        HighscoreResponse highscoreResponse = new();

        UnityWebRequest request = AnatidaeProxyWebRequest.Get(API_ENDPOINT + "scores/player/" + Data.PlayerInfo.ID);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"{request.result} {request.error}");
        }
        else
        {
            highscoreResponse = JsonUtility.FromJson<HighscoreResponse>(request.downloadHandler.text);
            foreach(HighscoreResponseElement element in highscoreResponse.scores)
            {
                Data.SongPersonalHighscores[element.songId] = new() 
                {
                    PlayerName = Data.PlayerInfo.Name,
                    Timestamp = element.createdAt,
                    Score = element.score,
                    MaxCombo = element.bestCombo,
                    Perfects = element.perfects,
                    Goods = element.goods,
                    Bads = element.bads,
                    Misses = element.misses,
                    Percentage = element.percentage,
                    Rank = element.rank
                };
            }
        }
        callback.Invoke();
        yield return null;
    }

    public static void ClearData()
    {
        Data = new()
        {
            PlayerInfo = new(),
            SongOnlineIDs = new(),
            SongHighscores = new(),
            SongPersonalHighscores = new()
        };
    }
}
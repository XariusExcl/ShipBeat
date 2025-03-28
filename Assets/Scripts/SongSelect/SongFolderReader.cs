using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using Unity.Serialization.Json;

public struct SongFolderFileSystemEntry
{
    public string name;
    public bool isDirectory;
    public string path;
    public List<SongFolderFileSystemEntry> children;
}

public class SongFolderReader : MonoBehaviour
{
    public static bool IsDataLoaded = false;
    bool dataLoadedNotified = false;
    public static UnityEvent OnDataLoaded = new UnityEvent();
    public static List<SongInfo> Songs = new List<SongInfo>();

    void Start()
    {
        StartCoroutine(ReadFolder());
    }

    void Update()
    {
        if (IsDataLoaded && !dataLoadedNotified)
        {
            dataLoadedNotified = true;
            OnDataLoaded.Invoke();
        }
    }

    IEnumerator ReadFolder()
    {
        UnityWebRequest request = UnityWebRequest.Get("http://localhost:3000/ShipBeat/StreamingAssets/");
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error reading folder: {request.error}");
            yield break;
        }

        List<SongFolderFileSystemEntry> songFolderSongs = new();
        try {
            songFolderSongs = JsonSerialization.FromJson<SongFolderFileSystemEntry>(request.downloadHandler.text).children.Find(x => x.name == "Songs").children;
        } catch (System.Exception) {
            Debug.LogError($"Song folder not found in StreamingAssets.");
            yield break;
        }

        foreach (SongFolderFileSystemEntry singleSongFolder in songFolderSongs)
        {
            if (!singleSongFolder.isDirectory) continue;
            foreach (SongFolderFileSystemEntry entry in singleSongFolder.children)
            {
                if (entry.isDirectory) continue;
                string fileName = entry.name;
                if (fileName.EndsWith(".osu"))
                {
                    UnityWebRequest fileRequest = UnityWebRequest.Get($"http://localhost:3000{entry.path}");
                    yield return fileRequest.SendWebRequest();
                    if (fileRequest.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError($"Error reading file {fileName}: {fileRequest.error}");
                        continue;
                    }
                    TextAsset file = new TextAsset(fileRequest.downloadHandler.text);
                    SongValidationResult result = OsuFileParser.ParseFile(file, true);
                    if (!result.Valid)
                    {
                        Debug.LogError($"Error parsing file {entry.name}: {result.Message}");
                        continue;
                    }
                    Songs.Add(result.Data.Info);
                }
                // order by ascending difficulty
                Songs.Sort((a, b) => a.DifficultyRating - b.DifficultyRating);
            }
        }
        IsDataLoaded = true;
    }
}

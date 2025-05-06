using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using Unity.Serialization.Json;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
# endif

public struct SongFolderFileSystemEntry
{
    public string name;
    public bool isDirectory;
    public string path;
    public List<SongFolderFileSystemEntry> children;
}

public struct FetchResult
{
    public UnityWebRequest.Result result;
    public object fetchedObject; // TextAsset, AudioClip or Sprite
}

public class SongFolderReader : MonoBehaviour
{
    public static bool IsDataLoaded = false;
    bool dataLoadedNotified = false;
    public static UnityEvent OnDataLoaded = new UnityEvent();
    public static UnityEvent OnDataFailed = new UnityEvent();
    public static List<SongInfo> SongInfos = new List<SongInfo>();

    // Caching
    const int maxCacheSize = 10;
    static Dictionary<string, Sprite> spriteCache = new();
    static Dictionary<string, AudioClip> audioCache = new();

    public static int Count { get { return SongInfos.Count; } private set{} }

    void Start() {
# if UNITY_EDITOR
        EditorApplication.playModeStateChanged += OnExitPlayMode;
# endif
        if (!IsDataLoaded)
            StartCoroutine(ReadFolder());
    }

    void Update() {
        if (IsDataLoaded && !dataLoadedNotified)
        {
            dataLoadedNotified = true;
            OnDataLoaded.Invoke();
        }
    }

    IEnumerator ReadFolder()
    {
        # if UNITY_EDITOR
            string path = Application.streamingAssetsPath + "/Songs/";
            string[] directories = System.IO.Directory.GetDirectories(path);
            List<SongFolderFileSystemEntry> songFolderSongs = new();
            foreach (string directory in directories)
            {
                string folderName = System.IO.Path.GetFileName(directory);
                SongFolderFileSystemEntry entry = new SongFolderFileSystemEntry
                {
                    name = folderName,
                    isDirectory = true,
                    path = directory,
                    children = new List<SongFolderFileSystemEntry>()
                };
                string[] files = System.IO.Directory.GetFiles(directory);
                foreach (string file in files)
                {
                string fileName = System.IO.Path.GetFileName(file);
                    entry.children.Add(new SongFolderFileSystemEntry
                    {
                        name = fileName,
                        isDirectory = false,
                        path = file
                    });
                }
                songFolderSongs.Add(entry);
            }
            yield return null;
        # else
            UnityWebRequest request = UnityWebRequest.Get("http://localhost:3000/ShipBeat/StreamingAssets/");
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error reading folder: {request.error}");
                OnDataFailed.Invoke();
                yield break;
            }

            List<SongFolderFileSystemEntry> songFolderSongs = new();
            try {
                songFolderSongs = JsonSerialization.FromJson<SongFolderFileSystemEntry>(request.downloadHandler.text).children.Find(x => x.name == "Songs").children;
            } catch (System.Exception) {
                Debug.LogError($"Song folder not found in StreamingAssets.");
                OnDataFailed.Invoke();
                yield break;
            }
        # endif

        foreach (SongFolderFileSystemEntry singleSongFolder in songFolderSongs)
        {
            string folderName = singleSongFolder.path;
            if (!singleSongFolder.isDirectory) continue;
            foreach (SongFolderFileSystemEntry entry in singleSongFolder.children)
            {
                if (entry.isDirectory) continue;
                string fileName = entry.name;
                if (fileName.EndsWith(".osu"))
                {
                    # if UNITY_EDITOR
                        TextAsset file = new TextAsset(System.IO.File.ReadAllText(entry.path));
                    # else
                        UnityWebRequest fileRequest = UnityWebRequest.Get($"http://localhost:3000{entry.path}");
                        yield return fileRequest.SendWebRequest();
                        if (fileRequest.result != UnityWebRequest.Result.Success) {
                            Debug.LogError($"Error reading file {fileName}: {fileRequest.error}");
                            continue;
                        }
                        TextAsset file = new TextAsset(fileRequest.downloadHandler.text);
                    # endif
                    SongValidationResult result = OsuFileParser.ParseFile(file, true);
                    if (!result.Valid) {
                        Debug.LogError($"Error parsing file {entry.name}: {result.Message}");
                        continue;
                    }
                    result.Data.Info.AudioFile = $"{folderName}/{result.Data.Info.AudioFile}";
                    result.Data.Info.ChartFile = entry.path;
                    result.Data.Info.BackgroundImage = $"{folderName}/{result.Data.Info.BackgroundImage}";
                    SongInfos.Add(result.Data.Info);
                }
            }
        }
        SongInfos.Sort((a, b) => a.DifficultyRating - b.DifficultyRating);
        IsDataLoaded = true;
    }

    public static IEnumerator FetchFile(string filePath, UnityAction<FetchResult> callback) {
        # if UNITY_EDITOR
            TextAsset file = new TextAsset(System.IO.File.ReadAllText(filePath));
            FetchResult result = new FetchResult
            {
                result = UnityWebRequest.Result.Success,
                fetchedObject = file
            };
            callback(result);
            yield break;
        # else 
            UnityWebRequest request = UnityWebRequest.Get($"http://localhost:3000{filePath}");
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success) {
                Debug.LogError($"Error loading song file: {request.error}");
            }
            
            FetchResult result = new FetchResult
            {
                result = request.result,
                fetchedObject = new TextAsset(request.downloadHandler.text)
            };
            callback(result);
        # endif
    }

    public static IEnumerator FetchImageFile(string filePath, UnityAction<FetchResult> callback) {
        if (spriteCache.ContainsKey(filePath)) {
            FetchResult result = new FetchResult
            {
                result = UnityWebRequest.Result.Success,
                fetchedObject = spriteCache[filePath]
            };
            callback(result);
            yield break;
        } else {
            Texture2D texture = new Texture2D(2, 2);
            # if UNITY_EDITOR
                byte[] fileData = System.IO.File.ReadAllBytes(filePath);
                texture.LoadImage(fileData);
                FetchResult result = new FetchResult
                {
                    result = UnityWebRequest.Result.Success,
                    fetchedObject = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f)
                };
            # else
                UnityWebRequest request = UnityWebRequestTexture.GetTexture($"http://localhost:3000{filePath}");
                yield return request.SendWebRequest();
                if (request.result != UnityWebRequest.Result.Success) {
                    Debug.LogError($"Error loading image file: {request.error}");
                }
                texture = DownloadHandlerTexture.GetContent(request);
                FetchResult result = new FetchResult
                {
                    result = request.result,
                    fetchedObject = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f)
                };
            # endif
            if (spriteCache.Count >= maxCacheSize)
                spriteCache.Remove(spriteCache.Keys.First());

            spriteCache[filePath] = result.fetchedObject as Sprite;
            callback(result);
            yield break;
        }
    }

    public static IEnumerator FetchAudioFile(string filePath, UnityAction<FetchResult> callback) {
        if (audioCache.ContainsKey(filePath)) {
            FetchResult result = new FetchResult
            {
                result = UnityWebRequest.Result.Success,
                fetchedObject = audioCache[filePath]
            };
            callback(result);
            yield break;
        } else {
            AudioType audioType = AudioType.UNKNOWN;
            if (filePath.EndsWith(".wav")) audioType = AudioType.WAV;
            else if (filePath.EndsWith(".mp3")) audioType = AudioType.MPEG;
            else if (filePath.EndsWith(".ogg")) audioType = AudioType.OGGVORBIS;
            else if (filePath.EndsWith(".aiff")) audioType = AudioType.AIFF;
            else if (filePath.EndsWith(".opus")) audioType = AudioType.OGGVORBIS;

            # if UNITY_EDITOR
                UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, audioType);
            # else
                UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip($"http://localhost:3000{filePath}", audioType);
            # endif
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success) {
                Debug.LogError($"Error loading audio file: {request.error}");
            }
            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(request);
            FetchResult result = new FetchResult
            {
                result = request.result,
                fetchedObject = audioClip,
            };
            if (audioCache.Count >= maxCacheSize)
                audioCache.Remove(audioCache.Keys.First());
            
            audioCache[filePath] = audioClip;
            callback(result);
        }
        yield break;
    }

#if UNITY_EDITOR
    void OnExitPlayMode(PlayModeStateChange state) {
        if (state == PlayModeStateChange.ExitingPlayMode) {
            ClearCache();
            EditorApplication.playModeStateChanged -= OnExitPlayMode;
        }
    }

    public static void ClearCache() {
        spriteCache.Clear();
        audioCache.Clear();
        IsDataLoaded = false;
        SongInfos.Clear();
    }
#endif
}

using System.Collections.Generic;
using UnityEngine;

public class SongFolderReader : MonoBehaviour
{
    public static SongInfo[] Songs { get {
        if (songs == null)
            songs = ReadFolder("Songs");
        return songs;
    } private set {} }

    static SongInfo[] songs;

    static SongInfo[] ReadFolder(string path)
    {
        TextAsset[] files = Resources.LoadAll<TextAsset>(path);
        List<SongInfo> songs = new();
        for (int i = 0; i < files.Length; i++)
        {
            SongValidationResult result = OsuFileParser.ParseFile(files[i]);
            if (!result.Valid)
            {
                Debug.LogError($"Error parsing file {files[i].name}: {result.Message}");
                continue;
            }
            songs.Add(result.Data.Info);
        }
        // order by ascending difficulty
        songs.Sort((a, b) => a.DifficultyRating - b.DifficultyRating);
        return songs.ToArray();
    }
}

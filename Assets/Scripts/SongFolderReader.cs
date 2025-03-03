using System.Collections.Generic;
using UnityEngine;

public class SongFolderReader : MonoBehaviour
{
    public static SongData[] ReadFolder(string path)
    {
        TextAsset[] files = Resources.LoadAll<TextAsset>(path);
        List<SongData> songs = new();
        for (int i = 0; i < files.Length; i++)
        {
            SongValidationResult result = OsuFileParser.ParseFile(files[i]);
            if (!result.Valid)
            {
                Debug.LogError($"Error parsing file {files[i].name}: {result.Message}");
                continue;
            }
            songs.Add(result.Data);
        }
        return songs.ToArray();
    }
}

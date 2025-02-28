using UnityEngine;

public class SongFolderReader : MonoBehaviour
{
    public static SongData[] ReadFolder(string path)
    {
        TextAsset[] files = Resources.LoadAll<TextAsset>(path);
        SongData[] songs = new SongData[files.Length];
        for (int i = 0; i < files.Length; i++)
        {
            songs[i] = OsuFileParser.ParseFile(files[i]).Data;
        }
        return songs;
    }
}

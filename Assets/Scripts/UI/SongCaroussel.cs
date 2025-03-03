using UnityEngine;
using TMPro;

public class SongCaroussel : MonoBehaviour
{
    public GameObject songPrefab;
    public Transform content;

    void Start()
    {
        SongData[] songs = SongFolderReader.ReadFolder("");
        foreach (SongData song in songs)
        {
            
            Debug.Log($"{song.Info.Title} - {song.Info.Artist} - {song.Info.Creator} - {song.Info.BPM} - {song.Info.DifficultyName} - {song.Info.DifficultyRating}");
            /*
            GameObject songObject = Instantiate(songPrefab, content);
            songObject.GetComponentInChildren<TextMeshProUGUI>().text = song.Info.Title;
            */
        }
    }
}
using System;
using System.Collections.Generic;
using Anatidae;
using UnityEngine;

[Serializable]
public struct BeatmapHighscore
{
    public string PlayerName;
    public string Timestamp;
    public int Score;
    public int Combo;
    public int MaxCombo;
    public int Perfects;
    public int Goods;
    public int Bads;
    public int Misses;
    public float Percentage;
    public char Rank;
}

public class BeatmapHighscores : MonoBehaviour
{
    [SerializeField] List<BeatmapHighscoreUI> highscoreUis;
    [SerializeField] BeatmapHighscoreUI selfHighscoreUi;
    void Awake()
    {
        SongCaroussel.OnCarousselUpdate.AddListener(UpdateHighscores);
    }

    void UpdateHighscores()
    {
        SongInfo info = SongFolderReader.SongInfos[SongCaroussel.CurrentSongIndex];
        
        string json = ExtradataManager.GetDataWithKey($"Scores/{info.Title}_{info.DifficultyName}");
        if (json is null)
        {
            HideAll();
            selfHighscoreUi.SetBlank();
            return;
        }
        else
        {
            List<BeatmapHighscore> highscores = JsonUtility.FromJson<HighscoreList>(json).list;
            for (int i = 0; i < highscoreUis.Count; i++)
            {
                if (i >= highscores.Count)
                {
                    highscoreUis[i].Hide();
                    continue;
                }
                highscoreUis[i].SetData(highscores[i]);
                highscoreUis[i].Show();
            }
            int idx = highscores.FindIndex(h => h.PlayerName == HighscoreManager.PlayerName);
            if (idx != -1)
                selfHighscoreUi.SetData(highscores[idx]);
            else
                selfHighscoreUi.SetBlank();
        }
    }

    void HideAll()
    {
        highscoreUis.ForEach(h => h.Hide());
    }
}

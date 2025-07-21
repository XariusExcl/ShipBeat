using System;
using System.Collections.Generic;
using Anatidae;
using UnityEngine;

[Serializable]
public struct HighscoreList
{
    public List<BeatmapHighscore> list;
}

public class Scoring
{
    public static int Score { get; private set; } = 0;
    public static int Combo { get; private set; } = 0;
    public static int MaxCombo { get; private set; } = 0;
    public static int Perfects { get; private set; } = 0;
    public static int Goods { get; private set; } = 0;
    public static int Bads { get; private set; } = 0;
    public static int Misses { get; private set; } = 0;
    public static float Percentage { get; private set; } = 0;
    public static char Rank { get { return GetRank(); } }
    public static void Reset()
    {
        Score = 0;
        Combo = 0;
        MaxCombo = 0;
        Perfects = 0;
        Goods = 0;
        Bads = 0;
        Misses = 0;
        Percentage = 100;
    }

    public static void AddScore(int score)
    {
        Score += score;
        GameUIManager.UpdateScore(Score);
        GameUIManager.UpdateCombo(Combo);
        CalculatePercentage();
        GameUIManager.UpdatePercentage(Percentage);
    }

    public static void AddPerfect()
    {
        Perfects++;
        Combo++;
        AddScore(100);
        if (Combo > MaxCombo) MaxCombo = Combo;
        GameUIManager.ShowTicker(TickerType.Perfect);
    }

    public static void AddGood()
    {
        Goods++;
        Combo++;
        AddScore(50);
        if (Combo > MaxCombo) MaxCombo = Combo;
        GameUIManager.ShowTicker(TickerType.Good);
    }

    public static void AddBad()
    {
        Bads++;
        AddScore(10);
        ResetCombo();
        GameUIManager.ShowTicker(TickerType.Bad);
    }

    public static void AddMiss()
    {
        Misses++;
        AddScore(0);
        ResetCombo();
        GameUIManager.ShowTicker(TickerType.Miss);
    }

    static void CalculatePercentage()
    {
        Percentage = (Perfects * 100f + Goods * 50f + Bads * 10f) / (Perfects + Goods + Bads + Misses);
    }

    static void ResetCombo()
    {
        Combo = 0;
        GameUIManager.ResetCombo();
    }

    static char GetRank()
    {
        if (Percentage == 100) return 'P';
        if (Percentage >= 95) return 'S';
        if (Percentage >= 90) return 'A';
        if (Percentage >= 80) return 'B';
        if (Percentage >= 70) return 'C';
        if (Percentage >= 60) return 'D';
        return 'F';
    }

    public static string SaveScore()
    {
        SongInfo info = SongFolderReader.SongInfos[SongCaroussel.CurrentSongIndex];
        HighscoreList highscores = new();
        highscores.list = new();
        string json = ExtradataManager.GetDataWithKey($"Scores/{info.Title}_{info.DifficultyName}");
        if (json is null)
        {
            highscores.list.Add(CreateHighscore());
        }
        else
        {
            highscores = JsonUtility.FromJson<HighscoreList>(json);
            if (highscores.list.Count == 0)
            {
                highscores.list.Add(CreateHighscore());
            }
            else
            {
                int index = highscores.list.FindIndex(h => h.PlayerName == HighscoreManager.PlayerName);
                if (index != -1)
                {
                    if (highscores.list[index].Percentage < Percentage)
                    {
                        BeatmapHighscore updatedHighscore = highscores.list[index];
                        updatedHighscore.Score = Score;
                        updatedHighscore.Combo = Combo;
                        updatedHighscore.MaxCombo = MaxCombo;
                        updatedHighscore.Timestamp = DateTime.Now.ToString("o");
                        updatedHighscore.Perfects = Perfects;
                        updatedHighscore.Goods = Goods;
                        updatedHighscore.Bads = Bads;
                        updatedHighscore.Misses = Misses;
                        updatedHighscore.Rank = Rank;

                        highscores.list[index] = updatedHighscore;
                    }
                    else
                    {
                        // Score not beaten
                    }
                }
                else
                {
                    // No scores for this player, create a new score
                    highscores.list.Add(CreateHighscore());
                }
            }
        }
        highscores.list.Sort((a, b) => b.Score - a.Score);
        return JsonUtility.ToJson(highscores);
    }

    static BeatmapHighscore CreateHighscore()
    {
        BeatmapHighscore highscore = new BeatmapHighscore
        {
            PlayerName = HighscoreManager.PlayerName ?? "GUE",
            Score = Score,
            Combo = Combo,
            MaxCombo = MaxCombo,
            Timestamp = DateTime.Now.ToString("o"),
            Percentage = Percentage,
            Perfects = Perfects,
            Goods = Goods,
            Bads = Bads,
            Misses = Misses,
            Rank = Rank
        };
        return highscore;
    }
}

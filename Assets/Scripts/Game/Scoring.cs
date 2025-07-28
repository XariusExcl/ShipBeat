using System;
using System.Collections.Generic;
using Anatidae;
using NUnit.Framework;
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
    public static int BestCombo { get; private set; } = 0;
    public static int Perfects { get; private set; } = 0;
    public static int Goods { get; private set; } = 0;
    public static int Bads { get; private set; } = 0;
    public static int Misses { get; private set; } = 0;
    public static float Percentage { get; private set; } = 0;
    public static char Rank { get { return GetRank(); } }
    public static bool IsPersonalHighscore;
    public static bool IsCabHighscore;
    public static void Reset()
    {
        Score = Combo = BestCombo = Perfects = Goods = Bads = Misses = 0;
        Percentage = 100;
        IsPersonalHighscore = IsCabHighscore = false;
    }

    public static void AddScore(int score)
    {
        Score += score;
        GameUIManager.UpdateCombo(Combo);
        CalculatePercentage();
    }

    public static void AddPerfect()
    {
        Perfects++;
        Combo++;
        AddScore(100);
        if (Combo > BestCombo) BestCombo = Combo;
        GameUIManager.ShowTicker(TickerType.Perfect);
    }

    public static void AddGood()
    {
        Goods++;
        Combo++;
        AddScore(50);
        if (Combo > BestCombo) BestCombo = Combo;
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
        if (Combo > 20) SFXManager.PlayComboBreakSound();
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
            highscores.list.Add(CreateHighscore());
        else
        {
            highscores = JsonUtility.FromJson<HighscoreList>(json);
            if (highscores.list.Count == 0)
                highscores.list.Add(CreateHighscore());
            else
            {
                int index = highscores.list.FindIndex(h => h.PlayerName == HighscoreManager.PlayerName);
                if (index != -1)
                    if (highscores.list[index].Percentage < Percentage) // Highscore is beaten
                    {
                        if (highscores.list[0].Percentage < Percentage)
                            IsCabHighscore = true;
                            
                        IsPersonalHighscore = true;
                        BeatmapHighscore updatedHighscore = highscores.list[index];
                        updatedHighscore.Score = Score;
                        updatedHighscore.Combo = Combo;
                        updatedHighscore.MaxCombo = BestCombo;
                        updatedHighscore.Timestamp = DateTime.Now.ToString("o");
                        updatedHighscore.Perfects = Perfects;
                        updatedHighscore.Goods = Goods;
                        updatedHighscore.Bads = Bads;
                        updatedHighscore.Misses = Misses;
                        updatedHighscore.Rank = Rank;

                        highscores.list[index] = updatedHighscore;
                    }
                    else
                        highscores.list.Add(CreateHighscore()); // No scores for this player, create a new score
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
            MaxCombo = BestCombo,
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

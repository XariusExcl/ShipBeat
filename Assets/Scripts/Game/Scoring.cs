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
    public static int TotalScore;

    public static void Init()
    {
        try
        {
            TotalScore = int.Parse(ExtradataManager.GetDataWithKey($"Player/{HighscoreManager.PlayerName}/TotalScore"));
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
            TotalScore = 0;
        }
    }

    public static void Reset()
    {
        Score = Combo = BestCombo = Perfects = Goods = Bads = Misses = 0;
        Percentage = 100;
        IsPersonalHighscore = IsCabHighscore = false;
    }

    public static void AddScore(JudgeType judge)
    {
        switch (judge)
        {
            case JudgeType.Perfect:
                Score += 100;
                Perfects++;
                Combo++;
                GameUIManager.UpdateCombo(Combo);
            break;
            case JudgeType.Great:
                Score += 50;
                Goods++;
                Combo++;
                GameUIManager.UpdateCombo(Combo);
            break;
            case JudgeType.Bad:
                Score += 10;
                Bads++;
                ResetCombo();
            break;
            case JudgeType.Miss:
                Misses++;
                ResetCombo();
            break;
        }

        if (Combo > BestCombo) BestCombo = Combo;
        Percentage = (Perfects * 100f + Goods * 50f + Bads * 10f) / (Perfects + Goods + Bads + Misses);
        GameUIManager.ShowTicker(judge);
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
        
        if (json is null) // Map has no score data
        {
            highscores.list.Add(CreateHighscore());
            TotalScore += Score;
        } 
        else // Map has score data, parse it
        {
            highscores = JsonUtility.FromJson<HighscoreList>(json);            
            if (highscores.list.Count == 0) // Data is fuxxed, no highscores
            {
                highscores.list.Add(CreateHighscore());
                TotalScore += Score;
            }
            else
            {
                if (highscores.list[0].Percentage < Percentage) // TODO: Why are we comparing percentages instead of scores?
                    IsCabHighscore = true;
                
                int index = highscores.list.FindIndex(h => h.PlayerName == HighscoreManager.PlayerName);
                if (index != -1) // If player has a score already saved
                {
                    if (highscores.list[index].Percentage < Percentage) // Highscore is beaten
                    {
                        TotalScore += Score - highscores.list[index].Score;
                        IsPersonalHighscore = true;
                        BeatmapHighscore updatedHighscore = highscores.list[index];
                        updatedHighscore.Score = Score;
                        updatedHighscore.Combo = Combo;
                        updatedHighscore.Percentage = Percentage;
                        updatedHighscore.MaxCombo = BestCombo;
                        updatedHighscore.Timestamp = DateTime.Now.ToString("o");
                        updatedHighscore.Perfects = Perfects;
                        updatedHighscore.Goods = Goods;
                        updatedHighscore.Bads = Bads;
                        updatedHighscore.Misses = Misses;
                        updatedHighscore.Rank = Rank;

                        highscores.list[index] = updatedHighscore;
                    }
                }
                else // No scores for this player, create a new score
                {
                    highscores.list.Add(CreateHighscore());
                    TotalScore += Score;
                }
            }
        }

        highscores.list.Sort((a, b) => b.Score - a.Score); // Reorder scores by descending order
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

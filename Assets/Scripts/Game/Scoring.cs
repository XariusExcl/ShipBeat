public class Scoring
{
    public static int Score { get; private set; } = 0;
    public static int Combo { get; private set; } = 0;
    public static int MaxCombo { get; private set; } = 0;
    public static int Perfects { get; private set; } = 0;
    public static int Goods { get; private set; } = 0;
    public static int Bads { get; private set; } = 0;
    public static int Misses { get; private set; } = 0;
    public static int NoteCount { get; set; } = 0;
    public static float Percentage { get; private set; } = 0;
    public static void Reset()
    {
        Score = 0;
        Combo = 0;
        MaxCombo = 0;
        Perfects = 0;
        Goods = 0;
        Bads = 0;
        Misses = 0;
        NoteCount = 0;
        Percentage = 0;
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
        ResetCombo();
        AddScore(10);
        GameUIManager.ShowTicker(TickerType.Bad);
    }

    public static void AddMiss()
    {
        Misses++;
        ResetCombo();
        AddScore(0);
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

    public static string GetRank()
    {
        if (Percentage == 100) return "P";
        if (Percentage >= 90) return "S";
        if (Percentage >= 80) return "A";
        if (Percentage >= 70) return "B";
        if (Percentage >= 60) return "C";
        if (Percentage >= 50) return "D";
        return "F";
    }
}

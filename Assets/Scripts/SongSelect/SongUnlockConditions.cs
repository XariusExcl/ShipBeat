using Anatidae;
using UnityEngine;

public struct SongLockStatus
{
    public bool IsLocked;
    public string UnlockCondition;
}

public class SongUnlockConditions : MonoBehaviour
{
    static bool isHardUnlocked;
    static bool isExpertUnlocked;

    public static void RunUnlockChecks()
    {
        isHardUnlocked = HardUnlockConditionsMet();
        isExpertUnlocked = ExpertUnlockConditionsMet();
    }

    public static bool HardUnlockConditionsMet()
    {
        if (ExtradataManager.GetDataWithKey($"Player/{HighscoreManager.PlayerName}/UnlockHard") is not null) return true;
        else if ((OnlineDataManager.Online ?
                OnlineDataManager.Data.PlayerInfo.TotalScore :
                long.Parse(ExtradataManager.GetDataWithKey($"Player/{HighscoreManager.PlayerName}/TotalScore")))
            >= 350000)
        {
            return true;
        } else return false;
    }

    public static bool ExpertUnlockConditionsMet()
    {
        if (ExtradataManager.GetDataWithKey($"Player/{HighscoreManager.PlayerName}/UnlockExpert") is not null) return true;
        else if ((OnlineDataManager.Online ?
                OnlineDataManager.Data.PlayerInfo.TotalScore :
                long.Parse(ExtradataManager.GetDataWithKey($"Player/{HighscoreManager.PlayerName}/TotalScore")))
            >= 1000000)
        {
            return true;
        } else return false;
    }

    public static SongLockStatus IsSongLocked(SongDataInfo info)
    {
        SongLockStatus lockStatus = new()
        {
            IsLocked = false,
            UnlockCondition = ""
        };

        if (info.DifficultyRating >= 12 && !isExpertUnlocked)
        {
            lockStatus.IsLocked = true;
            lockStatus.UnlockCondition += "\nAvoir au moins 1.000M de points de mission";
        } else if (info.DifficultyRating >= 8 && !isHardUnlocked)
        {
            lockStatus.IsLocked = true;
            lockStatus.UnlockCondition += "\nAvoir au moins 0.350M de points de mission";
        }   

        return lockStatus;
    }

    void OnDestroy()
    {
        isHardUnlocked = isExpertUnlocked = false;
    }
}
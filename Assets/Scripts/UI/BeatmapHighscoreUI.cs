using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System;

public class BeatmapHighscoreUI : MonoBehaviour
{
    [SerializeField] TMP_Text playerName;
    [SerializeField] TMP_Text time;
    [SerializeField] TMP_Text score;
    [SerializeField] TMP_Text percentage;
    [SerializeField] TMP_Text rank;
    [SerializeField] Image rankBorder;
    [SerializeField] Image fcBorder;
    [SerializeField] Image rankBg;
    [SerializeField] SongSelectSceneData songSelectSceneData;

    public void SetData(BeatmapHighscore data)
    {
        playerName.text = data.PlayerName;
        var timeSpan = DateTime.Now - DateTime.Parse(data.Timestamp);
        if (timeSpan.TotalDays >= 1)
            time.text = $"{(int)timeSpan.TotalDays}d";
        else if (timeSpan.TotalHours >= 1)
            time.text = $"{(int)timeSpan.TotalHours}h";
        else if (timeSpan.TotalMinutes >= 1)
            time.text = $"{(int)timeSpan.TotalMinutes}m";
        else
            time.text = "<1m";

        score.text = data.Score.ToString();
        percentage.text = $"{data.Percentage:F2}%";
        rank.text = data.Rank.ToString();

        Color rankColor;
        switch (data.Rank)
        {
            case 'P':
                rankColor = songSelectSceneData.pRankColor;
                break;
            case 'S':
                rankColor = songSelectSceneData.sRankColor;
                break;
            case 'A':
                rankColor = songSelectSceneData.aRankColor;
                break;
            case 'B':
                rankColor = songSelectSceneData.bRankColor;
                break;
            case 'C':
                rankColor = songSelectSceneData.cRankColor;
                break;
            case 'D':
                rankColor = songSelectSceneData.dRankColor;
                break;
            default:
                rankColor = songSelectSceneData.fRankColor;
                break;
        }

        rankBorder.color = rankColor;
        rankBg.color = new Color(rankColor.r, rankColor.g, rankColor.b, .1f);
        if (data.Misses == 0 && data.Bads == 0)
        {
            fcBorder.color = rankColor;
            fcBorder.gameObject.SetActive(true);    
        } else fcBorder.gameObject.SetActive(false);    
    }

    public void SetBlank()
    {
        playerName.text = "...";
        time.text = "...";
        score.text = "...";
        percentage.text = "...";
        rank.text = "";
        rankBorder.color = songSelectSceneData.fRankColor;
        rankBg.color = new Color(songSelectSceneData.fRankColor.r, songSelectSceneData.fRankColor.g, songSelectSceneData.fRankColor.b, .1f);
        fcBorder.gameObject.SetActive(false);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}

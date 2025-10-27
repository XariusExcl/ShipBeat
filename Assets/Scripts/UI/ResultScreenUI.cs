using UnityEngine;
using TMPro;
using System.Collections;
using System;
using UnityEngine.UI;

public class ResultScreenUI : MonoBehaviour
{
    bool buttonsEnabled = false;
    [SerializeField] TMP_Text tmp_PerfectCount;
    [SerializeField] TMP_Text tmp_GoodCount;
    [SerializeField] TMP_Text tmp_BadCount;
    [SerializeField] TMP_Text tmp_MissCount;
    [SerializeField] TMP_Text tmp_Rank;
    [SerializeField] Image rankRing;
    [SerializeField] Image button;
    [SerializeField] GameObject personalHighscore;
    [SerializeField] GameObject cabHighscore;
    [SerializeField] SongSelectSceneData songSelectSceneData;
    AudioSource audioSource;
    [SerializeField] AudioClip shuffleSound;
    [SerializeField] AudioClip rollSound;
    [SerializeField] AudioClip impactSound;
    [SerializeField] AudioClip highscoreSound;
    [SerializeField] AudioClip cabHighscoreSound;

    public void ShowResults()
    {
        audioSource = GetComponent<AudioSource>();
        tmp_PerfectCount.text = tmp_GoodCount.text = tmp_BadCount.text = tmp_MissCount.text = tmp_Rank.text = "";
        rankRing.fillAmount = 0f;
        button.color = Color.grey;
        StartCoroutine(ResultAnim());
        Invoke(nameof(ShuffleSound), .95f);
        Invoke(nameof(RollSound), 1.95f);
    }

    void ShuffleSound()
    {
        audioSource.PlayOneShot(shuffleSound);
    }

    void RollSound()
    {
        audioSource.PlayOneShot(rollSound);
    }

    void OnDisable()
    {
        button.color = Color.grey;
        buttonsEnabled = false;
    }

    void EnableButtons()
    {
        button.color = Color.white;
        buttonsEnabled = true;
    }

    void Update()
    {
        if (buttonsEnabled && Input.GetButtonDown("P1_B1"))
            GameUIManager.ReturnToSongSelect();
    }

    void ShowPersonalHighscore()
    {
        personalHighscore.SetActive(true);
        audioSource.PlayOneShot(highscoreSound, .75f);
    }

    void ShowCabHighscore()
    {
        cabHighscore.SetActive(true);
        audioSource.PlayOneShot(cabHighscoreSound, .75f);
    }

    IEnumerator ResultAnim()
    {
        float easeOutCubic(float x)
        {
            return -x * x + 2 * x;
        }

        float t = 0f;
        float fillAmount = 0f;
        while ((2f - Scoring.Percentage * 0.01f) * (t * .5f - 1f) < 1)
        {
            if (t > 0.95) tmp_PerfectCount.text = Mathf.Round(Scoring.Perfects * easeOutCubic(Math.Clamp(t - 1f, 0f, 1f))).ToString();
            if (t > 1.2) tmp_GoodCount.text = Mathf.Round(Scoring.Goods * easeOutCubic(Math.Clamp(t - 1.25f, 0f, 1f))).ToString();
            if (t > 1.45) tmp_BadCount.text = Mathf.Round(Scoring.Bads * easeOutCubic(Math.Clamp(t - 1.5f, 0f, 1f))).ToString();
            if (t > 1.7) tmp_MissCount.text = Mathf.Round(Scoring.Misses * easeOutCubic(Math.Clamp(t - 1.75f, 0f, 1f))).ToString();
            fillAmount = Scoring.Percentage * 0.01f * easeOutCubic(Math.Clamp((2f - Scoring.Percentage * 0.01f) * (t * .5f - 1f), 0f, 1f));
            rankRing.fillAmount = fillAmount;
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        tmp_PerfectCount.text = Scoring.Perfects.ToString();
        tmp_GoodCount.text = Scoring.Goods.ToString();
        tmp_BadCount.text = Scoring.Bads.ToString();
        tmp_MissCount.text = Scoring.Misses.ToString();
        rankRing.fillAmount = Scoring.Percentage * 0.01f;
        tmp_Rank.text = Scoring.Rank.ToString();
        tmp_Rank.GetComponent<Animation>().Play();
        audioSource.Stop();
        audioSource.PlayOneShot(impactSound);

        switch (Scoring.Rank)
        {
            case 'P':
                rankRing.color = tmp_Rank.color = songSelectSceneData.pRankColor;
                break;
            case 'S':
                rankRing.color = tmp_Rank.color = songSelectSceneData.sRankColor;
                break;
            case 'A':
                rankRing.color = tmp_Rank.color = songSelectSceneData.aRankColor;
                break;
            case 'B':
                rankRing.color = tmp_Rank.color = songSelectSceneData.bRankColor;
                break;
            case 'C':
                rankRing.color = tmp_Rank.color = songSelectSceneData.cRankColor;
                break;
            case 'D':
                rankRing.color = tmp_Rank.color = songSelectSceneData.dRankColor;
                break;
            default:
                rankRing.color = tmp_Rank.color = songSelectSceneData.fRankColor;
                break;
        }
        if (Scoring.IsCabHighscore)
        {
            yield return new WaitForSecondsRealtime(1f);
            ShowCabHighscore();
        }
        else if (Scoring.IsPersonalHighscore)
        {
            yield return new WaitForSecondsRealtime(1f);
            ShowPersonalHighscore();
        }
        EnableButtons();
        yield return null;
    }
}
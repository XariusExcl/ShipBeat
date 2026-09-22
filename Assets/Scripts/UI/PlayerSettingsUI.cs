using System;
using Anatidae;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public struct PlayerSettings
{
    public int ScrollSpeed;
    public float AudioLatency;
    public int HitAccuracyOption;
}

public class PlayerSettingsUI : MonoBehaviour
{
    [SerializeField] Selectable scrollSpeedSetting;
    [SerializeField] TMP_Text scrollSpeedSettingText;
    [SerializeField] Selectable audioLatencySetting;
    [SerializeField] TMP_Text audioLatencySettingText;
    [SerializeField] Selectable hitAccuracySetting;
    [SerializeField] TMP_Text hitAccuracySettingText;
    string[] hitAccuracyOptionsText = {"Basique","Avancé","Avancé + Graph"};
    Animation animation;
    public bool HasFocus = false;
    bool shown = false;
    PlayerSettings playerSettings;

    void Start()
    {
        animation = GetComponent<Animation>();
        string json = ExtradataManager.GetDataWithKey($"Player/{HighscoreManager.PlayerName}/Settings");
        if (json is not null)
        {
            playerSettings = JsonUtility.FromJson<PlayerSettings>(json);
            Maestro.GlobalOffset = playerSettings.AudioLatency;
            Maestro.LaneSpeed = playerSettings.ScrollSpeed;
            EarlyLateUI.HitAccuracyOption = playerSettings.HitAccuracyOption;
        }
        else
            playerSettings = new PlayerSettings { AudioLatency = Maestro.GlobalOffset, ScrollSpeed = Maestro.LaneSpeed, HitAccuracyOption = 0};

        UpdateUI();
    }

    float lastHorizontal;
    void Update()
    {
        if (!HasFocus)
        {
            if (shown)
            {
                shown = false;
                animation.Play("PlayerSettingsFadeout");
                SFXManager.PlayDeepBlipDownSound();
                EventSystem.current.SetSelectedGameObject(null);
                // Save values
                playerSettings.AudioLatency = Maestro.GlobalOffset;
                playerSettings.ScrollSpeed = Maestro.LaneSpeed;
                playerSettings.HitAccuracyOption = EarlyLateUI.HitAccuracyOption;
                StartCoroutine(ExtradataManager.SetExtraData($"Player/{HighscoreManager.PlayerName}/Settings", JsonUtility.ToJson(playerSettings)));
            }
            return;
        }

        if (!shown)
        {
            shown = true;
            animation.Play("PlayerSettingsFadein");
            SFXManager.PlayDeepBlipUpSound();
            EventSystem.current.SetSelectedGameObject(scrollSpeedSetting.gameObject);
        }

        if (lastHorizontal != Input.GetAxisRaw("P1_Horizontal"))
        {
            lastHorizontal = Input.GetAxisRaw("P1_Horizontal");
            if (lastHorizontal > .5) {
                SFXManager.PlayHorizontalBlipSound();
                ModifySetting(1);
            }

            else if (lastHorizontal < -.5) {
                SFXManager.PlayHorizontalBlipSound();
                ModifySetting(-1);
            }
            UpdateUI();
        }
    }

    void ModifySetting(int mod)
    {
        if (EventSystem.current.currentSelectedGameObject == scrollSpeedSetting.gameObject)
            Maestro.LaneSpeed += mod;
        else if (EventSystem.current.currentSelectedGameObject == audioLatencySetting.gameObject)
            Maestro.GlobalOffset += mod * 0.005f;
        else if (EventSystem.current.currentSelectedGameObject == hitAccuracySetting.gameObject)
            EarlyLateUI.HitAccuracyOption += mod;
    }

    void UpdateUI()
    {
        scrollSpeedSettingText.text = Maestro.LaneSpeed.ToString();
        audioLatencySettingText.text = $"{(Mathf.Sign(Maestro.GlobalOffset) == 1f ? "+" : "")}{Maestro.GlobalOffset * 1000:F0}ms";
        hitAccuracySettingText.text = hitAccuracyOptionsText[EarlyLateUI.HitAccuracyOption];
    }
}
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
}

public class PlayerSettingsUI : MonoBehaviour
{
    [SerializeField] Selectable scrollSpeedSetting;
    [SerializeField] TMP_Text scrollSpeedSettingText;
    [SerializeField] Selectable audioLatencySetting;
    [SerializeField] TMP_Text audioLatencySettingText;
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
        }
        else
            playerSettings = new PlayerSettings { AudioLatency = Maestro.GlobalOffset, ScrollSpeed = Maestro.LaneSpeed };
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
                EventSystem.current.SetSelectedGameObject(null);
                playerSettings = new PlayerSettings { AudioLatency = Maestro.GlobalOffset, ScrollSpeed = Maestro.LaneSpeed };
                StartCoroutine(ExtradataManager.SetExtraData($"Player/{HighscoreManager.PlayerName}/Settings", JsonUtility.ToJson(playerSettings)));
            }
            return;
        }

        if (!shown)
        {
            shown = true;
            animation.Play("PlayerSettingsFadein");
            EventSystem.current.SetSelectedGameObject(scrollSpeedSetting.gameObject);
        }

        if (lastHorizontal != Input.GetAxis("P1_Horizontal"))
        {
            lastHorizontal = Input.GetAxis("P1_Horizontal");
            if (lastHorizontal > .5)
                ModifySetting(1);

            else if (lastHorizontal < -.5)
                ModifySetting(-1);
        }
        UpdateUI();
    }

    void ModifySetting(int mod)
    {
        if (EventSystem.current.currentSelectedGameObject == scrollSpeedSetting.gameObject)
            Maestro.LaneSpeed += mod;
        else if (EventSystem.current.currentSelectedGameObject == audioLatencySetting.gameObject)
            Maestro.GlobalOffset += mod * 0.005f;
    }

    void UpdateUI()
    {
        scrollSpeedSettingText.text = Maestro.LaneSpeed.ToString();
        audioLatencySettingText.text = $"{(Mathf.Sign(Maestro.GlobalOffset) == 1f ? "+" : "")}{Maestro.GlobalOffset * 1000:F0}ms.";
    }
}
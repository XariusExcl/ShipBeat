using UnityEngine;
using UnityEngine.UI;

public class PlayerSettingsUI : MonoBehaviour
{
    [SerializeField] Selectable scrollSpeedSetting;
    [SerializeField] Selectable audioLatencySetting;
    public bool HasFocus = false;

    void Start()
    {
        // Initialize settings
        scrollSpeedSetting.Select();
        audioLatencySetting.Select();
    }

    void Update()
    {
        if (!HasFocus) return;

        // Handle input for settings
        if (Input.GetButtonDown("P1_B1"))
        {
            SFXManager.PlaySelectSound();
            // Apply the selected setting
            ApplySettings();
            HasFocus = false;
            gameObject.SetActive(false);
        }
        else if (Input.GetButtonDown("P1_B2"))
        {
            SFXManager.PlayReturnSound();
            // Close settings menu
            HasFocus = false;
            gameObject.SetActive(false);
        }
    }

    void ApplySettings()
    {
        // Maestro.LaneSpeed = scrollSpeed;
        // Maestro.GlobalOffset = audioLatency;

        // Debug.Log($"Settings applied: Scroll Speed = {scrollSpeed}, Audio Latency = {audioLatency}");
    }
}
using UnityEngine;
using TMPro;
using System;

public class FramerateDisplay : MonoBehaviour {
    int lowestFramerate;
    [SerializeField] TMP_Text tmp_Text;
    [SerializeField] float refreshPeriod;
    int frameCount = 0;

    void Start()
    {
        lowestFramerate = int.MaxValue;
        InvokeRepeating("UpdateText", refreshPeriod, refreshPeriod);
    }

    void Update()
    {
        int framerate = (int)(1 / Time.unscaledDeltaTime);
        lowestFramerate = Math.Min(lowestFramerate, framerate);
        frameCount++;
    }

    void UpdateText()
    {
        tmp_Text.text = $"{frameCount / refreshPeriod}-{lowestFramerate}";
        lowestFramerate = int.MaxValue;
        frameCount = 0;
    }
}
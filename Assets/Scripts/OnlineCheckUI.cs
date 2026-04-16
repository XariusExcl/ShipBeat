using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OnlineCheckUI : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TMP_Text debugText;
    [SerializeField] Sprite onlineSprite;
    [SerializeField] Sprite offlineSprite;

    void Start()
    {
        StartCoroutine(OnlineDataManager.OnlineCheck((online) =>
            {
                if (online)
                {
                    icon.sprite = onlineSprite;
                } else
                {
                    icon.sprite = offlineSprite;
                    debugText.text = $"{new Uri(OnlineDataManager.API_ENDPOINT).Host}";
                }
            }
        ));
    }
}

using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Ticker : MonoBehaviour
{
    TMP_Text tmpText;
    Animation anim;
    [SerializeField] List<string> textList = new();
    [SerializeField] List<Color> textColors = new();
    [SerializeField] List<AnimationClip> animClips = new();


    void Awake()
    {
        tmpText = GetComponentInChildren<TMP_Text>();
        anim = GetComponent<Animation>();
    }

    public void ShowTicker(JudgeType type)
    {
        if (type == JudgeType.Undefined) return;
        tmpText.text = textList[(int)type];
        tmpText.color = textColors[(int)type];
        anim.clip = animClips[(int)type];
        anim.Stop();
        anim.Play();
    }
}

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Splines;

public class TutobotWanderingManager : MonoBehaviour
{

    [SerializeField] SplineAnimate wagon;
    [SerializeField] YapperBehaviour tutoBot;
    [SerializeField] LookTarget tutoBotBodyTarget;
    [SerializeField] LookTarget tutoBotHeadTarget;
    [SerializeField] SplineContainer path1;
    [SerializeField] float path1Duration;
    [SerializeField] SplineContainer path2;
    [SerializeField] float path2Duration;
    [SerializeField][Range(0, 1)] float animProbability;
    [SerializeField] float animDelayMin;
    [SerializeField] float animDelayMax;
    Animation anim;
    bool takenOff;

    string selectedAnim;
    void Start()
    {
        // TODO, put an exception so the first play of the session doesn't show Tutobot

        SongSelectUI.OnSongValidated.AddListener(WaveGoodbye);
        anim = GetComponent<Animation>();
        List<string> clips = new List<string>();
        foreach (AnimationState st in anim) clips.Add(st.name);
        if (clips.Count == 0) return;

        if (Random.value <= animProbability)
        {
            int i = Random.Range(0, 3);
            selectedAnim = clips[i];
            Invoke("Play", Random.Range(animDelayMin, animDelayMax));
        }
    }

    void Play()
    {
        if (takenOff) return;
        anim.Play(selectedAnim);
    }

    /* ANIMATION EVENTS */
    public void Wander1Init()
    {
        wagon.Container = path1;
        wagon.Duration = path1Duration;
        tutoBot.SetEmote("open");
        tutoBot.SetEmote("happyWag");
    }

    public void Wander2Init()
    {
        wagon.Container = path1;
        wagon.Duration = path1Duration;
        tutoBot.SetEmote("open");
        tutoBot.SetEmote("happyWag");
    }

    public void Wander3Init()
    {
        wagon.Container = path2;
        wagon.Duration = path2Duration;
        tutoBot.SetEmote("open");
        tutoBot.SetEmote("happyWag");
    }

    public void WagonPause()
    {
        wagon.Pause();
    }
    
    public void WagonPlay()
    {
        wagon.Play();
    }


    public void Wander2Smile()
    {
        tutoBot.SetEmote("smile");
    }

    public void Wander2Idle()
    {
        tutoBot.SetEmote("open");
    }

    bool isOnTarmac = false;

    public void Wander3SetOnTarmac()
    {
        isOnTarmac = true;
    }

    public void Wander3SetOffTarmac()
    {
        isOnTarmac = true;
    }

    public void WaveGoodbye()
    {
        takenOff = true;

        if (isOnTarmac)
        {
            anim.Blend("RunOffTarmac", 1f, .5f);
            tutoBotBodyTarget.Track("camera");
            tutoBotHeadTarget.Track("camera");
            tutoBot.SetEmote("point");
            tutoBot.SetEmote("handsHoldHead");
            tutoBot.SetEmote("tailShock");
        } else
        {
            anim.Blend("WaveGoodbye", 1f, .5f);
            tutoBotBodyTarget.Track("camera");
            tutoBotHeadTarget.Track("camera");
        }
    }

    public void HandsWave()
    {
        tutoBot.SetEmote("handsWave");
    }

    public void RunOffTarmac()
    {
        tutoBotBodyTarget.Reset();
        tutoBotHeadTarget.Reset();
        tutoBot.SetEmote("guruguru");
        tutoBot.SetEmote("happyWag");
        tutoBot.SetEmote("handsIdle");
        wagon.Duration = path2Duration * .5f;
        wagon.ElapsedTime = wagon.ElapsedTime * .5f;
    }

    public void CheckIfOffTarmac()
    {
        if (wagon.NormalizedTime > .7f)
        {
            tutoBot.SetEmote("closeAAA");
            tutoBot.SetEmote("handsIdle");
            wagon.Pause();
        }
    }

    void OnDestroy()
    {
        SongSelectUI.OnSongValidated.RemoveListener(WaveGoodbye);

    }
}

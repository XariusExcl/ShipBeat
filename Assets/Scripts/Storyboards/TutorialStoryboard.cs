using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct StoryboardTrigger
{
    public int bar;
    public int beat;
    public delegate void TriggerAction();
    public TriggerAction action;
}

public class TutorialStoryboard : MonoBehaviour
{
    [SerializeField] GameObject tutoBot;
    [SerializeField] GameObject commandsPanel;
    [SerializeField] GameObject noteExplain;

    static TutorialStoryboard instance;
    List<StoryboardTrigger> triggers = new List<StoryboardTrigger>
    {
        new() { bar = 0, beat = 0, action = () => { instance.tutoBot.SetActive(true); } },
        new() { bar = 0, beat = 1, action = () => TextboxSystem.StartDialogue("tutorial_1", true) },
        new() { bar = 2, beat = 0, action = () => TextboxSystem.DisplayNextSentence() },
        new() { bar = 4, beat = 0, action = () => {
            TextboxSystem.DisplayNextSentence();
            instance.commandsPanel.SetActive(true);
        } },
        new() { bar = 8, beat = 0, action = () => {
            TextboxSystem.DisplayNextSentence();
            instance.commandsPanel.SetActive(false);
            instance.noteExplain.SetActive(true);
        } },
        new() { bar = 11, beat = 0, action = () => TextboxSystem.DisplayNextSentence() },
        new() { bar = 14, beat = 0, action = () => {
            TextboxSystem.DisplayNextSentence();
            instance.noteExplain.SetActive(false);
        } },
        new() { bar = 15, beat = 2, action = () => TextboxSystem.DisplayNextSentence() },
        new() { bar = 24, beat = 0, action = () => {
            if (Scoring.Percentage < 75f)
                TextboxSystem.StartDialogue("tutorial_bad_2", true);
            else
                TextboxSystem.StartDialogue("tutorial_good_2", true); } },
        new() { bar = 26, beat = 0, action = () => TextboxSystem.DisplayNextSentence() },
        new() { bar = 28, beat = 0, action = () => TextboxSystem.DisplayNextSentence() },
        new() { bar = 30, beat = 0, action = () => TextboxSystem.DisplayNextSentence() },
        new() { bar = 31, beat = 2, action = () => TextboxSystem.DisplayNextSentence() },
        new() { bar = 40, beat = 0, action = () => TextboxSystem.StartDialogue("tutorial_3", true)},
        new() { bar = 40, beat = 3, action = () => TextboxSystem.DisplayNextSentence() },
        new() { bar = 42, beat = 0, action = () => TextboxSystem.DisplayNextSentence() },
        new() { bar = 44, beat = 0, action = () => TextboxSystem.DisplayNextSentence() },
        new() { bar = 46, beat = 0, action = () => {
            TextboxSystem.DisplayNextSentence();
            missCountBeforeSlam = Scoring.Misses; } },
        new() { bar = 47, beat = 3, action = () => {
            if (missCountBeforeSlam < Scoring.Misses )
                TextboxSystem.StartDialogue("tutorial_bad_4", true);
            else
                TextboxSystem.StartDialogue("tutorial_good_4", true); } },
        new() { bar = 48, beat = 2, action = () => TextboxSystem.DisplayNextSentence() },
        new() { bar = 50, beat = 0, action = () => TextboxSystem.StartDialogue("tutorial_5", true)},
        new() { bar = 52, beat = 0, action = () => TextboxSystem.DisplayNextSentence() },
        new() { bar = 54, beat = 0, action = () => TextboxSystem.DisplayNextSentence() },
        new() { bar = 56, beat = 0, action = () => TextboxSystem.DisplayNextSentence() },
    };

    int triggerIndex = 0;
    static int missCountBeforeSlam;

    int bar = -1;
    int beat = -1;

    void Start()
    {
        StartCoroutine(PrewarmModels());
    }

    void Init()
    {
        instance = this;
        bar = -1;
        beat = -1;
        tutoBot.SetActive(false);
        GameObject[] friendlyships = GameObject.FindGameObjectsWithTag("Friendlyship");
        foreach (GameObject friendlyship in friendlyships)
            Destroy(friendlyship);
        commandsPanel.SetActive(false);
        noteExplain.SetActive(false);
        Maestro.StoryboardEnded = false;
    }

    void Update()
    {
        if (Maestro.SongStarted && !Maestro.SongEnded)
        {
            if (Maestro.Bar > bar)
            {
                bar++;
                beat = 0;
                NewBeat();
            }

            if (Maestro.Beat > beat)
            {
                beat++;
                NewBeat();
            }
        }
    }

    void NewBeat()
    {
        if (triggerIndex >= triggers.Count)
        {
            Maestro.StoryboardEnded = true;
            return; // Set some bool to tell the storyboard is done
        }

        while (triggerIndex < triggers.Count && triggers[triggerIndex].bar == bar && triggers[triggerIndex].beat == beat)
        {
            triggers[triggerIndex].action.Invoke();
            triggerIndex++;
        }
    }

    public IEnumerator PrewarmModels()
    {
        tutoBot.SetActive(true);
        commandsPanel.SetActive(true);
        noteExplain.SetActive(true);
        yield return new WaitForEndOfFrame();
        tutoBot.SetActive(false);
        commandsPanel.SetActive(false);
        noteExplain.SetActive(false);
        Init();
    }
}

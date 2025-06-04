using System;
using System.Collections.Generic;
using UnityEngine;

#if DEBUG
using System.Diagnostics;
#endif

public struct DialogueChoice
{
    public string Text;
    public string NextId;
}

public struct DialogueEvent
{
    public List<Textbox> Textboxes;
    public List<DialogueChoice> Choices;

    public override string ToString()
    {
        string output = "";

        foreach (var textbox in Textboxes)
            output += textbox.Name + ": \"" + textbox.Text + "\".\n";

        return output;
    }
}

public class LocalisationFileParser : MonoBehaviour
{
    static TextAsset localisationFile;
    static int keyCount = 0;
    static string keyName;
    static DialogueEvent currentDialogueEvent;
    public static Dictionary<string, DialogueEvent> textLibrary = new Dictionary<string, DialogueEvent>();
    static readonly string[] NewLineChars = { "\r\n", "\r", "\n" };

    public static void LoadLanguage(string language)
    {
        textLibrary.Clear();
        currentDialogueEvent = new DialogueEvent
        {
            Textboxes = new List<Textbox>(),
            Choices = new List<DialogueChoice>()
        };
        keyCount = 0;
        keyName = string.Empty;
        localisationFile = Resources.Load<TextAsset>($"Localisation/{language}");
        if (localisationFile == null)
        {
            UnityEngine.Debug.LogError($"Localisation file not found for language: {language}.");
            return;
        }

#if DEBUG
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
#endif
        ParseFile(localisationFile);
#if DEBUG
        stopwatch.Stop();
        UnityEngine.Debug.Log($"LocalisationFileParser: {keyCount} keys done in {stopwatch.Elapsed.TotalMilliseconds:F3}ms.");
#endif
    }

    static void ParseFile(TextAsset textAsset)
    {
        string[] lines = textAsset.text.Split(NewLineChars, StringSplitOptions.None);

        foreach (string line in lines)
        {
            if (string.IsNullOrEmpty(line)) continue; // Skip empty lines

            switch (line[0])
            {
                case '#': // Key
                    if (keyCount > 0)
                    {
                        textLibrary.Add(keyName, currentDialogueEvent);
                    }

                    keyCount++;
                    keyName = line.Substring(1);
                    currentDialogueEvent = new DialogueEvent
                    {
                        Textboxes = new List<Textbox>(),
                        Choices = new List<DialogueChoice>()
                    };
                    break;
                case '$': // Choice
                    DialogueChoice dc = new DialogueChoice();
                    string[] parts = line.Split(new[] { ' ' }, 2);
                    dc.NextId = parts[0].Substring(1);
                    dc.Text = parts[1];
                    currentDialogueEvent.Choices.Add(dc);
                break;
                case '/': // Comment
                    break;
                default:
                    Textbox tb = new Textbox
                    {
                        Name = "Name",
                        Text = line
                    };
                    currentDialogueEvent.Textboxes.Add(tb);
                    break;
            }
        }
        
        // Add last key dialogue of file
        textLibrary.Add(keyName, currentDialogueEvent);
    }
}

using System.Collections;
using System;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using Anatidae;

public class TextboxSystem : MonoBehaviour
{
	// Singleton instance
	static TextboxSystem instance;

	// Object references
	static YapperBehaviour yapper; // The object that speaks in the textbox

	// Components
	Coroutine co;
	public GameObject goTextbox;
	TextboxBehaviour textboxBehaviour;

	// Dialogue variables
	static Queue<Textbox> Textboxes;
	static Textbox currentTextbox;
	static bool Auto = false;
	[HideInInspector] public static List<DialogueChoice> choices = new List<DialogueChoice>();
	[HideInInspector] public static string fullText;
	bool typing = false;
	bool skip = false;

	void Awake()
	{
		if (instance == null)
			instance = this;

		if (goTextbox == null)
			Debug.LogError("TextboxSystem : goTextbox is not assigned!");
	}

	void Start()
	{
		Textboxes = new Queue<Textbox>();
		goTextbox.SetActive(false);
		textboxBehaviour = goTextbox.GetComponent<TextboxBehaviour>();
		LocalisationFileParser.LoadLanguage("fr");
	}

	void Update()
	{
		if (instance.goTextbox.activeSelf && !Auto && Input.GetButtonDown("P1_B1"))
		{
			SFXManager.PlayHorizontalBlipSound();
			if (typing)
				skip = true;
			else
				DisplayNextSentence();
		}
	}

	public static void StartDialogue(string dialogueId, bool auto = false)
	{
		// TODO: Set some UI variable to true to lock everything else but the dialogue;
		Auto = auto;

		Textboxes.Clear();
		if (!instance.goTextbox.activeSelf)
		{
			instance.goTextbox.SetActive(true);
			instance.textboxBehaviour.Initialize();
		}
		instance.textboxBehaviour.DisableAllButtons();

		if (!LocalisationFileParser.TextLibrary.ContainsKey(dialogueId))
		{
			Debug.LogWarning("DialogueManager : \"" + dialogueId + "\" ID does not exist!");
			EndDialogue();
		}
		else
		{
			// Start dialogue
			foreach (Textbox tb in LocalisationFileParser.TextLibrary[dialogueId].Textboxes)
			{
				Textboxes.Enqueue(tb);
			}
			choices = LocalisationFileParser.TextLibrary[dialogueId].Choices;
			DisplayNextSentence();
		}
	}

	public static void DisplayNextSentence()
	{
		instance.textboxBehaviour.DoneIndicatorActive(false);
		if (Textboxes.Count == 0)
		{
			instance.StopAllCoroutines();

			// if Choice exists
			if (choices.Count > 0)
			{
				instance.textboxBehaviour.SetChoices(choices);
				return;
			}
			else
			{
				instance.textboxBehaviour.FadeOut();
				return;
			}
		}
		currentTextbox = Textboxes.Dequeue();

		// Replacing textKeys with their value
		MatchEvaluator textKeyEvaluator = new MatchEvaluator(TextKeyReplace);
		currentTextbox.Text = Regex.Replace(currentTextbox.Text, @"\[k:(\S*?)\]", textKeyEvaluator);
		// Text formatting
		fullText = WrapText(currentTextbox.Text, instance.textboxBehaviour.GetMaxLineLength());

		instance.StopAllCoroutines();
		instance.StartCoroutine(instance.TypeSentence(fullText));
	}


	bool isItalics = false;
	int italicsStartIndex = 0;
	bool isColor = false;
	int colorStartIndex = 0;

	IEnumerator TypeSentence(string sentence, float shortDelay = .0167f, float longDelay = .0322f)
	{
		typing = true;
		isItalics = isColor = false;
		yield return new WaitForEndOfFrame(); // Prevents insta-skipping dialogue after button press
		skip = false;

		char[] punctuation = { ',', '.', '?', '!' };
		StringBuilder textboxTextSB = new StringBuilder();
		int length = sentence.Length;
		char letter;

		for (int i = 0; i < length; i++)
		{
			letter = sentence[i];

			// Check for effects
			CheckForEffects();

			void CheckForEffects()
			{
				if (letter != '[') return;

				int endEffectIndex = sentence.IndexOf(']', i);
				if (endEffectIndex == -1) return;

				string effectString = sentence.Substring(i + 1, endEffectIndex - i - 1);
				string[] effect = effectString.Split(':');

				if (effect.Length >= 2)
				{
					switch (effect[0])
					{
						case "s": // Font styling
							if (effect[1] == "i")
							{
								isItalics = true;
								italicsStartIndex = i;
								textboxTextSB.Append("<i>");
							}
							else
							{
								isItalics = false;
								textboxTextSB.Append("</i>");
							}
							break;
						case "e": // Emote
							yapper?.SetEmote(effect[1]);
							break;
						case "o": // Orientation direction
							yapper.UpdateBodyTarget(effect[1] ?? "");
							break;
						case "l": // Look at
							yapper.UpdateEyesTarget(effect[1] ?? "");
							break;
						case "y" // Set Yapper
							when effect[1].Length > 0:
							yapper = YapperBehaviour.FindByID(effect[1]);
							textboxBehaviour.UpdateYapper(yapper.gameObject);
							break;
						case "c": // Text color
							if (effect[1] == "")
							{
								textboxTextSB.Append("</color>");
								isColor = false;
							}
							else
							{
								textboxTextSB.Append("<color=" + effect[1] + ">");
								isColor = true;
								colorStartIndex = i;
							}
							break;
						case "!": // Event
							DialogueTriggers.TriggerEvent(effect[1]);
							break;
						case "ts": // Textbox size
							if (effect.Length >= 3 && float.TryParse(effect[1], out float width) && float.TryParse(effect[2], out float height))
							{
								instance.textboxBehaviour.SetSize(new Vector2(width, height));
								sentence = WrapText(currentTextbox.Text, instance.textboxBehaviour.GetMaxLineLength()); // recalculate text wrapping
								length = sentence.Length;
							}
							else
								Debug.LogWarning("Textbox size effect requires two float parameters: width and height.");
							break;
						case "tp": // Textbox position
							if (effect.Length >= 3 && float.TryParse(effect[1], out float x) && float.TryParse(effect[2], out float y))
								instance.textboxBehaviour.SetPosition(new Vector2(x, y));
							else
								Debug.LogWarning("Textbox position effect requires two float parameters: x and y.");
							break;
						default:
							Debug.LogWarning("Unsupported textbox effect: \"" + effect[0] + "\".");
							break;
					}
				}
				else
					Debug.LogWarning("Textbox effect " + effectString + " is not valid!");

				i = endEffectIndex + 1;
				if (i < length)
				{
					letter = sentence[i];
					CheckForEffects(); // Recursive check for back-to-back effects
				}
				// else return; // check for effects on end of string (might break stuff?)
			}

			if (letter == '_')
			{
				if (!skip) yield return new WaitForSeconds(longDelay * 2f);
			}
			else
			{
				textboxTextSB.Append(letter);
				string currentText = textboxTextSB.ToString();

				// Check if this line had no text (just effects)
				if (i >= length - 1 && !Regex.IsMatch(currentText, @"\S"))
				{
					// Early exit
					typing = false;
					skip = false;
					textboxBehaviour.FadeOut();
					yield break;
				}

				if (isItalics && isColor)
				{
					// Close tags in the right order
					if (italicsStartIndex > colorStartIndex)
						currentText += "</i></color>";
					else
						currentText += "</color></i>";
				}
				else
					currentText += isItalics ? "</i>" : "" + (isColor ? "</color>" : "");

				// Converts to string to be displayed by the textbox
				textboxBehaviour.SetText(currentText);

				// Sound and wait logic
				if (punctuation.Contains(letter)) // No sound, longer pause
				{
					if (!skip) yield return new WaitForSeconds(longDelay);
				}
				else if (letter != ' ')
				{ // Normal pause, no sound if ' '
					if (!skip)
					{
						if (i % 2 == 0) yapper?.Speak();
						yield return new WaitForSeconds(shortDelay);
					}
				}
			}
		}
		typing = false;
		skip = false;
		textboxBehaviour.DoneIndicatorActive(!Auto);
	}

	public static void EndDialogue()
	{
		instance.goTextbox.SetActive(false);
		// TODO: Set some UI variable to false to release UI interaction lock;
	}

	// Courtesy of github.com/Erinaaaaaaa
	private static readonly char[] _splitChars = { ' ', '\t', '\n', '\r' };
	public static string WrapText(string text, int maxWidth)
	{
		var words = text.Split(_splitChars, StringSplitOptions.RemoveEmptyEntries);
		var sb = new StringBuilder();
		var lineLength = 0;
		foreach (var word in words)
		{
			// Ignore commands in word length calculation
			int wordLength = Regex.Replace(word, @"(\[\S*?:\S*?\])|(_+)", "").Length;
			if (lineLength + wordLength > maxWidth)
			{
				sb.AppendLine();
				lineLength = 0;
			}
			sb.Append(word).Append(' ');
			lineLength += wordLength + 1;
		}
		return sb.ToString();
	}

	public static string TextKeyReplace(Match m)
	{
		string key = m.Groups[1].ToString();
		switch (key)
		{
			case "playerName":
				return HighscoreManager.PlayerName;
			case "score":
				return Scoring.Score.ToString();
			default:
				if (LocalisationFileParser.TextLibrary.ContainsKey(key))
					return LocalisationFileParser.TextLibrary[key].Textboxes[0].Text;
				else return "";
		}
	}
}
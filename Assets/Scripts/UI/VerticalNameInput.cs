using UnityEngine;
using System.Text.RegularExpressions;
using Anatidae;

public class VerticalNameInput : MonoBehaviour
{
    [SerializeField] VerticalNameInputLetter[] letters;
    private int[] letterIndices = new int[3];
    char[] playerName = new char[3];
    static readonly char[] Alphabet = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '♥', '\0' };
    private int nameLetterIndex = 0;
    bool blockInput = false;

    const float repeatTime = .6f;
    const int repeatFrameWait = 2;
    int currentRepeatFrameWait = 3;
    float lastKeyInputTime = 0f;
    bool neutralInput = true;
    bool oldNeutralInput = false;
    bool inputDown = false;
    bool inputUp = false;
    bool repeatDown = false;
    bool repeatUp = false;

    void Start()
    {
        for (int i = 0; i < letters.Length; i++)
        {
            letters[i].Index = i;
            letters[i].Letter = Alphabet[0];
            letterIndices[i] = 0;
            playerName[i] = Alphabet[0];
        }
        UpdateName();
    }

    void Update()
    {
        if (blockInput)
            return;

        inputDown = Input.GetAxisRaw("P1_Vertical") < -.9f;
        inputUp = Input.GetAxisRaw("P1_Vertical") > .9f;
        oldNeutralInput = neutralInput;
        neutralInput = !(inputDown || inputUp);

        if (neutralInput)
        {
            repeatDown = repeatUp = false;
            currentRepeatFrameWait = repeatFrameWait;
        }

        if (repeatDown)
        {
            if (currentRepeatFrameWait <= 0)
            {
                MoveDown();
                currentRepeatFrameWait = repeatFrameWait;
            }
            else currentRepeatFrameWait--;
        }

        if (repeatUp)
        {
            if (currentRepeatFrameWait <= 0)
            {
                MoveUp();
                currentRepeatFrameWait = repeatFrameWait;
            }
            else currentRepeatFrameWait--;
        }

        if (!neutralInput && oldNeutralInput)
        {
            lastKeyInputTime = Time.time;
            if (inputDown)
                MoveDown();
            else if (inputUp)
                MoveUp();
        }

        if (Time.time - lastKeyInputTime > repeatTime)
        {
            if (inputDown)
                repeatDown = true;

            if (inputUp)
                repeatUp = true;
        }
    }

    void MoveDown()
    {
        if (++letterIndices[nameLetterIndex] >= Alphabet.Length)
            letterIndices[nameLetterIndex] = 0;

        letters[nameLetterIndex].Letter = Alphabet[letterIndices[nameLetterIndex]];
    }

    void MoveUp()
    {
        if (--letterIndices[nameLetterIndex] < 0)
            letterIndices[nameLetterIndex] = Alphabet.Length - 1;

        letters[nameLetterIndex].Letter = Alphabet[letterIndices[nameLetterIndex]];
    }

    public void UpdateName()
    {
        HighscoreManager.PlayerName = Regex.Replace(new string(playerName), @"\0", "_");
    }

    public void LetterSelected(int letterIndex)
    {
        nameLetterIndex = letterIndex;
        blockInput = !CheckIfAnySelected();
    }

    public void LetterDeselected(int letterIndex)
    {
        blockInput = !CheckIfAnySelected();
    }

    bool CheckIfAnySelected()
    {
        bool select = false;
        foreach (VerticalNameInputLetter vnil in letters)
        {
            if (vnil.IsSelected)
                select = true;
        }
        return select;
    }
}

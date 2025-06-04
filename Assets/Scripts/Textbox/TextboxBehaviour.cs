using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public struct Textbox
{
    public string Text;
    public string Name;
    public string Effect;
}

public class TextboxBehaviour : MonoBehaviour
{
    GameObject yapper;
    RectTransform rectTransform;
    Image backgroundImage;
    [SerializeField] TextboxButton[] choiceButtons;
    [SerializeField] Transform tailTransform;
    [SerializeField] TMP_Text textboxText;
    EventSystem eventSystem;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        backgroundImage = GetComponent<Image>();
        eventSystem = EventSystem.current;
    }

    void OnEnable()
    {
        Initialize();
    }

    void OnDisable()
    {
        textboxText.text = "";
    }

    public void Initialize()
    {
        textboxText.gameObject.SetActive(true);
        DisableAllButtons();
    }

    public void SetText(string text)
    {
        textboxText.text = text;
    }

    public void SetChoices(List<DialogueChoice> choices)
    {
        DisableAllButtons();
        int i = 0;
        foreach (DialogueChoice c in choices)
        {
            choiceButtons[i].gameObject.SetActive(true);
            choiceButtons[i].SetData(c);
            i++;
        }
        eventSystem.SetSelectedGameObject(choiceButtons[0].gameObject);
    }

    public void DisableAllButtons()
    {
        foreach (TextboxButton b in choiceButtons)
        {
            b.gameObject.SetActive(false);
        }
    }

    public int GetMaxLineLength()
    {
        return (int)((textboxText.rectTransform.rect.width - 12f) / 4.65f);
    }

    public void FadeOut()
    {
        gameObject.SetActive(false);
    }

    public void SetSize(Vector2 size)
    {
        rectTransform.sizeDelta = size;
    }

    public void SetPosition(Vector2 position)
    {
        rectTransform.anchoredPosition = position;
    }

    public void SetBgColor(Color color = default)
    {
        if (color == default)
            color = new Color(.0f, .9f, 1f, 1f);

        backgroundImage.color = color;
    }

    public void UpdateYapper(GameObject newYapper)
    {
        yapper = newYapper;
        SetBgColor(yapper.GetComponent<YapperBehaviour>().Color);
    }


    void Update()
    {
        //  Tail Update
        if (yapper != null)
        {
            TailUpate();
        }
    }

    const float halfCanvasToScreen = 2.4f;
    void TailUpate()
    {
        Vector2 yapperScreenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, yapper.transform.position);

        float yPos, xPos;
        
        float above = Mathf.Sign(yapperScreenPos.y - rectTransform.position.y);
        yPos = above * rectTransform.rect.height * halfCanvasToScreen + rectTransform.position.y;

        float halfWsWidth = rectTransform.rect.width * halfCanvasToScreen - 70f;
        xPos = Mathf.Clamp(yapperScreenPos.x, rectTransform.position.x - halfWsWidth, rectTransform.position.x + halfWsWidth);

        tailTransform.localScale = new Vector3(-above, above, 1f);
        tailTransform.position = new Vector2(Mathf.Lerp(tailTransform.position.x, xPos, .1f), yPos);
    }
}

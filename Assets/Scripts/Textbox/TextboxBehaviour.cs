using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

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
    Vector2 size = new Vector2(250f, 80f);

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
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        // change size from 0,0 to size
        tailTransform.gameObject.SetActive(false);
        float elapsedTime = 0f;
        Vector2 startSize = new Vector2(0f, 7f);
        while (elapsedTime < 0.5f)
        {
            rectTransform.sizeDelta = new Vector2(
                Mathf.SmoothStep(startSize.x, size.x, elapsedTime / 0.25f),
                Mathf.SmoothStep(startSize.y, size.y, (elapsedTime - .25f) / 0.25f)
            );
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        rectTransform.sizeDelta = size;
        tailTransform.gameObject.SetActive(true);
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
        return (int)((size.x - 12f) / 5f);
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    IEnumerator FadeOutCoroutine()
    {
        // change size from size to 0,0
        tailTransform.gameObject.SetActive(false);
        float elapsedTime = 0f;
        Vector2 startSize = size;
        while (elapsedTime < 0.25f)
        {
            rectTransform.sizeDelta = new Vector2(
                startSize.x,
                Mathf.SmoothStep(startSize.y, 0f, elapsedTime / 0.25f)
            );
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false);
    }

    public void SetSize(Vector2 setSize)
    {
        size = setSize;
        rectTransform.sizeDelta = size;
    }

    public void SetPosition(Vector2 position)
    {
        rectTransform.anchoredPosition = position;
    }

    public void SetColor(Color color = default)
    {
        if (color == default)
            color = new Color(.0f, .9f, 1f, 1f);

        backgroundImage.color = color;
        tailTransform.GetComponent<Image>().color = color;
    }

    public void UpdateYapper(GameObject newYapper)
    {
        yapper = newYapper;
        SetColor(yapper.GetComponent<YapperBehaviour>().Color);
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

        tailTransform.localScale = new Vector3(Mathf.Sign(xPos - rectTransform.position.x), above, 1f);
        tailTransform.position = new Vector2(Mathf.Lerp(tailTransform.position.x, xPos, .1f), yPos);
    }
}

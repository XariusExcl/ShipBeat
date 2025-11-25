using UnityEngine;
using System.Runtime.InteropServices;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] TMP_Text quitText;
    const float AfkTime = 60f;
    float afkTimer = 0f;
    const float HeldQuitTime = 1.5f;
    float heldQuitTimer = 0f;
    const string MenuMessage = "Retour au menu";
    static bool backToMenuCalled = false;

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    public static extern void BackToMenu();
#else
    public static void BackToMenu()
    {
        // Debug.Log("Retour au menu de la borne d'arcade !");
    }
#endif

    void Start()
    {
        backToMenuCalled = false;
    }

    void Update()
    {
        if (!backToMenuCalled && heldQuitTimer >= HeldQuitTime || afkTimer >= AfkTime)
        {
            backToMenuCalled = true;
            BackToMenu();
        }

        if (Input.GetButton("Coin"))
            heldQuitTimer += Time.deltaTime;
        else
            heldQuitTimer = 0f;

        if (Mathf.Abs(Input.GetAxisRaw("P1_Horizontal")) > 0.5f || Mathf.Abs(Input.GetAxisRaw("P1_Vertical")) > 0.5f || Input.GetButton("P1_Start") || Input.GetButton("P1_B1") || Input.GetButton("P1_B2") || Input.GetButton("P1_B3") || Input.GetButton("P1_B4") || Input.GetButton("P1_B5") || Input.GetButton("P1_B6") ||
            Mathf.Abs(Input.GetAxisRaw("P2_Horizontal")) > 0.5f || Mathf.Abs(Input.GetAxisRaw("P2_Vertical")) > 0.5f || Input.GetButton("P2_Start") || Input.GetButton("P2_B1") || Input.GetButton("P2_B2") || Input.GetButton("P2_B3") || Input.GetButton("P2_B4") || Input.GetButton("P2_B5") || Input.GetButton("P2_B6"))
            afkTimer = 0f;
        else
            afkTimer += Time.deltaTime;

        if (heldQuitTimer != 0 || afkTimer - AfkTime + 6f > 0f)
        {
            quitText.gameObject.SetActive(true);
            quitText.text = MenuMessage + new string('.', (int)Mathf.Min(Mathf.Max(heldQuitTimer * 3f, afkTimer - AfkTime + 10f * 0.4f), 3));
        }
        else quitText.gameObject.SetActive(false);
    }

    public void OnApplicationQuit()
    {
        BackToMenu();
    }
}
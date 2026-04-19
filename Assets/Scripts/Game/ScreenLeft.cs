using TMPro;
using UnityEngine;

public class ScreenLeft : MonoBehaviour
{
    [SerializeField] TMP_Text text;

    void Start()
    {
        InvokeRepeating("GenerateRandomNumbers", 0, 1);
    }

    void GenerateRandomNumbers()
    {
        string randomNumbers1 = "";
        for (int i = 0; i < 8; i++)
        {
            randomNumbers1 += Random.Range(0, 100) + " ";
        }

        string randomNumbers2 = "";
        for (int i = 0; i < 4; i++)
        {
            randomNumbers2 += Random.Range(0, 100) + " ";
        }

        text.text = randomNumbers1 + "\n\n" + randomNumbers2;
    }
}

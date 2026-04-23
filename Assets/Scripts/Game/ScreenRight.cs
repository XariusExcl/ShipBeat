using TMPro;
using UnityEngine;

public class ScreenRight : MonoBehaviour
{
    [SerializeField] TMP_Text BPM;
    [SerializeField] TMP_Text Timesig;
    [SerializeField] TMP_Text Scoremul;

    void Update()
    {
        BPM.text = $"{Maestro.CurrentTimingPoint.BPM}\nBPM";
        Timesig.text = $"TIMESIG\n{Maestro.CurrentTimingPoint.Meter}/4";
        Scoremul.text = $"{(Maestro.IsKiaiTime?"1.5":"1.0")}X SCORE";
    }

}

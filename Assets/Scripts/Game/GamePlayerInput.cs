using System.Collections.Generic;
using UnityEngine;

public enum ButtonState
{
    Up,
    Down
}

public class GamePlayerInput : MonoBehaviour {
    [SerializeField] List<KeyBeam> Beams;

    float lastHorizontal = 0;
    float settingLastHorizontal = 0;
    float settingLastVertical = 0;
    void Update()
    {
        // Settings In-game
        if (Input.GetButton("P1_Start"))
        {
            if (settingLastVertical != Input.GetAxis("P1_Vertical")) {
                settingLastVertical = Input.GetAxis("P1_Vertical");
                if (settingLastVertical > .5) {
                    Maestro.LaneSpeed += 1; 
                    Debug.Log($"Set Scroll Speed to {Maestro.LaneSpeed}");
                    InfoToasterUI.ShowToaster($"Set Scroll Speed to {Maestro.LaneSpeed}");
                } else if (settingLastVertical < -.5) {
                    Maestro.LaneSpeed -= 1; 
                    Debug.Log($"Set Scroll Speed to {Maestro.LaneSpeed}");
                    InfoToasterUI.ShowToaster($"Set Scroll Speed to {Maestro.LaneSpeed}");
                }
            }

            if (settingLastHorizontal != Input.GetAxis("P1_Horizontal")) {
                settingLastHorizontal = Input.GetAxis("P1_Horizontal");
                if (settingLastHorizontal > .5) {
                    Maestro.GlobalOffset += 0.005f;
                    Debug.Log($"Set Audio offset to {(Mathf.Sign(Maestro.GlobalOffset)==1f?"+":"")}{Maestro.GlobalOffset*1000:F0}ms.");
                    InfoToasterUI.ShowToaster($"Set Audio offset to {(Mathf.Sign(Maestro.GlobalOffset)==1f?"+":"")}{Maestro.GlobalOffset*1000:F0}ms.");
                }
                
                else if (settingLastHorizontal < -.5) {
                    Maestro.GlobalOffset -= 0.005f;
                    Debug.Log($"Set Audio offset to {(Mathf.Sign(Maestro.GlobalOffset)==1f?"+":"")}{Maestro.GlobalOffset*1000:F0}ms.");
                    InfoToasterUI.ShowToaster($"Set Audio offset to {(Mathf.Sign(Maestro.GlobalOffset)==1f?"+":"")}{Maestro.GlobalOffset*1000:F0}ms.");
                }
            }
        }

        if (lastHorizontal != Input.GetAxis("P1_Horizontal"))
        {
            lastHorizontal = Input.GetAxis("P1_Horizontal");
            if (lastHorizontal > .5) {
                Judge.JudgePlayerInput(1, ButtonState.Down);
            }
            else if (lastHorizontal < -.5) {
                Judge.JudgePlayerInput(0, ButtonState.Down);
            }
        }

        for (int i = 1; i <= 6; i++)
        {
            if (Input.GetButtonDown("P1_B"+i))
            {
                Beams[i-1].HandleInput(ButtonState.Down);
                Judge.JudgePlayerInput(i+1, ButtonState.Down);
            }

            if (Input.GetButtonUp("P1_B"+i))
            {
                Beams[i-1].HandleInput(ButtonState.Up);
                Judge.JudgePlayerInput(i+1, ButtonState.Up);
            }
        }
    }
}
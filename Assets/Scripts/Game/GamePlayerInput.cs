using System.Collections.Generic;
using UnityEngine;

public enum ButtonState
{
    Released,
    Pressed,
    Left,
    Right
}

public class GamePlayerInput : MonoBehaviour {
    [SerializeField] List<Receptor> Receptors;
    [SerializeField] JostleEffect playerShip;
    [SerializeField] JostleEffect otherShips;


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
                    Maestro.LaneSpeed++;
                    InfoToasterUI.ShowToaster($"Vitesse de défilement : {Maestro.LaneSpeed}");
                } else if (settingLastVertical < -.5) {
                    Maestro.LaneSpeed--;
                    InfoToasterUI.ShowToaster($"Vitesse de défilement : {Maestro.LaneSpeed}");
                }
            }

            if (settingLastHorizontal != Input.GetAxis("P1_Horizontal")) {
                settingLastHorizontal = Input.GetAxis("P1_Horizontal");
                if (settingLastHorizontal > .5) {
                    Maestro.GlobalOffset += 0.005f;
                    Jukebox.SetPlaybackPosition(Maestro.SongTime - Maestro.GlobalOffset);
                    InfoToasterUI.ShowToaster($"Compensation latence audio : {(Mathf.Sign(Maestro.GlobalOffset)==1f?"+":"")}{Maestro.GlobalOffset*1000:F0}ms.");
                }
                
                else if (settingLastHorizontal < -.5) {
                    Maestro.GlobalOffset -= 0.005f;
                    Jukebox.SetPlaybackPosition(Maestro.SongTime - Maestro.GlobalOffset);
                    InfoToasterUI.ShowToaster($"Compensation latence audio : {(Mathf.Sign(Maestro.GlobalOffset)==1f?"+":"")}{Maestro.GlobalOffset*1000:F0}ms.");
                }
            }
        }

        if (lastHorizontal != Input.GetAxis("P1_Horizontal"))
        {
            if (lastHorizontal < .5f && Mathf.Abs(Input.GetAxis("P1_Horizontal")) > .5f) {
                Receptors[0].HandleInput(ButtonState.Released);
                if (Input.GetAxis("P1_Horizontal") > 0) {
                    Judge.JudgePlayerInput(0, ButtonState.Right);
                    playerShip.Jostle(ButtonState.Left);
                    otherShips.Jostle(ButtonState.Left);
                }
                else {
                    Judge.JudgePlayerInput(0, ButtonState.Left);
                    playerShip.Jostle(ButtonState.Right);
                    otherShips.Jostle(ButtonState.Right);
                }
            }
            
            lastHorizontal = Mathf.Abs(Input.GetAxis("P1_Horizontal"));
        }

        for (int i = 1; i <= 6; i++)
        {
            if (Input.GetButtonDown("P1_B"+i))
            {
                Receptors[i].HandleInput(ButtonState.Pressed);
                Judge.JudgePlayerInput(i, ButtonState.Pressed);
            }

            if (Input.GetButtonUp("P1_B"+i))
            {
                Receptors[i].HandleInput(ButtonState.Released);
                Judge.JudgePlayerInput(i, ButtonState.Released);
            }
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

public enum ButtonState
{
    Up,
    Down,
    Left,
    Right
}

public class GamePlayerInput : MonoBehaviour {
    [SerializeField] List<Receptor> Receptors;
    [SerializeField] ShipMovement playerShip;
    [SerializeField] ShipMovement otherShips;


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
                    Jukebox.SetPlaybackPosition(Maestro.SongTime - Maestro.GlobalOffset);
                    Debug.Log($"Set Audio offset to {(Mathf.Sign(Maestro.GlobalOffset)==1f?"+":"")}{Maestro.GlobalOffset*1000:F0}ms.");
                    InfoToasterUI.ShowToaster($"Set Audio offset to {(Mathf.Sign(Maestro.GlobalOffset)==1f?"+":"")}{Maestro.GlobalOffset*1000:F0}ms.");
                }
                
                else if (settingLastHorizontal < -.5) {
                    Maestro.GlobalOffset -= 0.005f;
                    Jukebox.SetPlaybackPosition(Maestro.SongTime - Maestro.GlobalOffset);
                    Debug.Log($"Set Audio offset to {(Mathf.Sign(Maestro.GlobalOffset)==1f?"+":"")}{Maestro.GlobalOffset*1000:F0}ms.");
                    InfoToasterUI.ShowToaster($"Set Audio offset to {(Mathf.Sign(Maestro.GlobalOffset)==1f?"+":"")}{Maestro.GlobalOffset*1000:F0}ms.");
                }
            }
        }

        if (lastHorizontal != Input.GetAxis("P1_Horizontal"))
        {
            if (lastHorizontal < .5f && Mathf.Abs(Input.GetAxis("P1_Horizontal")) > .5f) {
                Receptors[0].HandleInput(ButtonState.Up);
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
                Receptors[i].HandleInput(ButtonState.Down);
                Judge.JudgePlayerInput(i, ButtonState.Down);
            }

            if (Input.GetButtonUp("P1_B"+i))
            {
                Receptors[i].HandleInput(ButtonState.Up);
                Judge.JudgePlayerInput(i, ButtonState.Up);
            }
        }
    }
}
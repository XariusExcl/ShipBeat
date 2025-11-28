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
    float restartTimer = 0f;
    void Update()
    {
        if (Input.GetButton("P1_Start"))
        {
            // Quit/Restart macro
            if (Input.GetButton("P1_B1") && Input.GetButton("P1_B2") && Input.GetButton("P1_B3"))
            {
                restartTimer += Time.deltaTime;
                if(restartTimer >= 1f)
                {
                    // TODO : Call Restart
                }
            } else { restartTimer = 0f; }

            // In-game settings 
            if (settingLastVertical != Input.GetAxisRaw("P1_Vertical")) {
                settingLastVertical = Input.GetAxisRaw("P1_Vertical");
                if (settingLastVertical > .5) {
                    Maestro.LaneSpeed++;
                    InfoToasterUI.ShowToaster($"Vitesse de défilement : {Maestro.LaneSpeed}");
                } else if (settingLastVertical < -.5) {
                    Maestro.LaneSpeed--;
                    InfoToasterUI.ShowToaster($"Vitesse de défilement : {Maestro.LaneSpeed}");
                }
            }

            if (settingLastHorizontal != Input.GetAxisRaw("P1_Horizontal")) {
                settingLastHorizontal = Input.GetAxisRaw("P1_Horizontal");
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

        if (lastHorizontal != Input.GetAxisRaw("P1_Horizontal"))
        {
            if (lastHorizontal < .5f && Mathf.Abs(Input.GetAxisRaw("P1_Horizontal")) > .5f) {
                Receptors[0].HandleInput(ButtonState.Released);
                if (Input.GetAxisRaw("P1_Horizontal") > 0) {
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
            
            lastHorizontal = Mathf.Abs(Input.GetAxisRaw("P1_Horizontal"));
        }

        for (int i = 0; i <= 5; i++)
        {
            if (Input.GetButtonDown("P1_B"+(i+1)))
            {
                Receptors[(i%3)+1].HandleInput(ButtonState.Pressed);
                Judge.JudgePlayerInput((i%3)+1, ButtonState.Pressed);
            }

            if (Input.GetButtonUp("P1_B"+(i+1)))
            {
                Receptors[(i%3)+1].HandleInput(ButtonState.Released);
                Judge.JudgePlayerInput((i%3)+1, ButtonState.Released);
            }
        }
    }
}
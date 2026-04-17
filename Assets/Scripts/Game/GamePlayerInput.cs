using System.Collections.Generic;
using UnityEngine;

public enum ButtonState
{
    Released,
    Pressed,
    Left,
    Right
}

public class GamePlayerInput : MonoBehaviour
{
    [SerializeField] List<Receptor> Receptors;
    [SerializeField] List<Receptor> ShipButtons;
    [SerializeField] List<ShipLaser> ShipLasers;
    [SerializeField] ShipStick ShipStick;
    [SerializeField] JostleEffect playerShip;
    [SerializeField] JostleEffect otherShips;

    float lastInput = 0f;
    float lastHorizontal = 0;
    float settingLastHorizontal = 0;
    float settingLastVertical = 0;
    float restartTimer = 0f;
    void Update()
    {
        lastInput += Time.deltaTime;

        if (Input.GetButton("P1_Start"))
        {
            // Quit/Restart macro
            if (Input.GetButton("P1_B1") && Input.GetButton("P1_B3"))
            {
                restartTimer += Time.deltaTime;
                if (restartTimer >= 1f)
                    AbandonSong();
            }
            else { restartTimer = 0f; }

            // In-game settings 
            if (settingLastVertical != Input.GetAxisRaw("P1_Vertical"))
            {
                settingLastVertical = Input.GetAxisRaw("P1_Vertical");
                if (settingLastVertical > .5)
                {
                    Maestro.LaneSpeed++;
                    InfoToasterUI.ShowToaster($"Vitesse de défilement : {Maestro.LaneSpeed}");
                }
                else if (settingLastVertical < -.5)
                {
                    Maestro.LaneSpeed--;
                    InfoToasterUI.ShowToaster($"Vitesse de défilement : {Maestro.LaneSpeed}");
                }
            }

            if (settingLastHorizontal != Input.GetAxisRaw("P1_Horizontal"))
            {
                settingLastHorizontal = Input.GetAxisRaw("P1_Horizontal");
                if (settingLastHorizontal > .5)
                {
                    Maestro.GlobalOffset += 0.005f;
                    Jukebox.SetPlaybackPosition(Maestro.SongTime - Maestro.GlobalOffset);
                    InfoToasterUI.ShowToaster($"Compensation latence audio : {(Mathf.Sign(Maestro.GlobalOffset) == 1f ? "+" : "")}{Maestro.GlobalOffset * 1000:F0}ms.");
                }

                else if (settingLastHorizontal < -.5)
                {
                    Maestro.GlobalOffset -= 0.005f;
                    Jukebox.SetPlaybackPosition(Maestro.SongTime - Maestro.GlobalOffset);
                    InfoToasterUI.ShowToaster($"Compensation latence audio : {(Mathf.Sign(Maestro.GlobalOffset) == 1f ? "+" : "")}{Maestro.GlobalOffset * 1000:F0}ms.");
                }
            }
        }
        else
        {
            if (lastHorizontal != Input.GetAxisRaw("P1_Horizontal"))
            {
                if (lastHorizontal < .5f && Mathf.Abs(Input.GetAxisRaw("P1_Horizontal")) > .5f)
                {
                    ResetLastButtonTimer();
                    Receptors[0].HandleInput(ButtonState.Released);
                    if (Input.GetAxisRaw("P1_Horizontal") > 0)
                    {
                        Judge.JudgePlayerInput(0, ButtonState.Right);
                        playerShip.Jostle(ButtonState.Right);
                        otherShips.Jostle(ButtonState.Right);
                    }
                    else
                    {
                        Judge.JudgePlayerInput(0, ButtonState.Left);
                        playerShip.Jostle(ButtonState.Left);
                        otherShips.Jostle(ButtonState.Left);
                    }
                }

                lastHorizontal = Mathf.Abs(Input.GetAxisRaw("P1_Horizontal"));
            }

            for (int i = 0; i <= 5; i++)
            {
                if (Input.GetButtonDown("P1_B" + (i + 1)))
                {
                    ResetLastButtonTimer();
                    Judge.JudgePlayerInput((i % 3) + 1, ButtonState.Pressed);
                    Receptors[(i % 3) + 1].HandleInput(ButtonState.Pressed);
                    ShipButtons[i % 3].HandleInput(ButtonState.Pressed);
                    ShipLasers[i % 3].StartShooting();
                }

                if (Input.GetButtonUp("P1_B" + (i + 1)))
                {
                    Judge.JudgePlayerInput((i % 3) + 1, ButtonState.Released);
                    Receptors[(i % 3) + 1].HandleInput(ButtonState.Released);
                    ShipButtons[i % 3].HandleInput(ButtonState.Released);
                    ShipLasers[i % 3].StopShooting();
                }
            }
        }

        if (lastInput > 10f && Scoring.MissCombo > 20) GameUIManager.ShowGiveUpKeybind();
    }

    void ResetLastButtonTimer()
    {
        lastInput = 0f;
        GameUIManager.ShowGiveUpKeybind(false);
    }

    bool hasReturnedToMenu = false;
    void AbandonSong()
    {
        if (hasReturnedToMenu) return;
        hasReturnedToMenu = true;
        GameUIManager.ReturnToSongSelect();
    }
}
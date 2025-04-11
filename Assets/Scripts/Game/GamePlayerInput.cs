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
    void Update()
    {
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
                Judge.JudgePlayerInput(i, ButtonState.Up);
            }
        }
    }
}
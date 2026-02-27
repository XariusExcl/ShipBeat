using UnityEngine;

public class ShipStick : MonoBehaviour {

    public void HandleInput(ButtonState buttonState)
    {
        if (buttonState == ButtonState.Left)
           transform.localRotation = Quaternion.Euler(20f, -20f, 0f);
        else if (buttonState == ButtonState.Right)
            transform.localRotation = Quaternion.Euler(20f, 20f, 0f);
        else 
           transform.localRotation = Quaternion.Euler(20f, 0f, 0f);

    }
}
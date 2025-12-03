using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler
{
    public Vector2 LeftStick { get; set; }
    public Vector2 RightStick { get; set; }
    public bool ButtonA_Pressed { get; set; }
    public bool ButtonB_Pressed { get; set; }
    public bool ButtonX_Pressed { get; set; }
    public bool ButtonY_Pressed { get; set; }

    public Action OnButtonB_Canceled;
    public Action OnButtonX_Canceled;
    public Action OnButtonY_Canceled;

    public void OnLeftStick(InputAction.CallbackContext context)
    {
        LeftStick = context.ReadValue<Vector2>();
    }

    public void OnRightStick(InputAction.CallbackContext context)
    {
        RightStick = context.ReadValue<Vector2>();
    }

    public void OnButtonA(InputAction.CallbackContext context)
    {
        ButtonA_Pressed = context.started;
        if (context.canceled) ButtonA_Pressed = false;
    }

    public void OnButtonB(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            ButtonB_Pressed = true;
        }

        if (context.canceled)
        {
            ButtonB_Pressed = false;
            OnButtonB_Canceled?.Invoke();
        }
    }

    public void OnButtonX(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            ButtonX_Pressed = true;
        }

        if (context.canceled)
        {
            ButtonX_Pressed = false;
            OnButtonX_Canceled?.Invoke();
        }
    }

    public void OnButtonY(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            ButtonY_Pressed = true;
        }

        if (context.canceled)
        {
            ButtonY_Pressed = false;
            OnButtonY_Canceled?.Invoke();
        }
    }
}
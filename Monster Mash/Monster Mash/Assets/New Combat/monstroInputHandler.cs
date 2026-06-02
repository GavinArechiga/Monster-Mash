using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.InputSystem;

public class monstroInputHandler : MonoBehaviour
{
    PlayerInput playerInput;
    private monstroLocomotion locomotion;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        locomotion = GetComponent<monstroLocomotion>();
    }

    public void OnMove(CallbackContext context)
    {
        locomotion.movementInput(context.ReadValue<Vector2>());
    }

    public void OnJump(CallbackContext context)
    {
        if (context.started)
        {
            locomotion.jumpInput();
        }
    }

    public void OnPause(CallbackContext context)
    {
        if (context.started)
        {
            print("pause");//this will call out to some sort of menu manager to throw up a pause screen
        }
    }

    public void switchToMonsterControls()
    {
        locomotion.enabled = true;
        playerInput.SwitchCurrentActionMap("Monstro Movement");
    }
}

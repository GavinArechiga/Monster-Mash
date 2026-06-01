using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.InputSystem;

public class monstroInputHandler : MonoBehaviour
{
    MonstroControls monstroControls;
    private monstroLocomotion locomotion;

    private void Awake()
    {
        monstroControls = new MonstroControls();
        locomotion = GetComponent<monstroLocomotion>();
    }

    public void OnMove(CallbackContext context)
    {
        locomotion.setInputVector(context.ReadValue<Vector2>());
    }

    public void OnJump(CallbackContext context)
    {
        if (context.started)
        {
            locomotion.jumpInput();
        }
    }
}

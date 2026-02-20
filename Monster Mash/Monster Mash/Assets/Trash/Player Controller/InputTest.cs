using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputTest : MonoBehaviour
{
    private PlayerInput testInput;
    void Start()
    {
        testInput = GetComponent<input_handler>().playerInput;
        testInput.actions.FindActionMap("XBOX").FindAction("A Button").started += PrintPlayerName;
    }

    // Update is called once per frame
    void PrintPlayerName(InputAction.CallbackContext context)
    {
        print(this.gameObject.name);
    }


}

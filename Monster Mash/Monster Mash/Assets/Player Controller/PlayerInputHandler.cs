using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public monsterAttackSystem myMonster;
    public PlayerInput playerInput;
    public PlayerControls playerControlsMap;

    public InputActionMap startingActionMap;
    public InputActionMap UIcontrols;
    public InputActionMap monsterControls;
    public Vector2 lastInputDirectionVector;
    public float directionThreshold = 0.2f;
    public enum InputDirection { Forward = 1, Backward = -1, Up = 2, Down = 0 }
    public InputDirection lastInputDirection = InputDirection.Forward;
    public Vector2 rightJoystickVector;
    public Vector2 leftJoystickVector;
    public float leftJoystickValue;

    public Vector2 LeftStick { get; set; }
    public Vector2 RightStick { get; set; }
    public bool ButtonA_Pressed { get; set; }
    public bool ButtonB_Pressed { get; set; }
    public bool ButtonX_Pressed { get; set; }
    public bool ButtonY_Pressed { get; set; }

    public Action OnButtonB_Canceled;
    public Action OnButtonX_Canceled;
    public Action OnButtonY_Canceled;

    private void Awake()
    {
        OnButtonB_Canceled += () =>
        {
            myMonster.attackCancel(1);
        };

        OnButtonX_Canceled += () =>
        {
            myMonster.attackCancel(2);
        };

        OnButtonY_Canceled += () =>
        {
            myMonster.attackCancel(3);
        };

        playerInput = GetComponent<PlayerInput>();
        playerControlsMap = new PlayerControls();


        if (playerInput != null)
        {
            startingActionMap = playerInput.actions.FindActionMap("Starting Action Map");
            UIcontrols = playerInput.actions.FindActionMap("UI Controls");
            monsterControls = playerInput.actions.FindActionMap("Monster Controls");

            if (UIcontrols != null)
            {
                playerInput.SwitchCurrentActionMap("UI Controls");
                playerControlsMap.StartingActionMap.Disable();
                //print("New Action Map: " + playerInput.currentActionMap);
            }
        }
    }

    private void Start()
    {
        myMonster = GetComponent<NewPlayerController>().myMonster;
    }

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

    private void OnDestroy()
    {
        UnsubscribeActionMap();
    }

    public void SubscribeActionMap()
    {
        playerInput.actions.FindActionMap("Monster Controls").FindAction("Left Stick").performed += OnLeftStick;
        playerInput.actions.FindActionMap("Monster Controls").FindAction("Left Stick").canceled += OnLeftStick;

        playerInput.actions.FindActionMap("Monster Controls").FindAction("Right Stick").performed += OnRightStick;
        playerInput.actions.FindActionMap("Monster Controls").FindAction("Right Stick").canceled += OnRightStick;

        playerInput.actions.FindActionMap("Monster Controls").FindAction("A").started += OnButtonA;
        playerInput.actions.FindActionMap("Monster Controls").FindAction("A").canceled += OnButtonA;

        playerInput.actions.FindActionMap("Monster Controls").FindAction("B").started += OnButtonB;
        playerInput.actions.FindActionMap("Monster Controls").FindAction("B").canceled += OnButtonB;

        playerInput.actions.FindActionMap("Monster Controls").FindAction("X").started += OnButtonX;
        playerInput.actions.FindActionMap("Monster Controls").FindAction("X").canceled += OnButtonX;

        playerInput.actions.FindActionMap("Monster Controls").FindAction("Y").started += OnButtonY;
        playerInput.actions.FindActionMap("Monster Controls").FindAction("Y").canceled += OnButtonY;

        playerInput.actions.FindAction("ShowMenu").Enable();
        playerInput.actions.FindAction("ShowMenu").performed += ShowRemappingMenu;
    }

    public void UnsubscribeActionMap()
    {
        if (playerInput == null) { return; }

        playerInput.actions.FindActionMap("Monster Controls").FindAction("Left Stick").performed -= OnLeftStick;
        playerInput.actions.FindActionMap("Monster Controls").FindAction("Left Stick").canceled -= OnLeftStick;

        playerInput.actions.FindActionMap("Monster Controls").FindAction("Right Stick").performed -= OnRightStick;
        playerInput.actions.FindActionMap("Monster Controls").FindAction("Right Stick").canceled -= OnRightStick;

        playerInput.actions.FindActionMap("Monster Controls").FindAction("A").started -= OnButtonA;
        playerInput.actions.FindActionMap("Monster Controls").FindAction("A").canceled -= OnButtonA;

        playerInput.actions.FindActionMap("Monster Controls").FindAction("B").started -= OnButtonB;
        playerInput.actions.FindActionMap("Monster Controls").FindAction("B").canceled -= OnButtonB;

        playerInput.actions.FindActionMap("Monster Controls").FindAction("X").started -= OnButtonX;
        playerInput.actions.FindActionMap("Monster Controls").FindAction("X").canceled -= OnButtonX;

        playerInput.actions.FindActionMap("Monster Controls").FindAction("Y").started -= OnButtonY;
        playerInput.actions.FindActionMap("Monster Controls").FindAction("Y").canceled -= OnButtonY;

        playerInput.actions.FindAction("ShowMenu").Disable();
        playerInput.actions.FindAction("ShowMenu").performed -= ShowRemappingMenu;
    }

    private void ShowRemappingMenu(InputAction.CallbackContext ctx)
    {
        if (InputRemapper.Instance == null) { return; }

        InputRemapper.Instance.ShowMenu(playerInput);
    }
}
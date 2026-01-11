using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private NewPlayerController playerController;
    private monsterAttackSystem myMonster;

    public PlayerInput playerInput;
    public PlayerControls playerControlsMap;

    private InputActionMap startingActionMap;
    private InputActionMap UIcontrols;
    private InputActionMap monsterControls;

    // player flip vars
    public enum InputDirection
    {
        None,
        Forward,
        Backward,
        Up,
        Down
    }

    public Vector2 lastInputDirectionVector;
    public float directionThreshold = 0.2f;
    public InputDirection LastInputDirection { get; set; } = InputDirection.Backward;
    private InputDirection pendingInputDirection;
    private float directionTimer = 0f;
    private float flipDelay = 0.03f;

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
        playerController = GetComponent<NewPlayerController>();
    }

    public void PlayerInputSetUp()
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

        switchActionMap("Monster Controls");

        SubscribeActionMap();
    }

    public void switchActionMap(string newActionMap)
    {
        //might cut this first section, its a bit unecessary
        if (UIcontrols != null)
        {
            if (newActionMap == "UI Controls" && playerInput.currentActionMap == UIcontrols)
            {
                return;
            }
        }

        if (playerInput != null)
        {
            playerInput.SwitchCurrentActionMap(newActionMap);
        }

        if (newActionMap == "Monster Controls")
        {
            playerController.myRigidbody.bodyType = RigidbodyType2D.Dynamic;
            playerController.monsterControllerActive = true;
        }
        else
        {
            playerController.myRigidbody.bodyType = RigidbodyType2D.Kinematic;
            playerController.monsterControllerActive = false;
        }
    }

public void OnLeftStick(InputAction.CallbackContext context)
    {
        LeftStick = context.ReadValue<Vector2>();
        leftJoystickValue = context.ReadValue<Vector2>().magnitude;

        if (context.canceled)
        {
            if (playerController.currentState is MovementState movementState)
            {
                movementState.jumpButtonReset = true;
            }

            LastInputDirection = playerController.facingRight ? InputDirection.Forward : InputDirection.Backward;
            return;
        }

        if (context.performed)
        {
            if (Mathf.Abs(leftJoystickVector.x) > directionThreshold || Mathf.Abs(leftJoystickVector.y) > directionThreshold)
            {
                lastInputDirectionVector = leftJoystickVector;
                UpdateInputDirection(lastInputDirectionVector);
                
                if (LastInputDirection is InputDirection.Forward)
                {
                    playerController.flipRightVisual();
                }
                else if (LastInputDirection is InputDirection.Backward)
                {
                    playerController.flipLeftVisual();
                }
            }
        }
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

    public void UpdateInputDirection(Vector2 directionVector)
    {
        InputDirection detectedInputDirection = InputDirection.None;

        // is the detected input more horizontal then vertical?
        if (Mathf.Abs(directionVector.x) > Mathf.Abs(directionVector.y))
        {
            // if it is more horziontal is it positive or negative and is greater then the deadzone
            if (directionVector.x > directionThreshold)
            {
                detectedInputDirection = InputDirection.Forward;
            }
            else if (directionVector.x < -directionThreshold)
            {
                detectedInputDirection = InputDirection.Backward;
            }
        }
        // is the dtected input more vertical then horizontal?
        else if (Mathf.Abs(directionVector.y) > directionThreshold)
        {
            //if it is more vertical is it positive or negative and is greater then the deadzone
            if (directionVector.y > directionThreshold)
            {
                detectedInputDirection = InputDirection.Up;
            }
            else if (directionVector.y < -directionThreshold)
            {
                detectedInputDirection = InputDirection.Down;
            }
        }

        CheckIfDirectionCommittedTo(detectedInputDirection);
    }

    // Checks if the change in input direction is being committed to(held down) and not an acidental flick.
    private void CheckIfDirectionCommittedTo(InputDirection detectedInputDirection)
    {
        // did the direction change?
        if (detectedInputDirection != LastInputDirection && detectedInputDirection != InputDirection.None)
        {
            // player changed direction before it was fully committed. Most likely an acidental flick. Resets timer
            if (pendingInputDirection != detectedInputDirection)
            {
                pendingInputDirection = detectedInputDirection;
                directionTimer = 0f;
            }

            directionTimer += Time.deltaTime;
            // player fully committed to the change in input. Update the direction
            if (directionTimer >= flipDelay)
            {
                LastInputDirection = pendingInputDirection;
            }
        }
        // The direction has not changed. Reset timmer if it is still running
        else
        {
            directionTimer = 0f;
            pendingInputDirection = InputDirection.None;
        }
    }

    public int ConvertInputDirectionToAnimationID(InputDirection inputDirection)
    {
        switch (inputDirection)
        {
            case InputDirection.Forward:
            case InputDirection.Backward:
                return 1;
            case InputDirection.Up:
                return 2;
            case InputDirection.Down:
                return 0;
        }

        return 1;
    }

    private void ShowRemappingMenu(InputAction.CallbackContext ctx)
    {
        if (InputRemapper.Instance == null) { return; }

        InputRemapper.Instance.ShowMenu(playerInput);
    }
}
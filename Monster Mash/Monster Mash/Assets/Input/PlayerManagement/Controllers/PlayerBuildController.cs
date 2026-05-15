using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerBuildController : MonoBehaviour, IPlayerController
{
    private PlayerInput playerInput;
    [SerializeField] private bool isActive = false;

    [SerializeField] public GameObject monster;

    private Vector2 lookInput;

    private float stickMin = 0.45f;

    private bool isHoldingRight = false;
    private bool isHoldingLeft = false;

    private bool isHoldingY = false;

    private bool isHoldingDPadRight = false;
    private bool isHoldingDPadLeft = false;

    private ToolWheel toolWheel;

    private BAS_Camera bas_Camera;

    private BAS_Cursor bas_cursor;
    public void ActivateController()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("BuildAScare");
        isActive = true;

        toolWheel = FindObjectOfType<ToolWheel>();
        bas_Camera = FindObjectOfType<BAS_Camera>();
        bas_cursor = FindObjectOfType<BAS_Cursor>();

        playerInput.actions["LeftStickBS"].performed += LeftStick;
        playerInput.actions["LeftStickBS"].canceled += LeftStickCancel;
        playerInput.actions["RightStickBS"].performed += RightStick;
        playerInput.actions["RightStickBS"].canceled += RightStickCancel;
        playerInput.actions["RightTriggerBS"].performed += RightTrigger;
        playerInput.actions["LeftTriggerBS"].performed += LeftTrigger;
        playerInput.actions["RightTriggerBS"].canceled += RightTriggerCancel;
        playerInput.actions["LeftTriggerBS"].canceled += LeftTriggerCancel;
        playerInput.actions["A_BS"].performed += ClickA;
        playerInput.actions["B_BS"].performed += ClickB;
        playerInput.actions["X_BS"].performed += ClickX;
        playerInput.actions["Y_BS"].performed += ClickY;
        playerInput.actions["Y_BS"].canceled += CancelY;
        playerInput.actions["RightBumperBS"].performed += RightBumper;
        playerInput.actions["LeftBumperBS"].performed += LeftBumper;
        playerInput.actions["DPadRightBS"].performed += DPadRight;
        playerInput.actions["DPadRightBS"].canceled += DPadRightCancel;
        playerInput.actions["DPadLeftBS"].performed += DPadLeft;
        playerInput.actions["DPadLeftBS"].canceled += DPadLeftCancel;
    }

    public void DeactivateController()
    {
        isActive = false;

        playerInput.actions["LeftStickBS"].performed -= LeftStick;
        playerInput.actions["LeftStickBS"].canceled -= LeftStickCancel;
        playerInput.actions["RightStickBS"].performed -= RightStick;
        playerInput.actions["RightStickBS"].canceled -= RightStickCancel;
        playerInput.actions["RightTriggerBS"].performed -= RightTrigger;
        playerInput.actions["LeftTriggerBS"].performed -= LeftTrigger;
        playerInput.actions["RightTriggerBS"].canceled -= RightTriggerCancel;
        playerInput.actions["LeftTriggerBS"].canceled -= LeftTriggerCancel;
        playerInput.actions["A_BS"].performed -= ClickA;
        playerInput.actions["B_BS"].performed -= ClickB;
        playerInput.actions["X_BS"].performed -= ClickX;
        playerInput.actions["Y_BS"].performed -= ClickY;
        playerInput.actions["Y_BS"].canceled -= CancelY;
        playerInput.actions["RightBumperBS"].performed -= RightBumper;
        playerInput.actions["LeftBumperBS"].performed -= LeftBumper;
        playerInput.actions["DPadRightBS"].performed -= DPadRight;
        playerInput.actions["DPadRightBS"].canceled -= DPadRightCancel;
        playerInput.actions["DPadLeftBS"].performed -= DPadLeft;
        playerInput.actions["DPadLeftBS"].canceled -= DPadLeftCancel;
    }

    private void LeftStick(InputAction.CallbackContext context)
    {
        bas_cursor.LeftStickMove(context.ReadValue<Vector2>());
    }

    private void LeftStickCancel(InputAction.CallbackContext context)
    {
        bas_cursor.LeftStickMove(Vector2.zero);
    }

    private void RightStick(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
        lookInput.x = Deadzone(lookInput.x);
        lookInput.y = Deadzone(lookInput.y);
    }

    private void RightStickCancel(InputAction.CallbackContext context)
    {
        lookInput = Vector2.zero;
    }

    private void ClickA(InputAction.CallbackContext context)
    {
        bas_cursor.AButton();
    }
    private void ClickB(InputAction.CallbackContext context)
    {
        bas_cursor.BButton();
    }

    private void ClickX(InputAction.CallbackContext context)
    {
        bas_cursor.MirrorPreview();
    }
    private void ClickY(InputAction.CallbackContext context)
    {
        isHoldingY = true;
    }

    private void CancelY(InputAction.CallbackContext context)
    {
        isHoldingY = false;
    }

    private void Update()
    {
        RotateMonster();
        CameraZoom();
        RotatePreview();

        if (isHoldingDPadRight)
        {
            bas_cursor.DPadRight();
        }

        if (isHoldingDPadLeft)
        {
            bas_cursor.DPadLeft();
        }
    }

    private void RotateMonster()
    {
        if (bas_Camera) { bas_Camera.RotateMonster(lookInput); }
    }

    private float Deadzone(float x)
    {
        if (x > 0f)
        {
            if (x < stickMin)
            {
                x = 0f;
            }
        }
        else if (x < 0f)
        {
            if (x > -stickMin)
            {
                x = 0f;
            }
        }

        return x;
    }

    private void RightTrigger(InputAction.CallbackContext context)
    {
        isHoldingRight = true;
    }

    private void RightTriggerCancel(InputAction.CallbackContext context)
    {
        isHoldingRight = false;
    }

    private void LeftTrigger(InputAction.CallbackContext context)
    {
        isHoldingLeft = true;
    }

    private void LeftTriggerCancel(InputAction.CallbackContext context)
    {
        isHoldingLeft = false;
    }
    private void CameraZoom()
    {
        if (bas_Camera)
        {
            if (isHoldingRight)
            {
                bas_Camera.CameraZoomIn();
            }

            if (isHoldingLeft)
            {
                bas_Camera.CameraZoomOut();
            }
        }
    }

    private void RotatePreview()
    {
        if (bas_cursor)
        {
            if (isHoldingY)
            {
                bas_cursor.RotatePreview();
            }
        }
    }

    private void RightBumper(InputAction.CallbackContext context)
    {
        bas_cursor.ToolWheelRight();
    }

    private void LeftBumper(InputAction.CallbackContext context)
    {
        bas_cursor.ToolWheelLeft();
    }

    private void DPadRight(InputAction.CallbackContext context)
    {
        isHoldingDPadRight = true;
    }

    private void DPadRightCancel(InputAction.CallbackContext context)
    {
        isHoldingDPadRight = false;
    }

    private void DPadLeft(InputAction.CallbackContext context)
    {
        isHoldingDPadLeft = true;
    }

    private void DPadLeftCancel(InputAction.CallbackContext context)
    {
        isHoldingDPadLeft = false;
    }
}
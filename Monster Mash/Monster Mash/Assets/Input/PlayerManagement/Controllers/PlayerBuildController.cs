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
    [SerializeField] private GameObject crosshair;
    private Camera cam;
    private RectTransform canvas;

    private Vector2 lookInput;
    private float currentAngle = 0f;
    private float minAngle = -75f;
    private float maxAngle = 75f;

    private float stickMin = 0.45f;

    private float rotSpeed = 100f;

    [SerializeField] private RectTransform crossHair;
    private float crosshairSpeed = 300f;
    private Vector2 crosshairMove;
    public void ActivateController()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("BuildAScare");
        isActive = true;

        monster = GameObject.Find("TestTorsos").transform.GetChild(0).gameObject;
        cam = Camera.main;
        crossHair = GameObject.Find("crossHair").GetComponent<RectTransform>();
        canvas = FindObjectOfType<Canvas>().GetComponent<RectTransform>();

        playerInput.actions["LeftStickBS"].performed += LeftStick;
        playerInput.actions["LeftStickBS"].canceled += LeftStickCancel;
        playerInput.actions["RightStickBS"].performed += RightStick;
        playerInput.actions["RightStickBS"].canceled += RightStickCancel;
    }

    public void DeactivateController()
    {
        isActive = false;

        playerInput.actions["LeftStickBS"].performed -= LeftStick;
        playerInput.actions["LeftStickBS"].canceled -= LeftStickCancel;
        playerInput.actions["RightStickBS"].performed -= RightStick;
        playerInput.actions["RightStickBS"].canceled -= RightStickCancel;
    }

    private void LeftStick(InputAction.CallbackContext context)
    {
        crosshairMove = context.ReadValue<Vector2>();
    }

    private void LeftStickCancel(InputAction.CallbackContext context)
    {
        crosshairMove = Vector2.zero;
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

    private void Update()
    {
        RotateMonster();
        MoveCrosshair();
    }

    private void RotateMonster()
    {
        float delta = lookInput.y * rotSpeed * Time.deltaTime;

        float newAngle = Mathf.Clamp(currentAngle + delta, minAngle, maxAngle);

        float appliedRotation = newAngle - currentAngle;

        cam.transform.RotateAround(monster.transform.position, Vector3.right * 0.5f, appliedRotation);
        monster.transform.Rotate(Vector3.up, lookInput.x * -rotSpeed * Time.deltaTime, Space.World);

        currentAngle = newAngle;
    }

    private void MoveCrosshair()
    {
        crossHair.anchoredPosition += crosshairMove * crosshairSpeed * Time.deltaTime;

        Vector2 pos = crossHair.anchoredPosition;

        float xOffset = canvas.rect.xMax / 10f;
        float yOffset = canvas.rect.yMax / 5f;
        print("xmax: " + canvas.rect.xMax);
        print("ymax: " + canvas.rect.yMax);
        pos.x = Mathf.Clamp(pos.x, canvas.rect.xMin + xOffset, canvas.rect.xMax - xOffset);
        pos.y = Mathf.Clamp(pos.y, canvas.rect.yMin + yOffset, canvas.rect.yMax - yOffset);

        crossHair.anchoredPosition = pos;
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
}
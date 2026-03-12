using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBuildController : MonoBehaviour, IPlayerController
{
    private PlayerInput playerInput;
    [SerializeField] private bool isActive = false;

    [SerializeField] public GameObject monster;
    [SerializeField] private GameObject crosshair;
    private Camera cam;

    private Vector2 lookInput;
    private float currentAngle = 0f;
    private float minAngle = -75f;
    private float maxAngle = 75f;

    private float rotSpeed = 200f;
    public void ActivateController()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("BuildAScare");
        isActive = true;

        monster = GameObject.Find("TestTorsos").transform.GetChild(0).gameObject;
        cam = Camera.main;

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

    }

    private void LeftStickCancel(InputAction.CallbackContext context)
    {
        
    }

    private void RightStick(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    private void RightStickCancel(InputAction.CallbackContext context)
    {
        lookInput = Vector2.zero;
    }

    private void Update()
    {
        float delta = lookInput.y * rotSpeed * Time.deltaTime;

        float newAngle = Mathf.Clamp(currentAngle + delta, minAngle, maxAngle);

        float appliedRotation = newAngle - currentAngle;

        cam.transform.RotateAround(monster.transform.position, -Vector3.right * 0.5f, appliedRotation);
        monster.transform.Rotate(Vector3.up, lookInput.x * -rotSpeed * Time.deltaTime, Space.World);

        currentAngle = newAngle;
    }
}
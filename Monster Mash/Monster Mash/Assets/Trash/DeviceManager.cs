using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeviceManager : MonoBehaviour
{
    public GameObject playerPrefab;

    private void Start()
    {
        InputSystem.onDeviceChange += OnDeviceChange;

        foreach (var device in InputSystem.devices)
        {
            Debug.Log($"Connected device: {device.name}");
        }
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        Debug.Log($"Device change detected: {device.name}, Change: {change}");
        if (change == InputDeviceChange.Added)
        {
            Debug.Log($"Device added: {device.name}");
            // If a new gamepad or keyboard is connected, assign it to a player
            if (device is Gamepad || device is Keyboard)
            {
                print("called1");
                JoinPlayer(device);
            }
        }
    }

    private void JoinPlayer(InputDevice device)
    {
        // Determine the control scheme based on the device type
        string controlScheme = device is Gamepad ? "Gamepad" : "Keyboard";

        // Instantiate the player with the appropriate control scheme and device
        var player = PlayerInput.Instantiate(playerPrefab, controlScheme: controlScheme, pairWithDevice: device);
        // Additional setup for the player if needed
    }
}
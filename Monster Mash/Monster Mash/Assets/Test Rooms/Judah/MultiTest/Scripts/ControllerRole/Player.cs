using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public ControllerRole currentRole;
    [SerializeField] private Transform roleSocket;
    public PlayerInput playerInput;
    public ControllerRole gameplayRolePrefab;
    public ControllerRole uiRolePrefab;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }
    public void SwitchRole(ControllerRole newRolePrefab, string actionMapName)
    {
        if (currentRole != null)
        {
            currentRole.OnDeactivate();
            Destroy(currentRole.gameObject);
        }

        currentRole = Instantiate(newRolePrefab, roleSocket);
        currentRole.Initialize(playerInput);

        playerInput.SwitchCurrentActionMap(actionMapName);

        currentRole.OnActivate();
    }

    //Input Forwarding to children/roles
    private void OnMove(InputValue value)
    {
        currentRole?.SendMessage("OnMove", value, SendMessageOptions.DontRequireReceiver);
    }

    private void OnClick()
    {
        currentRole?.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
    }
}

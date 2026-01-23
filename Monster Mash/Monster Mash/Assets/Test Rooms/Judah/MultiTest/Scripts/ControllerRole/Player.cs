using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public ControllerRole currentRole;
    public Transform roleSocket;
    public PlayerInput playerInput;
    public ControllerRole gameplayerControllerPrefab;
    public ControllerRole uiControllerPrefab;

    public void SwitchRole(ControllerRole newRolePrefab, string actionMapName)
    {
        if (currentRole != null)
        {
            currentRole.OnDeactivate();
            Destroy(currentRole.gameObject);
        }

        currentRole = Instantiate(newRolePrefab, roleSocket);

        playerInput.SwitchCurrentActionMap(actionMapName);

        currentRole.OnActivate();
    }

}

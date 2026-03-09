using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPressToJoinController : MonoBehaviour, IPlayerController
{
    PlayerInput playerInput;

    private bool isActive = false;

    PlayerManager manager;

    Player player;

    public void ActivateController()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("PressToJoin");
        isActive = true;
        manager = FindObjectOfType<PlayerManager>();
        player = playerInput.GetComponent<Player>();

        playerInput.actions["AnyButtonJoin"].performed += Join;
    }

    public void DeactivateController()
    {
        playerInput.actions["AnyButtonJoin"].performed -= Join;
        isActive = false;
    }

    private void Join(InputAction.CallbackContext context)
    {
        player.SwitchControllerAndDestroyOld(manager.Controller.characterSelectController);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerManager : MonoBehaviour
{
    //this script manages all action map switching. any time a different script would need to do so, it must pass through here.

    //all actionMap controller prefabs are listed below, as of now there are only 2//////////////
    public GameObject uiController;
    public GameObject combatController;
    ///////////////////////////

    private PlayerManager manager;
    private void Awake()
    {
        manager = GetComponent<PlayerManager>();
    }

    public void SwitchOneController(PlayerInput playerInput, GameObject newController, bool destroyOldControllers) //pass in one player and switch its action map
    {
        Player player = playerInput.GetComponent<Player>();

        player.SwitchController(newController, destroyOldControllers);
    }
    public void SwitchAllControllers(GameObject controllerPrefab, bool destroyOldControllers) //change action maps for all current players in game
    {
        List<PlayerInput> playerInputs = manager.GetPlayers();

        for (int i = 0; i < playerInputs.Count; i++)
        {
            Player player = playerInputs[i].GetComponent<Player>();

            player.SwitchController(controllerPrefab, destroyOldControllers);
        }
    }
}

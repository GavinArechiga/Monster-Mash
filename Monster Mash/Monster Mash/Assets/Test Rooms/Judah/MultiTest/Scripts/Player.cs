using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //this script is for the player prefab created from PlayerJoinManager.
    //basically just gives each player instance the knowledge of its own current action map and the logic to switch it.
    //the switch will always be called from PlayerControllerManager

    private List<GameObject> currentControllers = new List<GameObject>(); //can have multiple at once. solely for pausing during combat.

    [SerializeField] private Transform spawn;

    //called from PlayerControllerManager to switch action maps.
    //new action maps are paired with an instantiated controller prefab designed
    //for the action map needs. the actual SwitchActionMap function is called from the controller script attached to each of these instantiated prefabs
    public void SwitchController(GameObject controllerPrefab, bool destroyCurrentController)
    {
        if (currentControllers.Count > 0)
        {
            if (destroyCurrentController)
            {
                DestroyAllControllers();
            }

            DestroyDuplicates(controllerPrefab);
        }

        GameObject currentController = Instantiate(controllerPrefab, spawn);
        currentControllers.Add(currentController);
    }
    private void DestroyAllControllers()
    {
        for (int i = currentControllers.Count - 1; i >= 0; i--) //loop backwards for List stuff
        {
            Destroy(currentControllers[i]);
            currentControllers.RemoveAt(i);
        }
    }

    private void DestroyDuplicates(GameObject controllerPrefab)
    {
        for (int i = currentControllers.Count - 1; i >= 0; i--)
        {
            if (currentControllers[i] == controllerPrefab)
            {
                Destroy(currentControllers[i]);
                currentControllers.RemoveAt(i);
            }
        }
    }
}

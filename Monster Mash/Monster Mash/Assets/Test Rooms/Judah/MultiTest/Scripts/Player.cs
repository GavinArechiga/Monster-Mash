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
    public void SwitchController(GameObject controllerPrefab)
    {
        if (currentControllers.Count > 0)
        {
            var controllerType = controllerPrefab.GetComponent<IPlayerController>().GetType();
            var existingController = currentControllers.Find(c => c.GetComponent(controllerType) != null);

            if (existingController != null)
            {
                existingController.GetComponent<IPlayerController>().ActivateController();

                DeactivateAllExcept(existingController);
                return;
            }
        }

        GameObject currentController = Instantiate(controllerPrefab, spawn);
        currentControllers.Add(currentController);
        DeactivateAllExcept(currentController);
    }

    public void DestroyControllerOfType<T>() where T : IPlayerController
    {
        for (int i = currentControllers.Count - 1; i >= 0; i--)
        {
            if (currentControllers[i].GetComponent<T>() != null)
            {
                Destroy(currentControllers[i]);
                currentControllers.RemoveAt(i);
            }
        }
    }
    public void DestroyAllControllers()
    {
        for (int i = currentControllers.Count - 1; i >= 0; i--) //loop backwards for List stuff
        {
            Destroy(currentControllers[i]);
            currentControllers.RemoveAt(i);
        }
    }

    private void DeactivateAllExcept(GameObject activeController)
    {
        for (int i = 0; i < currentControllers.Count; i++)
        {
            if (currentControllers[i] != activeController)
            {
                currentControllers[i].GetComponent<IPlayerController>().DeactivateController();
            }
        }
    }
}

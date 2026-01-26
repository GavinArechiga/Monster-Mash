using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    //public ControllerRole currentRole;
    public GameObject currentController;
    [SerializeField] private Transform spawn;
    public PlayerInput playerInput;
    public GameObject combatControllerPrefab;
    public GameObject uiControllerPrefab;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }
    public void SwitchRole(GameObject controllerPrefab)
    {
        if (currentController != null)
        {
            Destroy(currentController);
        }
        currentController = Instantiate(controllerPrefab, spawn);
    }
}

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

    //Input Forwarding to children/roles
    /*private void ForwardInputAction(string method, object value = null)
    {
        if (currentController == null) return;

        if (value == null)
        {
            currentController.SendMessage(method, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            currentController.SendMessage(method, value, SendMessageOptions.DontRequireReceiver);
        }
    }

    #region UI map
    public void OnNavigate()
    {
        ForwardInputAction("OnNavigate");
    }

    public void OnSubmit()
    {
        ForwardInputAction("OnSubmit");
    }
    public void OnCancel()
    {
        ForwardInputAction("OnCancel");
    }

    #endregion

    #region Gameplay Map

    public void OnMove(InputValue input)
    {
        ForwardInputAction("OnMove", input.Get<Vector2>());
    }

    #endregion*/
}

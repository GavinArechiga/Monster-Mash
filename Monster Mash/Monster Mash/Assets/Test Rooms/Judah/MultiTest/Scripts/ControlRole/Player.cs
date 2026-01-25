using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public ControllerRole currentRole;
    [SerializeField] private Transform roleSocket;
    public PlayerInput playerInput;
    public ControllerRole combatRolePrefab;
    public ControllerRole uiRolePrefab;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }
    public void SwitchRole(ControllerRole newRolePrefab)
    {
        if (currentRole != null)
        {
            Destroy(currentRole.gameObject);
        }

        currentRole = Instantiate(newRolePrefab, roleSocket);
        currentRole.Initialize(playerInput);

        currentRole.ApplyInput();
        currentRole.OnActivate();
    }

    //Input Forwarding to children/roles

    #region UI map
    public void OnNavigate()
    {
        currentRole?.SendMessage("OnNavigate", SendMessageOptions.DontRequireReceiver);
    }

    public void OnSubmit()
    {
        currentRole?.SendMessage("OnSubmit", SendMessageOptions.DontRequireReceiver);
    }
    public void OnCancel()
    {
        currentRole?.SendMessage("OnCancel", SendMessageOptions.DontRequireReceiver);
    }

    #endregion

    #region Gameplay Map

    public void OnMove(InputValue input)
    {
        currentRole?.SendMessage("OnMove", input.Get<Vector2>(), SendMessageOptions.DontRequireReceiver);
    }

    #endregion
}

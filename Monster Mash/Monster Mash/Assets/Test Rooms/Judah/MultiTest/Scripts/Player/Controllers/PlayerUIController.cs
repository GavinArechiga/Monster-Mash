using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerUIController : MonoBehaviour, IPlayerController
{
    PlayerInput playerInput;

    [SerializeField] private float moveSpeed = 5f;
    private Vector2 moveInput;

    [SerializeField]
    private bool clickerMode = false; //mouse or hand, player is in menu navigation mode, not actively a monster or in combat

    private bool isActive = true;

    #region (De)ActivateController
    public void ActivateController()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("UI");
        isActive = true;

        playerInput.actions["Pause"].performed += OnPause;
        playerInput.actions["Navigate"].performed += OnNavigate;
    }

    public void DeactivateController()
    {
        isActive = false;

        playerInput.actions["Pause"].performed -= OnPause;
        playerInput.actions["Navigate"].performed -= OnNavigate;
    }
    #endregion

    #region input actions
    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (!isActive) return;
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (!isActive) return;

        Debug.Log("toggle pause from ui");
        PauseManager.Instance?.TogglePause();
    }

    #endregion
}

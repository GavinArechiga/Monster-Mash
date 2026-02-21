using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerCombatControllerTemp : MonoBehaviour, IPlayerController
{
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 moveInput;

    PlayerInput playerInput;

    private bool isActive = true;
    void Update()
    {
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);
        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);
    }
    public string GetActionMap()
    {
        return "Combat";
    }

    #region (De)ActivateController
    public void ActivateController()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("Combat");
        isActive = true;

        playerInput.actions["Move"].performed += OnMove;
        playerInput.actions["Pause"].performed += OnPause;
    }

    public void DeactivateController()
    {
        isActive = false;

        playerInput.actions["Move"].performed -= OnMove;
        playerInput.actions["Pause"].performed -= OnPause;
    }
    #endregion

    #region input actions
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!isActive) return;

        moveInput = context.ReadValue<Vector2>();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (!isActive) return;

        Debug.Log("TogglePause from Combat");
        PauseManager.Instance?.TogglePause();
    }

    #endregion
}

using UnityEngine;
using UnityEngine.InputSystem;
public class CombatControllerTemp : MonoBehaviour, IPlayerController
{
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 moveInput;

    PlayerInput playerInput;

    private bool isActive = true;

    private void Awake()
    {
        isActive = true;
    }
    private void Start()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("Combat");

        Debug.Log(playerInput.currentActionMap);
    }
    void Update()
    {
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);
        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);
    }
    public string GetActionMap()
    {
        return "Combat";
    }

    public void ActivateController()
    {
        playerInput.SwitchCurrentActionMap("Combat");
        isActive = true;
    }

    public void DeactivateController()
    {
        isActive = false;
    }

    #region input actions called from PlayerInput
    public void OnMove(InputValue value)
    {
        if (!isActive) return;

        moveInput = value.Get<Vector2>();
    }

    public void OnPause()
    {
        if (!isActive) return;

        Debug.Log("TogglePause from Combat");
        PauseManager.Instance?.TogglePause();
    }

    #endregion
}

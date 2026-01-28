using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UIControllerTemp : MonoBehaviour, IPlayerController
{
    PlayerInput playerInput;

    [SerializeField] private float moveSpeed = 5f;
    private Vector2 moveInput;

    [SerializeField]
    private bool clickerMode = false; //mouse or hand, player is in menu navigation mode, not actively a monster or in combat

    private bool isActive = true;

    private void Awake()
    {
        isActive = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("UI");

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(FindObjectOfType<FirstSelectedUI>().gameObject);
        }
    }

    public void ActivateController()
    {
        playerInput.SwitchCurrentActionMap("UI");
        isActive = true;
    }

    public void DeactivateController()
    {
        isActive = false;
    }
    public void Click()
    {
        Debug.Log("I PUSHED THE BUTTON HAHAHA");
    }

    #region input actions called from PlayerInput
    public void OnNavigate()
    {
        if (!isActive) return;
    }

    public void OnPause()
    {
        if (!isActive) return;

        Debug.Log("toggle pause from ui");
        PauseManager.Instance?.TogglePause();
    }

    #endregion
}

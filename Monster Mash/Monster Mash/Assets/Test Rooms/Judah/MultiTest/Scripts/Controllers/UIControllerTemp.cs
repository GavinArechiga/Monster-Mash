using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UIControllerTemp : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 moveInput;

    [SerializeField]
    private bool clickerMode = false; //mouse or hand, player is in menu navigation mode, not actively a monster or in combat

    // Start is called before the first frame update
    void Start()
    {
        PlayerInput playerInput = GetComponentInParent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("UI");

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(FindObjectOfType<FirstSelectedUI>().gameObject);
        }
    }
    public void Click()
    {
        Debug.Log("I PUSHED THE BUTTON HAHAHA");
    }

    #region input actions called from Player script
    public void OnNavigate()
    {

    }

    #endregion
}

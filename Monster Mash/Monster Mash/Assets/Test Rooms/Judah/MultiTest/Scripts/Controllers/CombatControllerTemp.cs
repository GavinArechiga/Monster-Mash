using UnityEngine;
using UnityEngine.InputSystem;
public class CombatControllerTemp : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 moveInput;
    private void Start()
    {
        PlayerInput playerInput = GetComponentInParent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("Combat");

        Debug.Log(playerInput.currentActionMap);
    }
    void Update()
    {
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);
        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);
    }

    public void OnMove(Vector2 input)
    {
        moveInput = input;
    }

    public string GetActionMap()
    {
        return "Combat";
    }
}

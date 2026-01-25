using UnityEngine;
using UnityEngine.InputSystem;

public abstract class ControllerRole : MonoBehaviour
{
    public PlayerInput playerInput;
    public virtual void Initialize(PlayerInput input)
    {
        playerInput = input;
    }
    public abstract string ActionMap { get; }
    public virtual void ApplyInput()
    {
        playerInput.SwitchCurrentActionMap(ActionMap);
    }

    public abstract void OnActivate();
}

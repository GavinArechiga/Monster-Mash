using UnityEngine;
using UnityEngine.InputSystem;

public abstract class ControllerRole : MonoBehaviour
{
    public PlayerInput playerInput;

    public virtual void Initialize(PlayerInput input)
    {
        playerInput = input;
    }
    public abstract void OnActivate();
    public abstract void OnDeactivate();
}

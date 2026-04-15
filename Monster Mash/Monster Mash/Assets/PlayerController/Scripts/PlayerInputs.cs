using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerInputs : MonoBehaviour, IPlayerController
{
	//IPlayerController values
	private bool isActive = false; //for pausing controls
	PlayerInput playerInput;

	[Header("Character Input Values")] public Vector2 move;
	public bool jump;

	[Header("Movement Settings")] public bool analogMovement;

	public void ActivateController()
	{
		playerInput = GetComponentInParent<PlayerInput>();
		playerInput.SwitchCurrentActionMap("Monster Controls");
		isActive = true;

		playerInput.actions["Left Stick"].performed += LeftStick;
		playerInput.actions["Left Stick"].canceled += LeftStick;
		playerInput.actions["A"].performed += A;
		playerInput.actions["A"].canceled += A;
	}

	public void DeactivateController()
	{
		isActive = false;

		playerInput.actions["Left Stick"].performed -= LeftStick;
		playerInput.actions["Left Stick"].canceled -= LeftStick;
		playerInput.actions["A"].performed -= A;
		playerInput.actions["A"].canceled -= A;
	}

	public void LeftStick(InputAction.CallbackContext context)
	{
		MoveInput(context.ReadValue<Vector2>());
	}

	public void A(InputAction.CallbackContext context)
	{
		JumpInput(context.performed);
	}


	public void MoveInput(Vector2 newMoveDirection)
	{
		move = newMoveDirection;
	}

	public void JumpInput(bool newJumpState)
	{
		jump = newJumpState;
	}
}
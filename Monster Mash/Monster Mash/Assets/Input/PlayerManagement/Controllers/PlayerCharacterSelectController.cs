using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerCharacterSelectController : MonoBehaviour, IPlayerController
{
    PlayerInput playerInput;
    Player player;
    private bool isActive = false;

    public RectTransform myCursor;
    private CursorManager cManager;
    private RectTransform canvas;
    public GameObject window;
    private PlayerManager manager;

    private Vector2 move = new Vector2();
    private float speed = 400f;
    public void ActivateController()
    {
        print("select controller activate");
        playerInput = GetComponentInParent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("CharacterSelect");
        isActive = true;
        player = GetComponentInParent<Player>();
        manager = FindObjectOfType<PlayerManager>();
        cManager = FindObjectOfType<CursorManager>();
        cManager.AddPlayer(this);
        canvas = cManager.canvas;

        playerInput.actions["NavigateCS"].performed += Navigate;
        playerInput.actions["NavigateCS"].canceled += NavigateCancel;
        playerInput.actions["SubmitCS"].performed += Submit;
        playerInput.actions["CancelCS"].performed += Cancel;
    }

    public void DeactivateController()
    {
        playerInput.actions["NavigateCS"].performed -= Navigate;
        playerInput.actions["NavigateCS"].canceled -= NavigateCancel;
        playerInput.actions["SubmitCS"].performed -= Submit;
        playerInput.actions["CancelCS"].performed -= Cancel;
        cManager.RemovePlayer(this);
        isActive = false;
    }

    private void Navigate(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    private void NavigateCancel(InputAction.CallbackContext context)
    {
        move = Vector2.zero;
    }

    private void Submit(InputAction.CallbackContext context)
    {

    }

    private void Cancel(InputAction.CallbackContext context)
    {
        player.SwitchControllerAndDestroyOld(manager.Controller.pressToJoinController);
    }

    private void Move()
    {
        myCursor.anchoredPosition += move * speed * Time.deltaTime;

        Vector2 pos = myCursor.anchoredPosition;

        pos.x = Mathf.Clamp(pos.x, canvas.rect.xMin, canvas.rect.xMax);
        pos.y = Mathf.Clamp(pos.y, canvas.rect.yMin, canvas.rect.yMax);

        myCursor.anchoredPosition = pos;
    }

    private void Update()
    {
        Move();
    }
}

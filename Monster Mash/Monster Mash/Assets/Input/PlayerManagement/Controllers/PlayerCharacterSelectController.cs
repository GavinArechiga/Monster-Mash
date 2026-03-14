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
    [SerializeField] public GameObject selection;
    public Transform selectionPos;
    [SerializeField] private Vector3 selectionOGPos;

    private SceneController sceneController;

    private Vector2 move = new Vector2();
    private float speed = 400f;
    public void ActivateController()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("CharacterSelect");
        isActive = true;
        player = GetComponentInParent<Player>();
        manager = FindObjectOfType<PlayerManager>();
        cManager = FindObjectOfType<CursorManager>();
        cManager.AddPlayer(this);
        canvas = cManager.canvas;
        sceneController = FindObjectOfType<SceneController>();

        playerInput.actions["NavigateCS"].performed += Navigate;
        playerInput.actions["NavigateCS"].canceled += NavigateCancel;
        playerInput.actions["SubmitCS"].performed += Submit;
        playerInput.actions["CancelCS"].performed += Cancel;
        playerInput.actions["ConfirmCS"].performed += Confirm;

        cManager.CheckAllPlayersSelected();
    }

    public void DeactivateController()
    {
        playerInput.actions["NavigateCS"].performed -= Navigate;
        playerInput.actions["NavigateCS"].canceled -= NavigateCancel;
        playerInput.actions["SubmitCS"].performed -= Submit;
        playerInput.actions["CancelCS"].performed -= Cancel;
        playerInput.actions["ConfirmCS"].performed -= Confirm;
        cManager?.RemovePlayer(this);
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
        RaycastHit hit;

        Vector3 worldPos = myCursor.position;
        Ray myRay = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(worldPos));

        if (Physics.Raycast(myRay, out hit, 1000f))
        {
            if (hit.transform.GetComponent<CapsuleCollider>())
            {
                Select(hit.transform.gameObject);
            }
            else if (hit.transform.GetComponent<BoxCollider>() && hit.transform.GetComponent<BoxCollider>().enabled)
            {
                if (hit.transform.gameObject.name == "Random")
                {
                    if (cManager.thisIsCharacterSelect  )
                    {
                        Select(cManager.RandomCharacter());
                    }
                    else
                    {
                        Select(null);
                    }
                }
                else if (hit.transform.gameObject.name == "BackButton")
                {
                    hit.transform.GetComponent<BoxCollider>().enabled = false;
                    sceneController.LoadScene(1);
                }
            }
        }
    }

    private void Cancel(InputAction.CallbackContext context)
    {
        if (selection)
        {
            selection.transform.position = selectionOGPos;
            selection.GetComponent<CapsuleCollider>().enabled = true;
            selection = null;
            cManager.CheckAllPlayersSelected();
        }
        else
        {
            player.SwitchControllerAndDestroyOld(manager.Controller.pressToJoinController);
        }
    }

    private void Confirm(InputAction.CallbackContext context)
    {
        if (cManager.confirmButton.activeSelf)
        {
            cManager.confirmButton.SetActive(false);
            print("if i see this duplicated then scene loaded more than once :(");
            cManager.Confirmed();
        }
    }
    private void Move()
    {
        if (myCursor)
        {
            myCursor.anchoredPosition += move * speed * Time.deltaTime;

            Vector2 pos = myCursor.anchoredPosition;

            pos.x = Mathf.Clamp(pos.x, canvas.rect.xMin, canvas.rect.xMax);
            pos.y = Mathf.Clamp(pos.y, canvas.rect.yMin, canvas.rect.yMax);

            myCursor.anchoredPosition = pos;
        }
    }

    private void Update()
    {
        Move();
        Vector3 worldPos = myCursor.position;
        Ray myRay = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(worldPos));
        Debug.DrawRay(myRay.origin, myRay.direction * 1000f, Color.red);
    }

    private void Select(GameObject obj)
    {
        if (obj)
        {
            if (selection)
            {
                selection.transform.position = selectionOGPos;
                selection.GetComponent<CapsuleCollider>().enabled = true;
            }

            selection = obj;
            selectionOGPos = selection.transform.position;
            selection.transform.position = selectionPos.position;
            selection.GetComponent<CapsuleCollider>().enabled = false;
            cManager.CheckAllPlayersSelected();
            cManager.ResetCursorPos(this);

        }
    }
}

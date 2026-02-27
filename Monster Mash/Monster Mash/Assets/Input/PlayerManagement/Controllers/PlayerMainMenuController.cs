using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerMainMenuController : MonoBehaviour, IPlayerController
{
    PlayerInput playerInput;

    private bool isActive;

    GameObject cam;

    [SerializeField] private House[] house; //defined in MainMenuNav script

    [SerializeField] private int currLevel = 0; //index for house
    [SerializeField] private int currRoom = 1; //index for house

    private Vector2 bounds1;
    private Vector2 bounds2;
    [SerializeField] private Vector3 move;

    [SerializeField] private bool justUsedFreeCam = false;
    private GameObject lastSelectedButton;

    #region (De)Activate Controller
    public void ActivateController()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("MainMenu");
        isActive = true;


        cam = Camera.main.gameObject;

        house = FindObjectOfType<MainMenuNav>().GetHouse();

        currLevel = 0;
        currRoom = 1;

        MoveCamera();
        SelectButton();
        FindBounds();

        lastSelectedButton = EventSystem.current.currentSelectedGameObject;
    }

    public void DeactivateController()
    {
        isActive = false;
    }

    private void OnEnable()
    {
        ActivateController();
        playerInput.actions["DPadRight"].performed += DPadRight;
        playerInput.actions["DPadLeft"].performed += DPadLeft;
        playerInput.actions["DPadUp"].performed += DPadUp;
        playerInput.actions["DPadDown"].performed += DPadDown;
        playerInput.actions["RightStick"].performed += RightStickPerformed;
        playerInput.actions["RightStick"].canceled += RightStickCanceled;
        playerInput.actions["Navigate1"].performed += LeftStickPerformed;
    }

    private void OnDisable()
    {
        playerInput.actions["DPadRight"].performed -= DPadRight;
        playerInput.actions["DPadLeft"].performed -= DPadLeft;
        playerInput.actions["DPadUp"].performed -= DPadUp;
        playerInput.actions["DPadDown"].performed -= DPadDown;
        playerInput.actions["RightStick"].performed -= RightStickPerformed;
        playerInput.actions["RightStick"].canceled -= RightStickCanceled;
        playerInput.actions["Navigate1"].performed -= LeftStickPerformed;
    }

    #endregion

    #region Input Actions

    private void DPadRight (InputAction.CallbackContext context)
    {
        ChangeRooms("right");
    }

    private void DPadLeft(InputAction.CallbackContext context)
    {
        ChangeRooms("left");
    }

    private void DPadUp(InputAction.CallbackContext context)
    {
        ChangeRooms("up");
    }

    private void DPadDown(InputAction.CallbackContext context)
    {
        ChangeRooms("down");
    }

    private void RightStickPerformed(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
        justUsedFreeCam = true;
    }

    private void RightStickCanceled(InputAction.CallbackContext context)
    {
        move = Vector3.zero;
    }

    private void LeftStickPerformed(InputAction.CallbackContext context)
    {
        CheckMenuSwitch();
    }

    #endregion

    private void Update()
    {
        MoveFreeCamera();
    }
    private void ChangeRooms(string x)
    {
        if (x == "up")
        {
            if (house.Length - 1 >= currLevel + 1)
            {
                currLevel += 1;
            }
        }
        else if (x == "down")
        {
            if (0 <= currLevel -1)
            {
                currLevel -= 1;
            }
        }
        else if(x == "right")
        {
            if (house[currLevel].level.Length >= currRoom)
            {
                currRoom += 1;
            }
        }
        else if (x == "left")
        {
            if (0 <= currRoom - 1)
            {
                currRoom -= 1;
            }
        }

        CheckRoomRange();
        MoveCamera();
        lastSelectedButton = EventSystem.current.currentSelectedGameObject;
    }

    private void MoveCamera()
    {
        if (justUsedFreeCam)
        {
            ChooseClosestMenu();
        }

        StopCoroutine("MoveThatCamera");
        StartCoroutine("MoveThatCamera", house[currLevel].level[currRoom].pos.position);
        justUsedFreeCam = false;
    }

    private void SelectButton()
    {
        for (int i = 0; i < house.Length; i++)
        {
            for (int y = 0; y < house[i].level.Length; y++)
            {
                if (i == currLevel && y == currRoom)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    print("3) " + house[i].level[y].menu.GetComponentInChildren<Button>().gameObject);
                    EventSystem.current.SetSelectedGameObject(house[i].level[y].menu.GetComponentInChildren<Button>().gameObject);
                }
            }
        }
    }

    private void DeselectButton()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void CheckRoomRange()
    {
        if (currRoom > house[currLevel].level.Length - 1)
        {
            currRoom = house[currLevel].level.Length - 1;
        }
    }

    private IEnumerator MoveThatCamera(Vector3 targetPosition)
    {
        Vector3 startPos = cam.transform.position;
        float elapsedTime = 0f;
        float targetTime = 0.25f;

        while (elapsedTime < targetTime)
        {
            float t = elapsedTime / targetTime;
            cam.transform.position = Vector3.Lerp(startPos, targetPosition, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cam.transform.position = targetPosition;
        SelectButton();

        yield break;
    }

    private void MoveFreeCamera()
    {
        if (move != Vector3.zero)
        {
            StopCoroutine("MoveThatCamera");
            DeselectButton();
            cam.transform.position += move * 10f * Time.deltaTime;

            Vector3 pos = cam.transform.position;
            pos.x = Mathf.Clamp(pos.x, bounds1.x, bounds2.x);
            pos.y = Mathf.Clamp(pos.y, bounds1.y, bounds2.y);

            cam.transform.position = pos;
        }
    }

    private void FindBounds()
    {
        bounds1 = house[0].level[0].pos.position;
        bounds2 = house[^1].level[^1].pos.position;
    }

    private void ChooseClosestMenu()
    {
        float dist = 1000f;

        for (int i = 0; i < house.Length; i++)
        {
            for (int y = 0; y < house[i].level.Length; y++)
            {
                Vector2 roomPos = house[i].level[y].pos.position;

                if (Vector2.Distance(cam.transform.position, roomPos) <= dist)
                {
                    dist = Vector2.Distance(cam.transform.position, roomPos);
                    currLevel = i;
                    currRoom = y;
                }
            }
        }
    }

    private void CheckMenuSwitch()
    {
        if (EventSystem.current.currentSelectedGameObject && lastSelectedButton)
        {
            GameObject oldParent = lastSelectedButton.transform.parent.gameObject;
            GameObject newParent = EventSystem.current.currentSelectedGameObject.transform.parent.gameObject;
            if (oldParent != newParent)
            {
                lastSelectedButton = EventSystem.current.currentSelectedGameObject;
                ChooseRoomWithButtonParent();
                MoveCamera();
            }
        }
        else
        {
            SelectButton();
            lastSelectedButton = EventSystem.current.currentSelectedGameObject;
            ChooseClosestMenu();
            MoveCamera();
        }

    }

    private void ChooseRoomWithButtonParent()
    {
        GameObject targetParent = EventSystem.current.currentSelectedGameObject.transform.parent.gameObject;

        for (int i = 0; i < house.Length; i++)
        {
            for (int y = 0; y < house[i].level.Length; y++)
            {
                if (house[i].level[y].menu == targetParent)
                {
                    currLevel = i;
                    currRoom = y;
                }
            }
        }
    }
}

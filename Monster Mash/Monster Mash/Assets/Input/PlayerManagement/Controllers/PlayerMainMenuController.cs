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

    private House[] house; //defined in MainMenuNav script

    [SerializeField] private int currLevel = 0; //index for house
    [SerializeField] private int currRoom = 1; //index for house

    #region (De)Activate Controller
    public void ActivateController()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        isActive = true;

        playerInput.SwitchCurrentActionMap("MainMenu");

        cam = Camera.main.gameObject;

        house = FindObjectOfType<MainMenuNav>().GetHouse();

        playerInput.actions["DPadRight"].performed += DPadRight;
        playerInput.actions["DPadLeft"].performed += DPadLeft;
        playerInput.actions["DPadUp"].performed += DPadUp;
        playerInput.actions["DPadDown"].performed += DPadDown;

        MoveCamera();
        ChangeMenu();
    }

    public void DeactivateController()
    {
        isActive = false;

        playerInput.actions["DPadRight"].performed -= DPadRight;
        playerInput.actions["DPadLeft"].performed -= DPadLeft;
        playerInput.actions["DPadUp"].performed -= DPadUp;
        playerInput.actions["DPadDown"].performed -= DPadDown;
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

    #endregion

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
        MenuOff();
    }

    private void MoveCamera()
    {
        StopCoroutine("MoveThatCamera");
        StartCoroutine("MoveThatCamera", house[currLevel].level[currRoom].pos.position);
    }

    private void ChangeMenu()
    {
        for (int i = 0; i < house.Length; i++)
        {
            for (int y = 0; y < house[i].level.Length; y++)
            {
                if (i == currLevel && y == currRoom)
                {
                    house[i].level[y].menu.SetActive(true);
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(house[i].level[y].menu.GetComponentInChildren<Button>().gameObject);
                }
                else
                {
                    house[i].level[y].menu.SetActive(false);
                }
            }
        }
    }

    private void MenuOff()
    {
        for (int i = 0; i < house.Length; i++)
        {
            for (int y = 0; y < house[i].level.Length; y++)
            {
                house[i].level[y].menu.SetActive(false);
            }
        }
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
        float targetTime = 0.5f;

        while (elapsedTime < targetTime)
        {
            float t = elapsedTime / targetTime;
            cam.transform.position = Vector3.Lerp(startPos, targetPosition, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        ChangeMenu();

        yield break;
    }
}

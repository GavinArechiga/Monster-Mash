using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    [SerializeField] private bool pauseControls = false;

    EventSystem eventSystem;

    private GameObject titleCard;
    private GameObject idleVideo;

    private float idleTime = 10f;
    private float timer = 0f;
    private bool canStartTimer = false;
    [SerializeField] private bool playingIdleVideo = false;

    #region (De)Activate Controller
    public void ActivateController()
    {
        MakeSureSetUpHappened();

        MoveCamera();
        SelectButton();
        FindBounds();

        lastSelectedButton = eventSystem.currentSelectedGameObject;

        titleCard = GameObject.Find("TitleCard");
        idleVideo = GameObject.Find("IdleVideo");
        idleVideo?.SetActive(false);

        StartCoroutine("LoadTitleCard");
    }

    public void DeactivateController()
    {
        isActive = false;
    }

    private void OnEnable()
    {
        MakeSureSetUpHappened();
        playerInput.actions["DPadRight"].performed += DPadRight;
        playerInput.actions["DPadLeft"].performed += DPadLeft;
        playerInput.actions["DPadUp"].performed += DPadUp;
        playerInput.actions["DPadDown"].performed += DPadDown;
        playerInput.actions["RightStick"].performed += RightStickPerformed;
        playerInput.actions["RightStick"].canceled += RightStickCanceled;
        playerInput.actions["Navigate1"].performed += LeftStickPerformed;
        playerInput.actions["AnyButtonMM"].performed += AnyButton;
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
        playerInput.actions["AnyButtonMM"].performed -= AnyButton;
    }

    #endregion

    #region Input Actions

    private void DPadRight (InputAction.CallbackContext context)
    {
        if (!pauseControls)
        {
            ChangeRooms("right");
        }
    }

    private void DPadLeft(InputAction.CallbackContext context)
    {
        if (!pauseControls)
        {
            ChangeRooms("left");
        }
    }

    private void DPadUp(InputAction.CallbackContext context)
    {
        if (!pauseControls)
        {
            ChangeRooms("up");
        }
    }

    private void DPadDown(InputAction.CallbackContext context)
    {
        if (!pauseControls)
        {
            ChangeRooms("down");
        }
    }

    private void RightStickPerformed(InputAction.CallbackContext context)
    {
        if (!pauseControls)
        {
            move = context.ReadValue<Vector2>();
            justUsedFreeCam = true;
        }

        if (!playingIdleVideo)
        {
            timer = 0.0f;
        }
        else
        {
            StopIdleVideo();
        }
    }

    private void RightStickCanceled(InputAction.CallbackContext context)
    {
        if (!pauseControls)
        {
            move = Vector3.zero;
        }
    }

    private void LeftStickPerformed(InputAction.CallbackContext context)
    {
        if (!pauseControls)
        {
            CheckMenuSwitch();
        }

        if (!playingIdleVideo)
        {
            timer = 0.0f;
        }
        else
        {
            StopIdleVideo();
        }
    }

    private void AnyButton(InputAction.CallbackContext context)
    {
        if (!playingIdleVideo)
        {
            timer = 0.0f;
        }
        else
        {
            StopIdleVideo();
        }
    }

    #endregion

    private void Update()
    {
        if (!pauseControls)
        {
            MoveFreeCamera();
        }

        if (!playingIdleVideo)
        {
            timer += Time.deltaTime;
        }

        if (timer > idleTime)
        {
            StartIdleVideo();
        }
    }
    private void ChangeRooms(string x)
    {
        if (!pauseControls)
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
                if (0 <= currLevel - 1)
                {
                    currLevel -= 1;
                }
            }
            else if (x == "right")
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
            lastSelectedButton = eventSystem.currentSelectedGameObject;
        }
    }

    private void MoveCamera()
    {
        if (!pauseControls)
        {
            if (justUsedFreeCam)
            {
                ChooseClosestMenu();
            }

            StopCoroutine("MoveThatCamera");
            StartCoroutine("MoveThatCamera", house[currLevel].level[currRoom].pos.position);
            justUsedFreeCam = false;
        }
    }

    private void SelectButton()
    {
        if (!pauseControls)
        {
            for (int i = 0; i < house.Length; i++)
            {
                for (int y = 0; y < house[i].level.Length; y++)
                {
                    if (i == currLevel && y == currRoom)
                    {
                        eventSystem.SetSelectedGameObject(null);
                        eventSystem.SetSelectedGameObject(house[i].level[y].menu.GetComponentInChildren<Button>().gameObject);
                    }
                }
            }
        }
    }

    private void DeselectButton()
    {
        eventSystem.SetSelectedGameObject(null);
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
        if (eventSystem.currentSelectedGameObject && lastSelectedButton)
        {
            GameObject oldParent = lastSelectedButton.transform.parent.gameObject;
            GameObject newParent = eventSystem.currentSelectedGameObject.transform.parent.gameObject;
            if (oldParent != newParent)
            {
                lastSelectedButton = eventSystem.currentSelectedGameObject;
                ChooseRoomWithButtonParent();
                MoveCamera();
            }
        }
        else
        {
            SelectButton();
            lastSelectedButton = eventSystem.currentSelectedGameObject;
            ChooseClosestMenu();
            MoveCamera();
        }

    }

    private void ChooseRoomWithButtonParent()
    {
        GameObject targetParent = eventSystem.currentSelectedGameObject.transform.parent.gameObject;

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

    private IEnumerator LoadTitleCard()
    {
        bool load = FindObjectOfType<LoadTitleCard>();

        titleCard?.SetActive(load);

        if (load)
        {
            StopCoroutine("MoveThatCamera");
            PauseControls(true);
            yield return new WaitForSeconds(2.0f);
            titleCard?.SetActive(false);
            Destroy(FindObjectOfType<LoadTitleCard>().gameObject);
            PauseControls(false);
        }

        canStartTimer = true;
        yield break;
    }

    private void StartIdleVideo()
    {
        playingIdleVideo = true;
        timer = 0.0f;
        PauseControls(true);
        idleVideo?.SetActive(true);
    }

    private void StopIdleVideo()
    {
        timer = 0.0f;
        PauseControls(false);
        idleVideo?.SetActive(false);
        playingIdleVideo = false;
    }

    private void PauseControls(bool pause)
    {
        pauseControls = pause;

        if (pause)
        {
            eventSystem.enabled = false;
        }
        else
        {
            eventSystem.enabled = true;
            MoveCamera();
            SelectButton();
            FindBounds();

            lastSelectedButton = eventSystem.currentSelectedGameObject;
        }
    }

    private void MakeSureSetUpHappened()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("MainMenu");
        isActive = true;

        eventSystem = EventSystem.current;

        cam = Camera.main.gameObject;

        house = FindObjectOfType<MainMenuNav>().GetHouse();

        currLevel = 0;
        currRoom = 1;
    }
}

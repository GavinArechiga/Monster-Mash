using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MultiplayerCursor : MonoBehaviour
{
    PlayerInput player;

    VirtualMouseInput cursor;

    [SerializeField] private RectTransform canvasRectTransform;
    private Camera UICamera;
    public int cursorIndex;

    [HideInInspector]
    public MultiplayerJoinManager joinManager;

    [HideInInspector]
    public PlayerInputManager inputManager;

    bool selectedCharacter = false;

    GraphicRaycaster graphicRaycaster;
    PointerEventData eventData = new PointerEventData(null);
    EventSystem eventSystem;

    GameObject movingCursor;
    

    // Start is called before the first frame update
    public void Enabled(PlayerInput spawnedPlayer)
    {
        UICamera = canvasRectTransform.GetComponent<Canvas>().worldCamera;
        player = spawnedPlayer;

        spawnedPlayer.SwitchCurrentActionMap("UI Navagation");

        cursor = GetComponent<VirtualMouseInput>();

        InputActionAsset playerInput = player.actions;

        InputActionMap controllerMap = playerInput.FindActionMap("UI Navagation");

        InputAction selectAction = controllerMap.FindAction("Select Action - Generic Gamepad");

        InputAction deselectAction = controllerMap.FindAction("Deselect Action - Generic Gamepad");

        selectAction.started += SelectAction;

        deselectAction.started += DeselectCharacter;

        graphicRaycaster = FindObjectOfType<GraphicRaycaster>();

        eventSystem = FindObjectOfType<EventSystem>();

        movingCursor = transform.GetChild(0).gameObject;

        spawnedPlayer.onDeviceLost += DisableVisuals;

        spawnedPlayer.onDeviceRegained += EnableVisuals;
    }

    private void OnDisable()
    {
        player.onDeviceLost -= DisableVisuals;

        player.onDeviceRegained -= EnableVisuals;
    }

    // Update is called once per frame

    private void Update()
    {

    }
    void LateUpdate()
    {
        Vector2 mousePos = cursor.virtualMouse.position.ReadValue();

        // changed from using Screen.Width and Screen.Height to instead use the canvas rect so that it scales with the resolution.
        mousePos.x = Mathf.Clamp(mousePos.x, 0f, canvasRectTransform.rect.width);
        mousePos.y = Mathf.Clamp(mousePos.y, 0f, canvasRectTransform.rect.height);

        InputState.Change(cursor.virtualMouse.position, mousePos);
    }

    public void SelectAction(InputAction.CallbackContext context)
    {
        eventData = new PointerEventData(eventSystem);

        // Replaced eventData.position = transform.GetChild(0).localPosition; With world to screen point so that the cordinates allign at any resolution.
        eventData.position = RectTransformUtility.WorldToScreenPoint(UICamera, movingCursor.transform.position);

        List<RaycastResult> results = new List<RaycastResult>();

        graphicRaycaster.Raycast(eventData, results);

        if(results.Count > 0)
        {
            if(results[0].gameObject.TryGetComponent(out ICursorSelectable selectable))
            {
                selectable.OnSelect(this);
            }
        }

        /*
        RectTransform rect = movingCursor.GetComponent<RectTransform>();

        Vector3 mousePoint = new Vector3(rect.localPosition.x, rect.localPosition.y, 10);

        Ray ray = Camera.main.ScreenPointToRay(mousePoint);

        int layerMask = 1 << 14;

        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            hit.transform.gameObject.GetComponent<MultiplayerJoinManager.IQuickplayButtonable>().ButtonSelected(this);
        }
        */
    }

    public void SelectCharacter(MonsterData data)
    { 
        if (joinManager.charactersSelected < inputManager.playerCount && !selectedCharacter)
        {
            joinManager.charactersSelected++;

            joinManager.playerInfo[player.playerIndex].monster = data;

            joinManager.playerInfo[player.playerIndex].playerInput = player.gameObject;

            selectedCharacter = true;

            if (joinManager.charactersSelected == inputManager.playerCount && inputManager.playerCount != 1 || joinManager.soloTesting)
            {
                print("Start Game!");

                joinManager.allowStartGame = true;
            }
        }
    }

    public void DeselectCharacter(InputAction.CallbackContext context)
    {
        if(selectedCharacter && joinManager.currentScreen == MultiplayerJoinManager.CurrentScreen.CharacterSelect)
        {
            selectedCharacter = false;

            joinManager.allowStartGame = false;

            joinManager.charactersSelected--;

            joinManager.playerInfo[player.playerIndex].monster = null;

            joinManager.playerInfo[player.playerIndex].playerInput = null;
        }
    }

    void DisableVisuals(PlayerInput input)
    {
        Image cursorVisual = GetComponentInChildren<Image>();

        cursorVisual.enabled = false;

        //joinManager.playerTubes[input.playerIndex].SetActive(false);
    }

    void EnableVisuals(PlayerInput input)
    {
        Image cursorVisual = GetComponentInChildren<Image>();

        cursorVisual.enabled = true;

       // joinManager.playerTubes[input.playerIndex].SetActive(true);
    }
}

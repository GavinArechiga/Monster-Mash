using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class RedButtonWrapper
{
    public List<string> redButtons;
    public RedButtonWrapper()
    {
        redButtons = new List<string>();
    }
}


public class InputRemapper : MonoBehaviour
{
    public static InputRemapper Instance { get; private set; }
    public static UnityEvent OnRebind = new();

    [SerializeField] private GameObject remappingUI;

    private PlayerInput currentPlayer;
    private ControlItemData[] allControlItems;
    // using the input action name as a key to identify specific buttons when the game loads
    private RedButtonWrapper redButtonWrapper = new();
    private InputActionAsset playerActions;
    private readonly string redButtonsKey = "redButtons";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        allControlItems = GetComponentsInChildren<ControlItemData>();

        if (PlayerPrefs.HasKey(redButtonsKey))
        {
            redButtonWrapper = JsonUtility.FromJson<RedButtonWrapper>(PlayerPrefs.GetString(redButtonsKey));
        }

        remappingUI.SetActive(false);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnApplicationQuit()
    {
        EnableDevices();
    }

    private void EnableDevices()
    {
        foreach (var players in PlayerInput.all)
        {
            for (int i = 0; i < players.devices.Count; i++)
            {
                InputSystem.EnableDevice(players.devices[i]);
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnableDevices();
    }

    public void ShowMenu(PlayerInput player)
    {
        currentPlayer = player;
        playerActions = player.actions;

        if (!remappingUI.activeSelf)
        {
            LoadUIText();

            remappingUI.SetActive(true);

            if (allControlItems != null && allControlItems.Length > 0)
            {
                EventSystem.current.SetSelectedGameObject(allControlItems[0].buttonRef.gameObject);
            }

            foreach (var players in PlayerInput.all)
            {
                players.actions.FindActionMap("Monster Controls").Disable();

                if (players.playerIndex != currentPlayer.playerIndex)
                {
                    for (int i = 0; i < players.devices.Count; i++)
                    {
                        InputSystem.DisableDevice(players.devices[i]);
                    }
                }
            }
        }
        else
        {
            remappingUI.SetActive(false);

            foreach (var players in PlayerInput.all)
            {
                players.actions.FindActionMap("Monster Controls").Enable();

                if (players.playerIndex != currentPlayer.playerIndex)
                {
                    for (int i = 0; i < players.devices.Count; i++)
                    {
                        InputSystem.EnableDevice(players.devices[i]);
                    }
                }
            }
        }
    }

    // called from button event
    public void Rebind(ControlItemData controlItem)
    {
        controlItem.buttonRef.GetComponentInChildren<TextMeshProUGUI>().text = "";
        playerActions.FindAction(controlItem.rebindTarget.action.name).Disable();
        controlItem.rebindTarget.action.Disable();

       
        controlItem.rebindTarget.action.PerformInteractiveRebinding()
            .WithAction(playerActions.FindAction(controlItem.rebindTarget.action.name))
            .WithControlsHavingToMatchPath("<Gamepad>") // limits accepted inputs to gamepad buttons
            .WithCancelingThrough(Gamepad.current.buttonEast)
            .OnComplete(callback =>
            {
                SetButtonTextToDisplayString(controlItem);
                StartCoroutine(DelayReenable(playerActions.FindAction(controlItem.rebindTarget.action.name)));
                callback.Dispose();

                var rebinds = controlItem.rebindTarget.action.actionMap.SaveBindingOverridesAsJson();
                PlayerPrefs.SetString($"{currentPlayer.playerIndex}{controlItem.rebindTarget.action.actionMap.name}", rebinds);
                OnRebind.Invoke();
            })
            .OnCancel(callback => 
            { 
                SetButtonTextToDisplayString(controlItem);
                StartCoroutine(DelayReenable(playerActions.FindAction(controlItem.rebindTarget.action.name)));
                callback.Dispose();
            })
            .OnPotentialMatch(callback =>
            {
                if (controlItem.rebindTarget.action.actionMap.name.Equals("XBOX")) { return; }

                CheckForButtonConflicts(controlItem, callback);
            })
            .Start();
    }

    private void LoadUIText()
    {
        LoadRebinds();

        foreach (ControlItemData controlItem in allControlItems)
        {
            SetButtonTextToDisplayString(controlItem);

            if (redButtonWrapper.redButtons.Contains(controlItem.rebindTarget.action.name))
            {
                controlItem.buttonRef.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
            }
        }
    }

    private void CheckForButtonConflicts(ControlItemData controlItem, InputActionRebindingExtensions.RebindingOperation callback)
    {
        foreach (var binding in controlItem.rebindTarget.action.actionMap.bindings)
        {
            foreach (var candidate in callback.candidates)
            {
                // the path for candidates and the path for bindings are very anoyingly not the same except for the endings.
                // So we need to split the paths into seperate strings and then compare the last element of each array.
                var candidateStrings = candidate.path.Split("/");
                var bindingStrings = binding.path.Split("/");


                // ignore the action we are currentely rebinding when looking for conflicts
                if (binding.action.Equals(controlItem.rebindTarget.action.name)) { continue; }


                // ^1 == Length - 1
                if (candidateStrings[^1].Equals(bindingStrings[^1]))
                {
                    controlItem.buttonRef.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;

                    if (!redButtonWrapper.redButtons.Contains(controlItem.rebindTarget.action.name))
                    {
                        redButtonWrapper.redButtons.Add(controlItem.rebindTarget.action.name);
                        PlayerPrefs.SetString(redButtonsKey, JsonUtility.ToJson(redButtonWrapper));
                    }

                    return;
                }
                else
                {
                    controlItem.buttonRef.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;

                    if (redButtonWrapper.redButtons.Contains(controlItem.rebindTarget.action.name))
                    {
                        redButtonWrapper.redButtons.Remove(controlItem.rebindTarget.action.name);
                        PlayerPrefs.SetString(redButtonsKey, JsonUtility.ToJson(redButtonWrapper));
                    }
                }
            }
        }
    }

    private void LoadRebinds()
    {
        string lastActionMapName = "";

        foreach (ControlItemData controlItem in allControlItems)
        {
            string actionMapName = playerActions.FindAction(controlItem.rebindTarget.action.name).actionMap.name;
            string playerKey = $"{currentPlayer.playerIndex}{actionMapName}";

            if (lastActionMapName.Equals(actionMapName)) { continue; }

            if (PlayerPrefs.HasKey(playerKey))
            {
                string rebinds = PlayerPrefs.GetString(playerKey);
                playerActions.FindActionMap(actionMapName).LoadBindingOverridesFromJson(rebinds);
            }

            lastActionMapName = actionMapName;
        }
    }

    private void SetButtonTextToDisplayString(ControlItemData controlItem)
    {
        InputAction action = playerActions.FindAction(controlItem.rebindTarget.action.name);
        controlItem.buttonRef.GetComponentInChildren<TextMeshProUGUI>().text = action.GetBindingDisplayString();
    }

    IEnumerator DelayReenable(InputAction actionRef)
    {
        yield return new WaitUntil(() => remappingUI.activeSelf);
        actionRef.Enable();
    }
}

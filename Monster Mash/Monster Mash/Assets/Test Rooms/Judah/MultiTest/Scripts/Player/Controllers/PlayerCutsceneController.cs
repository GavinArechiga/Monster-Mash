using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerCutsceneController : MonoBehaviour, IPlayerController
{
    PlayerInput playerInput;
    TMP_Text skipText;
    private bool hasShownPrompt = false;

    private CutsceneManager manager;

    private bool isActive;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("Cutscene");

        Debug.Log(playerInput.currentActionMap);

        manager = FindObjectOfType<CutsceneManager>();

        skipText = GameObject.Find("SkipText").GetComponent<TMP_Text>();
        if (!skipText)
        {
            Debug.LogWarning("SkipText not found, it should be a TMP_Text on the canvas");
        }
        skipText.gameObject.SetActive(false);
    }

    public void OnAny(/*InputAction.CallbackContext context*/)
    {
        if (!hasShownPrompt)
        {
            //var gamepad = context.control.device as Gamepad;
            //string button = gamepad.buttonNorth.displayName;

            string button = "button";

            StartSkipText(button);
        }
    }

    private void OnSkip()
    {
        if (hasShownPrompt)
        {
            manager.LoadScene();
        }
    }

    private void StartSkipText(string button)
    {
        hasShownPrompt = true;

        skipText.text = "Press " + button + " to skip";
        skipText.gameObject.SetActive(true);
    }

    private void Skip()
    {

    }

    public void ActivateController()
    {
        playerInput.SwitchCurrentActionMap("Cutscene");
        isActive = true;
    }

    public void DeactivateController()
    {
        isActive = false;
    }
}

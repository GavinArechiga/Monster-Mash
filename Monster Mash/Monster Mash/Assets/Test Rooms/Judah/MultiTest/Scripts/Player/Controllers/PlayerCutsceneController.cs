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

    private LoadRing ring; //loading visual

    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<CutsceneManager>();

        skipText = GameObject.Find("SkipText").GetComponent<TMP_Text>();
        if (!skipText)
        {
            Debug.LogWarning("SkipText not found, it should be a TMP_Text on the canvas");
        }
        skipText.gameObject.SetActive(false);

        ring = FindObjectOfType<LoadRing>();
    }

    private void StartSkipText(string button)
    {
        hasShownPrompt = true;

        skipText.text = "Press " + button + " to skip";
        skipText.gameObject.SetActive(true);
    }

    #region (De)Activate Controller
    public void ActivateController()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("Cutscene");
        isActive = true;

        playerInput.actions["AnyButton"].performed += OnAnyButton;
        playerInput.actions["Skip"].performed += OnSkip;

        playerInput.actions["Skip"].started += StartVisual;
        playerInput.actions["Skip"].canceled += StopVisual;
    }

    public void DeactivateController()
    {
        isActive = false;

        playerInput.actions["AnyButton"].performed -= OnAnyButton;
        playerInput.actions["Skip"].performed -= OnSkip;
    }
    #endregion

    #region Input Actions
    private void OnAnyButton(InputAction.CallbackContext context)
    {
        if (!hasShownPrompt)
        {
            var gamepad = context.control.device as Gamepad;
            string button = gamepad.buttonNorth.displayName;

            StartSkipText(button);
        }
    }

    private void OnSkip(InputAction.CallbackContext context)
    {
        if (hasShownPrompt)
        {
            manager.LoadScene();
        }
    }

    private void StartVisual(InputAction.CallbackContext context)
    {
        if (ring) ring.StartTimer();
    }

    private void StopVisual(InputAction.CallbackContext context)
    {
        if (ring) ring.StopTimer();
    }

    #endregion
}

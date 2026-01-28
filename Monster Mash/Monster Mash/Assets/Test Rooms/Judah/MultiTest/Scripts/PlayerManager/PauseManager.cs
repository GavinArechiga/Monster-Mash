using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [SerializeField] private GameObject pauseMenu;
    private bool isPaused = false;

    private PlayerManager manager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        manager = FindObjectOfType<PlayerManager>();
    }
    public void TogglePause()
    {
        if (isPaused)
        {
            isPaused = false;
            DeactivatePauseMenu();
        }
        else
        {
            isPaused = true;
            ActivatePauseMenu();
        }
    }
    private void ActivatePauseMenu()
    {
        pauseMenu.SetActive(true);

        Time.timeScale = 0f;

        manager.Controller.SwitchAllControllers(manager.Controller.uiController); //switches all players to UI controllers
    }

    private void DeactivatePauseMenu()
    {
        pauseMenu.SetActive(false);

        Time.timeScale = 1f;

        manager.Controller.DestroyAllControllersOfType<UIControllerTemp>(); //destroys only UI controllers, assuming all players have a combat controller
                                                                            //if this scales poorly, just make a new Action Map just for pause menu and then it'll work again.
        manager.Controller.SwitchAllControllers(manager.Scene.sceneInfo.controllerForThisScene);
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}

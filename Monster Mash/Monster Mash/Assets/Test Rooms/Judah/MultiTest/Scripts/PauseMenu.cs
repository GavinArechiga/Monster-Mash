using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    private bool isPaused = false;
    public void ActivatePauseMenu()
    {
        pauseMenu.SetActive(true);

        Time.timeScale = 0f;

        isPaused = true;
    }

    public void DeactivatePauseMenu()
    {
        pauseMenu.SetActive(false);

        Time.timeScale = 1f;

        isPaused = false;
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}

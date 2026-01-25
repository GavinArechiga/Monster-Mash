using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MainMenuManager : MonoBehaviour
{
    public Scene nextScene;

    public void LoadNextScene()
    {
        SceneManager.LoadScene(nextScene.name);
    }
}

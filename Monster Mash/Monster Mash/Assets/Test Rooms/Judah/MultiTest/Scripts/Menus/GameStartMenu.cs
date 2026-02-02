using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartMenu : MonoBehaviour
{
    MenuManager manager;
    void Start()
    {
        manager = GetComponent<MenuManager>();
    }

    public void StartMainMenu()
    {
        manager.LoadScene("MainMenu");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcadeMenu : MonoBehaviour
{
    MenuManager manager;
    void Start()
    {
        manager = GetComponent<MenuManager>();
    }

    public void StartMainMenu(int i)
    {
        manager.LoadScene("MainMenu");
    }
    public void StartCombat()
    {
        manager.LoadScene("Combat");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    MenuManager manager;
    void Start()
    {
        manager = GetComponent<MenuManager>();
    }

    public void StartArcade()
    {
        manager.LoadScene("Arcade");
    }

    public void StartGacha()
    {
        manager.LoadScene("Gacha");
    }

    public void StartMainSettings()
    {
        manager.LoadScene("MainSettings");
    }
}

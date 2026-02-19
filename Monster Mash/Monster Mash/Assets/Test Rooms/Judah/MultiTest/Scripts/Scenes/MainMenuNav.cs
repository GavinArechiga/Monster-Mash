using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuNav : MonoBehaviour
{
    [SerializeField] private House[] house;

    public House[] GetHouse()
    {
        return house;
    }
}

[System.Serializable]
public class Level //think like floor of a building with rooms
{
    public GameObject menu;
    public Transform pos;
}

[System.Serializable]
public class House //house is made of multiple levels^
{
    public Level[] level;
}

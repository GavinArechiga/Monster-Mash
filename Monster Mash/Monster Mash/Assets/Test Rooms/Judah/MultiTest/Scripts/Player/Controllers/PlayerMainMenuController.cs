using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMainMenuController : MonoBehaviour, IPlayerController
{
    private bool isActive;

    GameObject cam;

    [SerializeField] private House[] house;

    PlayerInput playerInput;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateController()
    {
        isActive = true;

        playerInput = GetComponentInParent<PlayerInput>();
        playerInput.SwitchCurrentActionMap("MainMenu");
    }

    public void DeactivateController()
    {
        isActive = false;
    }
}

[System.Serializable]
public class Level
{
    public GameObject menu;
    public Transform pos;
}

[System.Serializable]
public class House
{
    public Level[] level;
}

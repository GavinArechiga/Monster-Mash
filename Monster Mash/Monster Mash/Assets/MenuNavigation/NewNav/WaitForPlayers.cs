using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;

public class WaitForPlayers : MonoBehaviour
{
    InputSystemUIInputModule uiModule;
    playerIndexManager playerManager;

    bool noPlayers = false;

    // Start is called before the first frame update
    void Awake()
    {
        uiModule = GetComponent<InputSystemUIInputModule>();
        uiModule.enabled = false;

        playerManager = FindObjectOfType<playerIndexManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerCheck())
        {
            noPlayers = false;
            uiModule.enabled = true;
            enabled = false;
        }
    }

    private bool PlayerCheck()
    {
        return playerManager.GetPlayers().Count > 0 ? true : false;
    }
}

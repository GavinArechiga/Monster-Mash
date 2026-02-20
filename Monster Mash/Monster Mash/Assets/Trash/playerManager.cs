using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

public class playerManager : MonoBehaviour
{
    private PlayerInputManager playerInputManager;
    public List<playerController> players = new List<playerController>();
    private List<string> playerNumbersAvailable = new List<string>() { "Player 1" , "Player 2", "Player 3", "Player 4"};
    public DynamicCamera battleCamera;

    public string intendedActionMap;
    public static playerManager Instance { get; private set; }

    [SerializeField]
    private Transform[] playerSpawnPoints;

    private int tempPlayerCount;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        playerInputManager = this.GetComponent<PlayerInputManager>();
    }

    private void OnPlayerJoined()
    {
        #region Rename, Add to Lists, and Player Order

        playerController[] potentialNewPlayer = FindObjectsOfType<playerController>();

        if (potentialNewPlayer.Length != players.Count) //only go through this process if its a new scene or a new player clone has been added
        {
            for (int i = 0; i < potentialNewPlayer.Length; i++)
            {
                if (players.Contains(potentialNewPlayer[i]) == false)//add to list of players in case its a new scene or this is a new player clone
                {
                    players.Add(potentialNewPlayer[i]);

                    if (playerNumbersAvailable.Contains(potentialNewPlayer[i].gameObject.name) == false) //rename if this is a new player clone
                    {
                        potentialNewPlayer[i].playerIndex = players.Count;
                        potentialNewPlayer[i].gameObject.name = "Player " + players.Count;
                    }
                }
            }

            //this is meant to reorder the list everytime a new player clone has been added or 
            for (int u = 0; u < potentialNewPlayer.Length; u++)
            {
                players[potentialNewPlayer[u].playerIndex - 1] = potentialNewPlayer[u];
            }

        }

        togglePlayerJoining();
        tempPlayerCount++;
        #endregion

        #region Correct Action Map

        if (intendedActionMap != null)
        {
            if (intendedActionMap != "")
            {
                switchAllPlayerActionMaps();
            }
        }

        #endregion

        #region Relocate Players
        //spawning players to designated spawn points if they exist
        if (playerSpawnPoints.Length > 0)
        {
            for (int i = 0; i < players.Count; i++)
            {
                players[i].gameObject.transform.position = playerSpawnPoints[i].position;
            }
        }
        #endregion

        #region Event System to Multiplayer Event System Conversion (if necessary)
        //this switches player event systems to determine whether one player controls the UI or all players control the UI
        if (players.Count > 0)
        {
            MultiplayerEventSystem[] multiplayerEventSystems = GameObject.FindObjectsOfType<MultiplayerEventSystem>();

            for (int i = 0; i < players.Count; i++)
            {
                /*GameObject potentialMultiplayerEventSystem = GameObject.Find("Player " + players[i].playerIndex + " Multiplayer System");

                if (potentialMultiplayerEventSystem != null)
                {
                    players[i].gameObject.GetComponent<PlayerInput>().uiInputModule = potentialMultiplayerEventSystem.GetComponent<InputSystemUIInputModule>();
                }
                else
                {
                    players[i].gameObject.GetComponent<PlayerInput>().uiInputModule = null;
                }*/

                if (multiplayerEventSystems.Length <= 0) { return; }

                if (multiplayerEventSystems[i] != null)
                {
                    players[i].GetComponent<PlayerInput>().uiInputModule = multiplayerEventSystems[i].GetComponent<InputSystemUIInputModule>();
                }
            }
        }
        #endregion

        if (battleCamera != null)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (battleCamera.playerTransforms.Contains(players[i].transform) == false)
                {
                    battleCamera.playerTransforms.Add(players[i].transform);
                }
            }
        }
    }

    //This is for removing that dumb redundant unity error
    private void togglePlayerJoining()
    {
        if (playerInputManager != null)
        {
            if (players.Count >= playerInputManager.maxPlayerCount && playerInputManager.joiningEnabled)
            {
                playerInputManager.DisableJoining();
            }
            else if (players.Count < playerInputManager.maxPlayerCount && !playerInputManager.joiningEnabled)
            {
                playerInputManager.EnableJoining();
            }
        }
    }

    private void switchAllPlayerActionMaps()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].switchActionMap(intendedActionMap);
        }
    }

    private void OnPlayerLeft()
    {

    }

    public void RemovePlayer(playerController player)
    {
        battleCamera.playerTransforms.Remove(player.transform);
        players.Remove(player);

        tempPlayerCount -= 1;
        tempPlayerCount = Mathf.Clamp(tempPlayerCount, 0, 4);

        Destroy(player);

        // temp solution for demo
        if (tempPlayerCount <= 1)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}

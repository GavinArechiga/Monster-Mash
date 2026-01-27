using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJoinManager : MonoBehaviour
{
    //this script manages all player joining and leaving, kinda self explanatory idk
    //it works a ton with the PlayerControllerManager and PlayerSceneManager to make sure all players are sent off to the correct spot.

    private PlayerManager manager;

    public GameObject playerPrefab;           // Assign Player prefab
    public Transform[] spawnPoints;       // Where players appear
    public int maxPlayers = 4; //this is a 4 player game 

    private bool firstPlayerHasJoined = false; //player 1 is handled differently and spawned in manually. if needed, we can change this but it made menu nav easier
    void Start()
    {
        if (manager.GetPlayerCount() == 0) // === Spawn Player 1 for main menu ===
        {
            SpawnFirstPlayer();
        }
    }
    private void Awake()
    {
        manager = GetComponent<PlayerManager>();
    }

    private void OnEnable()
    {
        manager.inputManager.onPlayerJoined += OnPlayerJoined;
    }

    private void OnDisable()
    {
        manager.inputManager.onPlayerJoined -= OnPlayerJoined;
    }
    private void OnPlayerJoined(PlayerInput newPlayer) //instantiates new player and sends it off to get the correct action map and world space positioning
    {
        if (firstPlayerHasJoined)
        {
            Debug.Log($"Player {manager.GetPlayerCount() + 1} joined!");
            DontDestroyOnLoad(newPlayer.gameObject);
            
            manager.AddPlayer(newPlayer);

            manager.Scene.SetupNewPlayer(newPlayer);
        }
    }
    private void SpawnFirstPlayer() //manually spawn PLAYER UNO. Makes player 1 god of the menu navigation
    {
        GameObject newPlayer = Instantiate(playerPrefab, spawnPoints[0].position, Quaternion.identity);
        DontDestroyOnLoad(newPlayer);

        PlayerInput playerInput = newPlayer.GetComponent<PlayerInput>();

        manager.Scene.SetupNewPlayer(playerInput);

        manager.AddPlayer(playerInput);

        firstPlayerHasJoined = true;
    }
    public void AllowPlayerJoining(bool allow) //I want the first player to be joined manually but on the character select screen, i need the rest of the players to join
    {
        if (allow)
        {
            manager.inputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersWhenButtonIsPressed;
        }
        else
        {
            manager.inputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersManually;
        }
    }
}

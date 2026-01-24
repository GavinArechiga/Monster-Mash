using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJoinManager : MonoBehaviour
{
    [Header("Player Prefab & Roles")]
    public GameObject playerPrefab;           // Assign your Player prefab
    private Player myPlayer;
    public Transform[] spawnPoints;       // Where players appear (character select)
    public int maxPlayers = 4;

    [SerializeField] private List<Player> players = new List<Player>();

    public PlayerInputManager inputManager;

    void Start()
    {
        // === Spawn Player 1 for main menu ===
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned!");
            return;
        }/*

        GameObject player1 = Instantiate(playerPrefab, spawnPoints[0].position, Quaternion.identity);
        myPlayer = player1.GetComponent<Player>();

        // Assign first connected gamepad if available
        if (Gamepad.all.Count > 0)
        {
            myPlayer.playerInput.SwitchCurrentControlScheme("Gamepad", Gamepad.all[0]);
        }

        // Assign UIController role
        myPlayer.SwitchRole(myPlayer.uiRolePrefab, "UI");

        players.Add(myPlayer);*/
    }

    private void Awake()
    {
        inputManager.onPlayerJoined += OnPlayerJoined;
    }
    private void OnPlayerJoined(PlayerInput newPlayer)
    {
        Debug.Log($"Player {players.Count} joined!");
        // Assign this specific gamepad to the new player

        Player newPlayerScript = newPlayer.GetComponent<Player>();

        // Assign UIController role
        newPlayerScript.SwitchRole(newPlayerScript.uiRolePrefab, "UI");

        players.Add(newPlayerScript);
    }
    public void AllowPlayerJoining(bool allow) //I want the first player to be joined manually but on the character select screen, i need the rest of the players to join
    {
        if (allow)
        {
            inputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersWhenButtonIsPressed;
        }
        else
        {
            inputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersManually;
        }
    }

    // Optional helper: get the current player list
    public List<Player> GetPlayers()
    {
        return players;
    }
}

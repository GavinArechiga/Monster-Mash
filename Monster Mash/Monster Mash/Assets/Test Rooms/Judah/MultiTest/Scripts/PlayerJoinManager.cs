using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJoinManager : MonoBehaviour
{
    [Header("Player Prefab & Roles")]
    public Player playerPrefab;           // Assign your Player prefab
    public Transform[] spawnPoints;       // Where players appear (character select)
    public int maxPlayers = 4;

    private List<Player> players = new List<Player>();

    // Phase flag: allow additional players to join
    private bool allowAdditionalPlayers = false;

    void Start()
    {
        // === Spawn Player 1 for main menu ===
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned!");
            return;
        }

        Player player1 = Instantiate(playerPrefab, spawnPoints[0].position, Quaternion.identity);

        // Assign first connected gamepad if available
        if (Gamepad.all.Count > 0)
        {
            player1.playerInput.SwitchCurrentControlScheme("Gamepad", Gamepad.all[0]);
        }

        // Assign UIController role
        player1.SwitchRole(player1.uiControllerPrefab, "UI");

        players.Add(player1);
    }

    void Update()
    {
        if (!allowAdditionalPlayers) return;

        // Loop through all connected gamepads
        foreach (var gamepad in Gamepad.all)
        {
            // Check if any button was pressed this frame
            if (gamepad.allControls.Exists(c => c.wasPressedThisFrame))
            {
                TryAddPlayer(gamepad);
            }
        }
    }

    void TryAddPlayer(Gamepad gamepad)
    {
        // Ignore if max players reached
        if (players.Count >= maxPlayers) return;

        // Ignore if this gamepad is already controlling a player
        foreach (var p in players)
        {
            if (p.playerInput.devices.Count > 0 && p.playerInput.devices[0] == gamepad)
                return;
        }

        // Spawn new player at the next spawn point
        int index = players.Count;
        if (index >= spawnPoints.Length)
        {
            Debug.LogWarning("No more spawn points available for additional players");
            return;
        }

        Player newPlayer = Instantiate(playerPrefab, spawnPoints[index].position, Quaternion.identity);

        // Assign this specific gamepad to the new player
        newPlayer.playerInput.SwitchCurrentControlScheme("Gamepad", gamepad);

        // Assign UIController role
        newPlayer.SwitchRole(newPlayer.uiControllerPrefab, "UI");

        players.Add(newPlayer);
        Debug.Log($"Player {players.Count} joined!");
    }

    // Call this when entering Character Select screen
    public void EnableAdditionalPlayers()
    {
        allowAdditionalPlayers = true;
    }

    // Optional helper: get the current player list
    public List<Player> GetPlayers()
    {
        return players;
    }
}

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

    private bool firstPlayerHasJoined = false;

    void Start()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned!");
            return;
        }

        // === Spawn Player 1 for main menu ===
        if (players.Count == 0)
        {
            SpawnFirstPlayer();
        }
    }
    private void Awake() //ensure only 1 instance of this between scenes or I AM COOKED
    {
        if (FindObjectsOfType<PlayerJoinManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        inputManager.onPlayerJoined += OnPlayerJoined;
    }
    private void OnPlayerJoined(PlayerInput newPlayer)
    {
        if (firstPlayerHasJoined)
        {
            Debug.Log($"Player {players.Count} joined!");
            DontDestroyOnLoad(newPlayer.gameObject);
            // Assign this specific gamepad to the new player

            Player newPlayerScript = newPlayer.GetComponent<Player>();

            // Assign UIController role
            newPlayerScript.SwitchRole(newPlayerScript.uiControllerPrefab);

            players.Add(newPlayerScript);
        }
    }
    private void SpawnFirstPlayer()
    {
        GameObject player1 = Instantiate(playerPrefab, spawnPoints[0].position, Quaternion.identity);
        DontDestroyOnLoad(player1);

        myPlayer = player1.GetComponent<Player>();

        Debug.Log("Player 1 in the hoooooouse!");

        // Assign UIController role, i can switch this as needed to a different "first" controller
        myPlayer.SwitchRole(myPlayer.uiControllerPrefab);

        players.Add(myPlayer);

        firstPlayerHasJoined = true;
    }
    public List<Player> GetPlayers()
    {
        return players;
    }
}

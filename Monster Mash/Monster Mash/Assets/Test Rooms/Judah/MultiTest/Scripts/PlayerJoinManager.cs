using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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

    private void Awake()
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
            // Assign this specific gamepad to the new player

            Player newPlayerScript = newPlayer.GetComponent<Player>();

            // Assign UIController role
            newPlayerScript.SwitchRole(newPlayerScript.uiRolePrefab, "UI");

            players.Add(newPlayerScript);
        }
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

    private void SpawnFirstPlayer()
    {
        GameObject player1 = Instantiate(playerPrefab, spawnPoints[0].position, Quaternion.identity);
        DontDestroyOnLoad(player1);

        myPlayer = player1.GetComponent<Player>();

        Debug.Log("Player 1 in the hoooooouse!");

        // Assign UIController role, i can switch this as needed to a different "first" controller
        myPlayer.SwitchRole(myPlayer.uiRolePrefab, "UI");

        players.Add(myPlayer);

        firstPlayerHasJoined = true;
    }
    public List<Player> GetPlayers()
    {
        return players;
    }

    //scene stuff + player reposition
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RepositionPlayersForScene(scene.name);
    }
    //handles players moved between scenes, turns off and on player joining per each scene
    void RepositionPlayersForScene(string sceneName)
    {
        if (sceneName == "CharacterSelect")
        {
            for (int i = 0; i < players.Count; i++)
            {
                players[i].transform.position = spawnPoints[i].position;
                players[i].SwitchRole(players[i].uiRolePrefab, "UI");
            }

            AllowPlayerJoining(true);
        }
        else if (sceneName == "Gameplay")
        {
            for (int i = 0; i < players.Count; i++)
            {
                players[i].SwitchRole(players[i].gameplayRolePrefab, "Gameplay");
            }

            AllowPlayerJoining(false);
        }
    }

}

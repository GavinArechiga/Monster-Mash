using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerSceneManager : MonoBehaviour
{
    private PlayerJoinManager joinManager;
    private PlayerInputManager inputManager;
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
        SetupPlayersForScene(scene.name);
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

    //handles players moved between scenes, turns off and on player joining, sets correct role/action map for each scene
    private void SetupPlayersForScene(string sceneName)
    {
        PlayerJoinManager manager = GetComponent<PlayerJoinManager>();
        List<Player> players = manager.GetPlayers();

        SceneInfo myInfo = FindObjectOfType<SceneInfo>();


        for (int i = 0; i < players.Count; i++)
        {
            players[i].transform.position = myInfo.spawnPoints[i].position;
            players[i].SwitchRole(myInfo.roleForThisScene);
        }

        if (sceneName == "CharacterSelect")//player 1 is already in by default, but players 2-4 should probably only join on the character select screen
        {
            AllowPlayerJoining(true);
        }
        //I can add more specific cases depending on our needs ex if (sceneName == poopScene) {players.GoPoopMode();}
    }
}

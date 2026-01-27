using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerSceneManager : MonoBehaviour
{
    //this script handles scene-to-scene logic for all player such as default action maps, spawn points, and any scene-related needs

    private PlayerManager manager;
    
    private SceneInfo sceneInfo; //one and only one of this script should be made for each scene. it holds variables which we can set in the inspector
    private void Awake()
    {
        manager = GetComponent<PlayerManager>();
    }
    private void SetSceneInfo() //called OnSceneLoad, it is subscribed below
    {
        sceneInfo = FindObjectOfType<SceneInfo>();
    }

    //handles players moved between scenes, turns off and on player joining, sets correct role/action map for each scene
    private void SetupPlayersForScene(string sceneName)
    {
        List<PlayerInput> players = manager.GetPlayers();

        manager.Controller.SwitchAllControllers(sceneInfo.controllerForThisScene, true);

        for (int i = 0; i < players.Count; i++)
        {
            players[i].transform.position = sceneInfo.spawnPoints[i].position;
        }

        if (sceneName == "CharacterSelect" || sceneInfo.allowJoining)//player 1 is already in by default, but players 2-4 should probably only join on the character select screen
        {
            manager.Join.AllowPlayerJoining(true);
        }
        else
        {
            manager.Join.AllowPlayerJoining(false);
        }
        //I can add more specific cases depending on our needs ex if (sceneName == poopScene) {players.GoPoopMode();}
    }

    public void SetupNewPlayer(PlayerInput newInput) //called from PlayerJoinManager, best for when players are being added at different times
    {
        Player newPlayer = newInput.GetComponent<Player>();

        List<PlayerInput> players = manager.GetPlayers();

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] == newPlayer)
            {
                newPlayer.transform.position = sceneInfo.spawnPoints[i].position;
            }
        }

        newPlayer.SwitchController(sceneInfo.controllerForThisScene, true);
    }

    #region event subscribing
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) //because it is subscribed to SceneManager.sceneLoaded, this function is automatically called on each scene load
    {
        SetSceneInfo();
        SetupPlayersForScene(scene.name);
    }

    #endregion
}

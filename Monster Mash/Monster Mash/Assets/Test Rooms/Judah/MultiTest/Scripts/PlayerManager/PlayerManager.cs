using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    //this is the "god script" which allows the scripts on this component to smoothly communicate.

    //these 3 script below need to reference eachother quite a bit, so i have them inheret from here/////////////
    public PlayerJoinManager Join { get; private set; }
    public PlayerSceneManager Scene { get; private set; }
    public PlayerControllerManager Controller { get; private set; }
    ////////////////////////////////////////////////////////////////////
    public PlayerInputManager inputManager { get; private set; }

    private List<PlayerInput> players = new List<PlayerInput>(); //this is a list of all players currently in the game! PlayerJoinManager keeps it up to date
    private void Awake()
    {
        Join = GetComponent<PlayerJoinManager>();
        Scene = GetComponent<PlayerSceneManager>();
        Controller = GetComponent<PlayerControllerManager>();
        inputManager = GetComponent<PlayerInputManager>();

        if (FindObjectsOfType<PlayerManager>().Length > 1) //ensures only 1 instance of this whole gameobject can exist! if there are more, we have problems
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
    public List<PlayerInput> GetPlayers()
    {
        return players;
    }

    public int GetPlayerCount()
    {
        return players.Count;
    }
    public void AddPlayer(PlayerInput newPlayer)
    {
        players.Add(newPlayer);
    }

    public void RemovePlayer(PlayerInput player)
    {
        players.Remove(player);
    }

    public void SubscribePlayerJoin() //this used to happen internally in PlayerJoinManager but its OnEnable was missing timing with this script's Awake :(
    {
        inputManager.onPlayerJoined += Join.OnPlayerJoined;
    }
}

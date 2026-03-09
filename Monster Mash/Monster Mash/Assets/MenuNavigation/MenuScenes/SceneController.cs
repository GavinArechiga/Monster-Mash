using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public bool allowJoining = false;//assuming player 1 is present, allows more players to join. players 2, 3 and 4

    public Transform[] spawnPoints; //where each player loads in

    public GameObject controllerForThisScene; //which action map should the players start in for this scene?

    public string[] myScenes;

    private PlayerManager manager;

    private void Start()
    {
        manager = FindObjectOfType<PlayerManager>();
    }
    public void LoadScene(int i)
    {
        SceneManager.LoadScene(myScenes[i]);
    }

    public void SetSelectedStage(int i)
    {
        manager.SetSelectedStage(i);
    }
}

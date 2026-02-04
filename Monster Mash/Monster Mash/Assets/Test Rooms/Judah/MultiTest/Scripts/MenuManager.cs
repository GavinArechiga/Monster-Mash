using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public bool allowJoining = false;//assuming player 1 is present, allows more players to join. players 2, 3 and 4

    public Transform[] spawnPoints; //where each player loads in

    public GameObject controllerForThisScene; //which action map should the players start in for this scene?

    public string[] myScenes;

    public void LoadScene(int i)
    {
        SceneManager.LoadScene(myScenes[i]);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    //new instance of this script in each scene, PlayerSceneManager pulls vars from here. can add more as scene need complexity grows
    //each scene with a menu nav should have a script on this same gameObject, referencing LoadScene() thru GetComponent<>

    public bool allowJoining = false;//assuming player 1 is present, allows more players to join. players 2, 3 and 4

    public Transform[] spawnPoints; //where each player loads in

    public GameObject controllerForThisScene; //which action map should the players start in for this scene?

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}

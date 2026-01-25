using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInfo : MonoBehaviour
{
    //new instance of this script in each scene, PlayerSceneManager pulls vars from here. can add more as scene need complexity grows

    public bool allowJoining = false;//assuming player 1 is present, allows more players to join. players 2, 3 and 4

    public Transform[] spawnPoints; //where each player loads in

    public ControllerRole roleForThisScene; //which action map should the players start in for this scene?

    //can be called from UIButtons or maybe something else as needed
    //for now, if in main screen we can use these
    [SerializeField] private string nextScene;
    public void LoadNextScene()
    {
        SceneManager.LoadScene(nextScene);
    }
}

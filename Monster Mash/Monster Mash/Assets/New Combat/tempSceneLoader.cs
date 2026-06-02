using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class tempSceneLoader : MonoBehaviour
{
    public void loadBlockParty()
    {
        SceneManager.LoadScene(1);
    }

    public void loadNewBugsburg()
    {
        SceneManager.LoadScene(2);
    }

    public void loadMonsterberryMunchMania()
    {
        SceneManager.LoadScene(3);
    }

    public void loadBeastdome()
    {
        SceneManager.LoadScene(4);
    }
}

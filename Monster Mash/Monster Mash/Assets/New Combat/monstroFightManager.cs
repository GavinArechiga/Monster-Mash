using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monstroFightManager : MonoBehaviour
{
    [SerializeField]
    private monstroInputHandler[] monstroMonsters;

    //this scripts goal is to take players loaded into the scene and place them on to spawn points
    //it will also tell player input handlers to switch to the monster action map
    private void Awake()
    {
        monstroMonsters = FindObjectsByType<monstroInputHandler>(FindObjectsSortMode.None);
        spawnAllMonsters();
    }

    private void spawnAllMonsters()
    {
        for (int i = 0; i < monstroMonsters.Length; i++)
        {
            #region Spawn Point Placement
            GameObject playerSpawnPoint = GameObject.Find(monstroMonsters[i].name + " Spawn");
            monstroMonsters[i].transform.position = playerSpawnPoint.transform.position;
            #endregion

            //turn on some sort of portal animation
            monstroMonsters[i].switchToMonsterControls();
        }
    }
}

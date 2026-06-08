using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class monstroFightManager : MonoBehaviour
{
    [SerializeField]
    private monstroInputHandler[] monstroMonsters;
    public int introDelayTime;

    private CinemachineTargetGroup cameraTargetGroup;

    //this scripts goal is to take players loaded into the scene and place them on to spawn points
    //it will also tell player input handlers to switch to the monster action map
    private void Awake()
    {
        monstroMonsters = FindObjectsByType<monstroInputHandler>(FindObjectsSortMode.None);
        cameraTargetGroup = FindFirstObjectByType<CinemachineTargetGroup>();
        StartCoroutine(introDelay());
    }

    private void spawnAllMonsters()
    {
        for (int i = 0; i < monstroMonsters.Length; i++)
        {
            StartCoroutine(playIntroSpawnPortal(monstroMonsters[i]));
        }
    }

    IEnumerator introDelay()
    {
        yield return new WaitForSeconds(introDelayTime);
        spawnAllMonsters();
    }

    IEnumerator playIntroSpawnPortal(monstroInputHandler spawningMonster)
    {
        //reset data
        spawningMonster.gameObject.GetComponent<monstroHealth>().resetHealth();

        //grab spawn point
        GameObject playerSpawnPoint = GameObject.Find(spawningMonster.name + " Spawn");

        //grab player number and delay spawning based on order
        int playerNumber = spawningMonster.gameObject.GetComponent<monstroIndex>().playerIndex;
        yield return new WaitForSeconds(playerNumber * 0.5f);

        //play portal animation on spawn point
        playerSpawnPoint.GetComponent<Animation>().Play();

        //allow animation to play until ready to spawn monster
        yield return new WaitForSeconds(1.5f);

        //move location of spawning player to correct spawn point
        spawningMonster.transform.position = playerSpawnPoint.transform.position;

        //add them to camera target group
        if (cameraTargetGroup != null)
        {
            cameraTargetGroup.AddMember(spawningMonster.transform, 1, 0.5f);
        }

        //change monster controls from UI to character movement
        spawningMonster.switchToMonsterControls();
    }

    public void respawnPlayer(GameObject outOfBoundsMonster)
    {
        StartCoroutine(playRespawnPortal(outOfBoundsMonster));
    }

    IEnumerator playRespawnPortal(GameObject respawningMonster)
    {
        //we can put a respawn delay here if we want
        respawningMonster.GetComponent<monstroLocomotion>().enabled = false;
        GameObject playerSpawnPoint = GameObject.Find(respawningMonster.name + " Spawn");
        playerSpawnPoint.GetComponent<Animation>().Stop();
        playerSpawnPoint.GetComponent<Animation>().Play();
        yield return new WaitForSeconds(1.5f);
        respawningMonster.transform.position = playerSpawnPoint.transform.position;
        respawningMonster.GetComponent<monstroLocomotion>().enabled = true;
    }
}

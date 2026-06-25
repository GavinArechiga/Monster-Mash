using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class monstroFightManager : MonoBehaviour
{
    [SerializeField]
    private monstroInputHandler[] monstroMonsters;
    public int introDelayTime;
    private int respawnDelay = 1;

    private CinemachineTargetGroup cameraTargetGroup;
    private stadiumCamera beastdomeCamera;//yes its super specific but it feels like the only way right now to find out when a monster is removed from the scene

    //this scripts goal is to take players loaded into the scene and place them on to spawn points
    //it will also tell player input handlers to switch to the monster action map
    private void Awake()
    {
        monstroMonsters = FindObjectsByType<monstroInputHandler>(FindObjectsSortMode.None);
        cameraTargetGroup = FindFirstObjectByType<CinemachineTargetGroup>();
        beastdomeCamera = FindFirstObjectByType<stadiumCamera>();
        turnOffAllMonsterVisuals();
        StartCoroutine(introDelay());
    }

    private void turnOffAllMonsterVisuals()
    {
        for (int i = 0; i < monstroMonsters.Length; i++)
        {
            //this is a temp visual enabler and disabler because james is annoying me lol

            //monstroMonsters[i].gameObject.GetComponent<MeshRenderer>().enabled = false;
            monstroMonsters[i].gameObject.GetComponent<monstroPartHandler>().hideMonster();
            monstroMonsters[i].gameObject.GetComponent<monstroMiscVisuals>().generatePlayerRing();
        }
    }

    IEnumerator introDelay()
    {
        yield return new WaitForSeconds(introDelayTime);
        spawnAllMonsters();
    }

    private void spawnAllMonsters()
    {
        for (int i = 0; i < monstroMonsters.Length; i++)
        {
            StartCoroutine(playIntroSpawnPortal(monstroMonsters[i]));
        }
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

        //this is a temp visual enabler and disabler because james is annoying me lol

        //spawningMonster.gameObject.GetComponent<MeshRenderer>().enabled = true;
        spawningMonster.gameObject.GetComponent<monstroPartHandler>().showMonster();
        spawningMonster.gameObject.GetComponent<monstroPartHandler>().startMonsterAnimations();
        spawningMonster.gameObject.GetComponent<monstroMiscVisuals>().showPlayerRing();

        //add them to camera target group
        if (cameraTargetGroup != null)
        {
            cameraTargetGroup.AddMember(spawningMonster.transform, 1, 0.5f);
        }

        if (beastdomeCamera != null)
        {
            beastdomeCamera.cameraTargets.Add(spawningMonster.transform);
        }

        //change monster controls from UI to character movement
        spawningMonster.switchToMonsterControls();
    }

    public void respawnPlayer(GameObject outOfBoundsMonster)
    {
        if (cameraTargetGroup != null)
        {
            cameraTargetGroup.RemoveMember(outOfBoundsMonster.transform);
        }

        if (beastdomeCamera != null)
        {
            beastdomeCamera.cameraTargets.Remove(outOfBoundsMonster.transform);
        }

        StartCoroutine(playRespawnPortal(outOfBoundsMonster));
    }

    IEnumerator playRespawnPortal(GameObject respawningMonster)
    {
        //this is a temp visual enabler and disabler because james is annoying me lol

        //respawningMonster.gameObject.GetComponent<MeshRenderer>().enabled = false;
        respawningMonster.gameObject.GetComponent<monstroPartHandler>().hideMonster();
        respawningMonster.gameObject.GetComponent<monstroMiscVisuals>().hidePlayerRing();
        respawningMonster.GetComponent<monstroLocomotion>().enabled = false;
        GameObject playerSpawnPoint = GameObject.Find(respawningMonster.name + " Spawn");
        respawningMonster.transform.position = playerSpawnPoint.transform.position;

        yield return new WaitForSeconds(respawnDelay);
        playerSpawnPoint.GetComponent<Animation>().Stop();
        playerSpawnPoint.GetComponent<Animation>().Play();
        if (cameraTargetGroup != null)
        {
            cameraTargetGroup.AddMember(respawningMonster.transform, 1, 0.5f);
        }

        if (beastdomeCamera != null)
        {
            beastdomeCamera.cameraTargets.Add(respawningMonster.transform);
        }

        yield return new WaitForSeconds(1.5f);

        //respawningMonster.gameObject.GetComponent<MeshRenderer>().enabled = true;
        respawningMonster.gameObject.GetComponent<monstroPartHandler>().showMonster();
        respawningMonster.gameObject.GetComponent<monstroMiscVisuals>().showPlayerRing();
        respawningMonster.GetComponent<monstroLocomotion>().enabled = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawnManager : MonoBehaviour
{
    [SerializeField]
    private Transform[] playerSpawnLocations;

    [SerializeField]
    private Transform players;

    [SerializeField]
    GameObject dynamicCamObj;

    DynamicCamera dynamicCam;
    void Awake()
    {
       SpawnPlayers();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SpawnPlayers()
    {
        dynamicCam = dynamicCamObj.GetComponentInChildren<DynamicCamera>();

        CharacterSelectManager transfer = FindObjectOfType<CharacterSelectManager>();

        GameObject playerPrefab = Resources.Load("Monster/PlayerPrefab") as GameObject;

        GameObject monsterEmpty = Resources.Load("Monster/MonsterParts") as GameObject;

        if (transfer == null) { return; }

        for (int i = 0; i < transfer.storedPlayerInformation.Count; i++)
        {

            GameObject spawnedPlayer = Instantiate(playerPrefab, playerSpawnLocations[i].position, Quaternion.identity);

            spawnedPlayer.name = ("Player " + (transfer.storedPlayerInformation[i].playerIndex + 1));

            dynamicCam.playerTransforms.Add(spawnedPlayer.transform);

            NewPlayerController controller = spawnedPlayer.GetComponent<NewPlayerController>();

            controller.playerIndex = (transfer.storedPlayerInformation[i].playerIndex + 1);

            controller.inputHandler.playerInput = transfer.storedPlayerInformation[i].playerInput.GetComponent<PlayerInput>();

            spawnedPlayer.transform.parent = players;

            GameObject emptyMonster = Instantiate(monsterEmpty, spawnedPlayer.transform);

            Transform monsterPartVisualsSpawn = emptyMonster.transform.GetChild(0).GetChild(0);

            monsterAttackSystem monsterControl = emptyMonster.GetComponent<monsterAttackSystem>();

            controller.myMonster = monsterControl;

            foreach (MonsterPartData partData in transfer.storedPlayerInformation[i].monster.monsterParts)
            {
                GameObject partPrefab = Resources.Load(partData.partPrefabPath) as GameObject;

                GameObject spawnedPart = Instantiate(partPrefab, monsterPartVisualsSpawn);

                spawnedPart.transform.localPosition = partData.partPosition;

                spawnedPart.transform.localRotation = partData.partRotation;

                spawnedPart.transform.localScale = partData.partScale;

                if (partData.partButton != MonsterPartData.Button.NONE)
                {
                    monsterControl.AssignMonsterPartAttackInfo(partData.partButton, spawnedPart.GetComponentInChildren<NewMonsterPart>());
                }

                IPartAdjustable[] partAdjustments = spawnedPart.GetComponentsInChildren<IPartAdjustable>();

                if(partAdjustments.Length > 0)
                {
                    foreach(IPartAdjustable adjustable in partAdjustments)
                    {
                        adjustable.PartAdjustment(partData);
                    }
                }
            }

            monsterControl.turnOnLimbConnectors();

            StartCoroutine(Delay(monsterControl, controller));
        }
    }

    IEnumerator Delay(monsterAttackSystem monsterControl, NewPlayerController controller)
    {
        yield return new WaitForSeconds(0.1f);

        monsterControl.AssignMyPlayer(controller);

        monsterControl.turnOffLimbConnectors();

        monsterControl.connectCurrentLimbs();

        yield return new WaitForSeconds(0.25f);

        monsterControl.awakenTheBeast();
        controller.PlayerInputSetUp(); // I moved this because there was an issue where you could move before everything was setup which threw errors. -Nick
    }
}

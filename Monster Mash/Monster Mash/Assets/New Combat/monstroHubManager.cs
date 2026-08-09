using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monstroHubManager : MonoBehaviour
{
    private CinemachineTargetGroup cameraTargetGroup;
    private Transform player1;
    private monstroInputHandler player1Inputs;
    private monstroPartHandler player1PartHandler;
    private monstroHealth player1Health;
    private monstroMiscVisuals player1MiscVisuals;
    public Transform monstroHolder1;
    public Transform playerSpawn;

    public CinemachineVirtualCamera followCamera;
    public CinemachineVirtualCamera overheadCamera;
    public CinemachineVirtualCamera portalCamera;
    public CinemachineVirtualCamera buildAScareDoorCamera;
    public CinemachineVirtualCamera buildAScareCamera;
    public CinemachineVirtualCamera chopShopCamera;
    public CinemachineVirtualCamera galleryDoorCamera;
    public CinemachineVirtualCamera galleryCamera;

    public Animation introLogo;
    public AnimationClip pressAnything;
    public AnimationClip anythingPressed;

    private void Awake()
    {
        followCamera.Priority = 0;
        overheadCamera.Priority = 20;
    }

    public void placeStarterMonster()
    {
        cameraTargetGroup = FindFirstObjectByType<CinemachineTargetGroup>();
        player1Inputs = FindFirstObjectByType<monstroInputHandler>();
        player1PartHandler = FindFirstObjectByType<monstroPartHandler>();
        player1Health = FindFirstObjectByType<monstroHealth>();
        player1MiscVisuals = FindFirstObjectByType<monstroMiscVisuals>();
        player1 = player1Inputs.transform;
        player1MiscVisuals.generatePlayerRing();
        StartCoroutine(introDelay());
    }

    IEnumerator introDelay()
    {
        introLogo.clip = anythingPressed;
        introLogo.Play();
        followCamera.Priority = 20;
        overheadCamera.Priority = 0;
        yield return new WaitForSeconds(1);
        playerStartUp();
    }

    private void playerStartUp()
    {
        monstroHolder1.gameObject.SetActive(true);
        player1PartHandler.assignMonstroPartHolder(monstroHolder1);
        player1.position = playerSpawn.position;
        player1Health.resetHealth();
        player1PartHandler.showMonster();
        player1PartHandler.startMonstroAnimations();
        player1MiscVisuals.showPlayerRing();
        player1Inputs.switchToHubControls();
        StartCoroutine(followCamDelay());
    }

    IEnumerator followCamDelay()
    {
        yield return new WaitForSeconds(1.5f);
        if (cameraTargetGroup != null)
        {
            cameraTargetGroup.AddMember(player1, 1, 0.5f);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stadiumCamera : MonoBehaviour
{
    public List<Transform> cameraTargets;
    private Transform chosenCameraTarget;
    private int cameraTargetIndex;
    public Vector3 cameraOffset;
    public Vector3 cameraCenterStagePosition;
    private bool centeredOnStage = false;
    private float cameraSmoothing = 0.1f;
    private Vector3 cameraVelocity;

    private monstroFightManager fightHandler;
    private int introDelay;
    private monstroStageLogic stageHandler;
    private int eventStartDelay;
    private int eventTimeLength;
    private int timeInBetweenEvents;

    private void Awake()
    {
        centeredOnStage = true;
        chosenCameraTarget = cameraTargets[0];
        transform.position = cameraCenterStagePosition;

        fightHandler = FindFirstObjectByType<monstroFightManager>();
        introDelay = fightHandler.introDelayTime;

        stageHandler = FindFirstObjectByType<monstroStageLogic>();
        eventStartDelay = stageHandler.startDelayTime;
        eventTimeLength = stageHandler.timeLengthOfEachEvent;
        timeInBetweenEvents = stageHandler.timeBetweenEvents;
        StartCoroutine(startDelayTimer());
        StartCoroutine(eventDelayTimer());
    }

    IEnumerator startDelayTimer()
    {
        yield return new WaitForSeconds(introDelay);//this may need more of a buffer depending on the intro spawn
        yield return new WaitForSeconds(3);
        centeredOnStage = false;
        chooseNewCameraTarget();
    }

    IEnumerator eventDelayTimer()
    {
        yield return new WaitForSeconds(eventStartDelay);
        StartCoroutine(centerStageTimer());
    }

    IEnumerator centerStageTimer()
    {
        centeredOnStage = true;
        chosenCameraTarget = cameraTargets[0];
        transform.position = cameraCenterStagePosition;
        StopCoroutine(randomCamTimer());
        yield return new WaitForSeconds(eventTimeLength);
        centeredOnStage = false;
        chooseNewCameraTarget();
        yield return new WaitForSeconds(timeInBetweenEvents);
        StartCoroutine(centerStageTimer());
    }

    private void chooseNewCameraTarget()
    {
        if (centeredOnStage) return;
        if (cameraTargets.Count == 1) return;
        StopCoroutine(randomCamTimer());

        if (cameraTargets.Count == 2)//just 1 player and the center stage
        {
            chosenCameraTarget = cameraTargets[1];
            transform.position = chosenCameraTarget.position + cameraOffset;
            return;
        }

        int randomTarget = Random.Range(1, cameraTargets.Count);
        if (cameraTargets[randomTarget] == chosenCameraTarget)
        {
            chooseNewCameraTarget();
            return;
        }

        chosenCameraTarget = cameraTargets[randomTarget];
        transform.position = chosenCameraTarget.position + cameraOffset;
        StartCoroutine(randomCamTimer());
    }

    IEnumerator randomCamTimer()
    {
        int randomTimeBetweenShots = Random.Range(8, 12);
        yield return new WaitForSeconds(randomTimeBetweenShots);
        chooseNewCameraTarget();
    }

    private void LateUpdate()
    {
        if (cameraTargets.Count == 1) return;

        if (centeredOnStage == false)
        {
            Vector3 newPosition = chosenCameraTarget.position + cameraOffset;
            transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref cameraVelocity, cameraSmoothing);
        }
    }

    /*

    private void LateUpdate()
    {
        if (cameraTargets.Contains(chosenCameraTarget) == false)
        {
            chooseNewTarget();
            return;
        }


        if (chosenCameraTarget != cameraTargets[0])
        {
            Vector3 newPosition = chosenCameraTarget.position + cameraOffset;
            transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref cameraVelocity, cameraSmoothing);
        }
    }


    IEnumerator centerStageFocusTimer()
    {
        yield return new WaitForSeconds(15);
        chooseNewTarget();
    }

    private void chooseNewTarget()
    {
        if (cameraTargets.Count == 2)//just 1 player and the center stage
        {
            chosenCameraTarget = cameraTargets[1];
            transform.position = chosenCameraTarget.position + cameraOffset;
            return;
        }

        int randomTarget = Random.Range(1, cameraTargets.Count);
        if (cameraTargets[randomTarget] == chosenCameraTarget)
        {
            chooseNewTarget();
            return;
        }
        chosenCameraTarget = cameraTargets[randomTarget];
        transform.position = chosenCameraTarget.position + cameraOffset;
        StartCoroutine(randomCamTimer());

    }

    IEnumerator randomCamTimer()
    {
        int randomTimeBetweenShots = Random.Range(8, 12);
        yield return new WaitForSeconds(randomTimeBetweenShots);
        chooseNewTarget();
    }
    */
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monstroStageLogic : MonoBehaviour
{
    public enum typesOfStage
    {
        cycle, //the stage events repeat every x seconds
        randomCycle,//the stage events are randomized every x seconds
        linear,//after x seconds the stage event occurs without ever repeating
        linearProgress //after actions occur, stage event occurs without ever repeating
    };

    public typesOfStage stageLogicDropDown = new typesOfStage();
    private string chosenStageLogic;
    public animationStarter[] animationEvents;
    private int animationNumber = -1;
    public int startDelayTime;
    public int timeLengthOfEachEvent;
    public int timeBetweenEvents;

    private void Awake()
    {
        chosenStageLogic = stageLogicDropDown.ToString();
        StartCoroutine(stageLogicDelay());
    }

    IEnumerator stageLogicDelay()
    {
        yield return new WaitForSeconds(startDelayTime);

        if (chosenStageLogic == "cycle")
        {
            StartCoroutine(cycleTimer());
        }

        if (chosenStageLogic == "randomCycle")
        {
            StartCoroutine(randomCycleTimer());
        }
    }

    IEnumerator cycleTimer()
    {
        if (animationEvents.Length == 0) yield return null;

        animationNumber++;

        if (animationNumber == animationEvents.Length)
        {
            animationNumber = 0;
        }

        animationEvents[animationNumber].playAnimation();
        yield return new WaitForSeconds(timeLengthOfEachEvent);
        yield return new WaitForSeconds(timeBetweenEvents);
        StartCoroutine(cycleTimer());
    }

    IEnumerator randomCycleTimer()
    {

        if (animationEvents.Length == 0) yield return null;

        int randomChosenEvent = Random.Range(0, animationEvents.Length);

        if (randomChosenEvent == animationNumber)
        {
            StartCoroutine(randomCycleTimer());
            yield return null;
        }
        else
        {
            animationNumber = randomChosenEvent;
            animationEvents[animationNumber].playAnimation();
        }

        yield return new WaitForSeconds(timeLengthOfEachEvent);
        yield return new WaitForSeconds(timeBetweenEvents);
        StartCoroutine(randomCycleTimer());
    }


}

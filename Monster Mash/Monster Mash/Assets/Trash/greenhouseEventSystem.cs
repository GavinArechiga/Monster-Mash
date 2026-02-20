using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class greenhouseEventSystem : MonoBehaviour
{
    public float timeUntilGameplay;
    public Animator lightingSystem;
    public GameObject[] plantSystems;
    public ParticleSystem[] dayEffects;
    public ParticleSystem[] rainEffects;
    public Animator[] branches;
    private int selectedPlantInArray;
    public float normalStageLifetime;
    public float plantStageLifetime;
    private bool readyForRain = false;
    public float rainReactionTime;
    public float rainStageLifetime;

    //start events is called by finished cinematic or outside script from someone skipping it
    //next is the normal stage timer
    //shuffle plants and turn on intended plant
    //next is the plant stage timer
    //kill the plant
    //next is the normal stage timer
    //next is the rain stage timer
    //reset and repeat

    private void Awake()
    {
        StartCoroutine(startSequence());
    }

    IEnumerator startSequence()
    {
        yield return new WaitForSeconds(timeUntilGameplay);
        startEvents();
    }

    public void forceStartEvents()
    {
        StopAllCoroutines();
        setNormalStage();
    }

    private void startEvents()
    {
        setNormalStage();
    }

    private void setNormalStage()
    {
        lightingSystem.SetBool("isRaining", false);
        StartCoroutine(normalStageTimer());
    }

    IEnumerator normalStageTimer()
    {
        yield return new WaitForSeconds(normalStageLifetime);

        if (readyForRain)
        {
            makeItRain();
        }
        else
        {
            shufflepPlants();
        }
    }

    private void shufflepPlants()
    {
        int potentialNewPlant = Random.Range(0, plantSystems.Length);

        if (potentialNewPlant == selectedPlantInArray)
        {
            shufflepPlants();
        }
        else
        {
            selectedPlantInArray = potentialNewPlant;
            plantSystems[selectedPlantInArray].SetActive(true);
            StartCoroutine(plantStageTimer());
        }
    }

    IEnumerator plantStageTimer()
    {
        yield return new WaitForSeconds(plantStageLifetime);
        killPlant();
    } 

    private void killPlant()
    {
        readyForRain = true;
        plantSystems[selectedPlantInArray].GetComponent<Animator>().SetTrigger("die");
        //plantSystems[selectedPlantInArray].SetActive(false);
        StartCoroutine(plantDeathTimer());
        setNormalStage();
    }

    IEnumerator plantDeathTimer()
    {
        yield return new WaitForSeconds(5);
        plantSystems[selectedPlantInArray].SetActive(false);
    }

    private void makeItRain()
    {
        lightingSystem.SetBool("isRaining", true);

        for (int i = 0; i < dayEffects.Length; i++)
        {
            dayEffects[i].Stop();
        }

        StartCoroutine(rainStageTimer());
        StartCoroutine(rainReactionTimer());
    }

    IEnumerator rainStageTimer()
    {
        yield return new WaitForSeconds(rainStageLifetime);
        stopRain();
    }

    IEnumerator rainReactionTimer()
    {
        yield return new WaitForSeconds(rainReactionTime);
        rainReaction();
    }

    private void rainReaction()
    {
        if (readyForRain)
        {
            for (int i = 0; i < rainEffects.Length; i++)
            {
                rainEffects[i].Play();
            }

            for (int i = 0; i < branches.Length; i++)
            {
                branches[i].SetBool("isRaining", true);
            }

            //slippery materials
        }
        else
        {
            for (int i = 0; i < rainEffects.Length; i++)
            {
                rainEffects[i].Stop();
            }

            for (int i = 0; i < branches.Length; i++)
            {
                branches[i].SetBool("isRaining", false);
            }

            for (int i = 0; i < dayEffects.Length; i++)
            {
                dayEffects[i].Play();
            }


            //stop slippery materials
        }
    }

    private void stopRain()
    {
        readyForRain = false;
        StartCoroutine(rainReactionTimer());
        setNormalStage();
       
    }
}

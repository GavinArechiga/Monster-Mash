using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monstroHealth : MonoBehaviour
{
    private monstroLocomotion locomotion;
    private monstroMiscVisuals monstroMiscVis;
    public int health = 200;
    private int healthPerPart = 200;
    public int mappedParts = 1;
    private int partsLeft = 1;
    public Transform launchPoint;

    //built in damage from hazards
    //hazard damage is built in to limit the traffic and back and forth needed for info that could just be sourced locally
    private int fireDamagePerSecond = 10;
    private bool isOnFire = false;
    private bool enteredFireCollider = false;
    private int carDamage = 50;
    private int sharkDamage = 30;
    private int pressDamage = 50;
    private int electricityDamage = 20;

    private void Awake()
    {
        locomotion = GetComponent<monstroLocomotion>();
        monstroMiscVis = GetComponent<monstroMiscVisuals>();
    }

    public void resetHealth()
    {
        partsLeft = mappedParts;
        health = healthPerPart * partsLeft;
    }

    public void takeDamage(int damageTotal)
    {
        if (health > damageTotal)
        {
            health = health - damageTotal;

            if (health < ((partsLeft - 1) * healthPerPart))//this is asking us if we've passed the threshold to lose another monster part
            {
                //lose part
                loseMonsterPart();
            }
        }
        else
        {
            //destruction
            totalDestruction();
        }
    }

    private void loseMonsterPart()
    {
        print("lost a monster part!");
        partsLeft = partsLeft - 1;
    }

    private void totalDestruction()
    {
        print("I have been destroyed!");
        health = 0;
    }

    private void OnTriggerEnter(Collider other)//Damage Triggers
    {
        //note, to return to the last version, plug in other instead of launch point
        //Vector3 exactHitPoint = other.ClosestPointOnBounds(other.transform.position);

        if (other.gameObject.tag == "Hazard") //Hazards just need a trigger box, the hazard tag, and a hazard script
        {
            hazard hazardHandler = other.GetComponent<hazard>();
            string hazardName = hazardHandler.selectedHazard;
            bool reverseLaunchNeeded = hazardHandler.needsDirectLaunchTarget;

            if (reverseLaunchNeeded)
            {
                launchPoint.transform.position = hazardHandler.directLaunchTarget.position;
            }
            else
            {
                Vector3 exactHitPoint = other.ClosestPoint(other.transform.position);
                launchPoint.transform.position = exactHitPoint;
            }

            if (hazardName == "fire" && isOnFire == false)
            {
                StartCoroutine(takeFireDamage());
                hazardHandler.playHazardAnimation();
                enteredFireCollider = true;
            }

            if(hazardName == "car")
            {
                locomotion.damageLaunch(launchPoint, true, reverseLaunchNeeded);
                takeDamage(carDamage);
                hazardHandler.playHazardAnimation();
            }

            if (hazardName == "trap")
            {
                hazardHandler.playTrapAnimation();
                hazardHandler.playHazardAnimation();
            }

            if (hazardName == "slice")
            {
                locomotion.damageLaunch(launchPoint, false, reverseLaunchNeeded);
                takeDamage(sharkDamage);
                hazardHandler.playHazardAnimation();
            }

            if (hazardName == "press")
            {
                takeDamage(pressDamage);
                locomotion.forceRespawn();
                hazardHandler.playHazardAnimation();
            }

            if (hazardName == "rubberBand")
            {
                if (locomotion.isStunLocked) //you will only bounce back if you were punched into the rubber band, otherwise it will ignore it
                {
                    locomotion.damageLaunch(launchPoint, false, reverseLaunchNeeded);
                    hazardHandler.playHazardAnimation();
                }
            }

            if (hazardName == "electricity")
            {
                locomotion.electricDamageLaunch(launchPoint, true, reverseLaunchNeeded);
                takeDamage(electricityDamage);
                monstroMiscVis.playElectricEffect();
                hazardHandler.playHazardAnimation();
            }

            if (hazardName == "antiAir")
            {
                locomotion.antiAirDamageLaunch(launchPoint, true, reverseLaunchNeeded);
                hazardHandler.playHazardAnimation();
            }

        }

        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Hazard")
        {
            hazard hazardHandler = other.GetComponent<hazard>();
            string hazardName = hazardHandler.selectedHazard;

            if (hazardName == "fire")
            {
                enteredFireCollider = false;
            }
        }
    }

    IEnumerator takeFireDamage()
    {
        //fire will inflict 50 damage overall over 4 seconds
        isOnFire = true;
        monstroMiscVis.playFireEffect();
        takeDamage(fireDamagePerSecond);
        yield return new WaitForSeconds(1);
        takeDamage(fireDamagePerSecond);
        yield return new WaitForSeconds(1);
        takeDamage(fireDamagePerSecond);
        yield return new WaitForSeconds(1);
        takeDamage(fireDamagePerSecond);
        yield return new WaitForSeconds(1);
        takeDamage(fireDamagePerSecond);
        isOnFire = false;
        monstroMiscVis.stopFireEffect();
        /*
        if (enteredFireCollider)//they're still in the fire
        {
            StartCoroutine(takeFireDamage());
            print("one more round of fire!");
        }
        else
        {
            isOnFire = false;
            monstroMiscVis.stopFireEffect();
        }
        */
        //I'll come back to this later, still not really sure if its a real issue that fire isn't reapplying while standing in the fire
    }
}

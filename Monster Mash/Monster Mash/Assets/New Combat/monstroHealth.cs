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

    //built in damage from hazards
    //hazard damage is built in to limit the traffic and back and forth needed for info that could just be sourced locally
    private int fireDamagePerSecond = 10;
    private bool isOnFire = false;
    private int carDamage = 50;
    private int sharkDamage = 30;
    private int toothDamage = 50;
    private int pressDamage = 50;

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
        if (other.gameObject.tag == "Hazard") //Hazards just need a trigger box, the hazard tag, and a hazard script
        {
            string hazardName = other.GetComponent<hazard>().selectedHazard;

            if (hazardName == "fire" && isOnFire == false)
            {
                StartCoroutine(takeFireDamage());
            }

            if(hazardName == "car")
            {
                locomotion.damageLaunch(other, true);
                takeDamage(carDamage);
            }

            if(hazardName == "shark")
            {
                locomotion.damageLaunch(other, false);
                takeDamage(sharkDamage);
            }

            if(hazardName == "teeth")
            {
                takeDamage(toothDamage);
                locomotion.forceRespawn();//we put things like a respawn first through the locomotion script to stop all velocity and movement
            }

            if (hazardName == "press")
            {
                takeDamage(pressDamage);
                locomotion.forceRespawn();
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
    }
}

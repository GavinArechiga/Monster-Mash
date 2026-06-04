using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monstroHealth : MonoBehaviour
{
    private monstroLocomotion locomotion;
    public int health = 200;
    private int healthPerPart = 200;
    public int mappedParts = 1;
    private int partsLeft = 1;

    //built in damage from hazards
    private int fireDamagePerSecond = 10;
    private bool isOnFire = false;
    private int carDamage = 50;

    private void Awake()
    {
        locomotion = GetComponent<monstroLocomotion>();
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
        if (other.gameObject.tag == "Fire" && isOnFire == false)
        {
            StartCoroutine(takeFireDamage());
        }

        if (other.gameObject.tag == "Car")
        {
            locomotion.damageLaunch(other, true);
            takeDamage(carDamage);
        }
    }

    IEnumerator takeFireDamage()
    {
        //fire will inflict 50 damage overall over 4 seconds
        isOnFire = true;
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
    }
}

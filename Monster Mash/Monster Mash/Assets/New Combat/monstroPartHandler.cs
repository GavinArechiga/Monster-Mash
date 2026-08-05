using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monstroPartHandler : MonoBehaviour
{
    //this script handles communication with all the monster parts
    //it will read context from the other scripts and handle when a new monster is chosen, when attack or movement inputs are read
    //this script will not do much context thinking though
    //Deep thinking should be handled by other scripts before calling on these functions
    private monstroInputHandler myInputHandler;
    private monstroHolder myMonsterPartHolder;
    public Transform characterRotator;
    private Transform monsterPartHolderTransform;
    private monstroPart[] myMonstroParts;
    public List<monstroPart> availableAttackingParts = new List<monstroPart>();
    public int numberOfMappedParts = 0;

    private void Awake()
    {
        myInputHandler = GetComponent<monstroInputHandler>();
    }
    public void assignMonstroPartHolder(Transform newMonsterPartHolder)
    {
        numberOfMappedParts = 0;
        monsterPartHolderTransform = newMonsterPartHolder;
        monsterPartHolderTransform.parent = characterRotator; //parent the new selected monster under the player and specifically under the object we rotate with movement
        myMonstroParts = GetComponentsInChildren<monstroPart>();
        myMonsterPartHolder = newMonsterPartHolder.GetComponent<monstroHolder>();
        availableAttackingParts.Clear();

        if(myInputHandler == null)
        {
            myInputHandler = GetComponent<monstroInputHandler>();
        }

        if (myMonsterPartHolder.buttonEast_MonstroPart != null)
        {
            myInputHandler.mappedMonstroParts[0] = myMonsterPartHolder.buttonEast_MonstroPart;
            availableAttackingParts.Add(myMonsterPartHolder.buttonEast_MonstroPart);
            numberOfMappedParts++;
        }

        if (myMonsterPartHolder.buttonWest_MonstroPart != null)
        {
            myInputHandler.mappedMonstroParts[1] = myMonsterPartHolder.buttonWest_MonstroPart;
            availableAttackingParts.Add(myMonsterPartHolder.buttonWest_MonstroPart);
            numberOfMappedParts++;
        }

        if (myMonsterPartHolder.buttonNorth_MonstroPart != null)
        {
            myInputHandler.mappedMonstroParts[2] = myMonsterPartHolder.buttonNorth_MonstroPart;
            availableAttackingParts.Add(myMonsterPartHolder.buttonNorth_MonstroPart);
            numberOfMappedParts++;
        }

        if (myMonsterPartHolder.leftBumper_MonstroPart != null)
        {
            myInputHandler.mappedMonstroParts[3] = myMonsterPartHolder.leftBumper_MonstroPart;
            availableAttackingParts.Add(myMonsterPartHolder.leftBumper_MonstroPart);
            numberOfMappedParts++;
        }

        if (myMonsterPartHolder.rightBumper_MonstroPart != null)
        {
            myInputHandler.mappedMonstroParts[4] = myMonsterPartHolder.rightBumper_MonstroPart;
            availableAttackingParts.Add(myMonsterPartHolder.rightBumper_MonstroPart);
            numberOfMappedParts++;
        }

        if (myMonsterPartHolder.leftTrigger_MonstroPart != null)
        {
            myInputHandler.mappedMonstroParts[5] = myMonsterPartHolder.leftTrigger_MonstroPart;
            availableAttackingParts.Add(myMonsterPartHolder.leftTrigger_MonstroPart);
            numberOfMappedParts++;
        }

        if (myMonsterPartHolder.rightTrigger_MonstroPart != null)
        {
            myInputHandler.mappedMonstroParts[6] = myMonsterPartHolder.rightTrigger_MonstroPart;
            availableAttackingParts.Add(myMonsterPartHolder.rightTrigger_MonstroPart);
            numberOfMappedParts++;
        }

        generateAnimators();
    }

    public void generateAnimators()
    {
        for (int i = 0; i < myMonstroParts.Length; i++)
        {
            myMonstroParts[i].createNewAnimator();
        }
    }

    public void startMonstroAnimations()
    {
        for (int i = 0; i < myMonstroParts.Length; i++)
        {
            myMonstroParts[i].playIdle();
        }
    }

    public void stopMonstroAnimations()
    {
        for (int i = 0; i < myMonstroParts.Length; i++)
        {
            myMonstroParts[i].stopAnimations();
        }
    }

    public void hideMonster()
    {
        for (int i = 0; i < myMonstroParts.Length; i++)
        {
            myMonstroParts[i].hideMonster();
        }
    }

    public void showMonster()
    {
        for (int i = 0; i < myMonstroParts.Length; i++)
        {
            myMonstroParts[i].showMonster();
        }
    }

    public void idle()
    {
        for (int i = 0; i < myMonstroParts.Length; i++)
        {
            myMonstroParts[i].playIdle();
        }
    }
    
    public void walk()
    {
        for (int i = 0; i < myMonstroParts.Length; i++)
        {
            myMonstroParts[i].playWalk();
        }
    }

    public void run()
    {
        for (int i = 0; i < myMonstroParts.Length; i++)
        {
            myMonstroParts[i].playRun();
        }
    }

    public void jump()
    {
        for (int i = 0; i < myMonstroParts.Length; i++)
        {
            myMonstroParts[i].playJump();
        }
    }

    public void doubleJump()
    {
        myMonsterPartHolder.playDoubleJump();

        for (int i = 0; i < myMonstroParts.Length; i++)
        {
            myMonstroParts[i].playDoubleJump();
        }
    }

    public void fall()
    {
        for (int i = 0; i < myMonstroParts.Length; i++)
        {
            myMonstroParts[i].playFall();
        }
    }

    public void land()
    {
        for (int i = 0; i < myMonstroParts.Length; i++)
        {
            myMonstroParts[i].playLand();
        }
    }

    public void windUp(monstroPart attackingPart)
    {
        attackingPart.playWindUp();

        for (int i = 0; i < myMonstroParts.Length; i++)
        {
            if (myMonstroParts[i].monstroPartDropDown.ToString() == "torso")
            {
                myMonstroParts[i].playWindUp();
            }
            else
            {

            }
        }
    }

    public void attack(monstroPart attackingPart, bool attackMarkedHeavy)//feed in here the monster part that is attacking
    {
        if (attackMarkedHeavy)
        {
            attackingPart.playHeavyAttack();
        }
        else
        {
            attackingPart.playLightAttack();
        }

        for (int i = 0; i < myMonstroParts.Length; i++)
        {
            if (myMonstroParts[i].monstroPartDropDown.ToString() == "torso")
            {

                if (attackMarkedHeavy)
                {
                    myMonstroParts[i].playHeavyAttack();
                }
                else
                {
                    myMonstroParts[i].playLightAttack();
                }
            }
            else
            {

                if (attackingPart.transform.localScale.x < 0)
                {
                    if (myMonstroParts[i] != attackingPart)
                    {
                        myMonstroParts[i].playLeftBrace();
                    }
                }
                else
                {
                    if (myMonstroParts[i] != attackingPart)
                    {
                        myMonstroParts[i].playRightBrace();
                    }
                }
            }
        }
    }

    public void lockAttackAnimation(monstroPart attackingPart)
    {
        attackingPart.lockAttackAnimation();
    }

    public void unlockAttackAnimation(monstroPart attackingPart)
    {
        attackingPart.unlockAttackAnimation();
    }

    public void lightHit()
    {
        for (int i = 0; i < myMonstroParts.Length; i++)
        {
            myMonstroParts[i].showDamageVisual();
            myMonstroParts[i].hideMonster();
            myMonstroParts[i].playLightHit();
        }

        StartCoroutine(damageFlash());
    }

    public void heavyHit()
    {
        myMonsterPartHolder.playHeavyHit();

        for (int i = 0; i < myMonstroParts.Length; i++)
        {
            myMonstroParts[i].showDamageVisual();
            myMonstroParts[i].hideMonster();
            myMonstroParts[i].playHeavyHit();
        }

        StartCoroutine(damageFlash());
    }

    public void showDamageVisual()
    {
        for (int i = 0; i < myMonstroParts.Length; i++)
        {
            myMonstroParts[i].showDamageVisual();
        }
    }

    public void hideDamageVisual()
    {
        for (int i = 0; i < myMonstroParts.Length; i++)
        {
            myMonstroParts[i].hideDamageVisual();
        }
    }

    IEnumerator damageFlash()
    {
        yield return new WaitForSeconds(0.1f);
        showMonster();
        hideDamageVisual();
    }

    public void removeRandomPart()
    {
        int randomPart = Random.Range(0, availableAttackingParts.Count);
        availableAttackingParts[randomPart].removeThisPart();
        availableAttackingParts.Remove(availableAttackingParts[randomPart]);
    }

    public void destroyMonster()
    {
        for (int i = 0; i < myMonstroParts.Length; i++)
        {
            myMonstroParts[i].removeThisPart();
        }

        availableAttackingParts.Clear();
    }

    public void burningReaction()
    {
        for (int i = 0; i < myMonstroParts.Length; i++)
        {
            myMonstroParts[i].playBurningReaction();
        }
    }

    public void electrocutionReaction()
    {
        for (int i = 0; i < myMonstroParts.Length; i++)
        {
            myMonstroParts[i].playElectrocutionReaction();
        }
    }

    public void endStatusEffect()
    {
        for (int i = 0; i < myMonstroParts.Length; i++)
        {
            myMonstroParts[i].endStatusEffectReaction();
        }
    }

    public void shock()
    {

    }

    public void launch()
    {

    }

    public void recover()
    {

    }
}

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
   

    private void Awake()
    {
        myInputHandler = GetComponent<monstroInputHandler>();
    }
    public void assignMonsterPartHolder(Transform newMonsterPartHolder)
    {
        monsterPartHolderTransform = newMonsterPartHolder;
        monsterPartHolderTransform.parent = characterRotator; //parent the new selected monster under the player and specifically under the object we rotate with movement
        myMonstroParts = GetComponentsInChildren<monstroPart>();
        myMonsterPartHolder = newMonsterPartHolder.GetComponent<monstroHolder>();

        if (myMonsterPartHolder.buttonEast_MonstroPart != null)
        {
            myInputHandler.mappedMonstroParts[0] = myMonsterPartHolder.buttonEast_MonstroPart;
        }

        if (myMonsterPartHolder.buttonWest_MonstroPart != null)
        {
            myInputHandler.mappedMonstroParts[1] = myMonsterPartHolder.buttonWest_MonstroPart;
        }

        if (myMonsterPartHolder.buttonNorth_MonstroPart != null)
        {
            myInputHandler.mappedMonstroParts[2] = myMonsterPartHolder.buttonNorth_MonstroPart;
        }

        if (myMonsterPartHolder.leftBumper_MonstroPart != null)
        {
            myInputHandler.mappedMonstroParts[3] = myMonsterPartHolder.leftBumper_MonstroPart;
        }

        if (myMonsterPartHolder.rightBumper_MonstroPart != null)
        {
            myInputHandler.mappedMonstroParts[4] = myMonsterPartHolder.rightBumper_MonstroPart;
        }

        if (myMonsterPartHolder.leftTrigger_MonstroPart != null)
        {
            myInputHandler.mappedMonstroParts[5] = myMonsterPartHolder.leftTrigger_MonstroPart;
        }

        if (myMonsterPartHolder.rightTrigger_MonstroPart != null)
        {
            myInputHandler.mappedMonstroParts[6] = myMonsterPartHolder.rightTrigger_MonstroPart;
        }

        //generateAnimators();
    }

    public void generateAnimators()
    {
        myInputHandler.mappedMonstroParts[0].createNewAnimator();
    }

    public void startMonsterAnimations()
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
            myMonstroParts[i].playLand();
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
                //play brace
            }
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

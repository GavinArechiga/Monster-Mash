using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monstroHolder : MonoBehaviour
{
    public Animator monstroHolderAnimator;
    public monstroPart buttonEast_MonstroPart;
    public monstroPart buttonWest_MonstroPart;
    public monstroPart buttonNorth_MonstroPart;
    public monstroPart leftBumper_MonstroPart;
    public monstroPart rightBumper_MonstroPart;
    public monstroPart leftTrigger_MonstroPart;
    public monstroPart rightTrigger_MonstroPart;

    public void cleanAnimations()
    {
        if (monstroHolderAnimator == null) return;

        monstroHolderAnimator.ForceResetTriggers();
    }
    public void playDoubleJump()
    {
        cleanAnimations();

        if (monstroHolderAnimator == null) return;

        monstroHolderAnimator.SetTrigger("double jump");
    }

    public void playHeavyDamage()
    {
        cleanAnimations();

        if (monstroHolderAnimator == null) return;

        monstroHolderAnimator.SetTrigger("heavy damage");
    }
}

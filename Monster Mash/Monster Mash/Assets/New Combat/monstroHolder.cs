using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static monstroPart;

public class monstroHolder : MonoBehaviour
{
    public enum typesOfLocomotion
    {
        brawler,
        aviator,
        heavy,
        ghost
    }
    public typesOfLocomotion monstroLocomotionDropDown = new typesOfLocomotion();

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

    public void playHeavyHit()
    {
        cleanAnimations();

        if (monstroHolderAnimator == null) return;

        monstroHolderAnimator.SetTrigger("heavy hit");
    }
}

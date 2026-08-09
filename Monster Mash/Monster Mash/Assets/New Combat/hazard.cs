using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hazard : MonoBehaviour
{
    public enum typesOfHazard
    {
        fire,
        car,
        slice,
        press,
        trap,
        rubberBand,
        electricity,
        antiAir
    };

    //header
    private Collider triggerCollider;
    private Animator hazardAnimator;//see if we can change this as well, i would rather drag something into it from the inspector so we aren't reliant on inheritance
    public animationStarter animationHandler; //rename to animation starter, animation handlers are the animation or animator components only
    //header
    public typesOfHazard hazardDropDown = new typesOfHazard();
    public string selectedHazard;
    //header
    public Transform directLaunchTarget;
    public bool needsDirectLaunchTarget;

    private void Awake()
    {
        selectedHazard = hazardDropDown.ToString();
        triggerCollider = this.GetComponent<Collider>();
        hazardAnimator = this.GetComponent<Animator>();
    }

    public void playTrapAnimation()
    {
        triggerCollider.enabled = false;
        hazardAnimator.SetTrigger("playAnimation");
        StartCoroutine(clearAnimatorData());
    }

    IEnumerator clearAnimatorData()
    {
        yield return new WaitForSeconds(0.5f);
        hazardAnimator.ResetTrigger("playAnimation");
    }

    public void playHazardAnimation()
    {
        if (animationHandler != null)
        {
            animationHandler.playAnimation();
        }
    }

}

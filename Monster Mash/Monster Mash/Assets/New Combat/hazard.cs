using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hazard : MonoBehaviour
{
    public enum typesOfHazard
    {
        fire,
        car,
        shark,
        teeth,
        press,
        trap
    };

    private Collider triggerCollider;
    private Animator hazardAnimator;
    public typesOfHazard hazardDropDown = new typesOfHazard();
    public string selectedHazard;

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

}

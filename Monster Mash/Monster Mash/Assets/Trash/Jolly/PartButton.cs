using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartButton : MonoBehaviour
{
    public Animator anim;
    [HideInInspector] public PartGrid grid;






    // animation stuff
    public void PlayJostleAnim()
    {
        anim.SetLayerWeight(1, 1);
        anim.SetTrigger("Jostle");
    }
    public void JostleAnimDone()
    {
        anim.SetLayerWeight(1, 0);
    }

    // checks this part's index in
    // the list, to see if the ScrollView
    // needs to scroll to another page
    public void CheckIndex()
    {
        grid.CheckScrollPage(gameObject);
    }

}

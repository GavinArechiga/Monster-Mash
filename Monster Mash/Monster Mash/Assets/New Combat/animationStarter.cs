using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationStarter : MonoBehaviour
{
    public Animation myAnimationHandler;
    public AnimationClip myAnimationClip;
    public bool isTrampolineAnimation;//the only reason this bool exists is to stop trampoline animations from going off when running into them,
                                      //the player needs to be above the trampoline and ready to bounce on them

    //throw this on to anything that you want to trigger an animation
    //we keep all these variables public instead of grabbed during awake so that we have the freedom to spur animations on to anything in the stage...
    //...not just animations tied to this object

    public void playAnimation()
    {
        myAnimationHandler.Stop();
        if(myAnimationClip != null)
        {
            myAnimationHandler.clip = myAnimationClip;
        }
        myAnimationHandler.Play();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationStarter : MonoBehaviour
{
    public Animation myAnimationHandler;
    public AnimationClip myAnimationClip;
    public bool isTrampolineAnimation;

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationStarter : MonoBehaviour
{
    public Animation myAnimationClip;

    public void playAnimation()
    {
        myAnimationClip.Stop();
        myAnimationClip.Play();
    }
}

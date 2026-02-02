using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtentionFunctions
{
    /// <summary>
    /// Resets all triggers in the animator.
    /// </summary>
    /// <param name="animator"></param>
    public static void ForceResetTriggers(this Animator animator)
    {
        foreach (var param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(param.name);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PartAttackBehaviour : StateMachineBehaviour
{
    public Action endAction;
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        endAction?.Invoke();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingState : PlayerState
{
    public AttackingState(NewPlayerController controller) : base(controller) { }

    public override void Enter()
    {

    }

    public override void HandleInput()
    {
        /*
        if (!controller.isAttacking)
        {
            controller.ChangeState(new IdleState(controller));
        }
        */
    }

    public override void Update()
    {
        
    }
}

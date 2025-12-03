using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(NewPlayerController controller) : base(controller) { }

    public override void Enter()
    {

    }

    public override void HandleInput()
    {
        if (controller.inputHandler.LeftStick.magnitude > 0.2f)
        {
            controller.ChangeState(new WalkingState(controller));
        }
    }

    public override void Update()
    {

    }
}

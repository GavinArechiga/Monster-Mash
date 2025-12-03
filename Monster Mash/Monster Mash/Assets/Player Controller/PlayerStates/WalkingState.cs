using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingState : PlayerState
{
    public WalkingState(NewPlayerController controller) : base(controller) { }

    public override void Enter()
    {
        controller.isWalking = true;
        controller.isRunning = false;
        controller.startWalkingVisual();
        controller.turnOffFriction();
    }

    public override void HandleInput()
    {

    }

    public override void Exit()
    {
        controller.isWalking = false;
        controller.stopWalkingVisual();
    }
}

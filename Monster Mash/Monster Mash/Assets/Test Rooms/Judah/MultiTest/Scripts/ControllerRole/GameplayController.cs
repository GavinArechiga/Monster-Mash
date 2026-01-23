using UnityEngine;

public class GameplayController : ControllerRole
{
    public override void OnActivate()
    {
        Debug.Log("gameplay role activated!");
    }

    public override void OnDeactivate()
    {
        Debug.Log("gameplay role deactivated!");
    }
}

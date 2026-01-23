using UnityEngine;

public class UIController : ControllerRole
{
    public override void OnActivate()
    {
        Debug.Log("UI role activated!");
    }

    public override void OnDeactivate()
    {
        Debug.Log("UI role deactivated!");
    }
}

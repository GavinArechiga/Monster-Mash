using UnityEngine;

public class UIRole : ControllerRole
{
    public override void OnActivate()
    {
        Debug.Log("UI role activated!");
    }

    public override void OnDeactivate()
    {
        Debug.Log("UI role deactivated!");
    }

    public void OnMove()
    {

    }

    public void OnClick()
    {

    }
}

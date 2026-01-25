using UnityEngine;
using UnityEngine.InputSystem;
public class GameplayRole : ControllerRole
{
    UIControllerTemp myController;
    public override void OnActivate()
    {
        Debug.Log("gameplay role activated!");
        myController = GetComponent<UIControllerTemp>();
    }

    public override void OnDeactivate()
    {
        Debug.Log("gameplay role deactivated!");
    }
}

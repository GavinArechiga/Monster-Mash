using UnityEngine;
using UnityEngine.InputSystem;
public class GameplayRole : ControllerRole
{
    PLayerMoveTemp myController;
    public override void OnActivate()
    {
        Debug.Log("gameplay role activated!");
        myController = GetComponent<PLayerMoveTemp>();
    }

    public override void OnDeactivate()
    {
        Debug.Log("gameplay role deactivated!");
    }

    public void OnMove(InputValue value)
    {
        myController.Move(value);
    }

    public void Click()
    {
        myController.Click();
    }
}

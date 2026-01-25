using UnityEngine;
using UnityEngine.InputSystem;
public class UIRole : ControllerRole
{
    PLayerMoveTemp myController; //REPLACE WItH NON GARBAGE SCRIPT. GOOD LORD THIS THNG IS TEMPORARY I PROMISE
    public override void OnActivate()
    {
        Debug.Log("UI role activated!");
        myController = GetComponent<PLayerMoveTemp>();
    }

    public override void OnDeactivate()
    {
        Debug.Log("UI role deactivated!");
    }

    public void OnMove(InputValue value)
    {
        myController.Move(value);
    }

    public void OnClick()
    {
        myController.Click();
    }
}

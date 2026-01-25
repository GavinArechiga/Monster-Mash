using UnityEngine;
using UnityEngine.InputSystem;
public class UIRole : ControllerRole
{
    UIControllerTemp myController; //REPLACE WItH NON GARBAGE SCRIPT. GOOD LORD THIS THNG IS TEMPORARY I PROMISE
    public override void OnActivate()
    {
        Debug.Log("UI role activated!");
        myController = GetComponent<UIControllerTemp>();
    }

    public override void OnDeactivate()
    {
        Debug.Log("UI role deactivated!");
    }

    public void OnNavigate()
    {
        myController.Navigate();
    }
}

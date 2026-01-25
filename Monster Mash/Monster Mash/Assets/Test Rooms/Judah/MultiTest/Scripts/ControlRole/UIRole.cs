using UnityEngine;
public class UIRole : ControllerRole
{
    UIControllerTemp myController; //REPLACE WItH NON GARBAGE SCRIPT. GOOD LORD THIS THNG IS TEMPORARY I PROMISE
    public override string ActionMap => "UI";

    public override void OnActivate()
    {
        myController = GetComponent<UIControllerTemp>();
    }
    public void OnNavigate()
    {
        myController.Navigate();
    }
}

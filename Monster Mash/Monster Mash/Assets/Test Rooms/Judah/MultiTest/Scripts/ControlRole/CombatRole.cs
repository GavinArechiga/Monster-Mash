using UnityEngine;
public class CombatRole : ControllerRole
{
    CombatControllerTemp myController;
    public override string ActionMap => "Combat";

    public override void OnActivate()
    {
        myController = GetComponent<CombatControllerTemp>();
    }
    public void OnMove(Vector2 value)
    {
        myController.Move(value);
    }
}

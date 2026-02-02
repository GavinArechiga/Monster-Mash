
//These are the Concrete Command implementations of the ICombatActionCommand Interface
//The Commands for Attacking and Taunting impliment a enum that corresponds to the button that was pressed
public class SouthButtonActionCommand : ICombatActionCommand
{
    public void ExecuteAction()
    {
        // Player Jump
    }
}

public class CombatButtonActionCommand : ICombatActionCommand
{
    private AttackButtons _attackButton;

    public CombatButtonActionCommand(AttackButtons attackButton)
    {
        _attackButton = attackButton;
    }

    public void ExecuteAction()
    {
        //Take in Player Button and Execute Corresponding Attack
    }
}

public class TauntButtonActionCommand : ICombatActionCommand
{
    private TauntButtons _tauntButton;

    public TauntButtonActionCommand(TauntButtons tauntButton)
    {
        _tauntButton = tauntButton;
    }
    public void ExecuteAction()
    {
        //Take in Player Taunt Direction and Execute Corresponding Taunt
    }
}

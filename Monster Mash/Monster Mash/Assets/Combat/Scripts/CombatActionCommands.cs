
public class BaseActionCommand
{
    protected int actionPriority { get; set; }

    public int GetActionPriority()
    {
        return actionPriority;
    }
}

public class BaseAttackCommand : BaseActionCommand
{
    protected AttackButtons _attackButton { get; set; }

    protected CombatMonster _combatMonster { get; set; }

    public AttackButtons GetAttackButton()
    {
        return _attackButton;
    }
}

//These are the Concrete Command implementations of the ICombatActionCommand Interface
//The Commands for Attacking and Taunting impliment an enum that corresponds to the button that was pressed
public class SouthButtonActionCommand : BaseActionCommand, ICombatActionCommand
{
    public SouthButtonActionCommand()
    {
        actionPriority = 1;
    }
    public void ExecuteAction()
    {
        // Player Jump
    }
}

public class CombatButtonActionCommand : BaseAttackCommand, ICombatActionCommand
{
    public CombatButtonActionCommand(AttackButtons attackButton, CombatMonster combatMonster)
    {
        actionPriority = 2;
        _attackButton = attackButton;
        _combatMonster = combatMonster;
    }

    public void ExecuteAction()
    {
        //Take in Player Button and Execute Corresponding Attack
        _combatMonster.MonsterPartAttack(_attackButton);
    }
}

public class HeavyStartActionCommand : BaseAttackCommand, ICombatActionCommand
{
    public HeavyStartActionCommand(AttackButtons attackButton, CombatMonster combatMonster)
    {
        actionPriority = 3;
        _attackButton = attackButton;
        _combatMonster = combatMonster;
    }
    public void ExecuteAction()
    {
        _combatMonster.HeavyAttackCharge(_attackButton);
    }
}

public class HeavyReleaseActionCommand : BaseAttackCommand, ICombatActionCommand
{
    public HeavyReleaseActionCommand(AttackButtons attackButton, CombatMonster combatMonster)
    {
        actionPriority = -1;
        _attackButton = attackButton;
        _combatMonster = combatMonster;
    }
    public void ExecuteAction()
    {
        _combatMonster.HeavyAttackRelease(_attackButton);
    }
}

public class TauntButtonActionCommand : BaseActionCommand, ICombatActionCommand
{
    private TauntButtons _tauntButton;

    public TauntButtonActionCommand(TauntButtons tauntButton)
    {
        actionPriority = 0;
        _tauntButton = tauntButton;
    }
    public void ExecuteAction()
    {
        //Take in Player Taunt Direction and Execute Corresponding Taunt
    }
}

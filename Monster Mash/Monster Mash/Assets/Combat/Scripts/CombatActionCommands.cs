
public class BaseActionCommand
{
    public int actionPriority;
}

//These are the Concrete Command implementations of the ICombatActionCommand Interface
//The Commands for Attacking and Taunting impliment a enum that corresponds to the button that was pressed
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

public class CombatButtonActionCommand : BaseActionCommand, ICombatActionCommand
{
    private AttackButtons _attackButton;

    private CombatMonster _combatMonster;

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

public class HeavyStartActionCommand : BaseActionCommand, ICombatActionCommand
{
    private AttackButtons _attackButton;

    private CombatMonster _combatMonster;

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

public class HeavyReleaseActionCommand : BaseActionCommand, ICombatActionCommand
{
    private AttackButtons _attackButton;

    private CombatMonster _combatMonster;

    public HeavyReleaseActionCommand(AttackButtons attackButton, CombatMonster combatMonster)
    {
        actionPriority = 3;
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

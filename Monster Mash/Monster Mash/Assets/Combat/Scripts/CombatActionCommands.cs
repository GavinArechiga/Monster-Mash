
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

    private CombatMonster _combatMonster;

    public CombatButtonActionCommand(AttackButtons attackButton, CombatMonster combatMonster)
    {
        _attackButton = attackButton;
        _combatMonster = combatMonster;
    }

    public void ExecuteAction()
    {
        //Take in Player Button and Execute Corresponding Attack
        _combatMonster.MonsterPartAttack(_attackButton);
    }
}

public class HeavyStartActionCommand : ICombatActionCommand
{
    private AttackButtons _attackButton;

    private CombatMonster _combatMonster;

    public HeavyStartActionCommand(AttackButtons attackButton, CombatMonster combatMonster)
    {
        _attackButton = attackButton;
        _combatMonster = combatMonster;
    }
    public void ExecuteAction()
    {
        _combatMonster.HeavyAttackCharge(_attackButton);
    }
}

public class HeavyReleaseActionCommand : ICombatActionCommand
{
    private AttackButtons _attackButton;

    private CombatMonster _combatMonster;

    public HeavyReleaseActionCommand(AttackButtons attackButton, CombatMonster combatMonster)
    {
        _attackButton = attackButton;
        _combatMonster = combatMonster;
    }
    public void ExecuteAction()
    {
        _combatMonster.HeavyAttackRelease(_attackButton);
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

public class JabNeutral : NeutralAttack
{

    public JabNeutral() 
    { 
        DamageRange = DamageRange.Range2; 
    }

    public override void PassDamage()
    {
        monsterPartRef.neutralColliderReference.resetAttackHistory();
        monsterPartRef.neutralColliderReference.damage = Damage;

        if (monsterPartRef.heavyColliderReference != null) 
        {
            monsterPartRef.heavyColliderReference.markedHeavy = false;
        }
    }

    public override void statusEffectAndDamageCalculations()
    {
        base.statusEffectAndDamageCalculations();
        monsterPartRef.neutralColliderReference.damage = Damage;
    }

    public override void triggerAttackRelease()
    {
        base.triggerAttackRelease();
        monsterPartRef.triggerJabOrSlashCollisionsOn();
    }

    public override void triggerNeutralAttackVisuals()
    {
        if (!monsterPartRef.jabOrSlashLanded && monsterPartVisualRef.neutralMissVFXHolder != null)
        {
            monsterPartVisualRef.neutralMissVFXManager.unleashJabOrSlash();
        }
    }
}

using System;

[Serializable]
public class JabNeutral : NeutralAttack
{

    public JabNeutral()
    {
        Attack = AttackType.Jab;
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

    public override void triggerAttackRelease(NewMonsterPart monsterPartRef)
    {
        base.triggerAttackRelease(monsterPartRef);
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

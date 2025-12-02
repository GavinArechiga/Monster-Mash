using System;

[Serializable]
public class SlashNeutral : NeutralAttack
{
    public SlashNeutral()
    {
        Attack = AttackType.Slash;
        DamageRange = DamageRange.Range3;
    }
    public override void PassDamage()
    {
        monsterPartRef.neutralColliderReference.resetAttackHistory();
        monsterPartRef.neutralColliderReference.damage = Damage;
        monsterPartRef.heavyColliderReference.markedHeavy = false;
    }

    public override void statusEffectAndDamageCalculations()
    {
        if (monsterPartRef.neutralColliderReference == null) { return; }
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

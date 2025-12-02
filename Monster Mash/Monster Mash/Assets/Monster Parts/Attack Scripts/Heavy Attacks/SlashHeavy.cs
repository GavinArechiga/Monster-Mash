using System;

[Serializable]
public class SlashHeavy : HeavyAttack
{
    public SlashHeavy()
    {
        Attack = HeavyAttackType.Slash;
        DamageRange = DamageRange.Range4;
    }
    public override void triggerHeavyAttackVisuals()
    {
        if (monsterPartRef.jabOrSlashLanded == false && monsterPartVisualRef.heavyMissVFXHolder != null)
        {
            //turn on miss visual if neutral vfx holder's script hasn't made contact
            monsterPartVisualRef.heavyMissVFXManager.unleashJabOrSlash();
        }
    }
    public override void PassDamage()
    {
        monsterPartRef.heavyColliderReference.resetAttackHistory();
        monsterPartRef.heavyColliderReference.damage = Damage;
        monsterPartRef.heavyColliderReference.markedHeavy = true;
    }
    public override void triggerAttackRelease(NewMonsterPart monsterPartRef)
    {
        base.triggerAttackRelease(monsterPartRef);
        monsterPartRef.triggerJabOrSlashCollisionsOn();
    }

    public override void statusEffectAndDamageCalculations()
    {
        ApplyStatusEffectsToColliderReference(monsterPartRef.heavyColliderReference);
    }
}

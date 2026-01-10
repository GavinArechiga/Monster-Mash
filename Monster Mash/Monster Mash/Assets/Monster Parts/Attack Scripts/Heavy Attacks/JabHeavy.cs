public class JabHeavy : HeavyAttack
{
    public JabHeavy()
    {
        DamageRange = DamageRange.Range3;
    }

    public override void triggerHeavyAttackVisuals()
    {
        if (monsterPartRef.jabOrSlashLanded == false && monsterPartVisualRef.heavyMissVFXHolder != null)
        {
            //turn on miss visual if neutral vfx holder's script hasn't made contact
            monsterPartVisualRef.heavyMissVFXManager.unleashJabOrSlash();
        }
    }

    public override void triggerAttackRelease()
    {
        base.triggerAttackRelease();
        monsterPartRef.triggerJabOrSlashCollisionsOn();
    }

    public override void PassDamage()
    {
        monsterPartRef.heavyColliderReference.resetAttackHistory();
        monsterPartRef.heavyColliderReference.damage = Damage;
        monsterPartRef.heavyColliderReference.markedHeavy = true;
    }

    public override void statusEffectAndDamageCalculations()
    {
        ApplyStatusEffectsToColliderReference(monsterPartRef.heavyColliderReference);
    }
}

public class SpinningHeavy : HeavyAttack
{
    public SpinningHeavy()
    {
        DamageRange = DamageRange.Range4;
    }

    public override void triggerAttackRelease()
    {
        base.triggerAttackRelease();
        monsterPartRef.triggerJabOrSlashCollisionsOn();
        monsterPartRef.myMainSystem.StartCoroutine(monsterPartRef.myMainSystem.SpinTimer());
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
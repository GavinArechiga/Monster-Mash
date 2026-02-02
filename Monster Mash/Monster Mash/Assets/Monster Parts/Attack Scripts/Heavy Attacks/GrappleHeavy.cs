public class GrappleHeavy : HeavyAttack
{
    public GrappleHeavy()
    {
        DamageRange = DamageRange.Range2;
    }

    public override void PassDamage()
    {
        monsterPartRef.heavyColliderReference.resetAttackHistory();
        monsterPartRef.heavyColliderReference.damage = Damage;
        monsterPartRef.heavyColliderReference.markedHeavy = true;
    }
}

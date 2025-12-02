public class ProjectileNeutral : NeutralAttack
{
    public ProjectileNeutral()
    {
        DamageRange = DamageRange.Range2;
    }

    public override void PassDamage()
    {
        if (monsterPartVisualRef.neutralHitVFXManager == null) { return; }
        monsterPartVisualRef.neutralHitVFXManager.damage = Damage;
        //monsterPartVisualRef.neutralHitVFXManager.updateDamageOnProjectiles();
    }

    public override void statusEffectAndDamageCalculations()
    {
        if (monsterPartVisualRef.neutralHitVFXManager == null) { return; }
        monsterPartVisualRef.neutralHitVFXManager.damage = Damage;
        //monsterPartVisualRef.neutralHitVFXManager.updateDamageOnSpray();
    }

    /*public override void triggerNeutralAttackVisuals()
    {
        if (monsterPartVisualRef.neutralAttackHitVFXArray.Length != 0)
        {
            monsterPartVisualRef.neutralHitVFXManager.faceRightDirection(monsterPartRef.facingRight);
            monsterPartVisualRef.neutralHitVFXManager.unleashSingleProjectile();

            if (monsterPartVisualRef.neutralDefaultSprayVFXManager != null)
            {
                monsterPartVisualRef.neutralDefaultSprayVFXManager.unleashAdditionalSprayVisual();
            }
        }
    }*/

    public override void SetupVFX()
    {
        if (monsterPartVisualRef.neutralHitVFXHolder != null || monsterPartVisualRef.neutralDefaultSprayVFXHolder != null)
        {
            StoredParentSetup();
        }
    }
}

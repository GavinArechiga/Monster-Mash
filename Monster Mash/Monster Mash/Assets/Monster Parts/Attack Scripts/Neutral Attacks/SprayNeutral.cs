using System;
public class SprayNeutral : NeutralAttack
{
    public SprayNeutral()
    {
        DamageRange = DamageRange.Range3;
    }

    public override void PassDamage()
    {
        monsterPartVisualRef.neutralHitVFXManager.damage = Damage;
        monsterPartVisualRef.neutralHitVFXManager.updateDamageOnProjectiles();
    }

    public override void statusEffectAndDamageCalculations()
    {
        base.statusEffectAndDamageCalculations();
        monsterPartVisualRef.neutralHitVFXManager.damage = Damage;
        monsterPartVisualRef.neutralHitVFXManager.updateDamageOnSpray();
    }

    public override void triggerNeutralAttackVisuals()
    {
        monsterPartVisualRef.neutralHitVFXHolder.transform.SetPositionAndRotation(monsterPartVisualRef.neutralMuzzle.transform.position, 
            monsterPartVisualRef.neutralMuzzle.transform.rotation);

        if (monsterPartVisualRef.neutralDefaultSprayVFXHolder != null)
        {
            monsterPartVisualRef.neutralDefaultSprayVFXHolder.transform.SetPositionAndRotation(monsterPartVisualRef.neutralMuzzle.transform.position, 
                monsterPartVisualRef.neutralMuzzle.transform.rotation);
        }

        monsterPartVisualRef.neutralHitVFXManager.unleashSpray();

        if (monsterPartVisualRef.neutralDefaultSprayVFXManager != null)
        {
            monsterPartVisualRef.neutralDefaultSprayVFXManager.unleashAdditionalSprayVisual();
        }
    }

    public override void SetupVFX()
    {
        if (monsterPartVisualRef.neutralHitVFXHolder != null || monsterPartVisualRef.neutralDefaultSprayVFXHolder != null)
        {
            StoredParentSetup();
        }
    }
}

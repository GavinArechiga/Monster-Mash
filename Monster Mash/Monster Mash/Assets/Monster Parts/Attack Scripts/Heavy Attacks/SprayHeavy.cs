using System;

[Serializable]
public class SprayHeavy : HeavyAttack
{
    public SprayHeavy()
    {
        Attack = HeavyAttackType.Spray;
        DamageRange = DamageRange.Range4;
    }
    public override void SetupVFX()
    {
        if (monsterPartVisualRef.heavyHitVFXHolder != null || monsterPartVisualRef.heavyDefaultSprayVFXHolder != null)
        {
            StoredParentSetup();
        }
    }

    public override void triggerHeavyAttackVisuals()
    {
        monsterPartVisualRef.heavyHitVFXHolder.transform.SetPositionAndRotation(monsterPartVisualRef.heavyMuzzle.transform.position, monsterPartVisualRef.heavyMuzzle.transform.rotation);

        if (monsterPartVisualRef.heavyDefaultSprayVFXHolder != null)
        {
            monsterPartVisualRef.heavyDefaultSprayVFXHolder.transform.SetPositionAndRotation(monsterPartVisualRef.heavyMuzzle.transform.position, monsterPartVisualRef.heavyMuzzle.transform.rotation);
        }

        monsterPartVisualRef.heavyHitVFXManager.unleashSpray();
        if (monsterPartVisualRef.heavyDefaultSprayVFXManager)
        {
            monsterPartVisualRef.heavyDefaultSprayVFXManager.unleashAdditionalSprayVisual();
        }
    }

    public override void PassDamage()
    {
        monsterPartVisualRef.heavyHitVFXManager.damage = Damage;
        monsterPartVisualRef.heavyHitVFXManager.updateDamageOnProjectiles();
    }

    public override void statusEffectAndDamageCalculations()
    {
        monsterPartVisualRef.heavyHitVFXManager.damage = Damage;
        monsterPartVisualRef.heavyHitVFXManager.updateDamageOnSpray();
        ApplyStatusEffectsToVFXHolder(monsterPartVisualRef.heavyHitVFXManager);
        monsterPartVisualRef.heavyHitVFXManager.updateStatusEffectsOnSpray();
    }
}

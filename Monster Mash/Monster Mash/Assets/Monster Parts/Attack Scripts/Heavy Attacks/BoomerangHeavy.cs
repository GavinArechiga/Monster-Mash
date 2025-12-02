using System;

[Serializable]
public class BoomerangHeavy : HeavyAttack
{
    public BoomerangHeavy()
    {
        Attack = HeavyAttackType.Boomerang;
        DamageRange = DamageRange.Range3;
    }
    public override void SetupVFX()
    {
        if (monsterPartVisualRef.heavyHitVFXHolder != null || monsterPartVisualRef.heavyDefaultSprayVFXHolder != null)
        {
            StoredParentSetup();
        }

        if (monsterPartVisualRef.heavyHitVFXHolder != null)
        {
            monsterPartVisualRef.heavyHitVFXManager.isBoomerangHolder = true;
        }
    }

    public override void triggerHeavyAttackVisuals()
    {
        if (monsterPartVisualRef.heavyAttackHitVFXArray.Length != 0)
        {
            monsterPartVisualRef.heavyHitVFXHolder.transform.SetPositionAndRotation(monsterPartVisualRef.heavyMuzzle.transform.position, monsterPartVisualRef.heavyMuzzle.transform.rotation);

            monsterPartVisualRef.heavyHitVFXManager.faceRightDirection(monsterPartRef.facingRight);
            monsterPartVisualRef.heavyHitVFXManager.unleashSingleProjectile();

            if (monsterPartVisualRef.heavyDefaultSprayVFXManager)
            {
                monsterPartVisualRef.heavyDefaultSprayVFXManager.unleashAdditionalSprayVisual();
            }
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
        monsterPartVisualRef.heavyHitVFXManager.updateDamageOnProjectiles();
        ApplyStatusEffectsToVFXHolder(monsterPartVisualRef.heavyHitVFXManager);
        monsterPartVisualRef.heavyHitVFXManager.updateStatusEffectsOnProjectiles();
    }
}

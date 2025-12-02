using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NeutralAttack: BaseAttack
{
    public enum AttackType 
    {
        None,
        Jab,
        Slash,
        Spray,
        Projectile,
        HomingMissile,
        Anvil,
        BowlingBall,
        Gaseous,
        Reflecting,
        Suction,
        AOE,
        Boomerang
    }
    // a property drawer relies on this var using string lookup. Please do not change the var name or the property drawer will stop working
    public AttackType Attack;

    // factory pattern to assign the subclass
    public NeutralAttack GetAttack()
    {
        return Attack switch
        {
            AttackType.Jab => new JabNeutral(),
            AttackType.Slash => new SlashNeutral(),
            AttackType.Spray => new SprayNeutral(),
            AttackType.Projectile => new ProjectileNeutral(),
            AttackType.Boomerang => new BoomerangNeutral(),
            _ => new NeutralAttack(),
        };
    }

    public virtual void triggerNeutralAttackVisuals()
    {
        
    }

    public virtual void SetupVFX()
    {

    }

    protected void StoredParentSetup()
    {
        monsterPartVisualRef.neutralVFXStoredParent = monsterPartVisualRef.neutralHitVFXHolder.transform.parent;
        monsterPartVisualRef.neutralVFXStoredPosition = monsterPartRef.transform.localPosition;
        monsterPartVisualRef.neutralVFXStoredRotation = monsterPartRef.transform.localRotation;
    }
}

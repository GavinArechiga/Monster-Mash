using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NeutralAttack: BaseAttack
{
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

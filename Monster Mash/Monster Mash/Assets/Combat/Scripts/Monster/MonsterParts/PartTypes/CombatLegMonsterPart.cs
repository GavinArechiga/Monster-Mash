using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CombatLegMonsterPart : CombatMonsterPart
{
    [Header("Monster Part Leg Parameters")]
    [SerializeField]
    float legWalkOffset;
    [SerializeField]
    float legRunOffset;

    public Action<float, float> setTrailingOffset;
    public void InitializeLegOffset()
    {
        GameObject parentObj = ReturnParentObject().gameObject;

        if(parentObj.CompareTag("Trailing Connection"))
        {
            //Set as Trailing With OffSet

            setTrailingOffset?.Invoke(legWalkOffset, legRunOffset);
        }
    }
}

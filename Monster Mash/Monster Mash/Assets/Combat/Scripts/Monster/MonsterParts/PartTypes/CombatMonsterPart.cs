using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CombatMonsterPart : BaseMonsterPart
{
    [Header("Combat Part Attributes")]
    [SerializeField] MonsterPartAttack neutralPartAttack;

    [SerializeField] MonsterPartAttack heavyPartAttack;

    IMonsterAttack neutralAttack;

    IMonsterAttack heavyAttack;

    public float maxChargeTime;

    bool isDisabled = false;

    int partUsage = 0;

    #region Events

    public Action partNeutralAttack;
    public Action startPartHeavyCharge;
    public Action releasePartHeavyAttack;

    #endregion
    public override void InitializeMonsterPart(AttackButtons partButton, CombatMonster combatMonster)
    {
        SetBaseMonsterPart(partButton, combatMonster);

        neutralAttack = neutralPartAttack.GetComponent<IMonsterAttack>();
        heavyAttack = heavyPartAttack.GetComponent<IMonsterAttack>();

        //Assign Max HP Here
    }
    public bool CheckedDisabled()
    {
        return isDisabled;
    }

    public void NeutralAttack()
    {
        //Execute Neutral Attack
        InscreasePartUsage();
        partNeutralAttack?.Invoke();
        neutralAttack.ExecuteNeutralAttack();
    }

    public void HeavyAttackRelease(float multiplier)
    {
        //Release Full Heavy Attack
        InscreasePartUsage();
        releasePartHeavyAttack?.Invoke();
        heavyAttack.ExecuteHeavyAttack(multiplier);
    }

    public void HeavyAttackStart()
    {
        //Activate Heavy Attack Charge State
        startPartHeavyCharge?.Invoke();
    }

    #region Input Buffer Info

    public AnimatorClipInfo[] ReturnCurrentAnimationClip()
    {
        return partAnim.GetCurrentAnimatorClipInfo(0);
    }

    public AnimatorStateInfo ReturnAnimatorStateInfo()
    {
        return partAnim.GetCurrentAnimatorStateInfo(0);
    }

    public PartAttackBehaviour[] ReturnAttackBehaviours()
    {
        return partAnim.GetBehaviours<PartAttackBehaviour>();
    }

    #endregion

    #region Part Usage
    void InscreasePartUsage()
    {
        partUsage++;
    }

    public int ReturnPartUsage()
    {
        return partUsage;
    }

    public void ResetPartUsage()
    {
        partUsage = 0;
    }

    public void DisablePart()
    {
        isDisabled = true;

        ResetPartUsage();
    }

    void ReEnablePart()
    {
        isDisabled = false;
    }

    #endregion
}

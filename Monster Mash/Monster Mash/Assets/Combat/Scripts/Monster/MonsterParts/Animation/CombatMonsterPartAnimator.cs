using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatMonsterPartAnimator : MonsterPartAnimator
{
    CombatMonsterPart _combatMonsterPart;
    public override void InitializeAnimator(BaseMonsterPart monsterPart, Animator partAnim)
    {
        SetAnimatorController(partAnim);

        SetBaseMonsterPart(monsterPart);

        SubscribeToBaseActions();

        _combatMonsterPart = monsterPart as CombatMonsterPart;

        SubscribeToCombatActions();
    }

    #region EventSubscriptions

    void SubscribeToCombatActions()
    {
        _combatMonsterPart.partNeutralAttack += PartNeutralAttack;
        _combatMonsterPart.startPartHeavyCharge += StartPartHeavyCharge;
        _combatMonsterPart.releasePartHeavyAttack += ReleasePartHeavyAttack;

        if(_combatMonsterPart is CombatLegMonsterPart)
        {
            var leg = _combatMonsterPart as CombatLegMonsterPart;

            leg.setTrailingOffset += SetTrailingOffset;
        }
    }

    void UnsubscribeToCombatActions()
    {
        _combatMonsterPart.partNeutralAttack -= PartNeutralAttack;
        _combatMonsterPart.startPartHeavyCharge -= StartPartHeavyCharge;
        _combatMonsterPart.releasePartHeavyAttack -= ReleasePartHeavyAttack;


        if (_combatMonsterPart is CombatLegMonsterPart)
        {
            var leg = _combatMonsterPart as CombatLegMonsterPart;

            leg.setTrailingOffset -= SetTrailingOffset;
        }
    }

    private void OnDisable()
    {
        UnsubscribeToBaseActions();
        UnsubscribeToCombatActions();
    }

    #endregion
    void PartNeutralAttack()
    {
        SetTrigger(MonsterPartAnimTrigger.n_Attk.ToString());
    }

    void StartPartHeavyCharge()
    {
        SetTrigger(MonsterPartAnimTrigger.h_Attk.ToString());
    }

    void ReleasePartHeavyAttack()
    {
        SetTrigger(MonsterPartAnimTrigger.release.ToString());
    }

    void SetTrailingOffset(float walk, float run)
    {
        SetFloat(MonsterPartAnimFloat.legWalkOffset.ToString(), walk);
        SetFloat(MonsterPartAnimFloat.legRunOffset.ToString(), run);

        SetFloat(MonsterPartAnimFloat.legBlendDir.ToString(), 1);
    }
}

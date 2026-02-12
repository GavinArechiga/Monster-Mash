using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatMonsterPart : BaseMonsterPart
{

    [SerializeField] MonsterPartAttack neutralPartAttack;

    [SerializeField] MonsterPartAttack heavyPartAttack;

    IMonsterAttack neutralAttack;

    IMonsterAttack heavyAttack;

    public float maxChargeTime;

    float maxHP;

    float currentHP;

    bool isDisabled = false;
    public override void InitializeMonsterPart()
    {
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
        neutralAttack.ExecuteNeutralAttack(partAnim);
    }

    public void HeavyAttackRelease(float multiplier)
    {
        //Release Full Heavy Attack
        heavyAttack.ExecuteHeavyAttack(multiplier, partAnim);
    }

    public void HeavyAttackStart()
    {
        //Activate Heavy Attack Charge State
        partAnim.SetTrigger(MonsterPartAnimTrigger.h_Attk.ToString());
    }

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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatMonsterPart : BaseMonsterPart
{
    PlayerCombatManager _playerCombatManager;

    [SerializeField] MonsterPartAttack neutralPartAttack;

    [SerializeField] MonsterPartAttack heavyPartAttack;

    IMonsterAttack neutralAttack;

    IMonsterAttack heavyAttack;

    public float maxChargeTime;

    //Temp Should Probably Move Part Animation Control To Its Own Script
    //Though it may be best practice to use this Combat Monster Part Script as the Initializer for the animator
    [SerializeField] Animator partAnim;

    float maxHP;

    float currentHP;

    bool isDisabled = false;
    public void InitializeMonsterPart(PlayerCombatManager playerCombatManager)
    {
        _playerCombatManager = playerCombatManager;
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashAttack : MonsterPartAttack, IMonsterAttack
{
    public void ExecuteNeutralAttack(Animator partAnim)
    {
        partAnim.SetTrigger(MonsterPartAnimTrigger.n_Attk.ToString());
        print("NEUTRAL SLASH SLASH! " + (damage));
    }
    public void ExecuteHeavyAttack(float multiplier, Animator partAnim)
    {
        partAnim.SetTrigger(MonsterPartAnimTrigger.release.ToString());
        print("HEAVY SLASH SLASH! " + (damage * multiplier));
    }
}

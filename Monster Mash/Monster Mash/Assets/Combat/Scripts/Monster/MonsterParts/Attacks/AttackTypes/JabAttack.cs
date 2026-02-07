using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JabAttack : MonsterPartAttack, IMonsterAttack
{
    public void ExecuteNeutralAttack(Animator partAnim)
    {
        partAnim.SetTrigger(MonsterPartAnimTrigger.n_Attk.ToString());
        print("NEUTRAL JAB JAB JAB! " + (damage));
    }
    public void ExecuteHeavyAttack(float multiplier, Animator partAnim)
    {
        partAnim.SetTrigger(MonsterPartAnimTrigger.release.ToString());
        print("HEAVY JAB JAB JAB! " + (damage * multiplier));
    }
}

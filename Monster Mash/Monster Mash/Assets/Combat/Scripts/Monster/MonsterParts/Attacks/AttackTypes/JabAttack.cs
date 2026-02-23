using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JabAttack : MonsterPartAttack, IMonsterAttack
{
    public void ExecuteNeutralAttack()
    {
        print("NEUTRAL JAB JAB JAB! " + (damage));
    }
    public void ExecuteHeavyAttack(float multiplier)
    {
        print("HEAVY JAB JAB JAB! " + (damage * multiplier));
    }
}

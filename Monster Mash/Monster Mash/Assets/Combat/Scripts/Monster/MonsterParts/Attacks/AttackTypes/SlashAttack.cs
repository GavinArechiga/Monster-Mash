using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashAttack : MonsterPartAttack, IMonsterAttack
{
    public void ExecuteNeutralAttack()
    {
        print("NEUTRAL SLASH SLASH! " + (damage));
    }
    public void ExecuteHeavyAttack(float multiplier)
    {
        print("HEAVY SLASH SLASH! " + (damage * multiplier));
    }
}

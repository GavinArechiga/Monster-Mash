using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashAttack : MonsterPartAttack, IMonsterAttack
{
    public void ExecuteAttack()
    {
        print("SLASH SLASH! " + damage + " " + isHeavy);
    }
}

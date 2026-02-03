using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JabAttack : MonsterPartAttack, IMonsterAttack
{
    public void ExecuteAttack()
    {
        print("JAB JAB JAB! " + damage + " " + isHeavy);
    }
}

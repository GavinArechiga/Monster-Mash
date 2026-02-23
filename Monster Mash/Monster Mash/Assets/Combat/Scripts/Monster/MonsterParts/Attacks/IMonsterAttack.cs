using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMonsterAttack
{

    void ExecuteNeutralAttack();
    void ExecuteHeavyAttack(float multiplier);
}

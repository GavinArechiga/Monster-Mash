using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMonsterAttack
{

    void ExecuteNeutralAttack(Animator partAnim);
    void ExecuteHeavyAttack(float multiplier, Animator partAnim);
}

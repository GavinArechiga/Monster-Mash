using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatMonsterPart : BaseMonsterPart
{

    [SerializeField] MonsterPartAttack neutralPartAttack;

    [SerializeField] MonsterPartAttack heavyPartAttack;

    IMonsterAttack neutralAttack;

    IMonsterAttack heavyAttack;

    private void Awake()
    {
        InitializeMonsterPart();
    }

    void InitializeMonsterPart()
    {
        neutralAttack = neutralPartAttack.GetComponent<IMonsterAttack>();
        heavyAttack = heavyPartAttack.GetComponent<IMonsterAttack>();
    }

    public void NeutralAttack()
    {
        neutralAttack.ExecuteAttack();
    }

    public void HeavyAttack()
    {
        heavyAttack.ExecuteAttack();
    }
}

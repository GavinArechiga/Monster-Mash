using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHurtBox : MonoBehaviour
{
    bool isInvincible = false;

    CombatMonster _combatMonster;
    public void InitializeHurtBox(CombatMonster combatMonster)
    {
        _combatMonster = combatMonster;
    }
    
    public void TakeDamage(int damage)
    {
        if(isInvincible)
        {
            return;
        }

        _combatMonster.MonsterHit(damage);
    }

    public void SetInvincible(bool status)
    {
        isInvincible = status;
    }
}

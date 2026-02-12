using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterDataStorage
{
    public AttackButtons _assignedButton;

    public BaseMonsterPart _monsterPart;

    public MonsterDataStorage(AttackButtons assignedButton, BaseMonsterPart monsterPart)
    {
        _assignedButton = assignedButton;

        _monsterPart = monsterPart;
    }
}

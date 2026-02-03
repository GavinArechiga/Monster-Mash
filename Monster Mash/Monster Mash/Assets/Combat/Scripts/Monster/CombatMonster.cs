using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatMonster : MonoBehaviour
{
    [SerializeField] List<AttackButtons> pressedButton;
    [SerializeField] List<CombatMonsterPart> monsterPart;

    Dictionary<AttackButtons, CombatMonsterPart> monsterData;

    private void Awake()
    {
        InitializeMonsterParts();
    }

    void InitializeMonsterParts()
    {
        monsterData = new Dictionary<AttackButtons, CombatMonsterPart>();

        for (int i = 0; i < pressedButton.Count; i++)
        {
            monsterData.Add(pressedButton[i], monsterPart[i]);
        }
    }

    public void MonsterPartAttack(AttackButtons attack)
    {

        if (monsterData.ContainsKey(attack))
        {
            monsterData[attack].NeutralAttack();
        }

        else
        {
            print("Part Not Found");
        }

    }

    public void HeavyAttackCharge(AttackButtons attack)
    {
        if (monsterData.ContainsKey(attack))
        {
            print("Charging!!!");
        }

        else
        {
            print("Part Not Found");
        }
    }

    public void HeavyAttackRelease(AttackButtons attack)
    {
        if (monsterData.ContainsKey(attack))
        {
            monsterData[attack].HeavyAttack();
        }

        else
        {
            print("Part Not Found");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatMonster : MonoBehaviour
{
    PlayerCombatManager _playerCombatManager;
    [SerializeField] List<AttackButtons> pressedButton;

    //Keeping a Separated Collection for the Parts Used in Combat means we can easily access them for taking damage and being knocked off
    //with out having to deal with parsing through the other parts
    List<CombatMonsterPart> attackingMonsterParts;

    //this will be temproarlily serialized and will be filled out by a different script when combat is Initialized
    [SerializeField] List<MonsterDataStorage> monsterData;

    Dictionary<AttackButtons, CombatMonsterPart> monsterAttackData;

    AttackButtons _currentAttack;
    float _maxChargeTime;
    float heavyMultiplier = 1;
    float timer = 0;
    bool isCharging;

    public void InitializeMonster(PlayerCombatManager playerCombatManager)
    {
        _playerCombatManager = playerCombatManager;
        InitializeMonsterParts();

        //Connect Monster Parts

        _playerCombatManager.SetPlayerState(PlayerState.Idle);
    }

    void InitializeMonsterParts()
    {
        monsterAttackData = new Dictionary<AttackButtons, CombatMonsterPart>();

        attackingMonsterParts = new List<CombatMonsterPart>();

        foreach(MonsterDataStorage partData in monsterData)
        {
            if(partData._assignedButton != AttackButtons.None)
            {
                monsterAttackData.Add(partData._assignedButton, partData._monsterPart as CombatMonsterPart);

                attackingMonsterParts.Add(partData._monsterPart as CombatMonsterPart);
            }

            partData._monsterPart.InitializeMonsterPart();
        }

        /*

        for (int i = 0; i < pressedButton.Count; i++)
        {
            monsterAttackData.Add(pressedButton[i], monsterParts[i]);

            monsterParts[i].InitializeMonsterPart();
        }

        */
    }

    public void MonsterPartAttack(AttackButtons attack)
    {

        if (monsterAttackData.ContainsKey(attack))
        {
            if(!monsterAttackData[attack].CheckedDisabled())
            {
                monsterAttackData[attack].NeutralAttack();

                _currentAttack = attack;

                _playerCombatManager.SetPlayerState(PlayerState.Attacking);
            }
        }

        else
        {
            print("Part Not Found");
        }

    }

    public void HeavyAttackCharge(AttackButtons attack)
    {
        if (monsterAttackData.ContainsKey(attack))
        {
            if(!monsterAttackData[attack].CheckedDisabled())
            {
                print("Charging!!!");

                StartChargeTimer(attack);

                _playerCombatManager.SetPlayerState(PlayerState.Charging);
            }
        }

        else
        {
            print("Part Not Found");
        }
    }

    public void HeavyAttackRelease(AttackButtons attack)
    {
        if (monsterAttackData.ContainsKey(attack))
        {
            if (!monsterAttackData[attack].CheckedDisabled())
            {
                if (isCharging)
                {
                    StopChargeTimer();

                    monsterAttackData[attack].HeavyAttackRelease(heavyMultiplier);

                    _playerCombatManager.SetPlayerState(PlayerState.Attacking);
                }
            }
        }

        else
        {
            print("Part Not Found");
        }
    }

    public bool CheckPartEnabledStatus(AttackButtons attack)
    {
        if(monsterAttackData.ContainsKey(attack))
        {
            if(!monsterAttackData[attack].CheckedDisabled())
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        else
        {
            return false;
        }
    }

    public AnimatorClipInfo[] ReturnAttackAnimationClip()
    {
        if(monsterAttackData.ContainsKey(_currentAttack))
        {
            return monsterAttackData[_currentAttack].ReturnCurrentAnimationClip();
        }

        else
        {
            return new AnimatorClipInfo[1];
        }
    }

    public AnimatorStateInfo ReturnAnimatorStateInfo()
    {
        if(monsterAttackData.ContainsKey(_currentAttack))
        {
            return monsterAttackData[_currentAttack].ReturnAnimatorStateInfo();
        }

        else
        {
            return new AnimatorStateInfo();
        }
    }

    public  List<MonsterPartAttackBehaviours> ReturnAttackBehaviours()
    {
        List < MonsterPartAttackBehaviours > allPartAttacks = new List<MonsterPartAttackBehaviours>();

        foreach(CombatMonsterPart part in attackingMonsterParts)
        {
            allPartAttacks.Add(new MonsterPartAttackBehaviours(part.ReturnAttackBehaviours()));
        }

        return allPartAttacks;
    }

    #region Heavy Charge Logic

    void StartChargeTimer(AttackButtons attack)
    {
        isCharging = true;

        _currentAttack = attack;

        _maxChargeTime = monsterAttackData[attack].maxChargeTime;

        heavyMultiplier = 1;

        monsterAttackData[attack].HeavyAttackStart();

        StartCoroutine(ChargeTimer());
    }

    IEnumerator ChargeTimer()
    {
        timer = 0;

        while(timer <= _maxChargeTime && isCharging)
        {
            timer += Time.deltaTime;

            yield return null;
        }

        if(isCharging)
        {
            HeavyAttackRelease(_currentAttack);
        }
    }

    void StopChargeTimer()
    {
        CalculateHeavyMultiplier();

        isCharging = false;

        _maxChargeTime = 0;
    }

    void CalculateHeavyMultiplier()
    {
        timer = Mathf.Clamp(timer, 0, _maxChargeTime);

        heavyMultiplier = (1 + (timer / _maxChargeTime));

        heavyMultiplier = (Mathf.Floor(heavyMultiplier*10)/10);

        heavyMultiplier = Mathf.Clamp(heavyMultiplier, 0, 2);
    }

    #endregion
}

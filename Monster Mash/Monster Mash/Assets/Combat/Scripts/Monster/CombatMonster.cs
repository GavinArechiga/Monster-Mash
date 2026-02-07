using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatMonster : MonoBehaviour
{
    PlayerCombatManager _playerCombatManager;
    [SerializeField] List<AttackButtons> pressedButton;
    [SerializeField] List<CombatMonsterPart> monsterPart;

    Dictionary<AttackButtons, CombatMonsterPart> monsterData;

    AttackButtons _currentAttack = AttackButtons.None;
    float _maxChargeTime;
    float heavyMultiplier = 1;
    float timer = 0;
    bool isCharging;

    public void InitializeMonster(PlayerCombatManager playerCombatManager)
    {
        _playerCombatManager = playerCombatManager;
        InitializeMonsterParts();

        _playerCombatManager.SetPlayerState(PlayerState.Idle);
    }

    void InitializeMonsterParts()
    {
        monsterData = new Dictionary<AttackButtons, CombatMonsterPart>();

        for (int i = 0; i < pressedButton.Count; i++)
        {
            monsterData.Add(pressedButton[i], monsterPart[i]);

            monsterPart[i].InitializeMonsterPart(_playerCombatManager);
        }
    }

    public void MonsterPartAttack(AttackButtons attack)
    {

        if (monsterData.ContainsKey(attack))
        {
            if(!monsterData[attack].CheckedDisabled())
            {
                monsterData[attack].NeutralAttack();

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
        if (monsterData.ContainsKey(attack))
        {
            if(!monsterData[attack].CheckedDisabled())
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
        if (monsterData.ContainsKey(attack))
        {
            if (!monsterData[attack].CheckedDisabled())
            {
                if (isCharging)
                {
                    StopChargeTimer();

                    monsterData[attack].HeavyAttackRelease(heavyMultiplier);

                    _playerCombatManager.SetPlayerState(PlayerState.Attacking);
                }
            }
        }

        else
        {
            print("Part Not Found");
        }
    }

    public AnimatorClipInfo[] ReturnAttackAnimationClip()
    {
        if(monsterData.ContainsKey(_currentAttack))
        {
            return monsterData[_currentAttack].ReturnCurrentAnimationClip();
        }

        else
        {
            return new AnimatorClipInfo[1];
        }
    }

    public AnimatorStateInfo ReturnAnimatorStateInfo()
    {
        if(monsterData.ContainsKey(_currentAttack))
        {
            return monsterData[_currentAttack].ReturnAnimatorStateInfo();
        }

        else
        {
            return new AnimatorStateInfo();
        }
    }

    #region Heavy Charge Logic

    void StartChargeTimer(AttackButtons attack)
    {
        isCharging = true;

        _currentAttack = attack;

        _maxChargeTime = monsterData[attack].maxChargeTime;

        heavyMultiplier = 1;

        monsterData[attack].HeavyAttackStart();

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

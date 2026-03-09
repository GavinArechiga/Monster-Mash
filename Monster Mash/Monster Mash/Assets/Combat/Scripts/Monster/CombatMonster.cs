using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//Temp
using UnityEditor;

public class CombatMonster : MonoBehaviour
{
    PlayerCombatManager _playerCombatManager;
    [SerializeField] List<AttackButtons> pressedButton;

    //Keeping a Separated Collection for the Parts Used in Combat means we can easily access them for taking damage and being knocked off
    //with out having to deal with parsing through the other parts
    [SerializeField]
    List<CombatMonsterPart> attackingMonsterParts;

    //this will be temproarlily serialized and will be filled out by a different script when combat is Initialized
    [SerializeField] List<MonsterDataStorage> monsterData;

    Dictionary<AttackButtons, CombatMonsterPart> monsterAttackData;

    AttackButtons _currentAttack;
    float _maxChargeTime;
    float heavyMultiplier = 1;
    float timer = 0;
    bool isCharging;

    float maxHP;

    float currentHP;

    float partCutoffHP;

    #region Events

    //Locomotion Events
    public Action<bool> setWalk;
    public Action<bool> setRun;
    public Action jump;
    public Action land;


    //Combat Events
    public Action<AttackButtons, Transform> startBrace;
    public Action<AttackButtons> endBrace;
    public Action releaseTorsoBrace;
    public Action onHit;

    #endregion

    #region Initialization

    public void InitializeMonster(PlayerCombatManager playerCombatManager)
    {
        _playerCombatManager = playerCombatManager;

        //Sort Part list Makes sure that the Torso is always first in the list of monster parts to properly initialize the part parents
        //It makes sure heads are second for the same reason

        SortPartList();

        InitializeMonsterParts();

        SubscribeToEvents();

        InitializeHealth(100);

        _playerCombatManager.SetPlayerState(PlayerState.Idle);
    }

    void InitializeHealth(float monsterMaxHP)
    {
        maxHP = monsterMaxHP;

        currentHP = maxHP;

        partCutoffHP = (maxHP / attackingMonsterParts.Count);
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

            partData._monsterPart.InitializeMonsterPart(partData._assignedButton, this);

            partData._monsterPart.InitializePartGameObject();
        }
    }

    void SortPartList()
    {
        monsterData.Sort(MonsterPartPriority.SortPartPriority);
    }

    #endregion

    #region Attack Logic

    public void MonsterPartAttack(AttackButtons attack)
    {

        if (monsterAttackData.ContainsKey(attack))
        {
            if(!monsterAttackData[attack].CheckedDisabled())
            {
                startBrace?.Invoke(attack, monsterAttackData[attack].ReturnParentObject());

                releaseTorsoBrace?.Invoke();

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

                    releaseTorsoBrace?.Invoke();

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

    void AttackEnd()
    {
        endBrace?.Invoke(_currentAttack);

        _currentAttack = AttackButtons.None;
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

    #endregion

    #region Heavy Charge Logic

    void StartChargeTimer(AttackButtons attack)
    {
        isCharging = true;

        _currentAttack = attack;

        _maxChargeTime = monsterAttackData[attack].maxChargeTime;

        heavyMultiplier = 1;

        startBrace?.Invoke(attack, monsterAttackData[attack].ReturnParentObject());

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

    #region Health and Damage

    public void MonsterHit(int damage)
    {
        print("Ouch!");

        onHit?.Invoke();

        TakeDamage(damage);
    }

    void TakeDamage(int damage)
    {
        
    }

    void SortMonsterPartsByUse()
    {
        attackingMonsterParts.Sort(SortPartUsage);

        attackingMonsterParts.Reverse();
    }

    int SortPartUsage(CombatMonsterPart part1, CombatMonsterPart part2)
    {
        return part1.ReturnPartUsage().CompareTo(part2.ReturnPartUsage());
    }
    #endregion

    #region Event Subscriptions

    void SubscribeToEvents()
    {
        _playerCombatManager.onHit += MonsterHit;
        _playerCombatManager.attackEnd += AttackEnd;
    }

    void UnsubscribeToEvents()
    {
        _playerCombatManager.onHit -= MonsterHit;
        _playerCombatManager.attackEnd -= AttackEnd;
    }

    private void OnDisable()
    {
        UnsubscribeToEvents();
    }

    #endregion

    #region Locomotion Animation Controls

    //Temp To Check Animations for Locomtion before we integrate Davids work

    public void StartWalk()
    {
        setWalk?.Invoke(true);
    }

    public void StartRun()
    {
        setRun?.Invoke(true);
    }

    public void EndMovement()
    {
        setRun?.Invoke(false);
        setWalk?.Invoke(false);
    }

    public void Jump()
    {
        jump?.Invoke();
    }

    public void Land()
    {
        land?.Invoke();
    }

    #endregion

#if UNITY_EDITOR
    [CustomEditor(typeof(CombatMonster))]
    public class CombatEditorButtons : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CombatMonster manager = target as CombatMonster;

            GUIStyle textStyle = new GUIStyle();

            textStyle.fontStyle = FontStyle.Bold;

            textStyle.normal.textColor = Color.white;

            GUILayout.Label("EDITOR TEST FUNCTIONS", textStyle);

            GUILayout.Label("Locomotion Animations");

            if (GUILayout.Button("Start Walk"))
            {
                manager.StartWalk();
            }

            if (GUILayout.Button("Start Run"))
            {
                manager.StartRun();
            }

            if (GUILayout.Button("End Movement"))
            {
                manager.EndMovement();
            }

            if (GUILayout.Button("Jump"))
            {
                manager.Jump();
            }

            if (GUILayout.Button("Land"))
            {
                manager.Land();
            }
        }
    }
#endif

}

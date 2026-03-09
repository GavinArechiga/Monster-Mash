using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BaseMonsterPart : MonoBehaviour
{
    [Header("Base Part Attributes")]
    [SerializeField] protected MonsterPartLimb partLimbType;

    [SerializeField] GameObject monsterPartVisual;

    [SerializeField] protected Animator partAnim;

    protected AttackButtons _partButton;

    Transform _parentObject;

    [SerializeField] MonsterPartAnimator monsterPartAnimator;

    CombatMonster _combatMonster;

    #region Events

    //Locomotion Related
    public Action<bool> setWalk;
    public Action<bool> setRun;
    public Action jump;
    public Action land;

    //Combat Related
    public Action<Transform> startBrace;
    public Action endBrace;
    public Action releaseTorsoBrace;
    public Action onHit;

    #endregion

    #region Initialization

    public virtual void InitializeMonsterPart(AttackButtons partButton, CombatMonster combatMonster)
    {
        SetBaseMonsterPart(partButton, combatMonster);
    }

    protected void SetBaseMonsterPart(AttackButtons partButton, CombatMonster combatMonster)
    {
        SetPartButton(partButton);

        SetCombatMonster(combatMonster);

        SubscribeToEvents();

        InitializePartAnimator();
    }

    void SetCombatMonster(CombatMonster combatMonster)
    {
        _combatMonster = combatMonster;
    }

    void SetPartButton(AttackButtons partButton)
    {
        _partButton = partButton;
    }

    void InitializePartAnimator()
    {
        monsterPartAnimator.InitializeAnimator(this, partAnim);
    }

    public void InitializePartGameObject()
    {
        SetPartParent(monsterPartVisual.transform.parent);

        if (this is CombatLegMonsterPart)
        {
            var leg = this as CombatLegMonsterPart;

            //Assign Legs Leading or Trailing Here

            leg.InitializeLegOffset();
        }
    }

    #endregion

    #region Set and Return Functions

    public void SetPartParent(Transform parentObject)
    {
        _parentObject = parentObject;
    }

    public Transform ReturnParentObject()
    {
        return _parentObject;
    }

    public MonsterPartLimb ReturnLimbType()
    {
        return partLimbType;
    }

    #endregion

    #region Event Subscriptions
    void SubscribeToEvents()
    {
        _combatMonster.startBrace += StartBrace;
        _combatMonster.endBrace += EndBrace;
        _combatMonster.releaseTorsoBrace += ReleaseTorsoBrace;
        _combatMonster.onHit += OnHit;

        //Temp Will Probably Be Replaced By A Different Script
        _combatMonster.setWalk += SetWalk;
        _combatMonster.setRun += SetRun;
        _combatMonster.jump += Jump;
        _combatMonster.land += Land;
    }

    void UnsubscribeToEvents()
    {
        _combatMonster.startBrace -= StartBrace;
        _combatMonster.endBrace -= EndBrace;
        _combatMonster.releaseTorsoBrace -= ReleaseTorsoBrace;
        _combatMonster.onHit -= OnHit;

        //Temp
        _combatMonster.setWalk -= SetWalk;
        _combatMonster.setRun -= SetRun;
        _combatMonster.jump -= Jump;
        _combatMonster.land -= Land;
    }

    private void OnDisable()
    {
        UnsubscribeToEvents();
    }

    #endregion

    #region Event Functions

    #region Combat

    //Combat
    public void StartBrace(AttackButtons attack, Transform dir)
    {
        if(_partButton == attack)
        {
            return;
        }

        startBrace?.Invoke(dir);
    }

    public void EndBrace(AttackButtons attack)
    {
        if(_partButton == attack)
        {
            return;
        }

        endBrace?.Invoke();
    }

    public void ReleaseTorsoBrace()
    {
        if(ReturnLimbType() is MonsterPartLimb.Torso)
        {
            releaseTorsoBrace?.Invoke();
        }
    }

    public void OnHit()
    {
        onHit?.Invoke();
    }
    #endregion

    #region Locomotion
    //Locomotion
    void SetWalk(bool value)
    {
        setWalk?.Invoke(value);
    }

    void SetRun(bool value)
    {
        setRun?.Invoke(value);
    }

    void Jump()
    {
        jump?.Invoke();
    }

    void Land()
    {
        land?.Invoke();
    }

    #endregion

    #endregion
}

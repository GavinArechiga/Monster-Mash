using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPartAnimator : MonoBehaviour
{
    Animator _partAnim;

    BaseMonsterPart _monsterPart;
    public virtual void InitializeAnimator(BaseMonsterPart monsterPart, Animator partAnim)
    {
        SetAnimatorController(partAnim);

        SetBaseMonsterPart(monsterPart);

        SubscribeToBaseActions();
    }

    protected void SetBaseMonsterPart(BaseMonsterPart monsterPart)
    {
        _monsterPart = monsterPart;
    }

    protected void SetAnimatorController(Animator partAnim)
    {
        _partAnim = partAnim;
    }

    #region Event Subscriptions

    protected void SubscribeToBaseActions()
    {
        _monsterPart.setWalk += SetWalk;
        _monsterPart.setRun += SetRun;
        _monsterPart.jump += Jump;
        _monsterPart.land += Land;

        _monsterPart.startBrace += StartPartBrace;
        _monsterPart.endBrace += EndPartBrace;
        _monsterPart.onHit += MonsterHit;
        _monsterPart.releaseTorsoBrace += ReleaseTorsoBrace;
    }

    protected void UnsubscribeToBaseActions()
    {
        _monsterPart.setWalk -= SetWalk;
        _monsterPart.setRun -= SetRun;
        _monsterPart.jump -= Jump;
        _monsterPart.land -= Land;

        _monsterPart.startBrace -= StartPartBrace;
        _monsterPart.endBrace -= EndPartBrace;
        _monsterPart.onHit -= MonsterHit;
        _monsterPart.releaseTorsoBrace -= ReleaseTorsoBrace;
    }

    private void OnDisable()
    {
        UnsubscribeToBaseActions();
    }

    #endregion

    #region Animations

    #region Locomotion
    void SetWalk(bool value)
    {
        SetBool(MonsterPartAnimBool.isWalking.ToString(), value);
    }

    void SetRun(bool value)
    {
        SetBool(MonsterPartAnimBool.isRunning.ToString(), value);
    }

    void Jump()
    {
        SetTrigger(MonsterPartAnimTrigger.jump.ToString());
    }

    void Land()
    {
        SetTrigger(MonsterPartAnimTrigger.land.ToString());
    }

    #endregion

    #region Combat

    //Combat Animations
    void StartPartBrace(Transform dir)
    {
        if(_monsterPart.ReturnLimbType() is MonsterPartLimb.Torso)
        {
            var torso = _monsterPart as TorsoMonsterPart;

            Vector2 blendDir = torso.ReturnBlendDir(dir);

            SetFloat(MonsterPartAnimFloat.torsoAttkDirX.ToString(), blendDir.x);
            SetFloat(MonsterPartAnimFloat.torsoAttkDirY.ToString(), blendDir.y);
        }

        SetTrigger(MonsterPartAnimTrigger.brace.ToString());
    }

    void EndPartBrace()
    {
        SetTrigger(MonsterPartAnimTrigger.unbrace.ToString());
    }

    void MonsterHit()
    {
        SetTrigger(MonsterPartAnimTrigger.hit.ToString());
    }

    void ReleaseTorsoBrace()
    {
        SetTrigger(MonsterPartAnimTrigger.release.ToString());
    }
    #endregion

    #endregion

    protected void SetTrigger(string trigger)
    {
        _partAnim.SetTrigger(trigger);
    }

    protected void SetBool(string name, bool value)
    {
        _partAnim.SetBool(name, value);
    }

    protected void SetFloat(string name, float value)
    {
        _partAnim.SetFloat(name, value);
    }
}

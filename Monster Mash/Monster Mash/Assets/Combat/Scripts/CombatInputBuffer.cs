using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatInputBuffer : MonoBehaviour
{
    CombatActionInvoker _actionInvoker;
    PlayerCombatManager _combatManager;
    CombatMonster _combatMonster;

    [SerializeField] int bufferFrames;

    Stack<ICombatActionCommand> bufferInputs;
    int highestActionPriority;

    public void InitializeCombatBuffer(CombatActionInvoker actionInvoker, CombatMonster combatMonster, PlayerCombatManager combatManager)
    {
        _actionInvoker = actionInvoker;
        _combatMonster = combatMonster;
        _combatManager = combatManager;

        bufferInputs = new Stack<ICombatActionCommand>();

        highestActionPriority = 0;
    }

    public void BufferCombatInput(ICombatActionCommand command)
    {
        HandleInputBuffer(command, _combatManager.ReturnPlayerState());
    }

    void HandleInputBuffer(ICombatActionCommand command, PlayerState currentState)
    {
        //Player Should Only Be Able to Buffer Inputs During the tail end of an Attack Anim and During Hit
        //We Can Decide Later on The Logic for Movement as Well
        switch(currentState)
        {
            case PlayerState.Idle:

                ExecuteActionCommand(command);

                break;

            case PlayerState.Charging:

                if(command is HeavyReleaseActionCommand)
                {
                    ExecuteActionCommand(command);
                }

                break;

            case PlayerState.Attacking:

                //Handle Players Input Buffer

                DetermineAttackBuffer(command);

                break;
        }
    }

    public void OnAttackEnd()
    {
        ReleaseInputBuffer();
    }

    public void ClearBuffer()
    {
        bufferInputs.Clear();
    }

    void ReleaseInputBuffer()
    {
        if(bufferInputs.Count >= 1)
        {
            //Release Buffer Code

            ICombatActionCommand[] bufferArray = bufferInputs.ToArray();

            foreach(ICombatActionCommand stackAction in bufferArray)
            {
                BaseActionCommand actionBase = stackAction as BaseActionCommand;

                if(actionBase.GetActionPriority() == highestActionPriority)
                {
                    print("Executing");

                    ExecuteActionCommand(stackAction);

                    CheckForHeavyReleaseBuffer(stackAction, bufferArray);

                    break;
                }
            }

            ClearBuffer();

            highestActionPriority = 0;
        }

        else
        {
            _combatManager.SetPlayerState(PlayerState.Idle);
        }

    }

    void DetermineAttackBuffer(ICombatActionCommand command)
    {
        if(command is BaseAttackCommand)
        {
            BaseAttackCommand attackProperties = command as BaseAttackCommand;

            if(!_combatMonster.CheckPartEnabledStatus(attackProperties.GetAttackButton()))
            {
                return;
            }
        }

        AnimatorClipInfo[] attackAnim = _combatMonster.ReturnAttackAnimationClip();

        AnimatorStateInfo animState = _combatMonster.ReturnAnimatorStateInfo();

        if(animState.IsTag(MonsterPartAnimStateTags.attack.ToString()))
        {
            int currentFrame = 0;

            currentFrame = (int)(animState.normalizedTime *
                (attackAnim[0].clip.length * attackAnim[0].clip.frameRate));

            float attackLength = 0;
            
            attackLength = (attackAnim[0].clip.length * attackAnim[0].clip.frameRate);

            DetermineInputBuffer(currentFrame, attackLength, bufferFrames, command);
        }
    }

    void DetermineInputBuffer(int currentFrame, float animFrameCount, int bufferFrames, ICombatActionCommand command)
    {
        if(CalculateBufferFrame(currentFrame, animFrameCount, bufferFrames))
        {
            BaseActionCommand actionCommand = (BaseActionCommand)command;

            if(actionCommand.GetActionPriority() > highestActionPriority)
            {
                highestActionPriority = actionCommand.GetActionPriority();
            }

            bufferInputs.Push(command);
        }
    }

    bool CalculateBufferFrame(int currentFrame, float animFrameCount, int bufferFrames)
    {
        if(currentFrame >= (animFrameCount - bufferFrames))
        {
            return true;
        }

        else
        {
            return false;
        }
    }

    void CheckForHeavyReleaseBuffer(ICombatActionCommand command, ICombatActionCommand[] inputs)
    {
        if(command is HeavyStartActionCommand)
        {
            foreach(ICombatActionCommand bufferCommand in inputs)
            {
                if(bufferCommand is HeavyReleaseActionCommand)
                {
                    ExecuteActionCommand(bufferCommand);

                    break;
                }
            }
        }
    }

    void ExecuteActionCommand(ICombatActionCommand command)
    {
        _actionInvoker.ExecuteCommand(command);
    }
}

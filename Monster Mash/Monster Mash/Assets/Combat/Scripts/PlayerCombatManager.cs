using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombatManager : MonoBehaviour
{
    [SerializeField] PlayerState currentState= PlayerState.Disabled;

    [SerializeField] PlayerInput playerInput;

    [SerializeField] CombatActionInitializer combatActionInitializer;

    [SerializeField] CombatMonster combatMonster;

    [SerializeField]
    CombatInputBuffer combatInputBuffer;

    private void Awake()
    {
        combatActionInitializer.InitializeCombat(playerInput, this, 
            combatInputBuffer, combatMonster);

        combatMonster.InitializeMonster(this);

        PartAttackBehaviour.endAction += OnAttackEnd;
    }

    public PlayerState ReturnPlayerState()
    {
        return currentState;
    }

    public void SetPlayerState(PlayerState newState)
    {
        currentState = newState;
    }

    void OnAttackEnd()
    {
        if(ReturnPlayerState() == PlayerState.Attacking)
        {
            //Run Buffer Code

            combatInputBuffer.OnAttackEnd();
        }

        else
        {
            //Run a Function that Clears the Input buffer

            //This is a use case for if you are hit while attacking, and you end up being changed to the hit state
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
//Temp
using UnityEditor;

public class PlayerCombatManager : MonoBehaviour
{
    [Header("Player Manager Attributes")]

    [SerializeField] PlayerState currentState= PlayerState.Disabled;

    [SerializeField] PlayerInput playerInput;

    [SerializeField] CombatActionInitializer combatActionInitializer;

    [SerializeField] CombatMonster combatMonster;

    [SerializeField]
    CombatInputBuffer combatInputBuffer;

    List<MonsterPartAttackBehaviours> _allPartAttacks;

    public Action<int> onHit;

    public Action attackEnd;
    private void Awake()
    {
        combatActionInitializer.InitializeCombat(playerInput, this, 
            combatInputBuffer, combatMonster);

        combatMonster.InitializeMonster(this);

        _allPartAttacks = combatMonster.ReturnAttackBehaviours();

        SubscribeToStateBehaviours(_allPartAttacks);
    }

    void SubscribeToStateBehaviours(List<MonsterPartAttackBehaviours> partAttacks)
    {
        foreach(MonsterPartAttackBehaviours parts in partAttacks)
        {
            foreach(PartAttackBehaviour attack in parts._partAttacks)
            {
                attack.endAction += OnAttackEnd;
            }
        }
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

            combatInputBuffer.ClearBuffer();
        }

        //End Brace

        attackEnd?.Invoke();
    }

    public void OnHit()
    {
        onHit?.Invoke(0);
    }

    private void OnDisable()
    {
        foreach (MonsterPartAttackBehaviours parts in _allPartAttacks)
        {
            foreach (PartAttackBehaviour attack in parts._partAttacks)
            {
                attack.endAction -= OnAttackEnd;
            }
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(PlayerCombatManager))]
    public class EditorButtons : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            PlayerCombatManager manager = target as PlayerCombatManager;

            GUIStyle textStyle = new GUIStyle();

            textStyle.fontStyle = FontStyle.Bold;

            textStyle.normal.textColor = Color.white;

            GUILayout.Label("EDITOR TEST FUNCTIONS", textStyle);

            GUILayout.Label("Damage");

            if(GUILayout.Button("Monster Hit"))
            {
                manager.OnHit();
            }
        }
    }
#endif
}

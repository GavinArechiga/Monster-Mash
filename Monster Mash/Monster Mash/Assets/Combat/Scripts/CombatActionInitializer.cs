using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatActionInitializer : MonoBehaviour
{
    //This Script is Responsible for injecting the dependencies into the other Combat scripts

    [SerializeField]
    private CombatActionController combatActionController;


    CombatInputBuffer _combatInputBuffer;

    
    PlayerInput _playerInput;

    CombatMonster _combatMonster;

    private CombatActionInvoker actionInvoker;
    private PlayerCombatManager _combatManager;
    public void InitializeCombat(PlayerInput playerInput, PlayerCombatManager combatManager, 
        CombatInputBuffer combatInputBuffer, 
        CombatMonster combatMonster)
    {
        _playerInput = playerInput;
        _combatManager = combatManager;
        _combatInputBuffer = combatInputBuffer;
        _combatMonster = combatMonster;

        InitializeActionInvoker();
        InitializeCombatScripts();
    }

    void InitializeActionInvoker()
    {
        actionInvoker = new CombatActionInvoker();
    }

    void InitializeCombatScripts()
    {
        _combatInputBuffer.InitializeCombatBuffer(actionInvoker, _combatMonster ,_combatManager);
        combatActionController.Initialize(_playerInput, _combatInputBuffer, _combatMonster);
    }
}

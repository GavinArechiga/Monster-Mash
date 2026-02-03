using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatActionInitializer : MonoBehaviour
{
    //This Script is Responsible for injecting the dependencies into the other Combat scripts

    [SerializeField]
    private CombatActionController combatActionController;

    //Later On We can replace this
    [SerializeField]
    PlayerInput playerInput;

    [SerializeField]
    CombatMonster combatMonster;

    private CombatActionInvoker actionInvoker;

    private void Awake()
    {
        InitializeActionInvoker();
        InitializeCombat();
    }

    void InitializeActionInvoker()
    {
        actionInvoker = new CombatActionInvoker();
    }

    void InitializeCombat()
    {
        combatActionController.Initialize(playerInput, actionInvoker, combatMonster);
    }
}

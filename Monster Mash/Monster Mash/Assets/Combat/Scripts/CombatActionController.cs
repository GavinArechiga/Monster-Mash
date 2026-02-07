using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class CombatActionController : MonoBehaviour
{
    private PlayerInput _playerInput;
    private CombatInputBuffer _inputBuffer;

    private CombatMonster _combatMonster;
   
    public void Initialize(PlayerInput playerInput, CombatInputBuffer inputBuffer ,CombatMonster combatMonster)
    {
        _playerInput = playerInput;
        _inputBuffer = inputBuffer;
        _combatMonster = combatMonster;
        SubscribeActions();
    }

    void SubscribeActions()
    {
        _playerInput.actions.FindActionMap("Monster Controls").FindAction("A").performed += ctx => HandleSouthButtonActionCommand();
        _playerInput.actions.FindActionMap("Monster Controls").FindAction("B").performed += HandleEastButtonActionCommand;
        _playerInput.actions.FindActionMap("Monster Controls").FindAction("X").performed += HandleWestButtonActionCommand;
        _playerInput.actions.FindActionMap("Monster Controls").FindAction("Y").performed += HandleNorthButtonActionCommand;

        _playerInput.actions.FindActionMap("Monster Controls").FindAction("B").canceled += HandleEastButtonRelease;
        _playerInput.actions.FindActionMap("Monster Controls").FindAction("X").canceled += HandleWestButtonRelease;
        _playerInput.actions.FindActionMap("Monster Controls").FindAction("Y").canceled += HandleNorthButtonRelease;
    }

    #region Button Presses

    void HandleSouthButtonActionCommand()
    {
        var southButtonCommand = new SouthButtonActionCommand();
        ExecuteActionCommand(southButtonCommand);
    }
    void HandleEastButtonActionCommand(InputAction.CallbackContext context)
    {
        HandleButtonInteractions(AttackButtons.BttnEast, _combatMonster, context);

    }
    void HandleWestButtonActionCommand(InputAction.CallbackContext context)
    {
        HandleButtonInteractions(AttackButtons.BttnWest, _combatMonster, context);
    }
    void HandleNorthButtonActionCommand(InputAction.CallbackContext context)
    {
        HandleButtonInteractions(AttackButtons.BttnNorth, _combatMonster, context);
    }

    void HandleButtonInteractions(AttackButtons button, CombatMonster combatMonster, InputAction.CallbackContext context)
    {
        if (context.duration < 0.3f)
        {
            var buttonActionCommand = new CombatButtonActionCommand(button, combatMonster);
            ExecuteActionCommand(buttonActionCommand);
        }

        else if (context.duration > 0.31f)
        {
            var heavyStartActionCommand = new HeavyStartActionCommand(button, combatMonster);
            ExecuteActionCommand(heavyStartActionCommand);
        }
    }

    #endregion

    #region Button Releases

    void HandleEastButtonRelease(InputAction.CallbackContext context)
    {
        HandleButtonRelease(AttackButtons.BttnEast, _combatMonster, context);
    }

    void HandleWestButtonRelease(InputAction.CallbackContext context)
    {
        HandleButtonRelease(AttackButtons.BttnWest, _combatMonster, context);
    }

    void HandleNorthButtonRelease(InputAction.CallbackContext context)
    {
        HandleButtonRelease(AttackButtons.BttnNorth, _combatMonster, context);
    }

    void HandleButtonRelease(AttackButtons attack, CombatMonster combatMonster, InputAction.CallbackContext context)
    {
        if (context.duration > 0.31f)
        {
            var heavyReleaseCommand = new HeavyReleaseActionCommand(attack, combatMonster);
            ExecuteActionCommand(heavyReleaseCommand);
        }
    }

    #endregion

    void ExecuteActionCommand(ICombatActionCommand command)
    {
        //_actionInvoker.ExecuteCommand(command);
        _inputBuffer.BufferCombatInput(command);
    }
}

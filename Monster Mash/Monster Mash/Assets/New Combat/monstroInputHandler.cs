using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.InputSystem;

public class monstroInputHandler : MonoBehaviour
{
    PlayerInput playerInput;
    private monstroLocomotion locomotion;
    private monstroPartHandler partHandler;
    public monstroPart[] mappedMonstroParts = new monstroPart[7];
    private monstroPart currentlyAttackingPart;
    private float heavyDetectionTime = 0.2f; //how long should a press be before it registers as a heavy
    private bool attackMarkedHeavy;
    private bool attackStarted = false;
    private bool attackCooled = true;
    private float heavyAttackCooldownTime = 0.5f;
    private float lightAttackCooldownTime = 0.2f;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        locomotion = GetComponent<monstroLocomotion>();
        partHandler = GetComponent<monstroPartHandler>();
    }

    public void OnMove(CallbackContext context)
    {
        if (locomotion.playerLock || locomotion.isStunLocked || locomotion.isElectricLocked) return;
        if (attackCooled == false) return;

        locomotion.movementInput(context.ReadValue<Vector2>());
    }

    public void OnJump(CallbackContext context)
    {
        if (locomotion.playerLock || locomotion.isStunLocked || locomotion.isElectricLocked) return;
        if (attackStarted) return;

        if (context.started)
        {
            locomotion.jumpInput();
        }
    }

    public void OnButtonEast(CallbackContext context)
    {
        int buttonIndex = 0;
        if (mappedMonstroParts[buttonIndex] == null) return;//there was never a part assigned to the button
        if (partHandler.availableAttackingParts.Contains(mappedMonstroParts[buttonIndex]) == false) return;//the part has likely been removed
        if (locomotion.playerLock || locomotion.isStunLocked || locomotion.isElectricLocked) return;//the player is stuck in a damage animation
        if (attackCooled == false) return;//attack is still in a cooldown phase
        if (currentlyAttackingPart != null && currentlyAttackingPart != mappedMonstroParts[buttonIndex]) return;//a different part is still going through its attack

        if (context.started)
        {
            attackStart(buttonIndex);
        }

        if (context.canceled && attackStarted)
        {
            attackRelease(buttonIndex);
        }
    }

    public void OnButtonWest(CallbackContext context)
    {
        int buttonIndex = 1;
        if (mappedMonstroParts[buttonIndex] == null) return;//there was never a part assigned to the button
        if (partHandler.availableAttackingParts.Contains(mappedMonstroParts[buttonIndex]) == false) return;//the part has likely been removed
        if (locomotion.playerLock || locomotion.isStunLocked || locomotion.isElectricLocked) return;//the player is stuck in a damage animation
        if (attackCooled == false) return;//attack is still in a cooldown phase
        if (currentlyAttackingPart != null && currentlyAttackingPart != mappedMonstroParts[buttonIndex]) return;//a different part is still going through its attack

        if (context.started)
        {
            attackStart(buttonIndex);
        }

        if (context.canceled && attackStarted)
        {
            attackRelease(buttonIndex);
        }
    }

    public void OnButtonNorth(CallbackContext context)
    {
        int buttonIndex = 2;
        if (mappedMonstroParts[buttonIndex] == null) return;//there was never a part assigned to the button
        if (partHandler.availableAttackingParts.Contains(mappedMonstroParts[buttonIndex]) == false) return;//the part has likely been removed
        if (locomotion.playerLock || locomotion.isStunLocked || locomotion.isElectricLocked) return;//the player is stuck in a damage animation
        if (attackCooled == false) return;//attack is still in a cooldown phase
        if (currentlyAttackingPart != null && currentlyAttackingPart != mappedMonstroParts[buttonIndex]) return;//a different part is still going through its attack

        if (context.started)
        {
            attackStart(buttonIndex);
        }

        if (context.canceled && attackStarted)
        {
            attackRelease(buttonIndex);
        }
    }

    public void OnLeftBumper(CallbackContext context)
    {
        int buttonIndex = 3;
        if (mappedMonstroParts[buttonIndex] == null) return;//there was never a part assigned to the button
        if (partHandler.availableAttackingParts.Contains(mappedMonstroParts[buttonIndex]) == false) return;//the part has likely been removed
        if (locomotion.playerLock || locomotion.isStunLocked || locomotion.isElectricLocked) return;//the player is stuck in a damage animation
        if (attackCooled == false) return;//attack is still in a cooldown phase
        if (currentlyAttackingPart != null && currentlyAttackingPart != mappedMonstroParts[buttonIndex]) return;//a different part is still going through its attack

        if (context.started)
        {
            attackStart(buttonIndex);
        }

        if (context.canceled && attackStarted)
        {
            attackRelease(buttonIndex);
        }
    }

    public void OnRightBumper(CallbackContext context)
    {
        int buttonIndex = 4;
        if (mappedMonstroParts[buttonIndex] == null) return;//there was never a part assigned to the button
        if (partHandler.availableAttackingParts.Contains(mappedMonstroParts[buttonIndex]) == false) return;//the part has likely been removed
        if (locomotion.playerLock || locomotion.isStunLocked || locomotion.isElectricLocked) return;//the player is stuck in a damage animation
        if (attackCooled == false) return;//attack is still in a cooldown phase
        if (currentlyAttackingPart != null && currentlyAttackingPart != mappedMonstroParts[buttonIndex]) return;//a different part is still going through its attack

        if (context.started)
        {
            attackStart(buttonIndex);
        }

        if (context.canceled && attackStarted)
        {
            attackRelease(buttonIndex);
        }
    }

    public void OnLeftTrigger(CallbackContext context)
    {
        int buttonIndex = 5;
        if (mappedMonstroParts[buttonIndex] == null) return;//there was never a part assigned to the button
        if (partHandler.availableAttackingParts.Contains(mappedMonstroParts[buttonIndex]) == false) return;//the part has likely been removed
        if (locomotion.playerLock || locomotion.isStunLocked || locomotion.isElectricLocked) return;//the player is stuck in a damage animation
        if (attackCooled == false) return;//attack is still in a cooldown phase
        if (currentlyAttackingPart != null && currentlyAttackingPart != mappedMonstroParts[buttonIndex]) return;//a different part is still going through its attack

        if (context.started)
        {
            attackStart(buttonIndex);
        }

        if (context.canceled && attackStarted)
        {
            attackRelease(buttonIndex);
        }
    }

    public void OnRightTigger(CallbackContext context)
    {
        int buttonIndex = 6;
        if (mappedMonstroParts[buttonIndex] == null) return;//there was never a part assigned to the button
        if (partHandler.availableAttackingParts.Contains(mappedMonstroParts[buttonIndex]) == false) return;//the part has likely been removed
        if (locomotion.playerLock || locomotion.isStunLocked || locomotion.isElectricLocked) return;//the player is stuck in a damage animation
        if (attackCooled == false) return;//attack is still in a cooldown phase
        if (currentlyAttackingPart != null && currentlyAttackingPart != mappedMonstroParts[buttonIndex]) return;//a different part is still going through its attack

        if (context.started)
        {
            attackStart(buttonIndex);
        }

        if (context.canceled && attackStarted)
        {
            attackRelease(buttonIndex);
        }
    }

    private void attackStart(int mappedButton)
    {
        attackStarted = true;
        StartCoroutine(buttonPressTimer());
        currentlyAttackingPart = mappedMonstroParts[mappedButton];
        partHandler.windUp(currentlyAttackingPart);
        partHandler.lockAttackAnimation(currentlyAttackingPart);
        locomotion.attackEngaged();
    }

    private void attackRelease(int mappedButton)
    {
        StopCoroutine(buttonPressTimer());
        StartCoroutine(attackCooldown());
        bool needsAttackMovement = false;

        if (attackMarkedHeavy)
        {
            if (mappedMonstroParts[mappedButton].heavyAttackTypeDropDown.ToString() == "physical")
            {
                needsAttackMovement = true;
            }
        }
        else
        {
            if (mappedMonstroParts[mappedButton].lightAttackTypeDropDown.ToString() == "physical")
            {
                needsAttackMovement = true;
            }
        }

        partHandler.attack(mappedMonstroParts[mappedButton], attackMarkedHeavy);
        locomotion.attackMovementConfirmed(attackMarkedHeavy, needsAttackMovement);
        attackStarted = false;
    }

    IEnumerator buttonPressTimer()
    {
        attackMarkedHeavy = false;
        yield return new WaitForSeconds(heavyDetectionTime);
        attackMarkedHeavy = true;
    }

    IEnumerator attackCooldown()
    {
        attackCooled = false;

        if (attackMarkedHeavy)
        {
            yield return new WaitForSeconds(heavyAttackCooldownTime);
        }
        else
        {
            yield return new WaitForSeconds(lightAttackCooldownTime);
        }

        partHandler.unlockAttackAnimation(currentlyAttackingPart);
        currentlyAttackingPart = null;
        attackCooled = true;
    }

    public void OnPause(CallbackContext context)
    {
        if (context.started)
        {
            print("pause");//this will call out to some sort of menu manager to throw up a pause screen
        }
    }

    public void switchToMonsterControls()
    {
        locomotion.enabled = true;
        playerInput.SwitchCurrentActionMap("Monstro Movement");
    }
}

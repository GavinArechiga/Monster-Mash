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
    private MonstroUI uiHandler;
    public MonstroUICursor cursorHandler;
    public monstroPart[] mappedMonstroParts = new monstroPart[7];
    private monstroPart currentlyAttackingPart;
    private int currentlyAttackingPartIndex;
    private float heavyDetectionTime = 0.2f; //how long should a press be before it registers as a heavy
    private bool attackMarkedHeavy;
    private bool attackStarted = false;
    private bool attackCooled = true;
    private bool jumpStarted = false;
    private bool jumpRecoveryCooled = true;
    private bool jumpMarkedForRecovery = false;
    private float heavyAttackCooldownTime = 0.5f;//0.5
    private float lightAttackCooldownTime = 0.25f;//0.2
    private float jumpRecoveryCooldownTime = 1f;

    private bool inInteractionZone = false;
    private hubInteractionTrigger currentPotentialInteraction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        locomotion = GetComponent<monstroLocomotion>();
        partHandler = GetComponent<monstroPartHandler>();
        uiHandler = GetComponent<MonstroUI>();
        cursorHandler = GetComponent<MonstroUICursor>();
    }

    #region Switching Controls

    public void switchToFightControls()
    {
        locomotion.enabled = true;
        uiHandler.enabled = false;
        cursorHandler.enabled = false;
        playerInput.SwitchCurrentActionMap("Monstro Fighter");
    }

    public void switchToHubInteractionControls()
    {
        locomotion.enabled = true;
        uiHandler.enabled = false;
        cursorHandler.enabled = false;
        playerInput.SwitchCurrentActionMap("Monstro Interaction");
    }

    public void switchToUIbuttonsControls()
    {
        locomotion.enabled = false;
        uiHandler.enabled = true;
        cursorHandler.enabled = false;
        playerInput.SwitchCurrentActionMap("Monstro UI Buttons");
    }

    public void switchToUICursorControls()
    {
        locomotion.enabled = false;
        uiHandler.enabled = false;
        cursorHandler.enabled = true;
        playerInput.SwitchCurrentActionMap("Monstro UI Cursor");
    }

    #endregion

    #region Movement, Jumping, and Recovery
    public void OnMove(CallbackContext context)
    {
        if (locomotion.playerLock || locomotion.isStunLocked || locomotion.isElectricLocked) return;
        //if (attackCooled == false) return;

        locomotion.movementInput(context.ReadValue<Vector2>());
    }

    public void OnJump(CallbackContext context)
    {
        if (locomotion.playerLock || locomotion.isStunLocked || locomotion.isElectricLocked) return;
        if (attackStarted) return;

        if (context.started)
        {
            if(jumpStarted == false)
            {
                StopCoroutine(jumpInputTimer());
                StartCoroutine(jumpInputTimer());
                jumpStarted = true;
            }
        }

        if (context.canceled && jumpMarkedForRecovery == false)
        {
            locomotion.jumpInput();
            jumpStarted = false;
            jumpMarkedForRecovery = false;
        }
        else if (context.canceled && jumpMarkedForRecovery)
        {
            locomotion.recover();
            jumpStarted = false;
            jumpMarkedForRecovery = false;
        }


    }

    private void jumpStart()
    {
        
    }

    private void jumpRelease()
    {
       
    }

    IEnumerator jumpInputTimer()
    {
        jumpMarkedForRecovery = false;
        yield return new WaitForSeconds(heavyDetectionTime);
        if (jumpStarted)
        {
            jumpMarkedForRecovery = true;
        }
    }


    #endregion

    #region Combat Functions
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
        currentlyAttackingPart = mappedMonstroParts[mappedButton];
        currentlyAttackingPartIndex = mappedButton;
        partHandler.windUp(currentlyAttackingPart);
        partHandler.lockAttackAnimation(currentlyAttackingPart);
        StartCoroutine(buttonPressTimer());
    }

    private void attackRelease(int mappedButton)
    {
        if (attackStarted == false) return;

        StopCoroutine(buttonPressTimer());
        StartCoroutine(attackCooldown());
        locomotion.attackEngaged();
        partHandler.attack(mappedMonstroParts[mappedButton], attackMarkedHeavy);

        if (attackMarkedHeavy)
        {
            locomotion.attackMovementConfirmed(attackMarkedHeavy, mappedMonstroParts[mappedButton].heavyAttackTypeDropDown.ToString());
        }
        else
        {
            locomotion.attackMovementConfirmed(attackMarkedHeavy, mappedMonstroParts[mappedButton].lightAttackTypeDropDown.ToString());
        }

        attackStarted = false;
    }

    IEnumerator buttonPressTimer()
    {
        attackMarkedHeavy = false;
        yield return new WaitForSeconds(heavyDetectionTime);

        if (attackCooled && attackStarted)//double checks that the attack is still active and ready to be adjusted to a heavy 
        {
            attackMarkedHeavy = true;
            locomotion.attackEngaged();
            yield return new WaitForSeconds(heavyDetectionTime);//additional time here reduces stuttering with model
            locomotion.activateChargedAttackMovement();
        }
    }

    IEnumerator attackCooldown()
    {
        attackCooled = false;
        StopCoroutine(buttonPressTimer());

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
        attackMarkedHeavy = false;
        locomotion.attackCleanup();
    }

    public void forceReleaseAttack()
    {
        attackRelease(currentlyAttackingPartIndex);
    }

    public void OnPause(CallbackContext context)
    {
        if (context.started)
        {
            print("pause");//this will call out to some sort of menu manager to throw up a pause screen
        }
    }

    #endregion

    #region Hub Interaction
    public void onHubInteraction(CallbackContext context)
    {
        if (inInteractionZone == false) return;
        if (currentPotentialInteraction == null) return;

        if (context.started)
        {
            currentPotentialInteraction.buttonInteraction();
        }
    }
    #endregion

    #region Level Interaction
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Hub Interaction")
        {
            inInteractionZone = true;
            hubInteractionTrigger potentialInteraction = other.gameObject.GetComponent<hubInteractionTrigger>();
            if(potentialInteraction != null)
            {
                currentPotentialInteraction = potentialInteraction;
            }

            if (currentPotentialInteraction != null)
            {
                if (currentPotentialInteraction.requiresButtonPrompt)
                {
                    switchToHubInteractionControls();
                }

                currentPotentialInteraction.enterInteractionArea();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Hub Interaction")
        {
            inInteractionZone = false;
            switchToFightControls();

            hubInteractionTrigger potentialInteraction = other.gameObject.GetComponent<hubInteractionTrigger>();
            if (potentialInteraction != null)
            {
                currentPotentialInteraction = potentialInteraction;
            }


            if (currentPotentialInteraction != null)
            {
                currentPotentialInteraction.exitInteractionArea();
            }

            currentPotentialInteraction = null;
        }
    }
    #endregion

    #region Cursor Controls, Character Select
    public void OnNavigateCS (CallbackContext context)
    {
        if (context.performed)
        {
            cursorHandler.CursorNavigate(true, context.ReadValue<Vector2>());
        }
        else if (context.canceled)
        {
            cursorHandler.CursorNavigate(false, context.ReadValue<Vector2>());
        }
    }

    public void OnSubmitCS(CallbackContext context)
    {
        if (context.performed)
        {
            cursorHandler.CursorSubmit();
        }
    }

    public void OnCancelCS(CallbackContext context)
    {
        if (context.performed)
        {
            cursorHandler.CursorCancel();
        }
    }

    public void OnConfirmCS(CallbackContext context)
    {
        if (context.performed)
        {
            cursorHandler.CursorConfirm();
        }
    }

    #endregion
}

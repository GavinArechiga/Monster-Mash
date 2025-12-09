using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPartVisual : MonoBehaviour
{
    //This script is a mess a lot of the vars are not assigned. I just chucked all the VFX in here so I could deal with it seperate from the rest of the monster part script

    [Header("Neutral Attack VFX Arrays")]
    public Transform[] neutralAttackHitVFXArray;
    public Transform[] neutralAttackForwardSwingVFXArray;
    public Transform[] neutralAttackBackwardSwingVFXArray;
    public Transform[] neutralAttackDownwardSwingVFXArray;
    public Transform[] neutralAttackMissVFXArray;
    public Transform[] neutralAttackDefaultVFXArray;
    public Transform[] neutralStompVFXArray;

    [Header("Neutral Attack VFX Holders")]
    public GameObject neutralHitVFXHolder;
    public GameObject neutralForwardSwingVFXHolder;
    public GameObject neutralBackwardSwingVFXHolder;
    public GameObject neutralDownwardSwingVFXHolder;
    [field: SerializeField] public GameObject neutralMissVFXHolder;
    public GameObject neutralDefaultSprayVFXHolder;
    public GameObject neutralStompVFXHolder;

    [Header("Heavy Attack VFX Arrays")]
    public Transform[] heavyAttackHitVFXArray;
    public Transform[] heavyAttackForwardSwingVFXArray;
    public Transform[] heavyAttackBackwardSwingVFXArray;
    public Transform[] heavyAttackDownwardSwingVFXArray;
    public Transform[] heavyAttackMissVFXArray;
    public Transform[] heavyAttackDefaultVFXArray;
    public Transform[] heavyStompVFXArray;

    [Header("Heavy Attack VFX Holders")]
    public GameObject heavyHitVFXHolder;
    public GameObject heavyForwardSwingVFXHolder;
    public GameObject heavyBackwardSwingVFXHolder;
    public GameObject heavyDownwardSwingVFXHolder;
    public GameObject heavyMissVFXHolder;
    public GameObject heavyDefaultSprayVFXHolder;
    public GameObject heavyStompVFXHolder;

    public ParticleSystem[] myIdleVFX;

    [HideInInspector] public vfxHolder neutralHitVFXManager;
    private vfxHolder neutralForwardSwingVFXManager;
    private vfxHolder neutralBackwardSwingVFXManager;
    private vfxHolder neutralDownwardSwingVFXManager;
    [HideInInspector] public vfxHolder neutralMissVFXManager;
    [HideInInspector] public vfxHolder neutralDefaultSprayVFXManager;
    private vfxHolder neutralStompVFXManager;

    [HideInInspector] public vfxHolder heavyHitVFXManager;
    private vfxHolder heavyForwardSwingVFXManager;
    private vfxHolder heavyBackwardSwingVFXManager;
    private vfxHolder heavyDownwardSwingVFXManager;
    [HideInInspector] public vfxHolder heavyMissVFXManager;
    [HideInInspector] public vfxHolder heavyDefaultSprayVFXManager;
    private vfxHolder heavyStompVFXManager;

    [HideInInspector] public Transform neutralVFXStoredParent;
    [HideInInspector] public Vector3 neutralVFXStoredPosition;
    [HideInInspector] public Quaternion neutralVFXStoredRotation;

    private int neutralVFXCount;

    private Transform neutralHitVFXParent;
    private Transform neutralMissVFXParent;
    private Transform neutralDefaultSprayVFXParent;
    private Vector3 neutralDefaultSprayVFXStoredPosition;
    private Quaternion neutralDefaultSprayVFXStoredRotation;
    public Transform heavyVFXStoredParent;
    public Vector3 heavyVFXStoredPosition;
    public Quaternion heavyVFXStoredRotation;
    private int heavyVFXCount;

    private NewMonsterPart monsterPartRef;


    public ParticleSystem chargeVisual;
    public ParticleSystem heavyChargeVisual;
    public GameObject specialRunVisual;
    public Transform neutralMuzzle;
    public Transform heavyMuzzle;

    #region Movement Commands
    [HideInInspector] public bool hasHeavyMovementCommand = false;
    [HideInInspector] public string forwardHeavyMovementCommand = "";
    [HideInInspector] public string upwardHeavyMovementCommand = "";
    [HideInInspector] public string backwardHeavyMovementCommand = "";
    [HideInInspector] public string downwardHeavyMovementCommand = "";

    [HideInInspector] public bool hasNeutralMovementCommand = false;
    [HideInInspector] public string forwardNeutralMovementCommand = "";
    [HideInInspector] public string upwardNeutralMovementCommand = "";
    [HideInInspector] public string backwardNeutralMovementCommand = "";
    [HideInInspector] public string downwardNeutralMovementCommand = "";


    [HideInInspector] public bool hasTorsoCommand = false;
    [HideInInspector] public bool hasHeadCommand = false;


    [HideInInspector] public string forwardInputTorsoCommand = "";
    [HideInInspector] public string backwardInputTorsoCommand = "";
    [HideInInspector] public string upwardInputTorsoCommand = "";
    [HideInInspector] public string downwardInputTorsoCommand = "";
    [HideInInspector] public string forwardInputHeadCommand = "";
    [HideInInspector] public string backwardInputHeadCommand = "";
    [HideInInspector] public string upwardInputHeadCommand = "";
    [HideInInspector] public string downwardInputHeadCommand = "";
    #endregion

    private void Awake()
    {
        monsterPartRef = GetComponent<NewMonsterPart>();
    }


    #region Animation Events
    #region Attack Events
    public void triggerChargeVisual()
    {
        if (chargeVisual != null)
        {
            chargeVisual.Stop();
            chargeVisual.Play();
        }
    }

    public void triggerEndChargeVisual()
    {
        if (chargeVisual != null)
        {
            chargeVisual.Stop();
        }
    }

    public void triggerHeavyChargeVisual()
    {
        if (heavyChargeVisual != null)
        {
            heavyChargeVisual.Stop();
            heavyChargeVisual.Play();
        }
    }

    public void triggerRunVisual()
    {
        //if we decide that multiple pieces other than grounded legs should have a trail visual, we will move this into a full network message
        if (specialRunVisual != null)
        {
            specialRunVisual.SetActive(true);
        }
    }

    public void triggerStompVisual()
    {
        if (neutralStompVFXManager != null && monsterPartRef.attackMarkedHeavy == false)
        {
            neutralStompVFXManager.unleashAdditionalSprayVisual();
        }
        else if (heavyStompVFXManager != null && monsterPartRef.attackMarkedHeavy == true)
        {
            heavyStompVFXManager.unleashAdditionalSprayVisual();
        }
    }

    public void triggerNeutralAttackVisuals() //called in attack animation //new attack types must be added here
    {
        monsterPartRef.neutralAttack.triggerNeutralAttackVisuals();

    }

    public void triggerNeutralSwingVisual()
    {
        if (neutralForwardSwingVFXManager && monsterPartRef.attackAnimationID == 1)
        {
            neutralForwardSwingVFXManager.unleashAdditionalSprayVisual();
        }
        else if (neutralBackwardSwingVFXManager && monsterPartRef.attackAnimationID == -1)
        {
            neutralBackwardSwingVFXManager.unleashAdditionalSprayVisual();
        }
        else if (neutralDownwardSwingVFXManager && monsterPartRef.attackAnimationID == 0)
        {
            neutralDownwardSwingVFXManager.unleashAdditionalSprayVisual();
        }
    }

    public void triggerHeavyAttackVisuals() //new attack types must be added here
    {
        monsterPartRef.heavyAttack.triggerHeavyAttackVisuals();
    }

    public void triggerHeavySwingVisual()
    {
        if (heavyForwardSwingVFXManager && monsterPartRef.attackAnimationID == 1)
        {
            heavyForwardSwingVFXManager.unleashAdditionalSprayVisual();
        }
        else if (heavyBackwardSwingVFXManager && monsterPartRef.attackAnimationID == -1)
        {
            heavyBackwardSwingVFXManager.unleashAdditionalSprayVisual();
        }
        else if (heavyDownwardSwingVFXManager && monsterPartRef.attackAnimationID == 0)
        {
            heavyDownwardSwingVFXManager.unleashAdditionalSprayVisual();
        }
    }

    public void triggerAttackAnticipation()
    {
        monsterPartRef.myMainSystem.correctAttackDirection(0);

        if (monsterPartRef.attackAnimationID == 1)
        {
            if (hasTorsoCommand)
            {
                monsterPartRef.connectedMonsterPart.SetTrigger(forwardInputTorsoCommand);
            }

            if (hasHeadCommand)
            {
                monsterPartRef.connectedMonsterPart.SetTrigger(forwardInputHeadCommand);
            }
        }
        else if (monsterPartRef.attackAnimationID == -1)
        {
            if (hasTorsoCommand)
            {
                monsterPartRef.connectedMonsterPart.SetTrigger(backwardInputTorsoCommand);
            }

            if (hasHeadCommand)
            {
                monsterPartRef.connectedMonsterPart.SetTrigger(backwardInputHeadCommand);
            }
        }
        else if (monsterPartRef.attackAnimationID == 0)
        {
            if (hasTorsoCommand)
            {
                monsterPartRef.connectedMonsterPart.SetTrigger(downwardInputTorsoCommand);
            }

            if (hasHeadCommand)
            {
                monsterPartRef.connectedMonsterPart.SetTrigger(downwardInputHeadCommand);
            }
        }
        else if (monsterPartRef.attackAnimationID == 2)
        {
            if (hasTorsoCommand)
            {
                monsterPartRef.connectedMonsterPart.SetTrigger(upwardInputTorsoCommand);
            }

            if (hasHeadCommand)
            {
                monsterPartRef.connectedMonsterPart.SetTrigger(upwardInputHeadCommand);
            }
        }
    }

    public void triggerFullHeavyEffect()
    {
        //check for some damage upgrade 
        monsterPartRef.myMainSystem.releaseFullHeavyVisual();
    }

    public void triggerAttackRelease()
    {

        //Debug.Log("Trigger Attack Release called");

        if (monsterPartRef.isJointed)
        {
            // not sure about this whole section, it looks important
            monsterPartRef.connectedMonsterPart.SetBool("Ready to Swing", true);
            monsterPartRef.connectedMonsterPart.SetBool("Walking", false);
            monsterPartRef.connectedMonsterPart.SetBool("Running", false);

            if (monsterPartRef.hasTorsoCommandOverride)
            {
                monsterPartRef.mainTorso.SetBool("Ready to Swing", true);
                monsterPartRef.mainTorso.ResetTrigger("Quick Forward Position");
                monsterPartRef.mainTorso.ResetTrigger("Quick Upward Position");
                monsterPartRef.mainTorso.SetBool("Walking", false);
                monsterPartRef.mainTorso.SetBool("Running", false);
            }

            monsterPartRef.isRunning = false;
            monsterPartRef.fullActiveHeavy = true;
            monsterPartRef.isAttacking = false;
            monsterPartRef.myMainSystem.correctWalkingAttackAnimations();
            GetComponent<MonsterPartVisual>().triggerEndChargeVisual();

            if (monsterPartRef.attackMarkedHeavy)
            {
                monsterPartRef.heavyAttack.PassDamage();
                triggerHeavyChargeVisual();

                if (hasHeavyMovementCommand)
                {
                    if (monsterPartRef.attackAnimationID == 1)
                    {
                        if (forwardHeavyMovementCommand == "Forward Leap") //arm swings, leg kicks, etc.
                        {
                            //myMainSystem.leapAttackForward();
                        }
                        else if (forwardHeavyMovementCommand == "Forward Spin") //tail spin attacks
                        {

                        }
                        else if (forwardHeavyMovementCommand == "Quick 360 Heavy") //tail spin attacks - alt
                        {

                        }
                    }
                    else if (monsterPartRef.attackAnimationID == 2)
                    {
                        if (upwardHeavyMovementCommand == "Upward Leap") //arm swings, etc.
                        {
                            //myMainSystem.leapAttackUpward();
                        }
                        else if (upwardHeavyMovementCommand == "Upward Spin") //tail spin attacks, spinjitsu leg kick
                        {
                            //myMainSystem.rollingUpwardsAttack();
                        }
                    }
                    else if (monsterPartRef.attackAnimationID == -1)
                    {
                        if (backwardHeavyMovementCommand == "Backward Leap") //leg kicks, recoil etc.
                        {
                            //myMainSystem.leapAttackBackward();
                        }
                        else if (backwardHeavyMovementCommand == "Backward Spin") //tail spin
                        {

                        }
                        else if (backwardHeavyMovementCommand == "Quick 180 Heavy") //surprise turn and attack
                        {

                        }
                    }
                    else if (monsterPartRef.attackAnimationID == 0)
                    {
                        if (downwardHeavyMovementCommand == "Downward Leap") //arm swings, etc.
                        {
                            //myMainSystem.leapAttackDownward();
                        }
                        else if (downwardHeavyMovementCommand == "Downward Spin") //tail spin attacks
                        {
                            //myMainSystem.rollingDownwardsAttack();
                        }
                        else if (downwardHeavyMovementCommand == "Heavy Stomp") //leg kick
                        {
                            //myMainSystem.stompAttack();
                        }
                    }
                }

                monsterPartRef.heavyAttack.TriggerAttackRelease();
            }
            else
            {
                monsterPartRef.neutralAttack.PassDamage();

                if (hasNeutralMovementCommand)
                {
                    if (monsterPartRef.attackAnimationID == 1)
                    {
                        if (forwardNeutralMovementCommand == "Forward Strike") //arm swings, leg kicks, etc.
                        {
                            //myMainSystem.smallLeapAttackForward();
                        }
                        else if (forwardNeutralMovementCommand == "Forward Single Spin") //tail spin attacks, spinjitsu leg kick
                        {

                        }
                        else if (forwardNeutralMovementCommand == "Quick 360") //tail spin attacks
                        {

                        }
                    }
                    else if (monsterPartRef.attackAnimationID == 2)
                    {
                        if (upwardNeutralMovementCommand == "Upward Strike") //arm swings, etc.
                        {
                            //myMainSystem.smallLeapAttackUpward();
                        }
                        else if (upwardNeutralMovementCommand == "Upward Single Spin") //tail spin attacks, spinjitsu leg kick
                        {

                        }
                    }
                    else if (monsterPartRef.attackAnimationID == -1)
                    {
                        if (backwardNeutralMovementCommand == "Backward Strike") //leg kicks, recoil etc.
                        {
                            //myMainSystem.smallLeapAttackBackward();
                        }
                        else if (backwardNeutralMovementCommand == "Backward Single Spin") //tail spin
                        {

                        }
                        else if (backwardNeutralMovementCommand == "Quick 180") //surprise turn and attack
                        {

                        }
                    }
                    else if (monsterPartRef.attackAnimationID == 0)
                    {
                        if (downwardNeutralMovementCommand == "Downward Strike") //arm swings, etc.
                        {
                            //myMainSystem.smallLeapAttackDownward();
                        }
                        else if (downwardNeutralMovementCommand == "Downward Single Spin") //tail spin attacks
                        {

                        }
                        else if (downwardNeutralMovementCommand == "Stomp") //leg kick
                        {
                            //myMainSystem.stompAttack();
                        }
                    }

                }

                monsterPartRef.neutralAttack.TriggerAttackRelease();
            }
        }
    }

    public void triggerAttackToIdle()
    {
        monsterPartRef.connectedMonsterPart.SetBool("Attack to Idle", true);
        monsterPartRef.connectedMonsterPart.SetBool("Ready to Swing", false);

        if (monsterPartRef.hasTorsoCommandOverride)
        {
            monsterPartRef.mainTorso.SetBool("Attack to Idle", true);
            monsterPartRef.mainTorso.SetBool("Ready to Swing", false);
        }

        monsterPartRef.myMainSystem.endBracing();
        endRemainingVFX();

    }
    #endregion
    #region Emote Events
    public void triggerEmoteEnd()
    {
        if (monsterPartRef.connected == false)
        {
            return;
        }

        if (monsterPartRef.PartType is MonsterPartType.Torso)
        {
            monsterPartRef.myMainSystem.emoteEnded();
        }
    }
    #endregion
    #endregion
    #region Animations
    public bool CheckAnimState(params string[] states)
    {
        foreach (string state in states)
        {
            if (monsterPartRef.myAnimator.GetCurrentAnimatorStateInfo(0).IsName(state))
            {
                return true;
            }
        }
        return false;
    }

    public void endRunVisual()
    {
        if (specialRunVisual != null)
        {
            specialRunVisual.SetActive(false);
        }
    }

    public void correctRollSpamControl()
    {
        monsterPartRef.myMainSystem.correctRollControl();
    }

    public void attackCalculations()
    {
        MonsterCalculations calc = AttackCalculationsSetUp(); //does this in a separate script, it's too beefy :/

        monsterPartRef.requiresBackwardStance = calc.requiresBackwardStance;
        monsterPartRef.requiresForwardStance = calc.requiresForwardStance;
        monsterPartRef.requiresRightStance = calc.requiresRightStance;
        monsterPartRef.requiresLeftStance = calc.requiresLeftStance;

        hasTorsoCommand = calc.hasTorsoCommand;
        forwardInputTorsoCommand = calc.forwardInputTorsoCommand;
        backwardInputTorsoCommand = calc.backwardInputTorsoCommand; //make sure monster flips before attacking
        upwardInputTorsoCommand = calc.upwardInputTorsoCommand;
        downwardInputTorsoCommand = calc.downwardInputTorsoCommand;

        hasHeadCommand = calc.hasHeadCommand;
        forwardInputHeadCommand = calc.forwardInputHeadCommand;
        backwardInputHeadCommand = calc.backwardInputHeadCommand;
        upwardInputHeadCommand = calc.upwardInputHeadCommand;
        downwardInputHeadCommand = calc.downwardInputHeadCommand;

        hasNeutralMovementCommand = calc.hasNeutralMovementCommand;
        forwardNeutralMovementCommand = calc.forwardNeutralMovementCommand;
        upwardNeutralMovementCommand = calc.upwardNeutralMovementCommand;
        backwardNeutralMovementCommand = calc.backwardNeutralMovementCommand;
        downwardNeutralMovementCommand = calc.downwardNeutralMovementCommand;

        hasHeavyMovementCommand = calc.hasHeavyMovementCommand;
        forwardHeavyMovementCommand = calc.forwardHeavyMovementCommand;
        upwardHeavyMovementCommand = calc.upwardHeavyMovementCommand;
        backwardHeavyMovementCommand = calc.backwardHeavyMovementCommand;
        downwardHeavyMovementCommand = calc.downwardHeavyMovementCommand;
    }

    private MonsterCalculations AttackCalculationsSetUp()
    {
        MonsterCalculations calc = new MonsterCalculations();

        calc.AttackCalculationSetUp(monsterPartRef);

        return calc;
    }

    #region Movement
    public void triggerWalk()
    {

        if (monsterPartRef.connected == false)
        {
            return;
        }

        if (monsterPartRef.isGroundedLimb)
        {
            monsterPartRef.myAnimator.ResetTrigger("Walk to Idle");
            monsterPartRef.myAnimator.SetBool("Walking", true);
            monsterPartRef.myAnimator.SetTrigger("Walk");
            monsterPartRef.myAnimator.SetBool("Running", false);
            monsterPartRef.isWalking = true;
            monsterPartRef.isRunning = false;

            if (monsterPartRef.PartType is MonsterPartType.Leg)
            {
                monsterPartRef.myAnimator.SetBool("Calm", false);
                monsterPartRef.myAnimator.SetBool("Teeter", false);
            }
        }

        if (monsterPartRef.PartType is MonsterPartType.Torso)
        {
            monsterPartRef.myAnimator.ResetTrigger("Walk to Idle");
            monsterPartRef.myAnimator.SetBool("Walking", true);
            monsterPartRef.myAnimator.SetBool("Running", false);
            monsterPartRef.isWalking = true;
            monsterPartRef.isRunning = false;

            if (monsterPartRef.PartType is MonsterPartType.Torso)
            {
                monsterPartRef.myAnimator.SetBool("Teeter", false);
            }
        }
        else if (monsterPartRef.PartType is MonsterPartType.Head or MonsterPartType.Wing or MonsterPartType.Tail or MonsterPartType.Arm)
        {
            monsterPartRef.myAnimator.SetBool("Walking", true);
            monsterPartRef.myAnimator.SetBool("Running", false);
            monsterPartRef.isWalking = true;
            monsterPartRef.isRunning = false;

            if (monsterPartRef.PartType is MonsterPartType.Arm)
            {
                monsterPartRef.myAnimator.SetBool("Swaying", false);
            }
        }

        endRunVisual();
    }
    public void triggerStopWalking()
    {
        if (monsterPartRef.connected == false)
        {
            return;
        }

        if (monsterPartRef.isGroundedLimb || monsterPartRef.PartType is MonsterPartType.Torso)
        {
            monsterPartRef.myAnimator.ResetTrigger("Walk to Idle");

            if (monsterPartRef.isWalking)
            {
                monsterPartRef.myAnimator.SetBool("Walking", false);
                monsterPartRef.myAnimator.SetTrigger("Walk to Idle");
                monsterPartRef.isWalking = false;
            }
        }
        else if (monsterPartRef.PartType is MonsterPartType.Head or MonsterPartType.Wing or MonsterPartType.Tail or MonsterPartType.Arm)
        {
            monsterPartRef.myAnimator.SetBool("Walking", false);
            monsterPartRef.isWalking = false;

            if (monsterPartRef.PartType is MonsterPartType.Arm)
            {
                monsterPartRef.myAnimator.SetBool("Swaying", false);
            }
        }
    }
    public void triggerRun()
    {
        if (monsterPartRef.connected == false)
        {
            return;
        }

        if (monsterPartRef.isGroundedLimb)
        {
            monsterPartRef.myAnimator.ResetTrigger("Walk to Idle");
            monsterPartRef.myAnimator.SetBool("Running", true);
            monsterPartRef.myAnimator.SetTrigger("Run");
            monsterPartRef.myAnimator.SetBool("Walking", false);
            monsterPartRef.isWalking = false;
            monsterPartRef.isRunning = true;

            if (monsterPartRef.PartType is MonsterPartType.Leg)
            {
                monsterPartRef.myAnimator.SetBool("Calm", false);
            }
        }

        if (monsterPartRef.PartType is MonsterPartType.Torso)
        {
            monsterPartRef.myAnimator.ResetTrigger("Walk to Idle");
            monsterPartRef.myAnimator.SetBool("Running", true);
            monsterPartRef.myAnimator.SetBool("Walking", false);
            monsterPartRef.isWalking = false;
            monsterPartRef.isRunning = true;
        }

        if (monsterPartRef.PartType is MonsterPartType.Head or MonsterPartType.Wing or MonsterPartType.Arm or MonsterPartType.Tail)
        {
            monsterPartRef.myAnimator.SetBool("Running", true);
            monsterPartRef.myAnimator.SetBool("Walking", false);
            monsterPartRef.isWalking = false;
            monsterPartRef.isRunning = true;

            if (monsterPartRef.PartType is MonsterPartType.Arm)
            {
                monsterPartRef.myAnimator.SetBool("Swaying", false);
            }
        }
    }
    public void triggerStopRunning()
    {
        if (monsterPartRef.connected == false)
        {
            return;
        }

        if (monsterPartRef.isGroundedLimb || monsterPartRef.PartType is MonsterPartType.Torso)
        {
            if (monsterPartRef.isRunning)
            {
                monsterPartRef.myAnimator.ResetTrigger("Walk to Idle");
                monsterPartRef.myAnimator.SetBool("Running", false);
                monsterPartRef.isRunning = false;
            }
        }

        if (monsterPartRef.PartType is MonsterPartType.Head or MonsterPartType.Wing or MonsterPartType.Arm or MonsterPartType.Tail)
        {
            monsterPartRef.myAnimator.SetBool("Running", false);
            monsterPartRef.isRunning = false;

            if (monsterPartRef.PartType is MonsterPartType.Arm)
            {
                monsterPartRef.myAnimator.SetBool("Swaying", false);
            }
        }

        endRunVisual();
    }
    public void triggerScreechingStop()
    {
        if (monsterPartRef.connected == false)
        {
            return;
        }

        if (monsterPartRef.isGroundedLimb || monsterPartRef.PartType is MonsterPartType.Torso)
        {
            monsterPartRef.myAnimator.SetTrigger("Run to Screech");
        }
    }
    public void triggerJump()
    {
        if (monsterPartRef.connected == false || monsterPartRef.PartType is MonsterPartType.Decor || monsterPartRef.attackFocusOn || (monsterPartRef.PartType is MonsterPartType.Torso && monsterPartRef.isBracing))
        {
            return;
        }

        if (monsterPartRef.myAnimator != null)
        {
            monsterPartRef.myAnimator.SetBool("Grounded", false);
            monsterPartRef.myAnimator.SetTrigger("Jump");
        }

        if (monsterPartRef.isGroundedLimb || monsterPartRef.PartType is MonsterPartType.Arm)
        {
            if (monsterPartRef.PartType is MonsterPartType.Leg)
            {
                monsterPartRef.myAnimator.SetBool("Calm", false);
            }
        }

        if (monsterPartRef.PartType is MonsterPartType.Head or MonsterPartType.Wing or MonsterPartType.Arm or MonsterPartType.Tail or MonsterPartType.Tail)
        {
            monsterPartRef.myAnimator.SetBool("Glide Activated", false);
            monsterPartRef.myAnimator.SetBool("Walking", false);
            monsterPartRef.isWalking = false;

            if (monsterPartRef.PartType is MonsterPartType.Arm)
            {
                monsterPartRef.myAnimator.SetBool("Swaying", false);
            }
        }

        monsterPartRef.grounded = false;
        endRunVisual();
    }
    public void triggerRoll(bool groundedWhenTriggered, bool trueRoll)
    {
        if (monsterPartRef.connected == false || monsterPartRef.PartType is MonsterPartType.Decor || monsterPartRef.PartType is MonsterPartType.Horn)
        {
            return;
        }

        if (monsterPartRef.PartType is MonsterPartType.Horn && monsterPartRef.myAnimator != null)
        {
            monsterPartRef.myAnimator.SetBool("Grounded", groundedWhenTriggered);
            monsterPartRef.grounded = groundedWhenTriggered;
        }

        if (monsterPartRef.myAnimator != null)
        {
            monsterPartRef.myAnimator.SetBool("Grounded", groundedWhenTriggered);
            monsterPartRef.myAnimator.SetTrigger("Roll");
        }

        if (monsterPartRef.isGroundedLimb || monsterPartRef.PartType is MonsterPartType.Head or MonsterPartType.Wing or MonsterPartType.Tail or MonsterPartType.Torso)
        {
            if (trueRoll)
            {
                monsterPartRef.myAnimator.SetBool("Walking", false);
                monsterPartRef.myAnimator.SetBool("Running", false);
                monsterPartRef.isWalking = false;
                monsterPartRef.isRunning = false;
            }
            else
            {
                monsterPartRef.myAnimator.SetBool("Walking", false);
                monsterPartRef.isWalking = false;
            }

            if (monsterPartRef.PartType is MonsterPartType.Wing or MonsterPartType.Head)
            {
                monsterPartRef.myAnimator.SetBool("Glide Activated", false);
            }

            if (monsterPartRef.PartType is MonsterPartType.Leg)
            {
                monsterPartRef.myAnimator.SetBool("Calm", false);
            }

            if (monsterPartRef.PartType is MonsterPartType.Torso)
            {
                monsterPartRef.myAnimator.SetBool("Glide Activated", false);
            }
        }

        if (monsterPartRef.PartType is MonsterPartType.Arm)
        {
            monsterPartRef.myAnimator.SetBool("Glide Activated", false);
            monsterPartRef.myAnimator.SetBool("Swaying", false);

            if (trueRoll)
            {
                monsterPartRef.myAnimator.SetBool("Running", false);
                monsterPartRef.isWalking = false;
                monsterPartRef.isRunning = false;
            }
            else
            {
                monsterPartRef.isWalking = false;
            }
        }

        monsterPartRef.grounded = groundedWhenTriggered;
        monsterPartRef.stopInfiniteRoll();
        endRunVisual();
    }
    public void triggerWingFlap()
    {
        if (monsterPartRef.connected == false)
        {
            return;
        }

        if (monsterPartRef.PartType is MonsterPartType.Torso)
        {
            monsterPartRef.myAnimator.SetTrigger("Upper Flap"); //change this so that its calculated at start with the other animations
            //allows us to use something like "lower flap" for wings on the butt
        }

        if (monsterPartRef.PartType is MonsterPartType.Wing)
        {
            monsterPartRef.myAnimator.SetTrigger("Big Flap");
        }

        if (monsterPartRef.PartType is MonsterPartType.Arm)
        {
            monsterPartRef.myAnimator.SetTrigger("Roll");
        }

        if (monsterPartRef.PartType is MonsterPartType.Leg or MonsterPartType.Head or MonsterPartType.Tail)
        {
            monsterPartRef.myAnimator.SetTrigger("Jump");
        }

        if ((monsterPartRef.PartType is MonsterPartType.Mouth or MonsterPartType.Eye) && monsterPartRef.myAnimator != null)
        {
            if (monsterPartRef.PartType is MonsterPartType.Mouth)
            {
                monsterPartRef.myAnimator.SetTrigger("Roll");
            }
            else
            {
                monsterPartRef.myAnimator.SetTrigger("Brace");
            }
        }
    }
    public void triggerFall()
    {
        if (monsterPartRef.connected == false ||monsterPartRef.PartType is MonsterPartType.Decor or MonsterPartType.Horn)
        {
            return;
        }

        if (monsterPartRef.myAnimator != null)
        {
            monsterPartRef.myAnimator.SetBool("Grounded", false);
            monsterPartRef.myAnimator.SetTrigger("Fall");
        }

        if (monsterPartRef.isGroundedLimb || monsterPartRef.PartType is MonsterPartType.Torso)
        {
            if (monsterPartRef.PartType is MonsterPartType.Leg)
            {
                monsterPartRef.myAnimator.SetBool("Calm", false);
            }
        }

        if (monsterPartRef.PartType is MonsterPartType.Head or MonsterPartType.Wing or MonsterPartType.Arm or MonsterPartType.Tail or MonsterPartType.Torso)
        {
            if (monsterPartRef.PartType is MonsterPartType.Arm)
            {
                monsterPartRef.myAnimator.SetBool("Swaying", false);
            }
        }

        monsterPartRef.grounded = false;
        endRunVisual();
    }
    public void triggerSimpleUngrounded()
    {
        if (monsterPartRef.connected == false || monsterPartRef.PartType is MonsterPartType.Decor || monsterPartRef.PartType is MonsterPartType.Horn)
        {
            return;
        }

        if (monsterPartRef.myAnimator != null)
        {
            monsterPartRef.myAnimator.SetBool("Grounded", false);
        }

        if (monsterPartRef.isGroundedLimb || monsterPartRef.PartType is MonsterPartType.Torso)
        {
            if (monsterPartRef.PartType is MonsterPartType.Leg)
            {
                monsterPartRef.myAnimator.SetBool("Calm", false);
            }
        }

        if (monsterPartRef.PartType is MonsterPartType.Head or MonsterPartType.Wing or MonsterPartType.Arm or MonsterPartType.Tail or MonsterPartType.Torso)
        {
            if (monsterPartRef.PartType is MonsterPartType.Arm)
            {
                monsterPartRef.myAnimator.SetBool("Swaying", false);
            }
        }

        monsterPartRef.grounded = false;
        endRunVisual();
    }
    public void triggerLand()
    {
        monsterPartRef.attackFocusOn = false;

        if (monsterPartRef.connected == false ||monsterPartRef.PartType is MonsterPartType.Decor or MonsterPartType.Horn)
        {
            return;
        }

        if (monsterPartRef.PartType is MonsterPartType.Horn && monsterPartRef.myAnimator != null)
        {
            monsterPartRef.myAnimator.SetBool("Grounded", true);
            monsterPartRef.grounded = true;
        }

        if (monsterPartRef.myAnimator != null)
        {
            monsterPartRef.myAnimator.SetBool("Grounded", true);

            if (monsterPartRef.isGroundedLimb || monsterPartRef.PartType is MonsterPartType.Torso or MonsterPartType.Head or MonsterPartType.Arm or MonsterPartType.Leg)
            {
                monsterPartRef.myAnimator.SetTrigger("Land");
            }
        }

        if (monsterPartRef.isGroundedLimb || monsterPartRef.PartType is MonsterPartType.Torso or MonsterPartType.Head or MonsterPartType.Wing or MonsterPartType.Tail)
        {
            if (monsterPartRef.PartType is MonsterPartType.Wing or MonsterPartType.Head)
            {
                monsterPartRef.myAnimator.SetBool("Glide Activated", false);
            }

            if (monsterPartRef.PartType is MonsterPartType.Leg)
            {
                monsterPartRef.myAnimator.SetBool("Calm", false);
            }

            if (monsterPartRef.PartType is MonsterPartType.Torso)
            {
                monsterPartRef.myAnimator.SetBool("Glide Activated", false);
            }
        }

        if (monsterPartRef.PartType is MonsterPartType.Arm)
        {
            monsterPartRef.myAnimator.SetBool("Glide Activated", false);
            monsterPartRef.myAnimator.SetBool("Swaying", false);
        }

        monsterPartRef.grounded = true;
        monsterPartRef.stopInfiniteRoll();
        endRunVisual();
    }
    public void triggerLateLand()
    {
        if (monsterPartRef.connected == false || monsterPartRef.PartType is MonsterPartType.Decor or MonsterPartType.Horn or MonsterPartType.Torso or MonsterPartType.Head || monsterPartRef.isAttacking)
        {
            if (monsterPartRef.isAttacking || monsterPartRef.PartType is MonsterPartType.Torso)
            {
                monsterPartRef.myAnimator.SetBool("Grounded", true);
            }
            return;
        }

        if (monsterPartRef.PartType is MonsterPartType.Horn && monsterPartRef.myAnimator != null)
        {
            monsterPartRef.myAnimator.SetBool("Grounded", true);
            monsterPartRef.grounded = true;
        }

        if (monsterPartRef.myAnimator != null)
        {
            monsterPartRef.myAnimator.SetBool("Grounded", true);

            if (monsterPartRef.isGroundedLimb || monsterPartRef.PartType is MonsterPartType.Head or MonsterPartType.Arm or MonsterPartType.Leg)
            {
                monsterPartRef.myAnimator.SetTrigger("Land");
            }
        }

        if (monsterPartRef.isGroundedLimb || monsterPartRef.PartType is MonsterPartType.Torso or MonsterPartType.Head or MonsterPartType.Wing or MonsterPartType.Tail)
        {
            if (monsterPartRef.PartType is MonsterPartType.Wing or MonsterPartType.Head)
            {
                monsterPartRef.myAnimator.SetBool("Glide Activated", false);
            }

            if (monsterPartRef.PartType is MonsterPartType.Leg)
            {
                monsterPartRef.myAnimator.SetBool("Calm", false);
            }

            if (monsterPartRef.PartType is MonsterPartType.Torso)
            {
                monsterPartRef.myAnimator.SetBool("Glide Activated", false);
            }
        }

        if (monsterPartRef.PartType is MonsterPartType.Arm)
        {
            monsterPartRef.myAnimator.SetBool("Glide Activated", false);
            monsterPartRef.myAnimator.SetBool("Swaying", false);
        }

        monsterPartRef.grounded = true;
        monsterPartRef.stopInfiniteRoll();
        endRunVisual();
    }
    public void triggerGlide()
    {
        if (monsterPartRef.connected == false)
        {
            return;
        }

        if (monsterPartRef.PartType is MonsterPartType.Wing or MonsterPartType.Head or MonsterPartType.Arm or MonsterPartType.Tail or MonsterPartType.Torso)
        {
            monsterPartRef.myAnimator.SetBool("Glide Activated", true);
        }

        monsterPartRef.stopInfiniteRoll();
    }
    public void triggerCrouch()
    {
        if (monsterPartRef.connected == false || monsterPartRef.PartType is MonsterPartType.Horn or MonsterPartType.Decor or MonsterPartType.Eye or MonsterPartType.Mouth)
        {
            return;
        }

        monsterPartRef.isWalking = false;
        monsterPartRef.isRunning = false;

        if (monsterPartRef.PartType is MonsterPartType.Torso)
        {
            monsterPartRef.myAnimator.SetBool("Crouching", true);
            monsterPartRef.myAnimator.SetTrigger("Crouch");
            monsterPartRef.myAnimator.SetBool("Walking", false);
            monsterPartRef.myAnimator.SetBool("Running", false);
        }

        if (monsterPartRef.isGroundedLimb)
        {
            monsterPartRef.myAnimator.SetBool("Crouching", true);
            monsterPartRef.myAnimator.SetTrigger("Crouch");
            monsterPartRef.myAnimator.SetBool("Calm", false);
            monsterPartRef.myAnimator.SetBool("Walking", false);
            monsterPartRef.myAnimator.SetBool("Running", false);
        }

        if (monsterPartRef.PartType is MonsterPartType.Arm)
        {
            monsterPartRef.myAnimator.SetBool("Crouching", true);
            monsterPartRef.myAnimator.SetTrigger("Crouch");
            monsterPartRef.myAnimator.SetBool("Swaying", false);
            monsterPartRef.myAnimator.SetBool("Walking", false);
            monsterPartRef.myAnimator.SetBool("Running", false);
        }

        if (monsterPartRef.PartType is MonsterPartType.Tail)
        {
            monsterPartRef.myAnimator.SetBool("Crouching", true);
            monsterPartRef.myAnimator.SetTrigger("Crouch");
            monsterPartRef.myAnimator.SetBool("Walking", false);
            monsterPartRef.myAnimator.SetBool("Running", false);
        }

        if (monsterPartRef.PartType is MonsterPartType.Wing or MonsterPartType.Head)
        {
            monsterPartRef.myAnimator.SetBool("Walking", false);
            monsterPartRef.myAnimator.SetBool("Running", false);
        }

        endRunVisual();
    }
    public void triggerCrouchStop()
    {
        if (monsterPartRef.connected == false || monsterPartRef.PartType is MonsterPartType.Horn or MonsterPartType.Decor or MonsterPartType.Eye or MonsterPartType.Mouth)
        {
            return;
        }

        if (monsterPartRef.isGroundedLimb || monsterPartRef.PartType is MonsterPartType.Torso or MonsterPartType.Arm or MonsterPartType.Tail)
        {
            monsterPartRef.myAnimator.SetBool("Crouching", false);
        }
    }
    public void triggerForceFall()
    {
        if (monsterPartRef.connected == false || monsterPartRef.PartType is MonsterPartType.Horn or MonsterPartType.Decor or MonsterPartType.Eye or MonsterPartType.Mouth || monsterPartRef.grounded)
        {
            return;
        }

        if (monsterPartRef.PartType is MonsterPartType.Torso)
        {
            monsterPartRef.myAnimator.SetBool("Force Falling", true);
        }

        if (monsterPartRef.PartType is MonsterPartType.Arm)
        {
            monsterPartRef.myAnimator.SetBool("Force Falling", true);
        }

        if (monsterPartRef.PartType is MonsterPartType.Tail)
        {
            monsterPartRef.myAnimator.SetBool("Force Falling", true);
        }

        endRunVisual();
    }
    public void triggerForceFallStop()
    {
        if (monsterPartRef.connected == false || monsterPartRef.PartType is MonsterPartType.Horn or MonsterPartType.Decor or MonsterPartType.Eye or MonsterPartType.Mouth)
        {
            return;
        }

        if (monsterPartRef.PartType is MonsterPartType.Torso or MonsterPartType.Arm or MonsterPartType.Tail)
        {
            monsterPartRef.myAnimator.SetBool("Force Falling", false);
        }
    }
    #endregion
    #region Attack Bracing
    public void triggerLeftAttackStance()
    {
        if (monsterPartRef.connected == false || monsterPartRef.attackFocusOn)
        {
            return;
        }

        monsterPartRef.isBracing = true;

        if (monsterPartRef.isGroundedLimb)
        {
            if (monsterPartRef.grounded)
            {
                if (monsterPartRef.isRightSidedLimb && monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                    monsterPartRef.myAnimator.SetTrigger("Backward Brace");
                }
                else if (monsterPartRef.isLeftSidedLimb && monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                    monsterPartRef.myAnimator.SetTrigger("Forward Brace");
                }

                monsterPartRef.myAnimator.SetBool("Walking", false);
                monsterPartRef.myAnimator.SetBool("Running", false);
            }
            else
            {
                if (monsterPartRef.isRightSidedLimb && monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                    monsterPartRef.myAnimator.SetTrigger("Airborn Brace");
                }
                else if (monsterPartRef.isLeftSidedLimb && monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                    monsterPartRef.myAnimator.SetTrigger("Airborn Brace");
                }
            }

        }
        else if (monsterPartRef.PartType is MonsterPartType.Leg)
        {
            if (monsterPartRef.isRightSidedLimb && monsterPartRef.isAttacking == false)
            {
                monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                monsterPartRef.myAnimator.SetTrigger("Airborn Brace");
            }
            else if (monsterPartRef.isLeftSidedLimb && monsterPartRef.isAttacking == false)
            {
                monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                monsterPartRef.myAnimator.SetTrigger("Airborn Brace");
            }
        }
        else if (monsterPartRef.PartType is MonsterPartType.Arm)
        {
            if (CheckAnimState("Idle", "Fall", "Land"))
            {
                if (monsterPartRef.isRightSidedLimb && monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.SetTrigger("Brace");
                }
                else if (monsterPartRef.isLeftSidedLimb && monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.SetTrigger("Brace");
                }
            }
        }
        else if (monsterPartRef.PartType is MonsterPartType.Mouth && monsterPartRef.myAnimator != null)
        {
            if (CheckAnimState("Idle", "Fall", "Land"))
            {
                if (monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.SetTrigger("Brace");
                }
            }
        }
        else if (monsterPartRef.PartType is MonsterPartType.Wing)
        {
            if (CheckAnimState("Idle Fly", "Idle Grounded", "Fall", "Land", "Glide", "Running"))
            {
                if (monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                    monsterPartRef.myAnimator.SetTrigger("Brace");
                    monsterPartRef.myAnimator.SetBool("Walking", false);
                    monsterPartRef.myAnimator.SetBool("Running", false);
                }
            }
        }
        else if (monsterPartRef.PartType is MonsterPartType.Tail)
        {
            if (CheckAnimState("Idle", "Fall", "Land", "Glide"))
            {
                if (monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                    monsterPartRef.myAnimator.SetTrigger("Brace");
                    monsterPartRef.myAnimator.SetBool("Walking", false);
                    monsterPartRef.myAnimator.SetBool("Running", false);
                }
            }
        }
    }
    public void triggerRightAttackStance()
    {
        if (monsterPartRef.connected == false || monsterPartRef.attackFocusOn)
        {
            return;
        }

        monsterPartRef.isBracing = true;

        if (monsterPartRef.isGroundedLimb)
        {
            if (monsterPartRef.grounded)
            {
                if (monsterPartRef.isLeftSidedLimb && monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                    monsterPartRef.myAnimator.SetTrigger("Backward Brace");
                }
                else if (monsterPartRef.isRightSidedLimb && monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                    monsterPartRef.myAnimator.SetTrigger("Forward Brace");
                }

                monsterPartRef.myAnimator.SetBool("Walking", false);
                monsterPartRef.myAnimator.SetBool("Running", false);
            }
            else
            {
                if (monsterPartRef.isRightSidedLimb && monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                    monsterPartRef.myAnimator.SetTrigger("Airborn Brace");
                }
                else if (monsterPartRef.isLeftSidedLimb && monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                    monsterPartRef.myAnimator.SetTrigger("Airborn Brace");
                }
            }

        }
        else if (monsterPartRef.PartType is MonsterPartType.Leg)
        {
            if (monsterPartRef.isRightSidedLimb && monsterPartRef.isAttacking == false)
            {
                monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                monsterPartRef.myAnimator.SetTrigger("Airborn Brace");
            }
            else if (monsterPartRef.isLeftSidedLimb && monsterPartRef.isAttacking == false)
            {
                monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                monsterPartRef.myAnimator.SetTrigger("Airborn Brace");
            }
        }
        else if (monsterPartRef.PartType is MonsterPartType.Arm)
        {
            if (CheckAnimState("Idle", "Fall", "Land"))
            {
                if (monsterPartRef.isLeftSidedLimb && monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.SetTrigger("Brace");
                }
                else if (monsterPartRef.isRightSidedLimb && monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.SetTrigger("Brace");
                }
            }
        }
        else if (monsterPartRef.PartType is MonsterPartType.Mouth && monsterPartRef.myAnimator != null)
        {
            if (CheckAnimState("Idle", "Fall", "Land"))
            {
                if (monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.SetTrigger("Brace");
                }
            }
        }
        else if (monsterPartRef.PartType is MonsterPartType.Wing)
        {
            if (CheckAnimState("Idle Fly", "Idle Grounded", "Fall", "Land", "Glide", "Running"))
            {
                if (monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                    monsterPartRef.myAnimator.SetTrigger("Brace");
                    monsterPartRef.myAnimator.SetBool("Walking", false);
                    monsterPartRef.myAnimator.SetBool("Running", false);
                }
            }
        }
        else if (monsterPartRef.PartType is MonsterPartType.Tail)
        {
            if (CheckAnimState("Idle", "Fall", "Land", "Glide"))
            {
                if (monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                    monsterPartRef.myAnimator.SetTrigger("Brace");
                    monsterPartRef.myAnimator.SetBool("Walking", false);
                    monsterPartRef.myAnimator.SetBool("Running", false);
                }
            }
        }
    }
    public void triggerForwardStance()
    {
        if (monsterPartRef.connected == false || monsterPartRef.attackFocusOn)
        {
            return;
        }

        monsterPartRef.isBracing = true;

        if (monsterPartRef.isGroundedLimb)
        {
            if (monsterPartRef.grounded)
            {
                if (monsterPartRef.isLeftSidedLimb && monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                    monsterPartRef.myAnimator.SetTrigger("Forward Brace");
                }
                else if (monsterPartRef.isRightSidedLimb && monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                    monsterPartRef.myAnimator.SetTrigger("Forward Brace");
                }

                monsterPartRef.myAnimator.SetBool("Walking", false);
                monsterPartRef.myAnimator.SetBool("Running", false);
            }
            else
            {
                if (monsterPartRef.isRightSidedLimb && monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                    monsterPartRef.myAnimator.SetTrigger("Airborn Brace");
                }
                else if (monsterPartRef.isLeftSidedLimb && monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                    monsterPartRef.myAnimator.SetTrigger("Airborn Brace");
                }
            }
        }
        else if (monsterPartRef.PartType is MonsterPartType.Leg)
        {
            if (monsterPartRef.isRightSidedLimb && monsterPartRef.isAttacking == false)
            {
                monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                monsterPartRef.myAnimator.SetTrigger("Airborn Brace");
            }
            else if (monsterPartRef.isLeftSidedLimb && monsterPartRef.isAttacking == false)
            {
                monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                monsterPartRef.myAnimator.SetTrigger("Airborn Brace");
            }
        }
        else if (monsterPartRef.PartType is MonsterPartType.Arm)
        {
            if (CheckAnimState("Idle", "Fall", "Land"))
            {
                if (monsterPartRef.isLeftSidedLimb && monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.SetTrigger("Brace");
                }
                else if (monsterPartRef.isRightSidedLimb && monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.SetTrigger("Brace");
                }
            }
        }
        else if (monsterPartRef.PartType is MonsterPartType.Mouth && monsterPartRef.myAnimator != null)
        {
            if (CheckAnimState("Idle", "Fall", "Land"))
            {
                if (monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.SetTrigger("Brace");
                }
            }
        }
        else if (monsterPartRef.PartType is MonsterPartType.Wing)
        {
            if (CheckAnimState("Idle Fly", "Idle Grounded", "Fall", "Land", "Glide", "Running"))
            {
                if (monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                    monsterPartRef.myAnimator.SetTrigger("Brace");
                    monsterPartRef.myAnimator.SetBool("Walking", false);
                    monsterPartRef.myAnimator.SetBool("Running", false);
                }
            }
        }
        else if (monsterPartRef.PartType is MonsterPartType.Tail)
        {
            if (CheckAnimState("Idle", "Fall", "Land", "Glide"))
            {
                if (monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                    monsterPartRef.myAnimator.SetTrigger("Brace");
                    monsterPartRef.myAnimator.SetBool("Walking", false);
                    monsterPartRef.myAnimator.SetBool("Running", false);
                }
            }
        }
    }
    public void triggerBackwardStance()
    {
        if (monsterPartRef.connected == false || monsterPartRef.attackFocusOn)
        {
            return;
        }

        monsterPartRef.isBracing = true;

        if (monsterPartRef.isGroundedLimb)
        {
            if (monsterPartRef.grounded)
            {
                if (monsterPartRef.isRightSidedLimb && monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                    monsterPartRef.myAnimator.SetTrigger("Launching Backward Brace");
                    monsterPartRef.myAnimator.SetBool("Needs Launch", true);
                }
                else if (monsterPartRef.isLeftSidedLimb && monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                    monsterPartRef.myAnimator.SetTrigger("Launching Backward Brace");
                    monsterPartRef.myAnimator.SetBool("Needs Launch", true);
                }

                monsterPartRef.myAnimator.SetBool("Walking", false);
                monsterPartRef.myAnimator.SetBool("Running", false);
            }
            else
            {
                if (monsterPartRef.isRightSidedLimb && monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                    monsterPartRef.myAnimator.SetTrigger("Airborn Brace");
                }
                else if (monsterPartRef.isLeftSidedLimb && monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                    monsterPartRef.myAnimator.SetTrigger("Airborn Brace");
                }
            }
        }
        else if (monsterPartRef.PartType is MonsterPartType.Leg)
        {
            if (monsterPartRef.isRightSidedLimb && monsterPartRef.isAttacking == false)
            {
                monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                monsterPartRef.myAnimator.SetTrigger("Airborn Brace");
            }
            else if (monsterPartRef.isLeftSidedLimb && monsterPartRef.isAttacking == false)
            {
                monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                monsterPartRef.myAnimator.SetTrigger("Airborn Brace");
            }
        }
        else if (monsterPartRef.PartType is MonsterPartType.Arm)
        {
            if (CheckAnimState("Idle", "Fall", "Land"))
            {
                if (monsterPartRef.isLeftSidedLimb && monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.SetTrigger("Brace");
                }
                else if (monsterPartRef.isRightSidedLimb && monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.SetTrigger("Brace");
                }
            }
        }
        else if (monsterPartRef.PartType is MonsterPartType.Mouth && monsterPartRef.myAnimator != null)
        {
            if (CheckAnimState("Idle", "Fall", "Land"))
            {
                if (monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.SetTrigger("Brace");
                }
            }
        }
        else if (monsterPartRef.PartType is MonsterPartType.Wing)
        {
            if (CheckAnimState("Idle Fly", "Idle Grounded", "Fall", "Land", "Glide", "Running"))
            {
                if (monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                    monsterPartRef.myAnimator.SetTrigger("Brace");
                    monsterPartRef.myAnimator.SetBool("Walking", false);
                    monsterPartRef.myAnimator.SetBool("Running", false);
                }
            }
        }
        else if (monsterPartRef.PartType is MonsterPartType.Tail)
        {
            if (CheckAnimState("Idle", "Fall", "Land", "Glide"))
            {
                if (monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                    monsterPartRef.myAnimator.SetTrigger("Brace");
                    monsterPartRef.myAnimator.SetBool("Walking", false);
                    monsterPartRef.myAnimator.SetBool("Running", false);
                }
            }
        }
    }
    public void triggerFlourishStance()
    {
        if (monsterPartRef.connected == false || monsterPartRef.attackFocusOn)
        {
            return;
        }


        monsterPartRef.isBracing = true;

        if (monsterPartRef.isGroundedLimb)
        {
            if (monsterPartRef.grounded)
            {
                if (monsterPartRef.isRightSidedLimb && monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                    monsterPartRef.myAnimator.SetTrigger("Flourish");
                    monsterPartRef.myAnimator.SetBool("Needs Launch", true);
                }
                else if (monsterPartRef.isLeftSidedLimb && monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                    monsterPartRef.myAnimator.SetTrigger("Flourish");
                    monsterPartRef.myAnimator.SetBool("Needs Launch", true);
                }

                monsterPartRef.myAnimator.SetBool("Walking", false);
                monsterPartRef.myAnimator.SetBool("Running", false);
            }
            else
            {
                if (monsterPartRef.isRightSidedLimb && monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                    monsterPartRef.myAnimator.SetTrigger("Airborn Brace");
                }
                else if (monsterPartRef.isLeftSidedLimb && monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                    monsterPartRef.myAnimator.SetTrigger("Airborn Brace");
                }
            }
        }
        else if (monsterPartRef.PartType is MonsterPartType.Leg)
        {
            if (monsterPartRef.isRightSidedLimb && monsterPartRef.isAttacking == false)
            {
                monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                monsterPartRef.myAnimator.SetTrigger("Airborn Brace");
            }
            else if (monsterPartRef.isLeftSidedLimb && monsterPartRef.isAttacking == false)
            {
                monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                monsterPartRef.myAnimator.SetTrigger("Airborn Brace");
            }
        }
        else if (monsterPartRef.PartType is MonsterPartType.Arm)
        {
            if (CheckAnimState("Idle", "Fall", "Land"))
            {
                if (monsterPartRef.isLeftSidedLimb && monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.SetTrigger("Brace");
                }
                else if (monsterPartRef.isRightSidedLimb && monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.SetTrigger("Brace");
                }
            }
        }
        else if (monsterPartRef.PartType is MonsterPartType.Mouth && monsterPartRef.myAnimator != null)
        {
            if (CheckAnimState("Idle", "Fall", "Land"))
            {
                if (monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.SetTrigger("Brace");
                }
            }
        }
        else if (monsterPartRef.PartType is MonsterPartType.Wing)
        {
            if (CheckAnimState("Idle Fly", "Idle Grounded", "Fall", "Land", "Glide", "Running"))
            {
                if (monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                    monsterPartRef.myAnimator.SetTrigger("Brace");
                    monsterPartRef.myAnimator.SetBool("Walking", false);
                    monsterPartRef.myAnimator.SetBool("Running", false);
                }
            }
        }
        else if (monsterPartRef.PartType is MonsterPartType.Tail)
        {
            if (CheckAnimState("Idle", "Fall", "Land", "Glide"))
            {
                if (monsterPartRef.isAttacking == false)
                {
                    monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                    monsterPartRef.myAnimator.SetTrigger("Brace");
                    monsterPartRef.myAnimator.SetBool("Walking", false);
                    monsterPartRef.myAnimator.SetBool("Running", false);
                }
            }
        }
    }
    public void triggerUnbrace()
    {
        if (monsterPartRef.connected == false)
        {
            return;
        }

        monsterPartRef.isBracing = false;

        if (monsterPartRef.isAttacking == false)
        {
            if (monsterPartRef.PartType is MonsterPartType.Leg)
            {
                monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                monsterPartRef.myAnimator.SetTrigger("Unbrace");
                monsterPartRef.myAnimator.ResetTrigger("Backward Brace");
                monsterPartRef.myAnimator.ResetTrigger("Forward Brace");
            }

            if (monsterPartRef.PartType is MonsterPartType.Arm or MonsterPartType.Wing or MonsterPartType.Tail)
            {
                monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                monsterPartRef.myAnimator.SetTrigger("Unbrace");
                monsterPartRef.myAnimator.ResetTrigger("Brace");

                if (monsterPartRef.PartType is MonsterPartType.Wing or MonsterPartType.Tail)
                {
                    monsterPartRef.myAnimator.SetBool("Glide Activated", false);
                }

                if (monsterPartRef.PartType is MonsterPartType.Arm)
                {
                    monsterPartRef.myAnimator.SetBool("Swaying", false);
                }
            }

            if (monsterPartRef.PartType is MonsterPartType.Mouth && monsterPartRef.myAnimator != null)
            {
                monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                monsterPartRef.myAnimator.SetTrigger("Unbrace");
                monsterPartRef.myAnimator.ResetTrigger("Brace");
            }

        }
        else
        {
            if (monsterPartRef.isGroundedLimb)
            {
                monsterPartRef.myAnimator.ResetTrigger("Unbrace");
                monsterPartRef.myAnimator.SetTrigger("Unbrace");
                monsterPartRef.myAnimator.ResetTrigger("Backward Brace");
                monsterPartRef.myAnimator.ResetTrigger("Forward Brace");
            }
        }
    }
    #endregion
    #endregion
    #region VFX
    public void setUpVFX()//new attack projectile-like types must be added here
    {
        monsterPartRef.neutralAttack.SetupVFX();
        monsterPartRef.heavyAttack.SetupVFX();

        #region Neutral Hit VFX Holder
        if (neutralHitVFXHolder != null)
        {
            if (neutralHitVFXHolder.GetComponent<vfxHolder>() != null)
            {
                neutralHitVFXManager = neutralHitVFXHolder.GetComponent<vfxHolder>();
            }

            neutralAttackHitVFXArray = new Transform[neutralHitVFXHolder.transform.childCount];
            for (int i = 0; i < neutralAttackHitVFXArray.Length; i++)
            {
                neutralAttackHitVFXArray[i] = neutralHitVFXHolder.transform.GetChild(i);
            }
        }
        #endregion

        #region Neutral Forward Swing VFX Holder
        if (neutralForwardSwingVFXHolder != null)
        {
            if (neutralForwardSwingVFXHolder.GetComponent<vfxHolder>() != null)
            {
                neutralForwardSwingVFXManager = neutralForwardSwingVFXHolder.GetComponent<vfxHolder>();
            }

            neutralAttackForwardSwingVFXArray = new Transform[neutralForwardSwingVFXHolder.transform.childCount];
            for (int i = 0; i < neutralAttackForwardSwingVFXArray.Length; i++)
            {
                neutralAttackForwardSwingVFXArray[i] = neutralForwardSwingVFXHolder.transform.GetChild(i);
            }
        }
        #endregion

        #region Neutral Backward Swing VFX Holder
        if (neutralBackwardSwingVFXHolder != null)
        {
            if (neutralBackwardSwingVFXHolder.GetComponent<vfxHolder>() != null)
            {
                neutralBackwardSwingVFXManager = neutralBackwardSwingVFXHolder.GetComponent<vfxHolder>();
            }

            neutralAttackBackwardSwingVFXArray = new Transform[neutralBackwardSwingVFXHolder.transform.childCount];
            for (int i = 0; i < neutralAttackBackwardSwingVFXArray.Length; i++)
            {
                neutralAttackBackwardSwingVFXArray[i] = neutralBackwardSwingVFXHolder.transform.GetChild(i);
            }
        }

        #endregion

        #region Neutral Downward Swing VFX Holder
        if (neutralDownwardSwingVFXHolder != null)
        {
            if (neutralDownwardSwingVFXHolder.GetComponent<vfxHolder>() != null)
            {
                neutralDownwardSwingVFXManager = neutralDownwardSwingVFXHolder.GetComponent<vfxHolder>();
            }

            neutralAttackDownwardSwingVFXArray = new Transform[neutralDownwardSwingVFXHolder.transform.childCount];
            for (int i = 0; i < neutralAttackDownwardSwingVFXArray.Length; i++)
            {
                neutralAttackDownwardSwingVFXArray[i] = neutralDownwardSwingVFXHolder.transform.GetChild(i);
            }
        }

        #endregion

        #region Neutral Miss VFX Holder
        if (neutralMissVFXHolder != null)
        {
            if (neutralMissVFXHolder.GetComponent<vfxHolder>() != null)
            {
                neutralMissVFXManager = neutralMissVFXHolder.GetComponent<vfxHolder>();
            }

            neutralAttackMissVFXArray = new Transform[neutralMissVFXHolder.transform.childCount];
            for (int i = 0; i < neutralAttackMissVFXArray.Length; i++)
            {
                neutralAttackMissVFXArray[i] = neutralMissVFXHolder.transform.GetChild(i);
            }
        }
        #endregion

        #region Neutral Default Spray Holder 
        //new sprayable attack types must be added here
        if (neutralDefaultSprayVFXHolder != null)
        {
            if (neutralDefaultSprayVFXHolder.GetComponent<vfxHolder>() != null)
            {
                neutralDefaultSprayVFXManager = neutralDefaultSprayVFXHolder.GetComponent<vfxHolder>();
            }

            neutralAttackDefaultVFXArray = new Transform[neutralDefaultSprayVFXHolder.transform.childCount];
            for (int i = 0; i < neutralAttackDefaultVFXArray.Length; i++)
            {
                neutralAttackDefaultVFXArray[i] = neutralDefaultSprayVFXHolder.transform.GetChild(i);
            }
        }
        #endregion

        #region Neutral Stomp VFX Holder
        if (neutralStompVFXHolder != null)
        {
            if (neutralStompVFXHolder.GetComponent<vfxHolder>() != null)
            {
                neutralStompVFXManager = neutralStompVFXHolder.GetComponent<vfxHolder>();
            }

            neutralStompVFXArray = new Transform[neutralStompVFXHolder.transform.childCount];
            for (int i = 0; i < neutralStompVFXArray.Length; i++)
            {
                neutralStompVFXArray[i] = neutralStompVFXHolder.transform.GetChild(i);
            }
        }

        #endregion


        #region Heavy Hit VFX Holder
        //new projectile-like attack types must be added here
        if (heavyHitVFXHolder != null)
        {
            if (heavyHitVFXHolder.GetComponent<vfxHolder>() != null)
            {
                heavyHitVFXManager = heavyHitVFXHolder.GetComponent<vfxHolder>();
            }

            heavyAttackHitVFXArray = new Transform[heavyHitVFXHolder.transform.childCount];
            for (int i = 0; i < heavyAttackHitVFXArray.Length; i++)
            {
                heavyAttackHitVFXArray[i] = heavyHitVFXHolder.transform.GetChild(i);
            }
        }
        #endregion

        #region Heavy Forward Swing VFX Holder
        if (heavyForwardSwingVFXHolder != null)
        {
            if (heavyForwardSwingVFXHolder.GetComponent<vfxHolder>() != null)
            {
                heavyForwardSwingVFXManager = heavyForwardSwingVFXHolder.GetComponent<vfxHolder>();
            }

            heavyAttackForwardSwingVFXArray = new Transform[heavyForwardSwingVFXHolder.transform.childCount];
            for (int i = 0; i < heavyAttackForwardSwingVFXArray.Length; i++)
            {
                heavyAttackForwardSwingVFXArray[i] = heavyForwardSwingVFXHolder.transform.GetChild(i);
            }
        }
        #endregion

        #region Heavy Backward Swing VFX Holder
        if (heavyBackwardSwingVFXHolder != null)
        {
            if (heavyBackwardSwingVFXHolder.GetComponent<vfxHolder>() != null)
            {
                heavyBackwardSwingVFXManager = heavyBackwardSwingVFXHolder.GetComponent<vfxHolder>();
            }

            heavyAttackBackwardSwingVFXArray = new Transform[heavyBackwardSwingVFXHolder.transform.childCount];
            for (int i = 0; i < heavyAttackBackwardSwingVFXArray.Length; i++)
            {
                heavyAttackBackwardSwingVFXArray[i] = heavyBackwardSwingVFXHolder.transform.GetChild(i);
            }
        }
        #endregion

        #region Heavy Downward Swing VFX Holder
        if (heavyDownwardSwingVFXHolder != null)
        {
            if (heavyDownwardSwingVFXHolder.GetComponent<vfxHolder>() != null)
            {
                heavyDownwardSwingVFXManager = heavyDownwardSwingVFXHolder.GetComponent<vfxHolder>();
            }

            heavyAttackDownwardSwingVFXArray = new Transform[heavyDownwardSwingVFXHolder.transform.childCount];
            for (int i = 0; i < heavyAttackDownwardSwingVFXArray.Length; i++)
            {
                heavyAttackDownwardSwingVFXArray[i] = heavyDownwardSwingVFXHolder.transform.GetChild(i);
            }
        }
        #endregion

        #region Heavy Miss VFX Holder
        if (heavyMissVFXHolder != null)
        {
            if (heavyMissVFXHolder.GetComponent<vfxHolder>() != null)
            {
                heavyMissVFXManager = heavyMissVFXHolder.GetComponent<vfxHolder>();
            }

            heavyAttackMissVFXArray = new Transform[heavyMissVFXHolder.transform.childCount];
            for (int i = 0; i < heavyAttackMissVFXArray.Length; i++)
            {
                heavyAttackMissVFXArray[i] = heavyMissVFXHolder.transform.GetChild(i);
            }
        }

        if (heavyDefaultSprayVFXHolder != null)
        {
            if (heavyDefaultSprayVFXHolder.GetComponent<vfxHolder>() != null)
            {
                heavyDefaultSprayVFXManager = heavyDefaultSprayVFXHolder.GetComponent<vfxHolder>();
            }

            heavyAttackDefaultVFXArray = new Transform[heavyDefaultSprayVFXHolder.transform.childCount];
            for (int i = 0; i < heavyAttackDefaultVFXArray.Length; i++)
            {
                heavyAttackDefaultVFXArray[i] = heavyDefaultSprayVFXHolder.transform.GetChild(i);
            }
        }
        #endregion

        #region Heavy Default Spray Holder
        //new sprayable attack types must be added here
        if (heavyDefaultSprayVFXHolder != null)
        {
            if (heavyDefaultSprayVFXHolder.GetComponent<vfxHolder>() != null)
            {
                heavyDefaultSprayVFXManager = heavyDefaultSprayVFXHolder.GetComponent<vfxHolder>();
            }

            heavyAttackDefaultVFXArray = new Transform[heavyDefaultSprayVFXHolder.transform.childCount];
            for (int i = 0; i < heavyAttackDefaultVFXArray.Length; i++)
            {
                heavyAttackDefaultVFXArray[i] = heavyDefaultSprayVFXHolder.transform.GetChild(i);
            }
        }
        #endregion

        #region Heavy Stomp VFX Holder
        if (heavyStompVFXHolder != null)
        {
            if (heavyStompVFXHolder.GetComponent<vfxHolder>() != null)
            {
                heavyStompVFXManager = heavyStompVFXHolder.GetComponent<vfxHolder>();
            }

            heavyStompVFXArray = new Transform[heavyStompVFXHolder.transform.childCount];
            for (int i = 0; i < heavyStompVFXArray.Length; i++)
            {
                heavyStompVFXArray[i] = heavyStompVFXHolder.transform.GetChild(i);
            }
        }
        #endregion

    }
    public void endRemainingVFX()
    {
        monsterPartRef.heavyAttack.endRemainingVFX();
        endRunVisual();
    }
    public void idleVFXSeparation()
    {
        ParticleSystem[] tempVFXGrab = GetComponentsInChildren<ParticleSystem>();
        List<GameObject> tempDefaultSprayVFX = new List<GameObject>(); //this is to catch any VFX from default spray holders which, unlike other attack VFX, are active at this time
        for (int i = 0; i < tempVFXGrab.Length; i++)
        {
            if (tempVFXGrab[i].transform.parent.GetComponent<vfxHolder>() != null)
            {
                tempVFXGrab[i].gameObject.SetActive(false);
                tempDefaultSprayVFX.Add(tempVFXGrab[i].gameObject);
            }
        }

        myIdleVFX = GetComponentsInChildren<ParticleSystem>();


        for (int i = 0; i < tempDefaultSprayVFX.Count; i++)
        {
            tempDefaultSprayVFX[i].SetActive(true);
        }
    }
    #endregion
}

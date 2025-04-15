using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class monsterPart : MonoBehaviour
{
    [Header("Monster Part Info")]
    public monsterAttackSystem myMainSystem;
    private SkinnedMeshRenderer[] mySkinnedMeshRenderers;
    private MeshRenderer[] myMeshRenderers;
    private SpriteRenderer[] mySpriteRenderers;
    public Animator connectedMonsterPart;
    public Animator mainTorso;
    private Animator myAnimator;
    public GameObject specialRunVisual;
    public bool requiresUniqueAnimationOffset;
    public Collider stompDetection;
    public int monsterPartHealth = 100;
    //monsterPartID - jumper parts = 0, organic parts = 1, and scientific parts = 2
    public int monsterPartID = 1;
    public int attackAnimationID = 1;
    public bool connected = true;
    //
    private AudioSource myPartAudio;

    //also because there is a pretty big oversight right now for "right" sided limbs that may end up being repositioned or rotated to act as a "left" sided limb
    //public enum WhichPart{ isArm, isLeg, isTail, isWing, isHead, isEye, isMouth, isTorso, isHorn, isDecor};
    //public WhichPart thisPart;
    [Header("Monster Part Questionaire")]

    public bool isArm;
    public bool isLeg;
    public bool isTail;
    public bool isWing;
    public bool isHead;
    public bool isEye;
    public bool isMouth;
    public bool isTorso;
    public bool isHorn;
    public bool isDecor;

    public SelectedMonsterPart SelectedMonsterPart;
    private MonsterPartType monsterPartTypeRef;

    [Header("Damage and Status Effects")]
    public int baseNeutralAttackDamage = 0;
    public int baseHeavyAttackDamage = 0;
    private int builtUpAttackPower = 0;
    public int builtUpAddedDamage = 0;
    private int damage = 0;
    //Status Effects
    public bool burnedStatusEffect;
    public bool electrifiedStatusEffect;
    public bool poisonedStatusEffect;
    public bool stinkyStatusEffect;
    public bool cursedStatusEffect;
    public bool confusedStatusEffect;
    public bool slimedStatusEffect;
    public bool frozenStatusEffect;
    public bool squashedStatusEffect;
    public bool slowedStatusEffect;
    private bool grabbedStatusEffect;

    [Header("Neutral Attack Questionaire")]
    public bool jabNeutralAttack;
    public bool slashNeutralAttack;
    public bool sprayNeutralAttack;
    public bool projectileNeutralAttack;
    public bool homingMissileNeutralAttack;
    public bool anvilNeutralAttack;
    public bool bowlingBallNeutralAttack;
    public bool gaseousNeutralAttack;
    public bool reflectingNeutralAttack;
    public bool suctionNeutralAttack;
    public bool AOENeutralAttack;
    public bool boomerangNeutralAttack;
    public GameObject neutralHitVFXHolder;
    public GameObject neutralForwardSwingVFXHolder;
    public GameObject neutralBackwardSwingVFXHolder;
    public GameObject neutralDownwardSwingVFXHolder;
    private Transform neutralHitVFXParent;
    public GameObject neutralMissVFXHolder;
    private Transform neutralMissVFXParent;
    public GameObject neutralDefaultSprayVFXHolder;
    public GameObject neutralStompVFXHolder;
    private Transform neutralDefaultSprayVFXParent;
    private Vector3 neutralDefaultSprayVFXStoredPosition;
    private Quaternion neutralDefaultSprayVFXStoredRotation;
    public Collider neutralCollider;
    private monsterPartReference neutralColliderReference;
    private vfxHolder neutralHitVFXManager;
    private vfxHolder neutralForwardSwingVFXManager;
    private vfxHolder neutralBackwardSwingVFXManager;
    private vfxHolder neutralDownwardSwingVFXManager;
    private vfxHolder neutralMissVFXManager;
    private vfxHolder neutralDefaultSprayVFXManager;
    private vfxHolder neutralStompVFXManager;
    public Transform neutralMuzzle;
    public bool needsReloadNeutral; //determines if part must be reloaded before attacking again
    public float reloadTimeNeutral = 1f; //jab/slash/ or anything not a projecile* with reload is handled here instead of projectile script
    [SerializeField] private bool isReloadedNeutral = true;
    [SerializeField] private bool isReloadedHeavy = true;
    public Transform[] neutralAttackHitVFXArray;
    public Transform[] neutralAttackForwardSwingVFXArray;
    public Transform[] neutralAttackBackwardSwingVFXArray;
    public Transform[] neutralAttackDownwardSwingVFXArray;
    public Transform[] neutralAttackMissVFXArray;
    public Transform[] neutralAttackDefaultVFXArray;
    public Transform[] neutralStompVFXArray;
    private Transform neutralVFXStoredParent;
    private Vector3 neutralVFXStoredPosition;
    private Quaternion neutralVFXStoredRotation;
    private int neutralVFXCount;
    private bool jabOrSlashLanded = false;
    public AudioClip[] neutralAttackHitAudioLibrary;
    private int neutralHitCounter = 0;

    [Header("Heavy Attack Questionaire")]
    public bool jabHeavyAttack;
    public bool slashHeavyAttack;
    public bool sprayHeavyAttack;
    public bool projectileHeavyAttack;
    public bool beamHeavyAttack;
    public bool reelHeavyAttack;
    public bool grappleHeavyAttack;
    public bool boomerangHeavyAttack;
    public bool homingMissileHeavyAttack;
    public bool anvilHeavyAttack;
    public bool bowlingBallHeavyAttack;
    public bool gaseousHeavyAttack;
    public bool powerBoostingHeavyAttack;
    public bool shieldHeavyAttack;
    public bool reflectingHeavyAttack;
    public bool eatingHeavyAttack;
    public bool suctionHeavyAttack;
    public bool AOEHeavyAttack;
    public bool chargingHeavyAttack;
    public bool selfExplodingHeavyAttack;
    //
    public GameObject heavyHitVFXHolder;
    public GameObject heavyForwardSwingVFXHolder;
    public GameObject heavyBackwardSwingVFXHolder;
    public GameObject heavyDownwardSwingVFXHolder;
    public GameObject heavyMissVFXHolder;
    public GameObject heavyDefaultSprayVFXHolder;
    public GameObject heavyStompVFXHolder;
    public Collider heavyCollider;
    private monsterPartReference heavyColliderReference;
    private vfxHolder heavyHitVFXManager;
    private vfxHolder heavyForwardSwingVFXManager;
    private vfxHolder heavyBackwardSwingVFXManager;
    private vfxHolder heavyDownwardSwingVFXManager;
    private vfxHolder heavyMissVFXManager;
    private vfxHolder heavyDefaultSprayVFXManager;
    private vfxHolder heavyStompVFXManager;
    public Transform heavyMuzzle;
    public bool needsReloadHeavy; //for projectiles, determines if projectile must be reloaded before shooting again
    public float reloadTimeHeavy = 1f; //jab/slash/ or anything not a projecile* with reload is handled here instead of projectile script
    public ParticleSystem chargeVisual;
    public ParticleSystem heavyChargeVisual;
    //public ParticleSystem fullHeavyChargeVisual;
    public Transform[] heavyAttackHitVFXArray;
    public Transform[] heavyAttackForwardSwingVFXArray;
    public Transform[] heavyAttackBackwardSwingVFXArray;
    public Transform[] heavyAttackDownwardSwingVFXArray;
    public Transform[] heavyAttackMissVFXArray;
    public Transform[] heavyAttackDefaultVFXArray;
    public Transform[] heavyStompVFXArray;
    private Transform heavyVFXStoredParent;
    private Vector3 heavyVFXStoredPosition;
    private Quaternion heavyVFXStoredRotation;
    private int heavyVFXCount;
    public AudioClip[] heavyAttackHitAudioLibrary;
    private int heavyHitCounter = 0;

    [Header("Monster Part Positioning Info")]
    [HideInInspector] public bool isJointed = true;
    [HideInInspector] public bool isRightShoulderLimb;
    [HideInInspector] public bool isLeftShoudlerLimb;
    [HideInInspector] public bool isRightPelvisLimb;
    [HideInInspector] public bool isLeftPelvisLimb;
    [HideInInspector] public bool isChestLimb;
    [HideInInspector] public bool isBellyLimb;
    [HideInInspector] public bool isNeckLimb;
    [HideInInspector] public bool isTailLimb;
    [HideInInspector] public bool isShoulderBladeLimb;
    [HideInInspector] public bool isTopHeadLimb;
    [HideInInspector] public bool isBacksideHeadLimb;
    [HideInInspector] public bool isRightEarLimb;
    [HideInInspector] public bool isLeftEarLimb;
    [HideInInspector] public bool isFacialLimb;
    [HideInInspector] public bool isRightSidedLimb;
    [HideInInspector] public bool isLeftSidedLimb;
    [HideInInspector] public bool isGroundedLimb;
    private string forwardInputTorsoCommand = "";
    private string backwardInputTorsoCommand = "";
    private string upwardInputTorsoCommand = "";
    private string downwardInputTorsoCommand = "";
    private string forwardInputHeadCommand = "";
    private string backwardInputHeadCommand = "";
    private string upwardInputHeadCommand = "";
    private string downwardInputHeadCommand = "";
    private string torsoCommand = "";
    private bool hasTorsoCommand = false;
    private string torsoCommandOverride = "";
    private bool hasTorsoCommandOverride = false; //refers to heads on torsos, torsos on torsos, torsos on heads on torsos, etc. that needs to move the main body
    private string headCommand = "";
    private bool hasHeadCommand = false;
    private bool leftAttackOverride;
    private bool rightAttackOverride;

    [Header("Internal Info - Don't Touch")]
    [HideInInspector] public bool isBracing = false;
    [HideInInspector] public bool attackMarkedHeavy = false;
    private bool heavyAttackInMotion = false;
    private bool fullActiveHeavy = false;
    public bool requiresRightStance = false;
    public bool requiresLeftStance = false;
    public bool requiresForwardStance = false;
    public bool requiresBackwardStance = false;
    private bool hasNeutralMovementCommand = false;
    private string forwardNeutralMovementCommand = "";
    private string upwardNeutralMovementCommand = "";
    private string backwardNeutralMovementCommand = "";
    private string downwardNeutralMovementCommand = "";
    private bool hasHeavyMovementCommand = false;
    private string forwardHeavyMovementCommand = "";
    private string upwardHeavyMovementCommand = "";
    private string backwardHeavyMovementCommand = "";
    private string downwardHeavyMovementCommand = "";
    public bool isLeadingLeg;
    public bool isFloatingGroundedLeg;
    public bool isFloatingTorso;
    public bool hasFlightedIdle = false;
    public Outline visualForAnimationTests;
    public bool hasHeavyBrace = false;
    private bool isAttacking = false;
    public bool attackFocusOn = false;
    private bool isWalking = false;
    public bool isRunning = false;
    public bool facingRight;
    public bool grounded = true;
    private bool haveGrabbedAMonster;

    private int reelAttackBuiltUpPower = 0;
    private int reelAttackCurrentThreshold = 0;
    private bool powerUpCheckAllowed = true;
    private bool reelAttackLanded = false;
    private monsterPartReference grabbedMonster;

    private bool forwardAttackQuickSwitch;
    private bool lowerAttackQuickSwitch;
    private bool upwardAttackQuickSwitch;
    private bool leftUpperAttackQuickSwitch;
    private bool rightUpperAttackQuickSwitch;
    private bool leftLowerAttackQuickSwitch;
    private bool rightLowerAttackQuickSwitch;

    public ParticleSystem[] myIdleVFX;
    public List<monsterPartReference> referencesToIgnore = new List<monsterPartReference>();

    #region Build a Scare Tools

    /// <summary>
    /// Assigns the monster part type to the monster part depending on the value of the selected monster part enum.
    /// </summary>
    public void TriggerPartTypeSetup()
    {
        if (SelectedMonsterPart == SelectedMonsterPart.None)
        {
            Debug.LogError("Monster part type not assigned in the inspector on " + gameObject.name);
            return;
        }

        // gets the path to the matching class name of the selected monster part enum and saves it as a string
        string selectedPartName = $"{typeof(MonsterPartType).Namespace}.{SelectedMonsterPart}, {typeof(MonsterPartType).Assembly.FullName}";
        // converts the string to a Type
        Type selectedClassType = Type.GetType(selectedPartName);
        // converts the type to MonsterPartType and creates an instance of it
        monsterPartTypeRef = Activator.CreateInstance(selectedClassType) as MonsterPartType;

        Debug.Log(monsterPartTypeRef);

    }

    public void changeAttackAnimationAtRuntime()
    {
        //this will be expanded to include all monster parts
        if (isLeg||isArm || isTail || isHorn || isEye) 
        {
            if (isLeg && attackAnimationID == 2) //corrections for if up attack is selected with leg
            {
                attackAnimationID = 1;
            }
            else if (isArm && attackAnimationID == -1) //corrections for if backwards attack is selected with arm
            {
                attackAnimationID = 1;
            }

            myAnimator.SetInteger("Attack Animation ID", attackAnimationID);

            torsoCommand = "";
            headCommand = "";
            torsoCommandOverride = "";
            upwardAttackQuickSwitch = false;
            forwardAttackQuickSwitch = false;
            requiresBackwardStance = false;
            requiresForwardStance = false;
            requiresRightStance = false;
            requiresLeftStance = false;

            attackCalculations();

            setUpVFX();
            setUpSFX();
        }
    }

    public void setUpOutline()
    {
        if (monsterPartID == 1)
        {
            if (gameObject.GetComponent<Outline>() != null)
            {
                visualForAnimationTests = gameObject.GetComponent<Outline>();
                visualForAnimationTests.enabled = true;
            }
            else
            {
                print("Oh No! You forgot to add an Outline component to" + " " + gameObject.name);
                return;
            }

            visualForAnimationTests.OutlineMode = Outline.Mode.OutlineVisible;
            visualForAnimationTests.OutlineColor = Color.yellow;
            visualForAnimationTests.OutlineWidth = 1f;
            disableOutline();
            myAnimator = this.GetComponent<Animator>();
        }
    }

    public void disableOutline() { if (visualForAnimationTests != null) visualForAnimationTests.enabled = false;}
    public void reenableOutline() { if (visualForAnimationTests != null) visualForAnimationTests.enabled = true;}

    #endregion

    #region Animation Set Up
    public void triggerAnimationSetUp()
    {
        myAnimator = this.GetComponent<Animator>();

        if(isLeg && isGroundedLimb == false)
        {
            myAnimator.SetBool("Grounded Limb", false); //This is just so that loose legs do a spike kick instead of a stomp on ground
        }

        if ((isLeg && isFloatingGroundedLeg) || (isTorso && isFloatingTorso))
        {
            myAnimator.SetBool("Has Floating Idle", true);
        }


        if (isLeg || isArm || isTail || isHorn || isEye) //this will be expanded to include heads and horns
        {
            myAnimator.SetInteger("Attack Animation ID", attackAnimationID);
        }

        if (isMouth && myAnimator != null) //this will be expanded to include arms, tails, wings and heads
        {
            myAnimator.SetInteger("Attack Animation ID", attackAnimationID);
        }

        if (isLeadingLeg)
        {
            myAnimator.SetBool("Is Leading Leg", true);
        }

        if (hasFlightedIdle)
        {
            myAnimator.SetBool("Has Flighted Idle", true);
        }

        torsoCommand = "";
        headCommand = "";
        torsoCommandOverride = "";
        upwardAttackQuickSwitch = false;
        forwardAttackQuickSwitch = false;
        requiresBackwardStance = false;
        requiresForwardStance = false;
        requiresRightStance = false;
        requiresLeftStance = false;

        attackCalculations();

        #region Separating Visual and Combat Elements for Dash Attacks
        //search through all my objects and gather everything with a skinned mesh renderer
        List<GameObject> hitboxesAndHurtboxes = new List<GameObject>();

        Transform[] childrenInObject = GetComponentsInChildren<Transform>();
        for (int i = 0; i < childrenInObject.Length; i++)
        {
            if (childrenInObject[i].gameObject.tag == "Hurtbox" || childrenInObject[i].gameObject.tag == "Hitbox" || childrenInObject[i].gameObject.tag == "Stomp Box")
            {
                hitboxesAndHurtboxes.Add(childrenInObject[i].gameObject);
                childrenInObject[i].gameObject.SetActive(false);
            }
        }

        mySkinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        myMeshRenderers = GetComponentsInChildren<MeshRenderer>();
        mySpriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        if (myIdleVFX.Length != 0) //checks for the idle VFX because of what needs to happen for the outline to not go around the VFX planes
        {
            for (int i = 0; i < myIdleVFX.Length; i++)
            {
                myIdleVFX[i].gameObject.SetActive(true);
            }
        }
        else
        {
            idleVFXSeparation();
        }

        for (int i = 0; i < hitboxesAndHurtboxes.Count; i++)
        {
            hitboxesAndHurtboxes[i].gameObject.SetActive(true);
        }
        #endregion

        setUpVFX();
        setUpSFX();
    }

    public void attackCalculations()
    {
        MonsterCalculations calc = AttackCalculationsSetUp(); //does this in a separate script, it's too beefy :/

        requiresBackwardStance = calc.requiresBackwardStance;
        requiresForwardStance = calc.requiresForwardStance;
        requiresRightStance = calc.requiresRightStance;
        requiresLeftStance = calc.requiresLeftStance;

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

    public void triggerAnimationOffsets()
    {
        if (monsterPartID != 1 || connected == false)
        {
            return;
        }

        if(myAnimator != null)
        {
            if (isGroundedLimb)
            {
                if (isRightSidedLimb)
                {
                    myAnimator.SetFloat("Idle Offset", 0.0f);
                    myAnimator.SetFloat("Walk Offset", 0.0f);
                    myAnimator.SetFloat("Run Offset", 0.0f);
                }
                else if (isLeftSidedLimb)
                {
                    float leftWalkOffset = myAnimator.GetFloat("Left Walk Offset");
                    float leftRunOffset = myAnimator.GetFloat("Left Run Offset");
                    myAnimator.SetFloat("Idle Offset", 0.5f);
                    myAnimator.SetFloat("Walk Offset", leftWalkOffset);
                    myAnimator.SetFloat("Run Offset", leftRunOffset);
                }
            }
            else if (isArm || isHead || isMouth || isTail || isLeg || requiresUniqueAnimationOffset)
            {
                float randomOffset = Random.Range(0, 0.5f);
                myAnimator.SetFloat("Idle Offset", randomOffset);
            }
        }
    }

    public void triggerIdle()
    {
        if (connected == false)
        {
            return;
        }

        if (myAnimator != null)
        {
            myAnimator.SetTrigger("Idle");
        }
    }
    #endregion

    #region Collision Occlusion and Collision Logic

    public void triggerCollisionLogic()
    {
        for (int u = 0; u < referencesToIgnore.Count; u++)
        {
            referencesToIgnore[u].referencesToIgnore = referencesToIgnore;
        }

        monsterPartReference[] internalPartReferences = GetComponentsInChildren<monsterPartReference>();

        for (int i = 0; i < internalPartReferences.Length; i++)
        {
            internalPartReferences[i].partReference = this.GetComponent<monsterPart>();
        }

        if (neutralCollider != null)
        {
            neutralColliderReference = neutralCollider.gameObject.GetComponent<monsterPartReference>();
        }

        if (heavyCollider!= null)
        {
            heavyColliderReference = heavyCollider.gameObject.GetComponent<monsterPartReference>();
        }

        statusEffectAndDamageCalculations();
    }

    #endregion

    #region Attack Animations
    public void triggerAttack(string animationName, int attackDirection)
    {
        if (connected == false)
        {
            return;
        }

        attackAnimationID = attackDirection;

        if (myAnimator != null)
        {
            isAttacking = true;
            myAnimator.SetInteger("Attack Animation ID", attackAnimationID);
            myAnimator.SetTrigger(animationName);
            myMainSystem.attackFocusOn();
            attackFocusOn = true;
            runToAttackCorrections();
            attackMarkedHeavy = false;
            heavyAttackInMotion = false;
            fullActiveHeavy = false;
            triggerNeutralOrHeavyRefresh(false);
        }
    }

    public void triggerAttackAnticipation()
    {
        myMainSystem.correctAttackDirection(0);

        if (attackAnimationID == 1)
        {
            if (hasTorsoCommand)
            {
                connectedMonsterPart.SetTrigger(forwardInputTorsoCommand);
            }
            
            if(hasHeadCommand)
            {
                connectedMonsterPart.SetTrigger(forwardInputHeadCommand);
            }
        }
        else if (attackAnimationID == -1)
        {
            if (hasTorsoCommand)
            {
                connectedMonsterPart.SetTrigger(backwardInputTorsoCommand);
            }

            if (hasHeadCommand)
            {
                connectedMonsterPart.SetTrigger(backwardInputHeadCommand);
            }
        }
        else if (attackAnimationID == 0)
        {
            if (hasTorsoCommand)
            {
                connectedMonsterPart.SetTrigger(downwardInputTorsoCommand);
            }

            if (hasHeadCommand)
            {
                connectedMonsterPart.SetTrigger(downwardInputHeadCommand);
            }
        }
        else if (attackAnimationID == 2)
        {
            if (hasTorsoCommand)
            {
                connectedMonsterPart.SetTrigger(upwardInputTorsoCommand);
            }

            if (hasHeadCommand)
            {
                connectedMonsterPart.SetTrigger(upwardInputHeadCommand);
            }
        }
    }

    #region Attack Bracing Animations
    public void triggerLeftAttackStance()
    {
        if (connected == false || attackFocusOn)
        {
            return;
        }

        isBracing = true;

        if (isGroundedLimb)
        {
            if (grounded)
            {
                if (isRightSidedLimb && isAttacking == false)
                {
                    myAnimator.ResetTrigger("Unbrace");
                    myAnimator.SetTrigger("Backward Brace");
                }
                else if (isLeftSidedLimb && isAttacking == false)
                {
                    myAnimator.ResetTrigger("Unbrace");
                    myAnimator.SetTrigger("Forward Brace");
                }

                myAnimator.SetBool("Walking", false);
                myAnimator.SetBool("Running", false);
            }
            else
            {
                if (isRightSidedLimb && isAttacking == false)
                {
                    myAnimator.ResetTrigger("Unbrace");
                    myAnimator.SetTrigger("Airborn Brace");
                }
                else if (isLeftSidedLimb && isAttacking == false)
                {
                    myAnimator.ResetTrigger("Unbrace");
                    myAnimator.SetTrigger("Airborn Brace");
                }
            }
           
        }
        else if (isLeg)
        {
            if (isRightSidedLimb && isAttacking == false)
            {
                myAnimator.ResetTrigger("Unbrace");
                myAnimator.SetTrigger("Airborn Brace");
            }
            else if (isLeftSidedLimb && isAttacking == false)
            {
                myAnimator.ResetTrigger("Unbrace");
                myAnimator.SetTrigger("Airborn Brace");
            }
        }
        else if (isArm)
        {
            if (CheckAnimState("Idle", "Fall", "Land"))
            {
                if (isRightSidedLimb && isAttacking == false)
                {
                    myAnimator.SetTrigger("Brace");
                }
                else if (isLeftSidedLimb && isAttacking == false)
                {
                    myAnimator.SetTrigger("Brace");
                }
            }
        }
        else if (isMouth && myAnimator != null)
        {
            if (CheckAnimState("Idle", "Fall", "Land"))
            {
                if (isAttacking == false)
                {
                    myAnimator.SetTrigger("Brace");
                }
            }
        }
        else if (isWing)
        {
            if (CheckAnimState("Idle Fly", "Idle Grounded", "Fall", "Land", "Glide", "Running"))
            {
                if (isAttacking == false)
                {
                    myAnimator.ResetTrigger("Unbrace");
                    myAnimator.SetTrigger("Brace");
                    myAnimator.SetBool("Walking", false);
                    myAnimator.SetBool("Running", false);
                }
            }
        }
        else if (isTail)
        {
            if (CheckAnimState("Idle","Fall","Land","Glide"))
            {
                if (isAttacking == false)
                {
                    myAnimator.ResetTrigger("Unbrace");
                    myAnimator.SetTrigger("Brace");
                    myAnimator.SetBool("Walking", false);
                    myAnimator.SetBool("Running", false);
                }
            }
        }
    }

    public void triggerRightAttackStance()
    {
        if (connected == false || attackFocusOn)
        {
            return;
        }

        isBracing = true;

        if (isGroundedLimb)
        {
            if (grounded)
            {
                if (isLeftSidedLimb && isAttacking == false)
                {
                    myAnimator.ResetTrigger("Unbrace");
                    myAnimator.SetTrigger("Backward Brace");
                }
                else if (isRightSidedLimb && isAttacking == false)
                {
                    myAnimator.ResetTrigger("Unbrace");
                    myAnimator.SetTrigger("Forward Brace");
                }

                myAnimator.SetBool("Walking", false);
                myAnimator.SetBool("Running", false);
            }
            else
            {
                if (isRightSidedLimb && isAttacking == false)
                {
                    myAnimator.ResetTrigger("Unbrace");
                    myAnimator.SetTrigger("Airborn Brace");
                }
                else if (isLeftSidedLimb && isAttacking == false)
                {
                    myAnimator.ResetTrigger("Unbrace");
                    myAnimator.SetTrigger("Airborn Brace");
                }
            }

        }
        else if (isLeg)
        {
            if (isRightSidedLimb && isAttacking == false)
            {
                myAnimator.ResetTrigger("Unbrace");
                myAnimator.SetTrigger("Airborn Brace");
            }
            else if (isLeftSidedLimb && isAttacking == false)
            {
                myAnimator.ResetTrigger("Unbrace");
                myAnimator.SetTrigger("Airborn Brace");
            }
        }
        else if (isArm)
        {
            if (CheckAnimState("Idle","Fall","Land"))
            {
                if (isLeftSidedLimb && isAttacking == false)
                {
                    myAnimator.SetTrigger("Brace");
                }
                else if (isRightSidedLimb && isAttacking == false)
                {
                    myAnimator.SetTrigger("Brace");
                }
            }
        }
        else if (isMouth && myAnimator != null)
        {
            if (CheckAnimState("Idle","Fall","Land"))
            {
                if (isAttacking == false)
                {
                    myAnimator.SetTrigger("Brace");
                }
            }
        }
        else if (isWing)
        {
            if (CheckAnimState("Idle Fly","Idle Grounded","Fall","Land","Glide","Running"))
            {
                if (isAttacking == false)
                {
                    myAnimator.ResetTrigger("Unbrace");
                    myAnimator.SetTrigger("Brace");
                    myAnimator.SetBool("Walking", false);
                    myAnimator.SetBool("Running", false);
                }
            }
        }
        else if (isTail)
        {
            if (CheckAnimState("Idle","Fall","Land","Glide"))
            {
                if (isAttacking == false)
                {
                    myAnimator.ResetTrigger("Unbrace");
                    myAnimator.SetTrigger("Brace");
                    myAnimator.SetBool("Walking", false);
                    myAnimator.SetBool("Running", false);
                }
            }
        }
    }

    public void triggerForwardStance()
    {
        if (connected == false || attackFocusOn)
        {
            return;
        }

        isBracing = true;

        if (isGroundedLimb)
        {
            if (grounded)
            {
                if (isLeftSidedLimb && isAttacking == false)
                {
                    myAnimator.ResetTrigger("Unbrace");
                    myAnimator.SetTrigger("Forward Brace");
                }
                else if (isRightSidedLimb && isAttacking == false)
                {
                    myAnimator.ResetTrigger("Unbrace");
                    myAnimator.SetTrigger("Forward Brace");
                }

                myAnimator.SetBool("Walking", false);
                myAnimator.SetBool("Running", false);
            }
            else
            {
                if (isRightSidedLimb && isAttacking == false)
                {
                    myAnimator.ResetTrigger("Unbrace");
                    myAnimator.SetTrigger("Airborn Brace");
                }
                else if (isLeftSidedLimb && isAttacking == false)
                {
                    myAnimator.ResetTrigger("Unbrace");
                    myAnimator.SetTrigger("Airborn Brace");
                }
            }
        }
        else if (isLeg)
        {
            if (isRightSidedLimb && isAttacking == false)
            {
                myAnimator.ResetTrigger("Unbrace");
                myAnimator.SetTrigger("Airborn Brace");
            }
            else if (isLeftSidedLimb && isAttacking == false)
            {
                myAnimator.ResetTrigger("Unbrace");
                myAnimator.SetTrigger("Airborn Brace");
            }
        }
        else if (isArm)
        {
            if (CheckAnimState("Idle","Fall","Land"))
            {
                if (isLeftSidedLimb && isAttacking == false)
                {
                    myAnimator.SetTrigger("Brace");
                }
                else if (isRightSidedLimb && isAttacking == false)
                {
                    myAnimator.SetTrigger("Brace");
                }
            }
        }
        else if (isMouth && myAnimator != null)
        {
            if (CheckAnimState("Idle","Fall","Land"))
            {
                if (isAttacking == false)
                {
                    myAnimator.SetTrigger("Brace");
                }
            }
        }
        else if (isWing)
        {
            if (CheckAnimState("Idle Fly","Idle Grounded","Fall","Land","Glide","Running"))
            {
                if (isAttacking == false)
                {
                    myAnimator.ResetTrigger("Unbrace");
                    myAnimator.SetTrigger("Brace");
                    myAnimator.SetBool("Walking", false);
                    myAnimator.SetBool("Running", false);
                }
            }
        }
        else if (isTail)
        {
            if (CheckAnimState("Idle","Fall","Land","Glide"))
            {
                if (isAttacking == false)
                {
                    myAnimator.ResetTrigger("Unbrace");
                    myAnimator.SetTrigger("Brace");
                    myAnimator.SetBool("Walking", false);
                    myAnimator.SetBool("Running", false);
                }
            }
        }
    }

    public void triggerBackwardStance()
    {
        if (connected == false || attackFocusOn)
        {
            return;
        }

        isBracing = true;

        if (isGroundedLimb)
        {
            if (grounded)
            {
                if (isRightSidedLimb && isAttacking == false)
                {
                    myAnimator.ResetTrigger("Unbrace");
                    myAnimator.SetTrigger("Launching Backward Brace");
                    myAnimator.SetBool("Needs Launch", true);
                }
                else if (isLeftSidedLimb && isAttacking == false)
                {
                    myAnimator.ResetTrigger("Unbrace");
                    myAnimator.SetTrigger("Launching Backward Brace");
                    myAnimator.SetBool("Needs Launch", true);
                }

                myAnimator.SetBool("Walking", false);
                myAnimator.SetBool("Running", false);
            }
            else
            {
                if (isRightSidedLimb && isAttacking == false)
                {
                    myAnimator.ResetTrigger("Unbrace");
                    myAnimator.SetTrigger("Airborn Brace");
                }
                else if (isLeftSidedLimb && isAttacking == false)
                {
                    myAnimator.ResetTrigger("Unbrace");
                    myAnimator.SetTrigger("Airborn Brace");
                }
            }
        }
        else if (isLeg)
        {
            if (isRightSidedLimb && isAttacking == false)
            {
                myAnimator.ResetTrigger("Unbrace");
                myAnimator.SetTrigger("Airborn Brace");
            }
            else if (isLeftSidedLimb && isAttacking == false)
            {
                myAnimator.ResetTrigger("Unbrace");
                myAnimator.SetTrigger("Airborn Brace");
            }
        }
        else if (isArm)
        {
            if (CheckAnimState("Idle","Fall","Land"))
            {
                if (isLeftSidedLimb && isAttacking == false)
                {
                    myAnimator.SetTrigger("Brace");
                }
                else if (isRightSidedLimb && isAttacking == false)
                {
                    myAnimator.SetTrigger("Brace");
                }
            }
        }
        else if (isMouth && myAnimator != null)
        {
            if (CheckAnimState("Idle","Fall","Land"))
            {
                if (isAttacking == false)
                {
                    myAnimator.SetTrigger("Brace");
                }
            }
        }
        else if (isWing)
        {
            if (CheckAnimState("Idle Fly","Idle Grounded","Fall","Land","Glide","Running"))
            {
                if (isAttacking == false)
                {
                    myAnimator.ResetTrigger("Unbrace");
                    myAnimator.SetTrigger("Brace");
                    myAnimator.SetBool("Walking", false);
                    myAnimator.SetBool("Running", false);
                }
            }
        }
        else if (isTail)
        {
            if (CheckAnimState("Idle","Fall","Land","Glide"))
            {
                if (isAttacking == false)
                {
                    myAnimator.ResetTrigger("Unbrace");
                    myAnimator.SetTrigger("Brace");
                    myAnimator.SetBool("Walking", false);
                    myAnimator.SetBool("Running", false);
                }
            }
        }
    }

    public void triggerFlourishStance()
    {
        if (connected == false || attackFocusOn)
        {
            return;
        }


        isBracing = true;

        if (isGroundedLimb)
        {

            if (grounded)
            {
                if (isRightSidedLimb && isAttacking == false)
                {
                    myAnimator.ResetTrigger("Unbrace");
                    myAnimator.SetTrigger("Flourish");
                    myAnimator.SetBool("Needs Launch", true);
                }
                else if (isLeftSidedLimb && isAttacking == false)
                {
                    myAnimator.ResetTrigger("Unbrace");
                    myAnimator.SetTrigger("Flourish");
                    myAnimator.SetBool("Needs Launch", true);
                }

                myAnimator.SetBool("Walking", false);
                myAnimator.SetBool("Running", false);
            }
            else
            {
                if (isRightSidedLimb && isAttacking == false)
                {
                    myAnimator.ResetTrigger("Unbrace");
                    myAnimator.SetTrigger("Airborn Brace");
                }
                else if (isLeftSidedLimb && isAttacking == false)
                {
                    myAnimator.ResetTrigger("Unbrace");
                    myAnimator.SetTrigger("Airborn Brace");
                }
            }
        }
        else if (isLeg)
        {
            if (isRightSidedLimb && isAttacking == false)
            {
                myAnimator.ResetTrigger("Unbrace");
                myAnimator.SetTrigger("Airborn Brace");
            }
            else if (isLeftSidedLimb && isAttacking == false)
            {
                myAnimator.ResetTrigger("Unbrace");
                myAnimator.SetTrigger("Airborn Brace");
            }
        }
        else if (isArm)
        {
            if (CheckAnimState("Idle","Fall","Land"))
            {
                if (isLeftSidedLimb && isAttacking == false)
                {
                    myAnimator.SetTrigger("Brace");
                }
                else if (isRightSidedLimb && isAttacking == false)
                {
                    myAnimator.SetTrigger("Brace");
                }
            }
        }
        else if (isMouth && myAnimator != null)
        {
            if (CheckAnimState("Idle","Fall","Land"))
            {
                if (isAttacking == false)
                {
                    myAnimator.SetTrigger("Brace");
                }
            }
        }
        else if (isWing)
        {
            if (CheckAnimState("Idle Fly","Idle Grounded","Fall","Land","Glide","Running"))
            {
                if (isAttacking == false)
                {
                    myAnimator.ResetTrigger("Unbrace");
                    myAnimator.SetTrigger("Brace");
                    myAnimator.SetBool("Walking", false);
                    myAnimator.SetBool("Running", false);
                }
            }
        }
        else if (isTail)
        {
            if (CheckAnimState("Idle","Fall","Land","Glide"))
            {
                if (isAttacking == false)
                {
                    myAnimator.ResetTrigger("Unbrace");
                    myAnimator.SetTrigger("Brace");
                    myAnimator.SetBool("Walking", false);
                    myAnimator.SetBool("Running", false);
                }
            }
        }
    }

    public void triggerUnbrace()
    {
        if (connected == false)
        {
            return;
        }

        isBracing = false;

        if (isAttacking == false)
        {
            if (isLeg)
            {
                myAnimator.ResetTrigger("Unbrace");
                myAnimator.SetTrigger("Unbrace");
                myAnimator.ResetTrigger("Backward Brace");
                myAnimator.ResetTrigger("Forward Brace");
            }

            if (isArm || isWing || isTail)
            {
                myAnimator.ResetTrigger("Unbrace");
                myAnimator.SetTrigger("Unbrace");
                myAnimator.ResetTrigger("Brace");

                if (isWing)
                {
                    myAnimator.SetBool("Glide Activated", false);
                }

                if (isArm)
                {
                    myAnimator.SetBool("Swaying", false);
                }
            }

            if (isMouth && myAnimator != null)
            {
                myAnimator.ResetTrigger("Unbrace");
                myAnimator.SetTrigger("Unbrace");
                myAnimator.ResetTrigger("Brace");
            }

        }
        else
        {
            if (isGroundedLimb)
            {
                myAnimator.ResetTrigger("Unbrace");
                myAnimator.SetTrigger("Unbrace");
                myAnimator.ResetTrigger("Backward Brace");
                myAnimator.ResetTrigger("Forward Brace");
            }
        }
    }

    #endregion

    #region Neutral to Heavy Input Shifts
    public void triggerNeutralOrHeavyRefresh(bool inputCanceled)
    {
        //most likely a canceled input after system already has registered the difference between input but before the attack has been unleashed
        //aka a canceled heavy attack

        if (fullActiveHeavy && attackFocusOn && beamHeavyAttack)
        {
            if (inputCanceled)
            {
                //end functions + turn off visuals
                triggerAttackToIdle();
                StartCoroutine(beamAttackDelay());
            }
            return;
        }

        if (fullActiveHeavy == false && attackFocusOn == true)
        {
            if (inputCanceled)
            {
                if (heavyAttackInMotion)
                {
                    myAnimator.SetTrigger("Cancel Heavy");
                }
                else
                {
                    attackMarkedHeavy = false;
                    myAnimator.SetBool("Attack Marked Heavy", false);
                    triggerNeutralOrHeavy();
                }
            }
            else
            {
                attackMarkedHeavy = true;
                myAnimator.SetBool("Attack Marked Heavy", true);
            }
        }
    }

    IEnumerator beamAttackDelay()
    {
        yield return new WaitForSeconds(0.2f);
        triggerAttackCorrections(); //delaying this to allow the body time to unbrace
    }

    public void triggerNeutralOrHeavy()
    {
        if (!attackMarkedHeavy && needsReloadNeutral )
        {
            if (!isReloadedNeutral)
            {
                triggerNeutralOrHeavyRefresh(true);
                return;
            }
        }

        if (attackMarkedHeavy && needsReloadHeavy)
        {
            if (!isReloadedHeavy)
            {
                triggerNeutralOrHeavyRefresh(true);
                return;
            }
        }

        if (beamHeavyAttack && attackMarkedHeavy)
        {
            heavyAttackInMotion = true;
            return;
        }

        if (attackMarkedHeavy)
        {

            heavyAttackInMotion = true;
            myMainSystem.switchBraceStance(); //for a stronger looking leg stance
            myMainSystem.heavyAttackActivated();
            triggerHeavyAttackPowerUp();//by triggering the heavy, 1 power up is granted
            triggerChargeVisual();
        }
        else
        {
            myAnimator.SetTrigger("Force Neutral Attack");
        }
    }

    public void triggerHeavyLegStance()
    {
        if (connected == false)
        {
            return;
        }

        if (isGroundedLimb)
        {
            myAnimator.SetTrigger("Switch Stance");
            hasHeavyBrace = true;
        }
    }

    public void inputCanceled()//this is when a heavy has been triggered but the player cancels it early
    {
        //if this part has a failure state or if it has the ability to cancel heavies early
    }

    #endregion

    public void triggerFullHeavyEffect()
    {
        //check for some damage upgrade 
        myMainSystem.releaseFullHeavyVisual();
    }

    public void triggerAttackRelease()
    {

        if (isJointed)
        {
            // not sure about this whole section, it looks important
            connectedMonsterPart.SetBool("Ready to Swing", true);
            connectedMonsterPart.SetBool("Walking", false);
            connectedMonsterPart.SetBool("Running", false);

            if (hasTorsoCommandOverride)
            {
                mainTorso.SetBool("Ready to Swing", true);
                mainTorso.ResetTrigger("Quick Forward Position");
                mainTorso.ResetTrigger("Quick Upward Position");
                mainTorso.SetBool("Walking", false);
                mainTorso.SetBool("Running", false);
            }

            isRunning = false;
            fullActiveHeavy = true;
            isAttacking = false;
            myMainSystem.correctWalkingAttackAnimations();
            triggerEndChargeVisual();

            if (attackMarkedHeavy)
            {
                heavyAttackPowerCalculation();
                triggerHeavyChargeVisual();

                if (hasHeavyMovementCommand)
                {
                    if(attackAnimationID == 1)
                    {
                        if(forwardHeavyMovementCommand == "Forward Leap") //arm swings, leg kicks, etc.
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
                    else if (attackAnimationID == 2)
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
                    else if (attackAnimationID == -1)
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
                    else if (attackAnimationID == 0)
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

                if(jabHeavyAttack || slashHeavyAttack)
                {
                    triggerJabOrSlashCollisionsOn();
                }
            }
            else
            {
                neutralAttackPowerCalculation();

                if (hasNeutralMovementCommand)
                {
                    if (attackAnimationID == 1)
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
                    else if (attackAnimationID == 2)
                    {
                        if (upwardNeutralMovementCommand == "Upward Strike") //arm swings, etc.
                        {
                            //myMainSystem.smallLeapAttackUpward();
                        }
                        else if (upwardNeutralMovementCommand == "Upward Single Spin") //tail spin attacks, spinjitsu leg kick
                        {
                            
                        }
                    }
                    else if (attackAnimationID == -1)
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
                    else if (attackAnimationID == 0)
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

                if (jabNeutralAttack || slashNeutralAttack)
                {
                    triggerJabOrSlashCollisionsOn();
                }
            }  
        }
    }

    public void walkToAttackCorrections()
    {
        if (connected == false)
        {
            return;
        }

        if (isTorso)
        {
            myAnimator.ResetTrigger("Walk to Idle");

            if (myAnimator.GetBool("Walking") == true)
            {
                myAnimator.SetBool("Walking", false);
                myAnimator.SetTrigger("Walk to Idle");
            }
        }

        if (isGroundedLimb)
        {
            myAnimator.SetBool("Walking", false);

            if (hasHeavyBrace || myAnimator.GetBool("Needs Launch"))
            {
                myAnimator.SetTrigger("Switch Stance Quick");
                hasHeavyBrace = false;
                myAnimator.SetBool("Needs Launch", false);
            }
        }

        if (isHead)
        {
            myAnimator.SetBool("Walking", false);
        }
    }

    public void triggerAttackCorrections()
    {
        myMainSystem.correctRunningAttackAnimations();
        myMainSystem.attackFocusOff();
        attackFocusOn = false;
        attackMarkedHeavy = false;
        isAttacking = false;
        fullActiveHeavy = false;

        connectedMonsterPart.SetBool("Attack to Idle", false);

        if (hasTorsoCommandOverride)
        {
            mainTorso.SetBool("Attack to Idle", false);
        }

        if (reelHeavyAttack)
        {
            myMainSystem.grabbingCanceled();
        }

        if (isArm)
        {
            myAnimator.SetBool("Swaying", false);
        }
    }

    public void runToAttackCorrections()
    {
        if (connected == false)
        {
            return;
        }

        if (isGroundedLimb || isTorso)
        {
            myAnimator.SetBool("Running", false);
            myAnimator.SetBool("Walking", false);
            isWalking = false;
            isRunning = false;
        }

        if (isArm)
        {
            myAnimator.SetBool("Running", false);
            isWalking = false;
            isRunning = false;
        }
        if (isHead || isTail || isWing)
        {
            myAnimator.SetBool("Running", false);
            myAnimator.SetBool("Walking", false);
            isWalking = false;
            isRunning = false;
        }
    }

    public void triggerAttackToIdle()
    {
        connectedMonsterPart.SetBool("Attack to Idle", true);
        connectedMonsterPart.SetBool("Ready to Swing", false);

        if (hasTorsoCommandOverride)
        {
            mainTorso.SetBool("Attack to Idle", true);
            mainTorso.SetBool("Ready to Swing", false);
        }

        myMainSystem.endBracing();
        endRemainingVFX();

    }

    #region Launching Attacks
    public void triggerRollingAttack()
    {
        if (connected == false || isDecor)
        {
            return;
        }

        grounded = false;

        if (attackFocusOn)
        {
            myAnimator.SetBool("Grounded", false);
            myAnimator.SetBool("Walking", false);
            myAnimator.SetBool("Running", false);
            return;
        }

        if (isHorn && myAnimator != null)
        {
            myAnimator.SetBool("Grounded", false);
            return;
        }

        if (myAnimator != null)
        {
            myAnimator.SetBool("Grounded", false);
            myAnimator.SetTrigger("Roll");
            myAnimator.SetBool("Ready to Unroll", false);
        }
        if (isGroundedLimb || isTorso || isHead || isWing || isTail)
        {
            myAnimator.SetBool("Walking", false);
            myAnimator.SetBool("Running", false);
            isWalking = false;
            isRunning = false;

            if (isWing || isHead)
            {
                myAnimator.SetBool("Glide Activated", false);
            }

            if (isLeg)
            {
                myAnimator.SetBool("Calm", false);
            }

            if (isTorso)
            {
                myAnimator.SetBool("Teeter", false);
            }
        }

        if (isArm)
        {
            myAnimator.SetBool("Glide Activated", false);
            myAnimator.SetBool("Running", false);
            myAnimator.SetBool("Swaying", false);
            isWalking = false;
            isRunning = false;
        }
    }

    public void triggerUpwardsLeapingAttack()
    {
        if (connected == false || isDecor)
        {
            return;
        }

        grounded = false;
        attackMarkedHeavy = false;

        if (attackFocusOn)
        {
            myAnimator.SetBool("Grounded", false);
            myAnimator.SetBool("Walking", false);
            myAnimator.SetBool("Running", false);
            return;
        }

        if (isTorso)
        {
            myAnimator.SetBool("Grounded", false);
            myAnimator.SetBool("Walking", false);
            myAnimator.SetBool("Running", false);
            myAnimator.SetBool("Teeter", false);
            isWalking = false;
            isRunning = false;
            return;
        }

        if (isHead)
        {
            myAnimator.SetBool("Grounded", false);
            myAnimator.SetBool("Glide Activated", false);
            myAnimator.SetBool("Walking", false);
            myAnimator.SetBool("Running", false);
            isWalking = false;
            isRunning = false;
            return;
        }

        if (isArm)
        {
            myAnimator.SetBool("Grounded", false);
            myAnimator.SetBool("Glide Activated", false);
            myAnimator.SetBool("Walking", false);
            myAnimator.SetBool("Running", false);
            isWalking = false;
            isRunning = false;

            if (isArm)
            {
                myAnimator.SetBool("Swaying", false);
            }

            return;
        }

        if (isHorn && myAnimator != null)
        {
            myAnimator.SetBool("Grounded", false);
            return;
        }

        if (myAnimator != null)
        {
            myAnimator.SetBool("Grounded", false);
            myAnimator.SetTrigger("Jump");
        }

        if (isGroundedLimb)
        {
            myAnimator.SetBool("Walking", false);
            myAnimator.SetBool("Running", false);
            myAnimator.SetBool("Calm", false);
            isWalking = false;
            isRunning = false;
        }

        if (isWing || isTail)
        {
            myAnimator.SetBool("Glide Activated", false);
            myAnimator.SetBool("Walking", false);
            myAnimator.SetBool("Running", false);
            isWalking = false;
            isRunning = false;

            if (isArm)
            {
                myAnimator.SetBool("Swaying", false);
            }
        }

    }

    #endregion

    #endregion

    #region Hit Colliders and Damage Output
    //Attack Specific Functions

    #region Jab and Slash Specific Functions
    public void triggerJabOrSlashHitDetect() //marks whether or not the hit VFX is needed
    {
        jabOrSlashLanded = true;
        //play out VFX

        if (attackMarkedHeavy == true)
        {
            if (heavyHitVFXManager != null)
            {
                heavyHitVFXManager.unleashJabOrSlash();
                playSuccessfulHeavyAttack();
            }
        }
        else
        {
            if (neutralHitVFXManager != null)
            {
                neutralHitVFXManager.unleashJabOrSlash();
                playSuccessNeutralAttack();
            }
        }
    }

    public void triggerJabOrSlashCollisionsOn() //called in attack animation
    {
        //turn on neutral vfx holder
        jabOrSlashLanded = false;

        if (attackMarkedHeavy == true && heavyCollider != null)
        {
            heavyCollider.enabled = true;
        }
        else if (attackMarkedHeavy == false && neutralCollider != null)
        {
            neutralCollider.enabled = true;
        }

    }

    public void triggerJabOrSlashCollisionsOff() //called in attack animation
    {
        //turn off neutral vfx holder
        jabOrSlashLanded = false;

        if (attackMarkedHeavy == true)
        {
            heavyCollider.enabled = false;
            heavyColliderReference.resetAttackHistory();
        }
        else
        {
            neutralCollider.enabled = false;
            neutralColliderReference.resetAttackHistory();
        }

    }
    #endregion

    #region Reel Attack Specific Functions
    public void triggerReelHitDetect() //marks whether or not the hit VFX is needed
    {
        //jabOrSlashLanded = true;
        reelAttackLanded = true;
        //play out VFX

        if (attackMarkedHeavy == true)
        {
            heavyHitVFXManager.unleashReelInVisual();
            triggerReelCollisionsOff();
            reelAttackBuiltUpPower = 0; //the reason that this always starts at 1 is that just by initiating the heavy, players are given a power up
            reelAttackCurrentThreshold = 0;
            powerUpCheckAllowed = false;
            myAnimator.ResetTrigger("Reel Back");
            myAnimator.SetTrigger("Reel Back");
        }
    }

    public void triggerReelCollisionsOn() //called in attack animation
    {
        //turn on neutral vfx holder
        myAnimator.ResetTrigger("Reel Back");
        reelAttackLanded = false;

        if (attackMarkedHeavy == true && heavyCollider != null)
        {
            heavyCollider.enabled = true;
        }
    }

    public void triggerReelCollisionsOff() //called in attack animation
    {
        //turn off neutral vfx holder
        reelAttackLanded = false;
        reelAttackBuiltUpPower = 0;
        reelAttackCurrentThreshold = 0;
        powerUpCheckAllowed = false;

        if (attackMarkedHeavy == true)
        {
            heavyCollider.enabled = false;
        }

    }
    #endregion

    #region Stomp Attack Specific Functions
    public void triggerStompDetectorOn()
    {
        stompDetection.enabled = true;
        myMainSystem.stompAttack();
    }

    public void triggerStompDetectionOff()
    {
        stompDetection.enabled = false;
    }
    #endregion

    #region Heavy Attack Power Up

    public void triggerHeavyAttackPowerUp() //built up in wind up animation
    {
        if (reelHeavyAttack)
        {
            reelAttackBuiltUpPower++;
            powerUpCheckAllowed = true;
        }
        else if (!beamHeavyAttack)
        {
            builtUpAttackPower++;
        }
    }

    public void triggerHeavyAttackPowerCheck() //called at same time intervals as power up but is instead called in the heavy animation 
    {
        if (reelHeavyAttack && powerUpCheckAllowed)
        {
            reelAttackCurrentThreshold++;

            if (reelAttackCurrentThreshold == reelAttackBuiltUpPower)
            {
                reelAttackBuiltUpPower = 0;
                reelAttackCurrentThreshold = 0;
                powerUpCheckAllowed = false;
                myAnimator.ResetTrigger("Reel Back");
                myAnimator.SetTrigger("Reel Back");
            }
        }
    }

    private void neutralAttackPowerCalculation() //new attack types must be added here
    {
        damage = baseNeutralAttackDamage;

        if (jabNeutralAttack || slashNeutralAttack)
        {
            neutralColliderReference.resetAttackHistory();
            neutralColliderReference.damage = damage;
            heavyColliderReference.markedHeavy = false;
        }
        else if (projectileNeutralAttack || sprayNeutralAttack || boomerangHeavyAttack)
        {
            neutralHitVFXManager.damage = damage;
            neutralHitVFXManager.updateDamageOnProjectiles();
        }

        damageClearance();
    }

    private void heavyAttackPowerCalculation() //new attack types must be added here
    {
        damage = baseHeavyAttackDamage + (builtUpAttackPower * builtUpAddedDamage);
        builtUpAttackPower = 0;

        if (jabHeavyAttack || slashHeavyAttack || grappleHeavyAttack)
        {
            heavyColliderReference.resetAttackHistory();
            heavyColliderReference.damage = damage;
            heavyColliderReference.markedHeavy = true;
        }
        else if (projectileHeavyAttack || sprayHeavyAttack || boomerangHeavyAttack)
        {
            heavyHitVFXManager.damage = damage;
            heavyHitVFXManager.updateDamageOnProjectiles();
        }
        else if (beamHeavyAttack)
        {

        }

        damageClearance();
    }
    #endregion

    private void damageClearance()
    {
        damage = 0;
    }
    #endregion

    #region Status Effects
    private void statusEffectAndDamageCalculations() //new attack types must be added here
    {
        if (jabNeutralAttack)
        {
            neutralColliderReference.damage = baseNeutralAttackDamage;
        }
        else if (slashNeutralAttack)
        {
            neutralColliderReference.damage = baseNeutralAttackDamage;
        }
        else if (projectileNeutralAttack)
        {
            neutralHitVFXManager.damage = baseNeutralAttackDamage;
            neutralHitVFXManager.updateDamageOnProjectiles();
        }
        else if (sprayNeutralAttack)
        {
            neutralHitVFXManager.damage = baseNeutralAttackDamage;
            neutralHitVFXManager.updateDamageOnSpray();
        }
        else if (boomerangNeutralAttack)
        {
            neutralHitVFXManager.damage = baseNeutralAttackDamage;
            neutralHitVFXManager.updateDamageOnProjectiles();
        }

        if (jabHeavyAttack || slashHeavyAttack)
        {
            heavyColliderReference.burnedStatusEffect = burnedStatusEffect;
            heavyColliderReference.electrifiedStatusEffect = electrifiedStatusEffect;
            heavyColliderReference.poisonedStatusEffect = poisonedStatusEffect;
            heavyColliderReference.stinkyStatusEffect = stinkyStatusEffect;
            heavyColliderReference.cursedStatusEffect = cursedStatusEffect;
            heavyColliderReference.confusedStatusEffect = confusedStatusEffect;
            heavyColliderReference.slimedStatusEffect = slimedStatusEffect;
            heavyColliderReference.frozenStatusEffect = frozenStatusEffect;
            heavyColliderReference.squashedStatusEffect = squashedStatusEffect;
            heavyColliderReference.slowedStatusEffect = slowedStatusEffect;
            heavyColliderReference.grabbedStatusEffect = grabbedStatusEffect;
        }
        else if (projectileHeavyAttack || boomerangHeavyAttack)
        {
            heavyHitVFXManager.damage = baseHeavyAttackDamage;
            heavyHitVFXManager.updateDamageOnProjectiles();
            heavyHitVFXManager.burnedStatusEffect = burnedStatusEffect;
            heavyHitVFXManager.electrifiedStatusEffect = electrifiedStatusEffect;
            heavyHitVFXManager.poisonedStatusEffect = poisonedStatusEffect;
            heavyHitVFXManager.stinkyStatusEffect = stinkyStatusEffect;
            heavyHitVFXManager.hauntedStatusEffect = cursedStatusEffect;
            heavyHitVFXManager.confusedStatusEffect = confusedStatusEffect;
            heavyHitVFXManager.slimedStatusEffect = slimedStatusEffect;
            heavyHitVFXManager.frozenStatusEffect = frozenStatusEffect;
            heavyHitVFXManager.squashedStatusEffect = squashedStatusEffect;
            heavyHitVFXManager.slowedStatusEffect = slowedStatusEffect;
            heavyHitVFXManager.grabbedStatusEffect = grabbedStatusEffect;
            heavyHitVFXManager.updateStatusEffectsOnProjectiles();
        }
        else if (beamHeavyAttack)
        {

        }
        else if (sprayHeavyAttack)
        {
            heavyHitVFXManager.damage = baseHeavyAttackDamage;
            heavyHitVFXManager.updateDamageOnSpray();
            heavyHitVFXManager.burnedStatusEffect = burnedStatusEffect;
            heavyHitVFXManager.electrifiedStatusEffect = electrifiedStatusEffect;
            heavyHitVFXManager.poisonedStatusEffect = poisonedStatusEffect;
            heavyHitVFXManager.stinkyStatusEffect = stinkyStatusEffect;
            heavyHitVFXManager.hauntedStatusEffect = cursedStatusEffect;
            heavyHitVFXManager.confusedStatusEffect = confusedStatusEffect;
            heavyHitVFXManager.slimedStatusEffect = slimedStatusEffect;
            heavyHitVFXManager.frozenStatusEffect = frozenStatusEffect;
            heavyHitVFXManager.squashedStatusEffect = squashedStatusEffect;
            heavyHitVFXManager.slowedStatusEffect = slowedStatusEffect;
            heavyHitVFXManager.grabbedStatusEffect = grabbedStatusEffect;
            heavyHitVFXManager.updateStatusEffectsOnSpray();
        }
    }
    #endregion

    #region Movement Animations
    public void triggerWalk()
    {
        if (connected == false)
        {
            return;
        }

        if (isGroundedLimb || isTorso)
        {
            myAnimator.ResetTrigger("Walk to Idle");
            myAnimator.SetBool("Walking", true);
            myAnimator.SetBool("Running", false);
            isWalking = true;
            isRunning = false;

            if (isLeg)
            {
                myAnimator.SetBool("Calm", false);
                myAnimator.SetBool("Teeter", false);
            }

            if (isTorso)
            {
                myAnimator.SetBool("Teeter", false);
            }
        }
        else if (isHead || isWing || isTail || isArm)
        {
            myAnimator.SetBool("Walking", true);
            myAnimator.SetBool("Running", false);
            isWalking = true;
            isRunning = false;

            if (isArm)
            {
                myAnimator.SetBool("Swaying", false);
            }
        }

        endRunVisual();
    }

    public void triggerStopWalking()
    {
        if (connected == false)
        {
            return;
        }

        if (isGroundedLimb || isTorso)
        {
            myAnimator.ResetTrigger("Walk to Idle");

            if (isWalking)
            {
                myAnimator.SetBool("Walking", false);
                myAnimator.SetTrigger("Walk to Idle");
                isWalking = false;
            }
        }
        else if (isHead || isWing || isTail || isArm)
        {
            myAnimator.SetBool("Walking", false);
            isWalking = false;

            if (isArm)
            {
                myAnimator.SetBool("Swaying", false);
            }
        }
    }

    public void triggerRun()
    {
        if (connected == false)
        {
            return;
        }

        if (isGroundedLimb || isTorso)
        {
            myAnimator.ResetTrigger("Walk to Idle");
            myAnimator.SetBool("Running", true);
            myAnimator.SetBool("Walking", false);
            isWalking = false;
            isRunning = true;

            if (isLeg)
            {
                myAnimator.SetBool("Calm", false);
            }
        }

        if (isHead || isWing || isArm || isTail)
        {
            myAnimator.SetBool("Running", true);
            myAnimator.SetBool("Walking", false);
            isWalking = false;
            isRunning = true;

            if (isArm)
            {
                myAnimator.SetBool("Swaying", false);
            }
        }
    }

    public void triggerStopRunning()
    {
        if (connected == false)
        {
            return;
        }

        if (isGroundedLimb || isTorso)
        {
            if (isRunning)
            {
                myAnimator.ResetTrigger("Walk to Idle");
                myAnimator.SetBool("Running", false);
                isRunning = false;
            }
        }

        if (isHead || isWing || isArm || isTail)
        {
            myAnimator.SetBool("Running", false);
            isRunning = false;

            if (isArm)
            {
                myAnimator.SetBool("Swaying", false);
            }
        }

        endRunVisual();
    }

    public void triggerScreechingStop()
    {
        if (connected == false)
        {
            return;
        }

        if (isGroundedLimb || isTorso)
        {
            myAnimator.SetTrigger("Run to Screech");
        }
    }

    public void triggerJump()
    {
        if (connected == false || isDecor || attackFocusOn || (isTorso && isBracing))
        {
            return;
        }

        if (myAnimator != null)
        {
            myAnimator.SetBool("Grounded", false);
            myAnimator.SetTrigger("Jump");
        }

        if (isGroundedLimb || isTorso)
        {
            if (isLeg)
            {
                myAnimator.SetBool("Calm", false);
            }
        }

        if (isHead || isWing || isArm || isTail || isTorso)
        {
            myAnimator.SetBool("Glide Activated", false);
            myAnimator.SetBool("Walking", false);
            isWalking = false;

            if (isArm)
            {
                myAnimator.SetBool("Swaying", false);
            }
        }

        grounded = false;
        endRunVisual();
    }

    public void triggerRoll(bool groundedWhenTriggered, bool trueRoll)
    {
        if (connected == false || isDecor || isHorn)
        {
            return;
        }

        if (isHorn && myAnimator != null)
        {
            myAnimator.SetBool("Grounded", groundedWhenTriggered);
            grounded = groundedWhenTriggered;
        }

        if (myAnimator != null)
        {
            myAnimator.SetBool("Grounded", groundedWhenTriggered);
            myAnimator.SetTrigger("Roll");
        }

        if (isGroundedLimb || isHead || isWing || isTail || isTorso)
        {
            if (trueRoll)
            {
                myAnimator.SetBool("Walking", false);
                myAnimator.SetBool("Running", false);
                isWalking = false;
                isRunning = false;
            }
            else
            {
                myAnimator.SetBool("Walking", false);
                isWalking = false;
            }

            if (isWing || isHead)
            {
                myAnimator.SetBool("Glide Activated", false);
            }

            if (isLeg)
            {
                myAnimator.SetBool("Calm", false);
            }

            if (isTorso)
            {
                myAnimator.SetBool("Glide Activated", false);
            }
        }

        if (isArm)
        {
            myAnimator.SetBool("Glide Activated", false);
            myAnimator.SetBool("Swaying", false);

            if (trueRoll)
            {
                myAnimator.SetBool("Running", false);
                isWalking = false;
                isRunning = false;
            }
            else
            {
                isWalking = false;
            }
        }

        grounded = groundedWhenTriggered;
        stopInfiniteRoll();
        endRunVisual();
    }

    public void triggerWingFlap()
    {
        if (connected == false)
        {
            return;
        }

        if (isTorso)
        {
            myAnimator.SetTrigger("Upper Flap"); //change this so that its calculated at start with the other animations
            //allows us to use something like "lower flap" for wings on the butt
        }

        if (isWing)
        {
            myAnimator.SetTrigger("Big Flap");
        }

        if (isArm)
        {
            myAnimator.SetTrigger("Roll");
        }

        if (isLeg || isHead || isTail)
        {
            myAnimator.SetTrigger("Jump");
        }

        if ((isMouth || isEye) && myAnimator != null)
        {
            if (isMouth)
            {
                myAnimator.SetTrigger("Roll");
            }
            else
            {
                myAnimator.SetTrigger("Brace");
            }
        }
    }

    public void correctRollSpamControl()
    {
        myMainSystem.correctRollControl();
    }

    //This fall function is saved for when the player is knocked off an edge or walks over an edge (not a jump related fall)
    public void triggerFall()
    {
        if (connected == false || isDecor || isHorn)
        {
            return;
        }

        if (myAnimator != null)
        {
            myAnimator.SetBool("Grounded", false);
            myAnimator.SetTrigger("Fall");
        }

        if (isGroundedLimb || isTorso)
        {
            if (isLeg)
            {
                myAnimator.SetBool("Calm", false);
            }
        }

        if (isHead || isWing || isArm || isTail || isTorso)
        {
            if (isArm)
            {
                myAnimator.SetBool("Swaying", false);
            }
        }

        grounded = false;
        endRunVisual();
    }

    public void triggerSimpleUngrounded()
    {
        if (connected == false || isDecor || isHorn)
        {
            return;
        }

        if (myAnimator != null)
        {
            myAnimator.SetBool("Grounded", false);
        }

        if (isGroundedLimb || isTorso)
        {
            if (isLeg)
            {
                myAnimator.SetBool("Calm", false);
            }
        }

        if (isHead || isWing || isArm || isTail || isTorso)
        {
            if (isArm)
            {
                myAnimator.SetBool("Swaying", false);
            }
        }

        grounded = false;
        endRunVisual();
    }

    public void triggerLand()
    {
        attackFocusOn = false;

        if (connected == false || isDecor || isHorn)
        {
            return;
        }

        if (isHorn && myAnimator != null)
        {
            myAnimator.SetBool("Grounded", true);
            grounded = true;
        }

        if (myAnimator != null)
        {
            myAnimator.SetBool("Grounded", true);

            if (isGroundedLimb || isTorso || isHead || isArm || isLeg)
            {
                myAnimator.SetTrigger("Land");
            }
        }

        if (isGroundedLimb || isTorso || isHead || isWing || isTail)
        {
            if (isWing || isHead)
            {
                myAnimator.SetBool("Glide Activated", false);
            }

            if (isLeg)
            {
                myAnimator.SetBool("Calm", false);
            }

            if (isTorso)
            {
                myAnimator.SetBool("Glide Activated", false);
            }
        }

        if (isArm)
        {
            myAnimator.SetBool("Glide Activated", false);
            myAnimator.SetBool("Swaying", false);
        }

        grounded = true;
        stopInfiniteRoll();
        endRunVisual();
    }

    public void triggerLateLand()
    {
        if (connected == false || isDecor || isHorn || isTorso || isHead || isAttacking)
        {
            if (isAttacking || isTorso)
            {
                myAnimator.SetBool("Grounded", true);
            }
            return;
        }

        if (isHorn && myAnimator != null)
        {
            myAnimator.SetBool("Grounded", true);
            grounded = true;
        }

        if (myAnimator != null)
        {
            myAnimator.SetBool("Grounded", true);

            if (isGroundedLimb || isHead || isArm || isLeg)
            {
                myAnimator.SetTrigger("Land");
            }
        }

        if (isGroundedLimb || isTorso || isHead || isWing || isTail)
        {
            if (isWing ||isHead)
            {
                myAnimator.SetBool("Glide Activated", false);
            }

            if (isLeg)
            {
                myAnimator.SetBool("Calm", false);
            }

            if (isTorso)
            {
                myAnimator.SetBool("Glide Activated", false);
            }
        }

        if (isArm)
        {
            myAnimator.SetBool("Glide Activated", false);
            myAnimator.SetBool("Swaying", false);
        }

        grounded = true;
        stopInfiniteRoll();
        endRunVisual();
    }

    public void triggerGlide()
    {
        if (connected == false)
        {
            return;
        }

        if (isWing || isHead || isArm || isTail || isTorso)
        {
            myAnimator.SetBool("Glide Activated", true);
        }

        stopInfiniteRoll();
    }

    public void triggerCrouch()
    {
        if (connected == false || isHorn || isDecor || isEye || isMouth)
        {
            return;
        }

        isWalking = false;
        isRunning = false;

        if (isTorso)
        {
            myAnimator.SetBool("Crouching", true);
            myAnimator.SetTrigger("Crouch");
            myAnimator.SetBool("Walking", false);
            myAnimator.SetBool("Running", false);
        }

        if (isGroundedLimb)
        {
            myAnimator.SetBool("Crouching", true);
            myAnimator.SetTrigger("Crouch");
            myAnimator.SetBool("Calm", false);
            myAnimator.SetBool("Walking", false);
            myAnimator.SetBool("Running", false);
        }

        if (isArm)
        {
            myAnimator.SetBool("Crouching", true);
            myAnimator.SetTrigger("Crouch");
            myAnimator.SetBool("Swaying", false);
            myAnimator.SetBool("Walking", false);
            myAnimator.SetBool("Running", false);
        }

        if (isTail)
        {
            myAnimator.SetBool("Crouching", true);
            myAnimator.SetTrigger("Crouch");
            myAnimator.SetBool("Walking", false);
            myAnimator.SetBool("Running", false);
        }

        if (isWing || isHead)
        {
            myAnimator.SetBool("Walking", false);
            myAnimator.SetBool("Running", false);
        }

        endRunVisual();
    }

    public void triggerCrouchStop()
    {
        if (connected == false || isHorn || isDecor || isEye || isMouth)
        {
            return;
        }

        if (isGroundedLimb || isTorso || isArm || isTail)
        {
            myAnimator.SetBool("Crouching", false);
        }
    }

    public void triggerForceFall()
    {
        if (connected == false || isHorn || isDecor || isEye || isMouth || grounded)
        {
            return;
        }

        if (isTorso)
        {
            myAnimator.SetBool("Force Falling", true);
        }

        if (isArm)
        {
            myAnimator.SetBool("Force Falling", true);
        }

        if (isTail)
        {
            myAnimator.SetBool("Force Falling", true);
        }

        endRunVisual();
    }

    public void triggerForceFallStop()
    {
        if (connected == false || isHorn || isDecor || isEye || isMouth)
        {
            return;
        }

        if (isTorso || isArm || isTail)
        {
            myAnimator.SetBool("Force Falling", false);
        }
    }

    #endregion

    #region Health

    public void disconnectThisPart()
    {
        if (monsterPartID == 1)
        {
            connected = false;
            myAnimator.SetTrigger("Blank State");
            disableOutline();
            this.transform.parent = null;
            this.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    public void turnOffPhysics()
    {
        if (monsterPartID == 1)
        {
            this.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    public void triggerNeutralDamage(bool damageAltNeeded)
    {
        if (connected == false || isDecor || isHorn)
        {
            return;
        }

        if (attackFocusOn)
        {
            attackFocusOn = false;
            isAttacking = false;
            fullActiveHeavy = false;
            attackMarkedHeavy = false;
            heavyAttackInMotion = false;
            connectedMonsterPart.SetBool("Attack to Idle", false);
            connectedMonsterPart.SetBool("Ready to Swing", false);

            if (hasTorsoCommandOverride)
            {
                mainTorso.SetBool("Attack to Idle", false);
                mainTorso.SetBool("Ready to Swing", false);
            }

            if (reelHeavyAttack)
            {
                myMainSystem.grabbingCanceled();
            }
        }

        if (myAnimator != null)
        {
            myAnimator.SetTrigger("Neutral Damage");
            myAnimator.ResetTrigger("Recover");
            isAttacking = false;
        }

        if (isGroundedLimb || isTorso || isHead || isTail)
        {
            myAnimator.SetBool("Walking", false);
            myAnimator.SetBool("Running", false);
            isWalking = false;
            isRunning = false;

            if (isTorso)
            {
                myAnimator.SetBool("Teeter", false);
                myAnimator.ResetTrigger("Upper Flap");
                myAnimator.SetBool("Damage Alt", damageAltNeeded);
            }

            if (isGroundedLimb)
            {
                myAnimator.SetBool("Teeter", false);
            }
        }

        if (isArm)
        {
            myAnimator.SetBool("Running", false);
            myAnimator.SetBool("Swaying", false);
            isWalking = false;
            isRunning = false;
        }

        if (isWing)
        {
            myAnimator.SetBool("Glide Activated", false);
            myAnimator.ResetTrigger("Big Flap");
        }

        stopInfiniteRoll();
    }

    public void triggerNeutralDamageRecovery()
    {
        if (connected == false || isDecor || isHorn)
        {
            return;
        }

        if (myAnimator != null)
        {
            myAnimator.SetTrigger("Recover");
        }
    }

    public void triggerHeavyDamageRecovery()
    {
        if (connected == false || isDecor || isHorn)
        {
            return;
        }

        if (myAnimator != null)
        {
            myAnimator.SetTrigger("Recover");
        }
    }

    public void triggerHeavyDamage()
    {
        if (connected == false || isDecor || isHorn)
        {
            return;
        }

        if (attackFocusOn)
        {
            attackFocusOn = false;
            isAttacking = false;
            fullActiveHeavy = false;
            attackMarkedHeavy = false;
            heavyAttackInMotion = false;
            connectedMonsterPart.SetBool("Attack to Idle", false);
            connectedMonsterPart.SetBool("Ready to Swing", false);

            if (hasTorsoCommandOverride)
            {
                mainTorso.SetBool("Attack to Idle", false);
                mainTorso.SetBool("Ready to Swing", false);
            }

            if (reelHeavyAttack)
            {
                myMainSystem.grabbingCanceled();
            }
        }

        if (myAnimator != null)
        {
            myAnimator.SetTrigger("Heavy Damage");
            isAttacking = false;
        }

        if (isGroundedLimb || isTorso ||isHead || isTail)
        {
            myAnimator.SetBool("Walking", false);
            myAnimator.SetBool("Running", false);
            isWalking = false;
            isRunning = false;

            if (isTorso)
            {
                myAnimator.SetBool("Teeter", false);
                myAnimator.ResetTrigger("Upper Flap");
            }

            if (isGroundedLimb)
            {
                myAnimator.SetBool("Teeter", false);
            }
        }

        if (isArm)
        {
            myAnimator.SetBool("Running", false);
            myAnimator.SetBool("Swaying", false);
            isWalking = false;
            isRunning = false;
        }

        if (isWing)
        {
            myAnimator.SetBool("Glide Activated", false);
            myAnimator.ResetTrigger("Big Flap");
        }

        stopInfiniteRoll();
    }

    public void triggerDamageAirtime()
    {
        if (connected == false || isDecor ||isHorn)
        {
            return;
        }

        if (isTorso)
        {
            myAnimator.SetTrigger("Airtime");
        }
    }

    public void triggerLaunch()
    {
        if (connected == false || isDecor || isHorn)
        {
            return;
        }

        if (attackFocusOn)
        {
            attackFocusOn = false;
            isAttacking = false;
            fullActiveHeavy = false;
            attackMarkedHeavy = false;
            heavyAttackInMotion = false;
            connectedMonsterPart.SetBool("Attack to Idle", false);
            connectedMonsterPart.SetBool("Ready to Swing", false);

            if (hasTorsoCommandOverride)
            {
                mainTorso.SetBool("Attack to Idle", false);
                mainTorso.SetBool("Ready to Swing", false);
            }

            if (reelHeavyAttack)
            {
                myMainSystem.grabbingCanceled();
            }
        }

        if (isTorso)
        {
            myAnimator.SetBool("Grounded", false);
            myAnimator.SetBool("Teeter", false);
            myAnimator.ResetTrigger("Upper Flap");
            myAnimator.SetTrigger("Launch");
            myAnimator.ResetTrigger("Recover");
            return;
        }

        if (isGroundedLimb)
        {
            myAnimator.SetBool("Grounded", false);
            myAnimator.SetBool("Teeter", false);
            myAnimator.SetTrigger("Launch");
            myAnimator.ResetTrigger("Recover");
            return;
        }

        if (isArm)
        {
            myAnimator.SetBool("Grounded", false);
            myAnimator.SetTrigger("Heavy Damage");
            myAnimator.SetBool("Running", false);
            myAnimator.SetBool("Swaying", false);
            myAnimator.ResetTrigger("Recover");
            isWalking = false;
            isRunning = false;
            return;
        }

        if (isHead || isTail)
        {
            myAnimator.SetBool("Grounded", false);
            myAnimator.SetTrigger("Heavy Damage");
            myAnimator.SetBool("Walking", false);
            myAnimator.SetBool("Running", false);
            myAnimator.ResetTrigger("Recover");
            isWalking = false;
            isRunning = false;
            return;
        }

        if (myAnimator != null)
        {
            myAnimator.SetBool("Grounded", false);
            myAnimator.SetTrigger("Neutral Damage");
            myAnimator.ResetTrigger("Recover");
            isAttacking = false;
        }

        if (isArm)
        {
            myAnimator.SetBool("Grounded", false);
            myAnimator.SetBool("Running", false);
            myAnimator.SetBool("Swaying", false);
            isWalking = false;
            isRunning = false;
        }

        if (isWing)
        {
            myAnimator.SetBool("Grounded", false);
            myAnimator.SetBool("Glide Activated", false);
            myAnimator.ResetTrigger("Big Flap");
        }
    }

    #endregion

    #region VFX

    private void idleVFXSeparation()
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

    private void setUpVFX()//new attack projectile-like types must be added here
    {

        #region Neutral Hit VFX Holder
        if (neutralHitVFXHolder != null)
        {
            if (neutralHitVFXHolder.GetComponent<vfxHolder>() != null)
            {
                neutralHitVFXManager = neutralHitVFXHolder.GetComponent<vfxHolder>();
            }

            if (projectileNeutralAttack || sprayNeutralAttack || boomerangNeutralAttack)
            {
                if (boomerangNeutralAttack)
                {
                    neutralHitVFXManager.isBoomerangHolder = true;
                }

                neutralVFXStoredParent = neutralHitVFXHolder.transform.parent;
                neutralVFXStoredPosition = transform.localPosition;
                neutralVFXStoredRotation = transform.localRotation;
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

            if (projectileNeutralAttack || sprayNeutralAttack || boomerangNeutralAttack)
            {
                neutralVFXStoredParent = neutralHitVFXHolder.transform.parent;
                neutralVFXStoredPosition = transform.localPosition;
                neutralVFXStoredRotation = transform.localRotation;
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

            if (projectileHeavyAttack || sprayHeavyAttack || boomerangHeavyAttack)
            {
                if (boomerangHeavyAttack)
                {
                    heavyHitVFXManager.isBoomerangHolder = true;
                }

                heavyVFXStoredParent = heavyHitVFXHolder.transform.parent;
                heavyVFXStoredPosition = transform.localPosition;
                heavyVFXStoredRotation = transform.localRotation;
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

            if (projectileHeavyAttack || sprayHeavyAttack || boomerangHeavyAttack)
            {
                heavyVFXStoredParent = heavyHitVFXHolder.transform.parent;
                heavyVFXStoredPosition = transform.localPosition;
                heavyVFXStoredRotation = transform.localRotation;
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

            if (projectileHeavyAttack || sprayHeavyAttack || boomerangHeavyAttack)
            {
                heavyVFXStoredParent = heavyHitVFXHolder.transform.parent;
                heavyVFXStoredPosition = transform.localPosition;
                heavyVFXStoredRotation = transform.localRotation;
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

    public void endRunVisual()
    {
        if (specialRunVisual != null)
        {
            specialRunVisual.SetActive(false);
        }
    }

    public void triggerStompVisual()
    {
        if (neutralStompVFXManager != null && attackMarkedHeavy == false)
        {
            neutralStompVFXManager.unleashAdditionalSprayVisual();
        }
        else if (heavyStompVFXManager != null && attackMarkedHeavy == true)
        {
            heavyStompVFXManager.unleashAdditionalSprayVisual();
        }
    }

    public void triggerNeutralAttackVisuals() //called in attack animation //new attack types must be added here
    {
        if (jabNeutralAttack)
        {
            if (jabOrSlashLanded == false && neutralMissVFXHolder != null)
            {
                //turn on miss visual if neutral vfx holder's script hasn't made contact
                neutralMissVFXManager.unleashJabOrSlash();
            }
        }
        else if (slashNeutralAttack)
        {
            if (jabOrSlashLanded == false && neutralMissVFXHolder != null)
            {
                //turn on miss visual if neutral vfx holder's script hasn't made contact
                neutralMissVFXManager.unleashJabOrSlash();
            }
        }
        else if (sprayNeutralAttack)
        {
            neutralHitVFXHolder.transform.position = neutralMuzzle.transform.position;
            neutralHitVFXHolder.transform.rotation = neutralMuzzle.transform.rotation;

            if (neutralDefaultSprayVFXHolder != null)
            {
                neutralDefaultSprayVFXHolder.transform.position = neutralMuzzle.transform.position;
                neutralDefaultSprayVFXHolder.transform.rotation = neutralMuzzle.transform.rotation;
            }

            neutralHitVFXManager.unleashSpray();
            if (neutralDefaultSprayVFXManager)
            {
                neutralDefaultSprayVFXManager.unleashAdditionalSprayVisual();
            }
        }
        else if (projectileNeutralAttack && neutralAttackHitVFXArray.Length != 0)
        {
            neutralHitVFXManager.faceRightDirection(facingRight);
            neutralHitVFXManager.unleashSingleProjectile();

            if (neutralDefaultSprayVFXManager)
            {
                neutralDefaultSprayVFXManager.unleashAdditionalSprayVisual();
            }
        }
        else if (boomerangNeutralAttack && neutralAttackHitVFXArray.Length != 0)
        {
            neutralHitVFXManager.faceRightDirection(facingRight);
            neutralHitVFXManager.unleashSingleProjectile();

            if (neutralDefaultSprayVFXManager)
            {
                neutralDefaultSprayVFXManager.unleashAdditionalSprayVisual();
            }
        }
    }

    public void triggerNeutralSwingVisual()
    {
        if (neutralForwardSwingVFXManager && attackAnimationID == 1)
        {
            neutralForwardSwingVFXManager.unleashAdditionalSprayVisual();
        }
        else if (neutralBackwardSwingVFXManager && attackAnimationID == -1)
        {
            neutralBackwardSwingVFXManager.unleashAdditionalSprayVisual();
        }
        else if (neutralDownwardSwingVFXManager && attackAnimationID == 0)
        {
            neutralDownwardSwingVFXManager.unleashAdditionalSprayVisual();
        }
    }

    public void triggerHeavyAttackVisuals() //new attack types must be added here
    {
        if (jabHeavyAttack)
        {
            if (jabOrSlashLanded == false && heavyMissVFXHolder != null)
            {
                //turn on miss visual if neutral vfx holder's script hasn't made contact
                heavyMissVFXManager.unleashJabOrSlash();
            }
        }
        else if (slashHeavyAttack)
        {
            if (jabOrSlashLanded == false && heavyMissVFXHolder != null)
            {
                //turn on miss visual if neutral vfx holder's script hasn't made contact
                heavyMissVFXManager.unleashJabOrSlash();
            }

        }
        else if (sprayHeavyAttack)
        {
            heavyHitVFXHolder.transform.position = heavyMuzzle.transform.position;
            heavyHitVFXHolder.transform.rotation = heavyMuzzle.transform.rotation;

            if (heavyDefaultSprayVFXHolder != null)
            {
                heavyDefaultSprayVFXHolder.transform.position = heavyMuzzle.transform.position;
                heavyDefaultSprayVFXHolder.transform.rotation = heavyMuzzle.transform.rotation;

            }

            heavyHitVFXManager.unleashSpray();
            if (heavyDefaultSprayVFXManager)
            {
                heavyDefaultSprayVFXManager.unleashAdditionalSprayVisual();
            }
        }
        else if (projectileHeavyAttack && heavyAttackHitVFXArray.Length != 0)
        {
            heavyHitVFXHolder.transform.position = heavyMuzzle.transform.position;
            heavyHitVFXHolder.transform.rotation = heavyMuzzle.transform.rotation;

            heavyHitVFXManager.faceRightDirection(facingRight);
            heavyHitVFXManager.unleashSingleProjectile();

            if (heavyDefaultSprayVFXManager)
            {
                heavyDefaultSprayVFXManager.unleashAdditionalSprayVisual();
            }
        }
        else if (reelHeavyAttack)
        {
            if (reelAttackLanded == false)
            {
                //miss visual
                triggerReelCollisionsOff();
            }

            reelAttackBuiltUpPower = 0;
            reelAttackCurrentThreshold = 0;
            powerUpCheckAllowed = false;
        }
        else if (beamHeavyAttack)
        {
            heavyHitVFXManager.unleashBeamVisual();
        }
        else if (boomerangHeavyAttack && heavyAttackHitVFXArray.Length != 0)
        {
            heavyHitVFXHolder.transform.position = heavyMuzzle.transform.position;
            heavyHitVFXHolder.transform.rotation = heavyMuzzle.transform.rotation;

            heavyHitVFXManager.faceRightDirection(facingRight);
            heavyHitVFXManager.unleashSingleProjectile();

            if (heavyDefaultSprayVFXManager)
            {
                heavyDefaultSprayVFXManager.unleashAdditionalSprayVisual();
            }
        }
    }

    public void triggerHeavySwingVisual()
    {
        if (heavyForwardSwingVFXManager && attackAnimationID == 1)
        {
            heavyForwardSwingVFXManager.unleashAdditionalSprayVisual();
        }
        else if (heavyBackwardSwingVFXManager && attackAnimationID == -1)
        {
            heavyBackwardSwingVFXManager.unleashAdditionalSprayVisual();
        }
        else if (heavyDownwardSwingVFXManager && attackAnimationID == 0)
        {
            heavyDownwardSwingVFXManager.unleashAdditionalSprayVisual();
        }
    }

    public void endRemainingVFX()
    {
        if (beamHeavyAttack)
        {
            heavyHitVFXManager.endBeamVisual();
        }

        endRunVisual();
    }

    #endregion

    #region SFX

    public void setUpSFX()
    {
        myPartAudio = GetComponent<AudioSource>();
    }

    public void playSuccessNeutralAttack()
    {
        if (myPartAudio != null && neutralAttackHitAudioLibrary[0] != null)
        {
            if (neutralHitCounter < neutralAttackHitAudioLibrary.Length - 1)
            {
                neutralHitCounter++;
            }
            else
            {
                neutralHitCounter = 0;
            }

            myPartAudio.Stop();
            myPartAudio.clip = neutralAttackHitAudioLibrary[neutralHitCounter];
            myPartAudio.Play();
        }
    }

    public void playSuccessfulHeavyAttack()
    {
        if (myPartAudio != null && heavyAttackHitAudioLibrary[0] != null)
        {
            if (heavyHitCounter < heavyAttackHitAudioLibrary.Length - 1)
            {
                heavyHitCounter++;
            }
            else
            {
                heavyHitCounter = 0;
            }

            myPartAudio.Stop();
            myPartAudio.clip = heavyAttackHitAudioLibrary[heavyHitCounter];
            myPartAudio.Play();
        }
    }

    #endregion

    #region Reaction Animations

    public void triggerVisualDissappearance()
    {
        if (connected == false)
        {
            return;
        }

        for (int i = 0; i < mySkinnedMeshRenderers.Length; i++)
        {
            mySkinnedMeshRenderers[i].enabled = false;
        }

        for (int i = 0; i < myMeshRenderers.Length; i++)
        {
            myMeshRenderers[i].enabled = false;
        }

        for (int i = 0; i < mySpriteRenderers.Length; i++)
        {
            mySpriteRenderers[i].enabled = false;
        }

        for (int i = 0; i < myIdleVFX.Length; i++)
        {
            myIdleVFX[i].gameObject.SetActive(false);
        }

        endRemainingVFX();
    }

    public void triggerVisualReappearance()
    {
        if (connected == false)
        {
            return;
        }

        for (int i = 0; i < mySkinnedMeshRenderers.Length; i++)
        {
            mySkinnedMeshRenderers[i].enabled = true;
        }

        for (int i = 0; i < myMeshRenderers.Length; i++)
        {
            myMeshRenderers[i].enabled = true;
        }

        for (int i = 0; i < mySpriteRenderers.Length; i++)
        {
            mySpriteRenderers[i].enabled = true;
        }

        for (int i = 0; i < myIdleVFX.Length; i++)
        {
            myIdleVFX[i].gameObject.SetActive(true);
        }
    }

    public void calmedDown()
    {
        if (connected == false)
        {
            return;
        }

        if (isGroundedLimb)
        {
            myAnimator.SetBool("Calm", true);
        }
    }

    public void teeterOnEdge()
    {
        if (connected == false)
        {
            return;
        }

        if (isTorso)
        {
            myAnimator.SetBool("Teeter", true);
        }

        if (isGroundedLimb)
        {
            myAnimator.SetBool("Calm", true);
            myAnimator.SetBool("Teeter", true);
        }

        if (isArm)
        {
            myAnimator.SetBool("Swaying", true);
            myAnimator.SetBool("Teeter", true);
        }
    }

    public void reelBackReaction()
    {
        if (attackFocusOn)
        {
            myAnimator.SetTrigger("Reel Back");
        }
    }

    

    public void triggerChargeForward()
    {
        if (attackFocusOn)
        {
            heavyCollider.gameObject.GetComponent<monsterPartReference>().isFullyChargedHeavy = true;
            myAnimator.SetBool("Charge Attack Active", true);
            myMainSystem.chargeForward();
        }
    }

    public void endChargeForward()
    {
        if (attackFocusOn)
        {
            myAnimator.SetBool("Charge Attack Active", false);
        }
    }

    #endregion

    #region Emote Animations
    public void fierceEmote()
    {
        if (connected == false)
        {
            return;
        }

        if (isTorso)
        {
            myAnimator.ResetTrigger("Force Stop Emote");
            myAnimator.SetTrigger("Fierce Emote");
        }

        if (isGroundedLimb)
        {
            myAnimator.ResetTrigger("Force Stop Emote");
            myAnimator.SetTrigger("Emote");
            myAnimator.SetBool("Calm", false);
        }


    }

    public void gasEmote()
    {
        if (connected == false)
        {
            return;
        }

        if (isTorso)
        {
            myAnimator.ResetTrigger("Force Stop Emote");
            myAnimator.SetTrigger("Gas Emote");
        }

        if (isGroundedLimb)
        {
            myAnimator.ResetTrigger("Force Stop Emote");
            myAnimator.SetTrigger("Emote");
            myAnimator.SetBool("Calm", false);
        }
    }

    public void mockingEmote()
    {
        if (connected == false)
        {
            return;
        }

        if (isTorso)
        {
            myAnimator.ResetTrigger("Force Stop Emote");
            myAnimator.SetTrigger("Mocking Emote");
        }

        if (isGroundedLimb)
        {
            myAnimator.ResetTrigger("Force Stop Emote");
            myAnimator.SetTrigger("Emote");
            myAnimator.SetBool("Calm", false);
        }

        if (isArm)
        {
            myAnimator.ResetTrigger("Force Stop Emote");
            myAnimator.SetBool("Swaying", true);
        }
    }

    public void danceEmote()
    {
        if (connected == false)
        {
            return;
        }

        if (isTorso)
        {
            myAnimator.ResetTrigger("Force Stop Emote");
            myAnimator.SetTrigger("Dance Emote");
        }

        if (isGroundedLimb)
        {
            myAnimator.ResetTrigger("Force Stop Emote");
            myAnimator.SetTrigger("Emote");
            myAnimator.SetBool("Calm", false);
        }

        if (isArm)
        {
            myAnimator.ResetTrigger("Force Stop Emote");
            myAnimator.SetBool("Dancing", true);
        }
    }

    public void jackEmote()
    {
        if (connected == false)
        {
            return;
        }

        if (isTorso)
        {
            myAnimator.ResetTrigger("Force Stop Emote");
            myAnimator.SetTrigger("Jack Emote");
        }

        if (isGroundedLimb)
        {
            myAnimator.ResetTrigger("Force Stop Emote");
            myAnimator.SetTrigger("Emote");
            myAnimator.SetBool("Calm", false);
        }

        if (isArm)
        {
            myAnimator.ResetTrigger("Force Stop Emote");
            myAnimator.SetBool("Swaying", true);
        }
    }

    public void thinkingEmote()
    {

    }

    public void booEmote()
    {
        if (connected == false)
        {
            return;
        }

        if (isTorso)
        {
            myAnimator.ResetTrigger("Force Stop Emote");
            myAnimator.SetTrigger("Boo Emote");
        }

        if (isGroundedLimb)
        {
            myAnimator.ResetTrigger("Force Stop Emote");
            myAnimator.SetTrigger("Emote");
            myAnimator.SetBool("Calm", false);
        }
    }

    public void excerciseEmote()
    {

    }

    public void hulaEmote()
    {
        if (connected == false)
        {
            return;
        }

        if (isTorso)
        {
            myAnimator.ResetTrigger("Force Stop Emote");
            myAnimator.SetTrigger("Hula Emote");
        }

        if (isGroundedLimb)
        {
            myAnimator.ResetTrigger("Force Stop Emote");
            myAnimator.SetTrigger("Emote");
            myAnimator.SetBool("Calm", false);
        }

        if (isArm)
        {
            myAnimator.ResetTrigger("Force Stop Emote");
            myAnimator.SetBool("Swaying", true);
        }
    }

    public void vomitEmote()
    {
        if (connected == false)
        {
            return;
        }

        if (isTorso)
        {
            myAnimator.ResetTrigger("Force Stop Emote");
            myAnimator.SetTrigger("Vomit Emote");
        }

        if (isGroundedLimb)
        {
            myAnimator.ResetTrigger("Force Stop Emote");
            myAnimator.SetTrigger("Emote");
            myAnimator.SetBool("Calm", false);
        }
    }

    public void brianEmote()
    {

    }

    public void sleepEmote()
    {
        if (connected == false)
        {
            return;
        }

        if (isTorso)
        {
            myAnimator.ResetTrigger("Force Stop Emote");
            myAnimator.SetTrigger("Sleep Emote");
        }

        if (isGroundedLimb)
        {
            myAnimator.ResetTrigger("Force Stop Emote");
            myAnimator.SetTrigger("Emote");
            myAnimator.SetBool("Calm", false);
        }
    }

    public void explosiveEmote()
    {
        if (connected == false)
        {
            return;
        }

        if (isTorso)
        {
            myAnimator.ResetTrigger("Force Stop Emote");
            myAnimator.SetTrigger("Boo Emote");
        }

        if (isGroundedLimb)
        {
            myAnimator.ResetTrigger("Force Stop Emote");
            myAnimator.SetTrigger("Emote");
            myAnimator.SetBool("Calm", false);
        }

        if (isArm)
        {
            myAnimator.ResetTrigger("Force Stop Emote");
            myAnimator.SetBool("Swaying", true);
        }
    }

    public void laughingEmote()
    {

    }

    public void sneezingEmote()
    {
        if (connected == false)
        {
            return;
        }

        if (isTorso)
        {
            myAnimator.ResetTrigger("Force Stop Emote");
            myAnimator.SetTrigger("Sneeze Emote");
        }

        if (isGroundedLimb)
        {
            myAnimator.ResetTrigger("Force Stop Emote");
            myAnimator.SetTrigger("Emote");
            myAnimator.SetBool("Calm", false);
        }

        if (isArm)
        {
            myAnimator.ResetTrigger("Force Stop Emote");
            myAnimator.SetBool("Swaying", true);
        }
    }

    public void triggerEmoteEnd()
    {
        if (connected == false)
        {
            return;
        }

        if (isTorso)
        {
            myMainSystem.emoteEnded();
        }
    }

    public void triggerForceStopEmote()
    {
        if (connected == false)
        {
            return;
        }

        if (isTorso)
        {
            myAnimator.SetTrigger("Force Stop Emote");
        }

        if (isGroundedLimb)
        {
            myAnimator.SetTrigger("Force Stop Emote");
            myAnimator.SetBool("Calm", false);
        }

        if (isArm)
        {
            myAnimator.SetTrigger("Force Stop Emote");
            myAnimator.SetBool("Dancing", false);
            myAnimator.SetBool("Swaying", false);
        }
    }

    public void emoteCorrections()
    {
        if (connected == false)
        {
            return;
        }

        if (isArm)
        {
            myAnimator.SetBool("Dancing", false);
            myAnimator.SetBool("Swaying", false);
        }

        if (isGroundedLimb)
        {
            myAnimator.ResetTrigger("Force Stop Emote");
            myAnimator.SetBool("Calm", true);
        }
    }

    #endregion

    #region Audio Triggers

    public void playFootstepSFXOnAnimation()
    {
        if (isLeg && myMainSystem.SFXManager)
        {
            myMainSystem.SFXManager.footstepSFX(this);
        }
    }

    public void playRunSFXOnAnimation()
    {
        if (isLeg && myMainSystem.SFXManager)
        {
            myMainSystem.SFXManager.runSFX(this);
        }
    }

    #endregion

    #region Corrections

    public void isAtEdge()
    {
        if (isGroundedLimb || isTorso || isArm)
        {
            myAnimator.SetBool("Teeter", true);
        }
    }

    public void notAtEdge()
    {
        if (isGroundedLimb || isTorso || isArm)
        {
            myAnimator.SetBool("Teeter", false);
        }
    }

    public void resetBracing()
    {
        if (connected == false)
        {
            return;
        }

        if (isAttacking == false)
        {
            if (isLeg || isArm || isWing || isTail || (isMouth && myAnimator != null))
            {
                myAnimator.ResetTrigger("Unbrace");

                if (isGroundedLimb)
                {
                    myAnimator.ResetTrigger("Switch Stance");
                    myAnimator.ResetTrigger("Switch Stance Quick");
                }
            }
        }
    }

    public void bounceCorrections(bool bounceAllowed)
    {
        if (connected == false)
        {
            return;
        }

        if (isGroundedLimb)
        {
            myAnimator.SetBool("Idle Bounce Allowed", bounceAllowed);
            myAnimator.SetBool("Calm", !bounceAllowed);
        }

        if (isTorso)
        {
            myAnimator.SetBool("Teeter", false);
        }
    }

    public void stopInfiniteRoll()
    {
        if (connected == false || isDecor || isHorn)
        {
            return;
        }

        if (myAnimator != null)
        {
            myAnimator.SetBool("Ready to Unroll", true);
        }
    }

    #endregion

    #region Cutscene and Cinematic Specifics

    public void headTurnToTarget()
    {
        if (isHead)
        {
            myAnimator.SetBool("Force Idle on Start", true);
            myAnimator.enabled = false;
            //turn on some sort of script that moves the head bone independently
        }
    }

    public void returnHeadToNormalState() 
    {
        if (isHead)
        {
            myAnimator.enabled = true;
            //turn off head turning script
            StartCoroutine(returningDelay());
        }
    }

    public void torsoTurnToTarget()
    {
        if (isTorso)
        {
            myAnimator.SetBool("Force Idle on Start", true);
            myAnimator.enabled = false;
            //turn on some sort of script that moves the top or middle spine bone independently
        }
    }

    public void returnTorsoToNormalState()
    {
        if (isTorso)
        {
            myAnimator.enabled = true;
            //turn off head turning script
            StartCoroutine(returningDelay());
        }
    }

    IEnumerator returningDelay()
    {
        yield return new WaitForSeconds(0.5f);

        if (isTorso || isHead)
        {
            myAnimator.SetBool("Force Idle on Start", false);
        }
    }

    #endregion

    #region Reloading Piece Logic 
    //allows non-projectiles to reload if intended
    public void ReloadMeleeNeutral()
    {
        isReloadedNeutral = false;
        StartCoroutine(ReloadMeleeAttack(reloadTimeNeutral, true));
    }

    public void ReloadMeleeHeavy()
    {
        isReloadedHeavy = false;
        StartCoroutine(ReloadMeleeAttack(reloadTimeHeavy, false));
    }
    private IEnumerator ReloadMeleeAttack(float reloadTime, bool isNeutral) //bool isNeutral, true == reload neutral, false == reload heavy
    {
        yield return new WaitForSeconds(reloadTime);

        if (isNeutral)
        {
            isReloadedNeutral = true;
            yield break;
        }

        isReloadedHeavy = true;
        yield break;
    }
    public bool GetReloadNeutral ()
    {
        return isReloadedNeutral;
    }

    public void SetReloadHeavy (bool reloaded)
    {
        isReloadedHeavy = reloaded;
    }

    public bool GetReloadHeavy()
    {
        return isReloadedHeavy;
    }

    public void SetReloadNeutral(bool reloaded)
    {
        isReloadedNeutral = reloaded;
    }
    #endregion

    #region optimization functions

    private bool CheckAnimState(string state)
    {
        return myAnimator.GetCurrentAnimatorStateInfo(0).IsName(state);
    }

    private bool CheckAnimState(params string[] states)
    {
        foreach (string state in states)
        {
            if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName(state))
            {
                return true;
            }
        }
        return false;
    }

    private MonsterCalculations AttackCalculationsSetUp()
    {
        MonsterCalculations calc = new MonsterCalculations();

        calc.AttackCalculationSetUp(this);

        return calc;
    }
    #endregion
}

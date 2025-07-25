using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMonsterPart : MonoBehaviour
{
    [Header("Monster Part Info")]
    public monsterAttackSystem myMainSystem;
    private SkinnedMeshRenderer[] mySkinnedMeshRenderers;
    private MeshRenderer[] myMeshRenderers;
    private SpriteRenderer[] mySpriteRenderers;
    public Animator connectedMonsterPart;
    public Animator mainTorso;
    [HideInInspector] public Animator myAnimator;
    [HideInInspector] public MonsterPartVisual PartVisual;
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
   
#if UNITY_EDITOR
    void OnValidate()
    {
        if (neutralAttack == null)
            neutralAttack = new NeutralAttack();

        if (heavyAttack == null)
            heavyAttack = new HeavyAttack();
    }
#endif
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

    public MonsterPartType PartType;

    [Header("Damage and Status Effects")]
    public int baseNeutralAttackDamage = 0;
    public int baseHeavyAttackDamage = 0;
    [HideInInspector] public int builtUpAttackPower = 0;
    public int builtUpAddedDamage = 0;
    [HideInInspector] public int damage = 0;
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
    public bool grabbedStatusEffect;

   [Header("Neutral Attack Questionaire")]
   [SerializeReference] public NeutralAttack neutralAttack = new NeutralAttack();

    public Collider neutralCollider;
    public monsterPartReference neutralColliderReference;
    private vfxHolder neutralHitVFXManager;
    public bool needsReloadNeutral; //determines if part must be reloaded before attacking again
    public float reloadTimeNeutral = 1f; //jab/slash/ or anything not a projecile* with reload is handled here instead of projectile script
    [SerializeField] private bool isReloadedNeutral = true;
    [SerializeField] private bool isReloadedHeavy = true;
    public bool jabOrSlashLanded = false;
    public AudioClip[] neutralAttackHitAudioLibrary;
    private int neutralHitCounter = 0;

    [Header("Heavy Attack Questionaire")]

    [SerializeReference] public HeavyAttack heavyAttack = new HeavyAttack();
    //
    public Collider heavyCollider;
    public monsterPartReference heavyColliderReference;
    private vfxHolder heavyHitVFXManager;
    public Transform heavyMuzzle;
    public bool needsReloadHeavy; //for projectiles, determines if projectile must be reloaded before shooting again
    public float reloadTimeHeavy = 1f; //jab/slash/ or anything not a projecile* with reload is handled here instead of projectile script
    public AudioClip[] heavyAttackHitAudioLibrary;
    private int heavyHitCounter = 0;

    [Header("Monster Part Positioning Info")]
    [HideInInspector] public bool isJointed = true;
   
    [HideInInspector] public bool isRightSidedLimb;
    [HideInInspector] public bool isLeftSidedLimb;
    [HideInInspector] public MonsterPartConnectionPoint connectionPoint = MonsterPartConnectionPoint.None;
    [HideInInspector] public bool isGroundedLimb;
    private string torsoCommand = "";
    private string torsoCommandOverride = "";
    [HideInInspector] public bool hasTorsoCommandOverride = false; //refers to heads on torsos, torsos on torsos, torsos on heads on torsos, etc. that needs to move the main body
    private string headCommand = "";
    private bool leftAttackOverride;
    private bool rightAttackOverride;

    [Header("Internal Info - Don't Touch")]
    [HideInInspector] public bool isBracing = false;
    [HideInInspector] public bool attackMarkedHeavy = false;
    [HideInInspector] public bool heavyAttackInMotion = false;
    public bool fullActiveHeavy = false;
    public bool requiresRightStance = false;
    public bool requiresLeftStance = false;
    public bool requiresForwardStance = false;
    public bool requiresBackwardStance = false;
    public bool isLeadingLeg;
    public bool isFloatingGroundedLeg;
    public bool isFloatingTorso;
    public bool hasFlightedIdle = false;
    public Outline visualForAnimationTests;
    public bool hasHeavyBrace = false;
    [HideInInspector] public bool isAttacking = false;
    public bool attackFocusOn = false;
    [HideInInspector] public bool isWalking = false;
    public bool isRunning = false;
    public bool facingRight;
    public bool grounded = true;
    private bool haveGrabbedAMonster;

    [HideInInspector] public int reelAttackBuiltUpPower = 0;
    [HideInInspector] public int reelAttackCurrentThreshold = 0;
    [HideInInspector] public bool powerUpCheckAllowed = true;
    [HideInInspector] public bool reelAttackLanded = false;
    private monsterPartReference grabbedMonster;

    private bool forwardAttackQuickSwitch;
    private bool lowerAttackQuickSwitch;
    private bool upwardAttackQuickSwitch;
    private bool leftUpperAttackQuickSwitch;
    private bool rightUpperAttackQuickSwitch;
    private bool leftLowerAttackQuickSwitch;
    private bool rightLowerAttackQuickSwitch;

    private bool attackSetupDone = false;

    public ParticleSystem[] myIdleVFX;
    public List<monsterPartReference> referencesToIgnore = new List<monsterPartReference>();

    #region Build a Scare Tools
    public void AttackSetup()
    {
        if (attackSetupDone) { return; }

        PartVisual = GetComponent<MonsterPartVisual>();

        neutralAttack.Init(this, PartVisual);
        heavyAttack.Init(this, PartVisual);

        attackSetupDone = true;
    }
    public void changeAttackAnimationAtRuntime()
    {
        //this will be expanded to include all monster parts
        if (isLeg || isArm || isTail || isHorn || isEye)
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

            PartVisual.attackCalculations();

            PartVisual.setUpVFX();
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

    public void disableOutline() { if (visualForAnimationTests != null) visualForAnimationTests.enabled = false; }
    public void reenableOutline() { if (visualForAnimationTests != null) visualForAnimationTests.enabled = true; }

    // This is bad practice, but I have to do it this way untill I finish refactoring the script.
    public void AssignType()
    {
        if (isArm)
        {
            PartType = MonsterPartType.Arm;
        }
        else if (isLeg)
        {
            PartType = MonsterPartType.Leg;
        }
        else if(isHead)
        {
            PartType = MonsterPartType.Head;
        }
        else if(isTail)
        {
            PartType = MonsterPartType.Tail;
        }
        else if(isMouth)
        {
            PartType = MonsterPartType.Mouth;
        }
        else if (isEye)
        {
            PartType = MonsterPartType.Eye;
        }
        else if(isHorn)
        {
            PartType = MonsterPartType.Horn;
        }
        else if (isWing)
        {
            PartType = MonsterPartType.Wing;
        }
        else if (isDecor)
        {
            PartType = MonsterPartType.Decor;
        }
        else
        {
            PartType = MonsterPartType.None;
        }
    }

    #endregion

    #region Animation Set Up

    public void triggerAnimationSetUp()
    {
        myAnimator = this.GetComponent<Animator>();

        if (isLeg && isGroundedLimb == false)
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

        PartVisual.attackCalculations();

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
            PartVisual.idleVFXSeparation();
        }

        for (int i = 0; i < hitboxesAndHurtboxes.Count; i++)
        {
            hitboxesAndHurtboxes[i].gameObject.SetActive(true);
        }
        #endregion

        PartVisual.setUpVFX();
        setUpSFX();
    }

    public void triggerAnimationOffsets()
    {
        if (monsterPartID != 1 || connected == false)
        {
            return;
        }

        if (myAnimator != null)
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
            internalPartReferences[i].partReference = this.GetComponent<NewMonsterPart>();
        }

        if (neutralCollider != null)
        {
            neutralColliderReference = neutralCollider.gameObject.GetComponent<monsterPartReference>();
        }

        if (heavyCollider != null)
        {
            heavyColliderReference = heavyCollider.gameObject.GetComponent<monsterPartReference>();
        }

        heavyAttack.statusEffectAndDamageCalculations();
        neutralAttack.statusEffectAndDamageCalculations();
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

    #region Neutral to Heavy Input Shifts
    public void triggerNeutralOrHeavyRefresh(bool inputCanceled)
    {
        //most likely a canceled input after system already has registered the difference between input but before the attack has been unleashed
        //aka a canceled heavy attack

        if (fullActiveHeavy && attackFocusOn && heavyAttack is BeamHeavy beam)
        {
            if (inputCanceled)
            {
                beam.CancelAttack();
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

    public void triggerNeutralOrHeavy()
    {
        if (!attackMarkedHeavy && needsReloadNeutral)
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

        if (heavyAttack is BeamHeavy beam && attackMarkedHeavy)
        {
            beam.TriggerAttack();
            return;
        }

        if (attackMarkedHeavy)
        {

            heavyAttackInMotion = true;
            myMainSystem.switchBraceStance(); //for a stronger looking leg stance
            myMainSystem.heavyAttackActivated();
            triggerHeavyAttackPowerUp();//by triggering the heavy, 1 power up is granted
            PartVisual.triggerChargeVisual();
        }
        else
        {
            myAnimator.SetTrigger("Force Neutral Attack");
        }
    }

    public void triggerHeavyAttackPowerUp()
    {
        heavyAttack.triggerHeavyAttackPowerUp();
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

        if (heavyAttack is ReelHeavy reel)
        {
            reel.CancelGrab();
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
        if (heavyAttack is ReelHeavy)
        {
            // casting heavy attack to ReelHeavy so that we can acess the functions specific to reelHeavy
            ReelHeavy reelAttack = heavyAttack as ReelHeavy;
            reelAttack.triggerReelCollisionsOff();
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
    public void triggerHeavyAttackPowerCheck() //called at same time intervals as power up but is instead called in the heavy animation 
    {
        heavyAttack.triggerHeavyAttackPowerCheck();
    }
    #endregion

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

            if (heavyAttack is ReelHeavy reel)
            {
                reel.CancelGrab();
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

            if (heavyAttack is ReelHeavy reel)
            {
                reel.CancelGrab();
            }
        }

        if (myAnimator != null)
        {
            myAnimator.SetTrigger("Heavy Damage");
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
        if (connected == false || isDecor || isHorn)
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

            if (heavyAttack is ReelHeavy reel)
            {
                reel.CancelGrab();
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

        PartVisual.endRemainingVFX();
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
    public bool GetReloadNeutral()
    {
        return isReloadedNeutral;
    }

    public void SetReloadHeavy(bool reloaded)
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

}

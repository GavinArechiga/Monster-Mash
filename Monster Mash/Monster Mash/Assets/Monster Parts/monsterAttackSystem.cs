using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class monsterAttackSystem : MonoBehaviour
{
    public bool facingRight = false;
    public bool isGrounded = true;
    private bool isWinged = false;
    private bool fullyFlighted = false;
    public bool isFloatingMonster = false;
    private int jumpsAllowed_DoubleJump = 2;
    private int jumpsAllowed_FlightedJump = 4;
    private int jumpsLeft = 0;
    public bool isWalking = false;
    public bool isRunning = false;
    private bool isGliding = false;
    private bool isCrouching = false;
    public bool focusedAttackActive = false;
    private bool heavyAttackActive = false;
    private bool canRoll = true;
    private bool canDashAttack = true;
    float timeSinceLastCall;
    private int flourishCounter = 0;
    private float timeSinceLastDamage;
    private int damageComboCounter = 0;
    private bool damageAnimationAltNeeded = false;
    bool requiresFlourish = false;
    bool requiresFlourishingTwirl = false;
    bool requiresFlourishingRoll = false;
    bool calm = false;
    bool emoteActive = false;
    public bool onPlatformEdge;
    private bool damageLocked = false;
    private bool forceFallingActivated = false;
    private bool isLaunching = false;

    public playerController myPlayer;
    private Animator myAnimator;
    private Animator mainTorso;
    public monsterPart[] attackSlotMonsterParts = new monsterPart[8];
    private int[] attackSlotMonsterID = new int[8];
    private List<monsterPartReference> listOfInternalReferences = new List<monsterPartReference>();
    public monsterPart[] allMonsterParts;
    private List<monsterPart> nonMappedMonsterParts = new List<monsterPart>();

    [Header("Damage and Status Effects")]
    public int health = 800;
    private int monsterPartIndividualHealth = 0;
    public bool burnedStatusEffect;
    public bool electrifiedStatusEffect;
    public bool poisonedStatusEffect;
    public bool stinkyStatusEffect;
    public bool hauntedStatusEffect;
    public bool confusedStatusEffect;
    public bool slimedStatusEffect;
    public bool stunnedStatusEffect;
    public bool frozenStatusEffect;
    public bool squashedStatusEffect;
    public bool slowedStatusEffect;
    private bool grabbedStatusEffect;

    [Header("VFX")]
    public ParticleSystem partDestructionVisual;
    public ParticleSystem destructionVisual;
    public GameObject destructionPhysicsHelper;
    public ParticleSystem glideVisual;
    public ParticleSystem fieryVisual;
    public ParticleSystem gasVisual;
    public ParticleSystem musicVisual;
    public ParticleSystem spookyVisual;
    public ParticleSystem vomitVisual;
    public ParticleSystem sleepVisual;
    public Animation hulaHoopVisual;
    public ParticleSystem sneezeVisual;
    public ParticleSystem rollingAttackVisual;
    public ParticleSystem leapingAttackVisual;
    public ParticleSystem jumpVisual;
    public ParticleSystem doubleJumpVisual;
    public ParticleSystem landVisual;
    public ParticleSystem runVisual;
    //public vfxHolder runVFXHolder;
    public ParticleSystem floatingRunVisual;
    public GameObject forwardTeleportal;
    public GameObject backwardTeleportal;
    public ParticleSystem fullHeavyVFX;

    [Header("Attack Necessities")]
    public GameObject dashSplat;
    public GameObject wallSplat;
    private Vector3 leftDashSplatRotation = new Vector3 (0, 30, 0);
    private Vector3 rightDashSplatRotation = new Vector3(0, -30, 0);
    public Collider stompCollider;
    private bool grabActivated;
    public Transform nativeReel;
    private Transform foreignReel;
    private monsterAttackSystem foreignMonster;
    public SFXManager SFXManager;
    public GameObject floorCheck;

    #region Monster Start Up

    public void connectNecessaryLocomotionComponents()
    {
        //dash splat 
        //spin control
        //idle bounce
        //stomp collider
        //reel anchor
        //WIND ZONE
        //torso
        //heads
    }

    public void removeAllLimbParenting() //we use this to remove all parenting during a full refresh
    {
        monsterPart[] monsterPartList = GetComponentsInChildren<monsterPart>();
        for (int i = 0; i < monsterPartList.Length; i++)
        {
            if (monsterPartList[i].isTorso == false && monsterPartList[i].isHead == false && monsterPartList[i].monsterPartID == 1)
            {
                monsterPartList[i].transform.parent = null;
            }
        }

        autoLimb_Connection[] limbConnections = GetComponentsInChildren<autoLimb_Connection>();
        for (int i = 0; i < limbConnections.Length; i++)
        {
            limbConnections[i].clearAllMonsterPartMemory();
        }
    }

    /*
    public void removeSpecificLimbParenting(monsterPart partToRemoveFromMonster) //we use this anytime a limb is grabbed/altered in editor
    {
        partToRemoveFromMonster.transform.parent = null;

        autoLimb_Connection[] limbConnections = GetComponentsInChildren<autoLimb_Connection>();
        for (int i = 0; i < limbConnections.Length; i++)
        {
            limbConnections[i].clearSpecificMonsterPartMemory(partToRemoveFromMonster);
        }
    }
    */

    public void turnOnLimbConnectors()
    {
        autoLimb_Connection[] limbConnections = GetComponentsInChildren<autoLimb_Connection>();
        for (int i = 0; i < limbConnections.Length; i++)
        {
            limbConnections[i].enableColliders();
        }
    }

    public void turnOffLimbConnectors() //make sure to turn off the connectors before awakening the beast
    {
        autoLimb_Connection[] limbConnections = GetComponentsInChildren<autoLimb_Connection>();
        for (int i = 0; i < limbConnections.Length; i++)
        {
            limbConnections[i].disableColliders();
        }
    }

    public void connectCurrentLimbs()
    {
        autoLimb_Connection[] limbConnections = GetComponentsInChildren<autoLimb_Connection>();
        for (int i = 0; i < limbConnections.Length; i++)
        {
            limbConnections[i].connectMonsterParts();
        }
    }

    public void awakenTheBeast()
    {
        myAnimator = this.GetComponent<Animator>();
        floorCheck.SetActive(false);
        grabAttackSlotInfo();
        myAnimator.SetBool("Facing Right", facingRight);

        allMonsterParts = GetComponentsInChildren<monsterPart>();

        #region Figuring out if this Monster has one leg, many legs, or is a legless guy

        bool hasLeftGroundedLegs = false;
        bool hasRightGroundedLegs = false;
        bool hasWings = false;
        List<monsterPart> allGroundedRightLegs = new List<monsterPart>();
        List<monsterPart> allGroundedLeftLegs = new List<monsterPart>();
        List<monsterPart> allWings = new List<monsterPart>();

        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            if (allMonsterParts[i].isGroundedLimb)
            {
                if (allMonsterParts[i].isRightSidedLimb)
                {
                    allGroundedRightLegs.Add(allMonsterParts[i]);
                    hasRightGroundedLegs = true;
                }
                else if (allMonsterParts[i].isLeftSidedLimb)
                {
                    allGroundedLeftLegs.Add(allMonsterParts[i]);
                    hasLeftGroundedLegs = true;
                }
            }

            if (allMonsterParts[i].isGroundedLimb == false && allMonsterParts[i].isLeg)
            {
                if (allMonsterParts[i].isLeftSidedLimb)
                {
                    allMonsterParts[i].isLeadingLeg = false;
                }
                else
                {
                    allMonsterParts[i].isLeadingLeg = true;
                }
            }

            if (allMonsterParts[i].isWing)
            {
                allWings.Add(allMonsterParts[i]);
                hasWings = true;
                isWinged = true;
            }

            if (allMonsterParts[i].isTorso)
            {
                mainTorso = allMonsterParts[i].GetComponent<Animator>();
                allMonsterParts[i].isFloatingTorso = isFloatingMonster;// this is here just for monsters who are set to float even though they have grounded legs
            }


        }

        if (hasRightGroundedLegs == true && hasLeftGroundedLegs == true)
        {
            for (int u = 0; u < allGroundedRightLegs.Count; u++)
            {
                allGroundedRightLegs[u].isLeadingLeg = true;
            }
        }
        else
        {
            if (hasRightGroundedLegs == true) //high possibility that this is a one legged creature
            {
                for (int u = 0; u < allGroundedRightLegs.Count; u++)
                {
                    allGroundedRightLegs[u].isLeadingLeg = true;
                }
            }
            else if (hasLeftGroundedLegs == true) //high possibility that this is a one legged creature
            {
                for (int u = 0; u < allGroundedLeftLegs.Count; u++)
                {
                    allGroundedLeftLegs[u].isLeadingLeg = true;
                }
            }
            else //oh this creature has no grounded legs, high likelihood of it being a floating/flying monster
            {
                if (hasWings)
                {
                    for (int u = 0; u < allWings.Count; u++)
                    {
                        allWings[u].hasFlightedIdle = true;
                    }

                    fullyFlighted = true;
                    mainTorso.SetBool("Flighted Monster", isWinged);
                }
                else //this thing is just an orb basically
                {
                    for (int i = 0; i < allMonsterParts.Length; i++)
                    {
                        if (allMonsterParts[i].isTorso)
                        {
                            isFloatingMonster = true;
                            allMonsterParts[i].isFloatingTorso = isFloatingMonster;
                        }
                    }
                }
            }
        }


        if (isFloatingMonster)
        {
            if (hasRightGroundedLegs)
            {
                for (int u = 0; u < allGroundedRightLegs.Count; u++)
                {
                    allGroundedRightLegs[u].isFloatingGroundedLeg = true;
                }
            }

            if (hasLeftGroundedLegs)
            {
                for (int u = 0; u < allGroundedLeftLegs.Count; u++)
                {
                    allGroundedLeftLegs[u].isFloatingGroundedLeg = true;
                }
            }
        }


        #endregion

        monsterPartReference[] internalPartReferences = GetComponentsInChildren<monsterPartReference>();
        vfxHolder[] internalVFXHolders = GetComponentsInChildren<vfxHolder>();

        for (int i = 0; i < internalVFXHolders.Length; i++)
        {
            if (internalVFXHolders[i].isMonsterSystemVFXHolder == false)
            {
                internalVFXHolders[i].grabReferences();

                for (int u = 0; u < internalVFXHolders[i].damageGivingVFX.Length; u++)
                {
                    listOfInternalReferences.Add(internalVFXHolders[i].damageGivingVFX[u]);
                }
            }
        }

        for (int i = 0; i < internalPartReferences.Length; i++)
        {
            listOfInternalReferences.Add(internalPartReferences[i]);
            internalPartReferences[i].mainSystem = this;
        }

        for (int i = 0; i < internalVFXHolders.Length; i++)
        {
            if (internalVFXHolders[i].isMonsterSystemVFXHolder == false)
            {
                internalVFXHolders[i].referencesToIgnore = listOfInternalReferences;
                internalVFXHolders[i].collisionOcclusion();
            }
            else
            {
                internalVFXHolders[i].collisionOcclusion();//this is played just because the set up is wrapped in this function
            }
        }

        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].myMainSystem = this;
            allMonsterParts[i].mainTorso = mainTorso;
            allMonsterParts[i].referencesToIgnore = listOfInternalReferences;
            allMonsterParts[i].TriggerPartTypeSetup();
            allMonsterParts[i].triggerAnimationSetUp();
            allMonsterParts[i].triggerAnimationOffsets();
            allMonsterParts[i].triggerCollisionLogic(); //collision logic must come after animation set up because animation set up includes projectile set up 
            allMonsterParts[i].triggerIdle();
        }

        myAnimator.SetBool("Idle Bounce Allowed", true);
        StartCoroutine(spawnRenactment());
        myAnimator.SetBool("Calm", false);
        calm = false;

        SFXManager = FindObjectOfType<SFXManager>();
    }

    IEnumerator spawnRenactment()
    {
        myAnimator.SetBool("Spawning In", true);
        myAnimator.SetTrigger("Spawn In");
        yield return new WaitForSeconds(0.2f);

        fierceEmote();//something here to choose the emote
        //gasEmote();
        //danceEmote();
        //jackEmote();
        //mockingEmote();

        yield return new WaitForSeconds(2f);
        //myAnimator.SetBool("Idle Bounce Allowed", true);
        //forceEndEmote();
        myAnimator.SetBool("Spawning In", false);
        myAnimator.ResetTrigger("Spawn In");
    }

    public void grabAttackSlotInfo()
    {
        for (int i = 0; i < attackSlotMonsterParts.Length; i++)
        {
            if (attackSlotMonsterParts[i] != null)
            {
                attackSlotMonsterID[i] = attackSlotMonsterParts[i].monsterPartID; //this tells us what type of part class it is

                /*
                if (attackSlotMonsterParts[i].isWing == true)
                {
                    isWinged = true;
                }

                */
            }
        }

        if (isWinged)
        {
            jumpsLeft = jumpsAllowed_FlightedJump;
        }
        else
        {
            jumpsLeft = jumpsAllowed_DoubleJump;
        }
    }

    public void resetMonster(bool turnToTheRight)
    {
        jumpsLeft = 0;
        Array.Clear(attackSlotMonsterID, 0, attackSlotMonsterID.Length);
        facingRight = turnToTheRight;
        myAnimator.SetBool("Facing Right", facingRight);
        myAnimator.SetBool("Idle Bounce Allowed", false);
        myAnimator.SetBool("Calm", false);
        calm = false;
        myAnimator = null;
        Array.Clear(allMonsterParts, 0, allMonsterParts.Length);
        isWinged = false;
        fullyFlighted = false;
        mainTorso.SetBool("Flighted Monster", false);
        mainTorso = null;

        for (int i = 0; i < allMonsterParts.Length; i++)
        {

            if (allMonsterParts[i].isWing)
            {
                allMonsterParts[i].hasFlightedIdle = false;
            }

            if (allMonsterParts[i].isLeg)
            {
                allMonsterParts[i].isLeadingLeg = false;
            }
        }

        monsterPartReference[] internalPartReferences = GetComponentsInChildren<monsterPartReference>();
        vfxHolder[] internalVFXHolders = GetComponentsInChildren<vfxHolder>();

        for (int i = 0; i < internalPartReferences.Length; i++)
        {
            internalPartReferences[i].mainSystem = null;
        }

        listOfInternalReferences.Clear();


        for (int i = 0; i < internalVFXHolders.Length; i++)
        {
            internalVFXHolders[i].referencesToIgnore = listOfInternalReferences;
            internalVFXHolders[i].collisionOcclusion();
        }

        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].myMainSystem = null;
            allMonsterParts[i].mainTorso = null;
            allMonsterParts[i].referencesToIgnore = listOfInternalReferences;
            //gotta take care of all the alt versions of these functions for monster parts specifically
            allMonsterParts[i].triggerAnimationSetUp();
            allMonsterParts[i].triggerAnimationOffsets();
            allMonsterParts[i].triggerCollisionLogic(); //collision logic must come after animation set up because animation set up includes projectile set up 
            allMonsterParts[i].triggerIdle();
        }
    }

    #endregion

    #region Attacks

    private void Update()
    {
        if (timeSinceLastCall < 10)
        {
            timeSinceLastCall += Time.deltaTime;
        }

        if (timeSinceLastDamage < 10)
        {
            timeSinceLastDamage += Time.deltaTime;
        }

        if (grabActivated)
        {
            foreignReel.position = new Vector3(nativeReel.position.x, nativeReel.position.y, foreignReel.position.z);
        }
    }

    public void attack(int attackSlot, int attackDirection)
    {
        if (damageLocked)
        {
            return;
        }

        if (!attackSlotMonsterParts[attackSlot].attackMarkedHeavy && attackSlotMonsterParts[attackSlot].needsReloadNeutral && !attackSlotMonsterParts[attackSlot].GetReloadNeutral())
        {
            print("cancelled!!!!");
            return;
        }

        if (attackSlotMonsterParts[attackSlot] != null)
        {
            if (attackSlotMonsterParts[attackSlot].connected == false)
            {
                return;
            }
        }
        else
        {
            return;
        }

        getOutOfLaunch();

        if (attackSlotMonsterParts[attackSlot] != null && focusedAttackActive == false)
        {
            if (attackSlotMonsterID[attackSlot] == 0)
            {
                jump();
            }
            else if (attackSlotMonsterID[attackSlot] == 1)
            {
                if (focusedAttackActive == false)
                {
                    flourishCounter++;
                    if (timeSinceLastCall <= 1f)
                    {
                        if (flourishCounter >= 3)
                        {
                            requiresFlourish = true;
                            flourishCounter = 0;
                        }
                    }
                    else
                    {
                        flourishCounter = 1;
                    }
                    timeSinceLastCall = 0;
                }

                //Redo this section to clean it up
                if (attackSlotMonsterParts[attackSlot].attackAnimationID == 0)
                {
                    if (isGrounded || attackSlotMonsterParts[attackSlot].isTail)
                    {
                        requiresFlourishingTwirl = false;
                        requiresFlourishingRoll = false;
                    }
                    else
                    {
                        requiresFlourishingTwirl = false;
                        requiresFlourishingRoll = false;
                    }
                }
                else if (attackSlotMonsterParts[attackSlot].attackAnimationID == 1)
                {
                    if (attackSlotMonsterParts[attackSlot].isLeg && attackSlotMonsterParts[attackSlot].isGroundedLimb)
                    {
                        requiresFlourishingTwirl = false;
                        requiresFlourishingRoll = false;
                    }
                    else
                    {
                        requiresFlourishingTwirl = false;
                        requiresFlourishingRoll = false;
                    }

                }
                else if (attackSlotMonsterParts[attackSlot].attackAnimationID == -1)
                {
                    if (attackSlotMonsterParts[attackSlot].isTail)
                    {
                        requiresFlourishingTwirl = false;
                        requiresFlourishingRoll = false;
                    }
                    else if (attackSlotMonsterParts[attackSlot].isLeg && attackSlotMonsterParts[attackSlot].isGroundedLimb)
                    {
                        requiresFlourishingTwirl = false;
                        requiresFlourishingRoll = false;
                    }
                    else
                    {
                        requiresFlourishingTwirl = false;
                        requiresFlourishingRoll = false;
                    }
                }
                else if (attackSlotMonsterParts[attackSlot].attackAnimationID == 2)
                {
                    requiresFlourishingTwirl = false;
                    requiresFlourishingRoll = false;
                }

                if (isGrounded)
                {
                    if (focusedAttackActive == false)
                    {
                        attackSlotMonsterParts[attackSlot].triggerAttack("Ground Attack", attackDirection);

                        myAnimator.SetBool("Idle Bounce Allowed", false);
                        #region Bracing for Attacks 
                        if (requiresFlourish && (requiresFlourishingTwirl || requiresFlourishingRoll))
                        {
                            braceForFlourishImpact();
                        }
                        else
                        {
                            if (attackSlotMonsterParts[attackSlot].requiresRightStance)
                            {
                                braceForRightImpact();
                            }

                            if (attackSlotMonsterParts[attackSlot].requiresLeftStance)
                            {
                                braceForLeftImpact();
                            }

                            if (attackSlotMonsterParts[attackSlot].requiresForwardStance)
                            {
                                braceForForwardImpact();
                            }

                            if (attackSlotMonsterParts[attackSlot].requiresBackwardStance)
                            {
                                braceForBackwardImpact();
                            }
                        }

                        isRunning = false;
                        requiresFlourish = false;
                        #endregion
                    }
                }
                else
                {
          
                    if (focusedAttackActive == false)
                    {
                        attackSlotMonsterParts[attackSlot].triggerAttack("Airborn Attack", attackDirection);

                        myAnimator.SetBool("Idle Bounce Allowed", false);
                        #region Bracing for Attacks

                        if (requiresFlourish && isGliding == false && (requiresFlourishingTwirl || requiresFlourishingRoll))
                        {
                            //roll
                            braceForFlourishImpact();
                        }
                        else
                        {
                            if (attackSlotMonsterParts[attackSlot].requiresRightStance)
                            {
                                braceForRightImpact();
                            }

                            if (attackSlotMonsterParts[attackSlot].requiresLeftStance)
                            {
                                braceForLeftImpact();
                            }

                            if (attackSlotMonsterParts[attackSlot].requiresForwardStance)
                            {
                                braceForForwardImpact();
                            }

                            if (attackSlotMonsterParts[attackSlot].requiresBackwardStance)
                            {
                                braceForBackwardImpact();
                            }
                        }

                        isRunning = false;
                        requiresFlourish = false;
                        #endregion

                        if (isGliding)
                        {
                            glideToAttack();
                        }
                    }
                    //Add to this later so that we can having bracing while attacking in the air
                }
            }
            else if (attackSlotMonsterID[attackSlot] == 2)
            {
                //scientific module
            }
        }
    }

    public void attackCancel(int attackSlot)
    {
        if (damageLocked)
        {
            return;
        }

        if (attackSlotMonsterID[attackSlot] == 1)
        {
            attackSlotMonsterParts[attackSlot].triggerNeutralOrHeavyRefresh(true);
        }


    }

    public void dashAttack()
    {
        if (damageLocked)
        {
            return;
        }

        if ((isRunning || isGrounded == false) && canDashAttack && canRoll && focusedAttackActive == false)
        {
            //isRunning = false;
            canRoll = false;
            canDashAttack = false;
            attackFocusOn();
            stopForceFall();
            //StartCoroutine(dashVisuals());

            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].triggerRoll(isGrounded, true);
            }

            //myAnimator.SetFloat("Flipping Speed", 1.5f);
            //myAnimator.ResetTrigger("Roll");
            //myAnimator.SetTrigger("Roll");
            isGliding = false;
            glideVisual.Stop();
            myAnimator.SetBool("Gliding", false);
            myAnimator.ResetTrigger("Glide to Attack");

            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].triggerVisualDissappearance();
            }

            dashSplat.SetActive(true);
        }
    }

    IEnumerator dashVisuals()
    {
        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].triggerRoll(isGrounded, true);
        }

        myAnimator.SetFloat("Flipping Speed", 2f);
        myAnimator.ResetTrigger("Roll");
        myAnimator.SetTrigger("Roll");
        isGliding = false;
        glideVisual.Stop();
        myAnimator.SetBool("Gliding", false);
        myAnimator.ResetTrigger("Glide to Attack");

        yield return new WaitForSeconds(0.02f);

        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].triggerVisualDissappearance();
        }

        dashSplat.SetActive(true);
    }

    public void endDashAttack()
    {
        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].triggerVisualReappearance();
            allMonsterParts[i].triggerRoll(isGrounded, true);
        }

        myAnimator.ResetTrigger("Roll");
        myAnimator.SetTrigger("Roll");
        dashSplat.SetActive(false);
        attackFocusOff();

        if (isGrounded)
        {
            canDashAttack = true;
            canRoll = true;
        }
    }
    public void wallGrabbedCorrections()
    {
        canDashAttack = true;
    }

    public void wallGrab()
    {
        dashSplat.SetActive(false);
        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].triggerVisualDissappearance();
        }
        wallSplat.SetActive(true);
    }

    public void endWallGrab()
    {
        wallSplat.SetActive(false);

        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].triggerVisualReappearance();
        }

        if (isGrounded)
        {
            canDashAttack = true;
            canRoll = true;
        }
    }


    public void stompAttack()
    {
        if (damageLocked)
        {
            return;
        }

        stompCollider.enabled = true;
        StartCoroutine(stompReset());
    }

    IEnumerator stompReset()
    {
        yield return new WaitForSeconds(0.2f);
        stompCollider.enabled = false;
    }

    public void rollingUpwardsAttack()
    {
        if (damageLocked)
        {
            return;
        }

        isGrounded = false;
        isRunning = false;
        isWalking = false;
        canRoll = false;
        glideVisual.Stop();
        myAnimator.SetBool("Gliding", false);
        myAnimator.ResetTrigger("Glide to Attack");

        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].triggerRollingAttack();
        }

        myAnimator.SetFloat("Flipping Speed", 1f);
        myAnimator.SetTrigger("Nonstop Roll");
        myAnimator.SetBool("Idle Bounce Allowed", false);
        myAnimator.SetBool("Calm", false);
        calm = false;
        forceEndEmote();
        forceStopCrouch();
        stopForceFall();
        StartCoroutine(rollingAttackTimer());
    }

    public void rollingDownwardsAttack()
    {
        if (damageLocked)
        {
            return;
        }

        isGrounded = false;
        isRunning = false;
        isWalking = false;
        canRoll = false;
        glideVisual.Stop();
        myAnimator.SetBool("Gliding", false);
        myAnimator.ResetTrigger("Glide to Attack");

        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].triggerRollingAttack();
        }

        myAnimator.SetFloat("Flipping Speed", 1f);
        myAnimator.SetTrigger("Nonstop Roll");
        myAnimator.SetBool("Idle Bounce Allowed", false);
        myAnimator.SetBool("Calm", false);
        calm = false;
        forceEndEmote();
        forceStopCrouch();
        stopForceFall();
        StartCoroutine(rollingAttackTimer());
    }

    IEnumerator rollingAttackTimer()
    {
        rollingAttackVisual.Play();
        yield return new WaitForSeconds(0.5f);


        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].stopInfiniteRoll();
        }

        rollingAttackVisual.Stop();
    }

    public void leapingUpwardAttack()
    {
        if (damageLocked)
        {
            return;
        }

        isGrounded = false;
        isRunning = false;
        isWalking = false;
        canRoll = false;
        glideVisual.Stop();
        myAnimator.SetBool("Gliding", false);
        myAnimator.ResetTrigger("Glide to Attack");

        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].triggerUpwardsLeapingAttack();
        }

        myAnimator.SetTrigger("Jump");
        myAnimator.SetBool("Idle Bounce Allowed", false);
        myAnimator.SetBool("Calm", false);
        calm = false;
        forceEndEmote();
        forceStopCrouch();
        stopForceFall();
        leapingAttackVisual.Stop();
        leapingAttackVisual.Play();
    }

    #endregion

    #region Player Controller Reactions i.e. Leaping with attacks
    public void smallLeapAttackForward()
    {
        myPlayer.smallLeapAttackForward();
    }

    public void smallLeapAttackBackward()
    {
        myPlayer.smallLeapAttackBackward();
    }

    public void smallLeapAttackUpward()
    {
        myPlayer.smallLeapAttackUpward();
    }

    public void smallLeapAttackDownward()
    {
        myPlayer.smallLeapAttackDownward();
    }

    public void leapAttackForward()
    {
        myPlayer.leapAttackForward();
    }

    public void leapAttackBackward()
    {
        myPlayer.leapAttackBackward();
    }

    public void leapAttackUpward()
    {
        myPlayer.leapAttackUpward();
    }

    public void leapAttackDownward()
    {
        myPlayer.leapAttackDownward();
    }

    #endregion

    #region Movement

    public void flipCharacter()
    {
        if (damageLocked)
        {
            return;
        }

        if (focusedAttackActive == false)
        {

            if (isRunning)
            {
                screechingStop();
                StartCoroutine(characterFlipDelay(true));
            }
            else
            {
                StartCoroutine(characterFlipDelay(false));
            }

            if (isCrouching)
            {
                //myAnimator.SetBool("Idle Bounce Allowed", true);
            }

            forceEndEmote();
            forceStopCrouch();
            getOutOfLaunch();
        }
    }

    IEnumerator characterFlipDelay(bool needsDelay)
    {
        if (needsDelay)
        {
            yield return new WaitForSeconds(0.35f);
            myAnimator.SetFloat("Flipping Speed", 1);
        }
        else
        {
            yield return new WaitForSeconds(0);
            myAnimator.SetFloat("Flipping Speed", 1.5f);
        }

        if (facingRight)
        {
            facingRight = false;
            myAnimator.SetBool("Facing Right", facingRight);
            myAnimator.SetTrigger("Flip to Left");
            dashSplat.transform.localEulerAngles = leftDashSplatRotation;
            if (wallSplat != null)
            {
                wallSplat.transform.localEulerAngles = leftDashSplatRotation;
            }
            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].facingRight = false;
            }
        }
        else
        {
            facingRight = true;
            myAnimator.SetBool("Facing Right", facingRight);
            myAnimator.SetTrigger("Flip to Right");
            dashSplat.transform.localEulerAngles = rightDashSplatRotation;
            if (wallSplat != null)
            {
                wallSplat.transform.localEulerAngles = rightDashSplatRotation;
            }
            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].facingRight = true;
            }
        }
    }


    public void flipLeft()
    {
        if (damageLocked)
        {
            return;
        }
        myAnimator.ResetTrigger("Flip to Left");

        /*
        if (focusedAttackActive == false)
        {
            myAnimator.SetFloat("Flipping Speed", 1.5f);
            facingRight = false;
            myAnimator.SetBool("Facing Right", facingRight);
            myAnimator.SetTrigger("Flip to Left");
            dashSplat.transform.localEulerAngles = leftDashSplatRotation;
            if (wallSplat != null)
            {
                wallSplat.transform.localEulerAngles = leftDashSplatRotation;
            }
            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].facingRight = false;
            }
            forceEndEmote();
            forceStopCrouch();
            getOutOfLaunch();
        }
        */

        myAnimator.SetFloat("Flipping Speed", 1.5f);
        facingRight = false;
        myAnimator.SetBool("Facing Right", facingRight);
        myAnimator.SetTrigger("Flip to Left");
        dashSplat.transform.localEulerAngles = leftDashSplatRotation;
        if (wallSplat != null)
        {
            wallSplat.transform.localEulerAngles = leftDashSplatRotation;
        }
        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].facingRight = false;
        }
        forceEndEmote();
        forceStopCrouch();
        getOutOfLaunch();
    }

    public void flipRight()
    {
        if (damageLocked)
        {
            return;
        }

        myAnimator.ResetTrigger("Flip to Right");

        /*
        if (focusedAttackActive == false)
        {
            myAnimator.SetFloat("Flipping Speed", 1.5f);
            facingRight = true;
            myAnimator.SetBool("Facing Right", facingRight);
            myAnimator.SetTrigger("Flip to Right");
            dashSplat.transform.localEulerAngles = rightDashSplatRotation;
            if (wallSplat != null)
            {
                wallSplat.transform.localEulerAngles = rightDashSplatRotation;
            }
            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].facingRight = true;
            }
            forceEndEmote();
            forceStopCrouch();
            getOutOfLaunch();
        }
        */

        myAnimator.SetFloat("Flipping Speed", 1.5f);
        facingRight = true;
        myAnimator.SetBool("Facing Right", facingRight);
        myAnimator.SetTrigger("Flip to Right");
        dashSplat.transform.localEulerAngles = rightDashSplatRotation;
        if (wallSplat != null)
        {
            wallSplat.transform.localEulerAngles = rightDashSplatRotation;
        }
        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].facingRight = true;
        }
        forceEndEmote();
        forceStopCrouch();
        getOutOfLaunch();
    }

    public void walk()
    {
        if (damageLocked)
        {
            return;
        }

        if (isGrounded)
        {
            isRunning = false;
            isWalking = true;

            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].triggerWalk();
            }

            myAnimator.SetBool("Idle Bounce Allowed", false);
            myAnimator.SetBool("Calm", false);
            calm = false;
            forceEndEmote();
            forceStopCrouch();

        }
        else
        {
            isWalking = true;
            getOutOfLaunch();

            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].triggerWalk();
            }
        }
    }

    public void stopWalking()
    {
        if (damageLocked)
        {
            return;
        }

        if (isWalking)
        {
            if (isGrounded)
            {
                isWalking = false;

                for (int i = 0; i < allMonsterParts.Length; i++)
                {
                    allMonsterParts[i].triggerStopWalking();
                }

                //myAnimator.SetBool("Idle Bounce Allowed", true);
                //myAnimator.SetBool("Calm", false);
                //calm = false;
                if (isRunning == false)
                {
                    calmedDown();
                }
            }
        }
    }

    public void run()
    {
        if (damageLocked)
        {
            return;
        }

        if (isGrounded)
        {
            isWalking = false;
            isRunning = true;

            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].triggerRun();
            }

            myAnimator.SetBool("Idle Bounce Allowed", false);
            myAnimator.SetBool("Calm", false);
            calm = false;
            forceEndEmote();
            forceStopCrouch();

            if (fullyFlighted == false)
            {
                releaseRunVFX();
            }
        }
        else
        {
            isRunning = true;
            getOutOfLaunch();

            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].triggerRun();
            }
        }
    }

    public void releaseRunVFX()
    {
        if (isRunning)
        {
            //runVFXHolder.releaseMonsterSystemVisual();
            runVisual.Stop();
            runVisual.Play();
        }
    }

    public void stopRunVFX()
    {
        runVisual.Stop();
    }

    public void stopRunning()
    {
        if (damageLocked)
        {
            return;
        }

        stopRunVFX();

        if (isRunning)
        {
            if (isGrounded)
            {
                isRunning = false;

                for (int i = 0; i < allMonsterParts.Length; i++)
                {
                    allMonsterParts[i].triggerStopRunning();
                }

                if (isWalking == false)
                {
                    myAnimator.SetBool("Idle Bounce Allowed", true);
                    myAnimator.SetBool("Calm", false);
                    calm = false;
                }
            }
            else
            {
                isRunning = false;

                for (int i = 0; i < allMonsterParts.Length; i++)
                {
                    allMonsterParts[i].triggerStopRunning();
                }
            }
        }
    }

    public void screechingStop()
    {
        if (isGrounded && isRunning)
        {
            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].triggerScreechingStop();
            }
        }
    }

    public void jump()
    {
        if (damageLocked)
        {
            return;
        }

        isGrounded = false;
        //isRunning = false;
        //isWalking = false;
        focusedAttackActive = false;
        isGliding = false;
        jumpsLeft--;
        canDashAttack = true;
        canRoll = true;
        stopRunVFX();

        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].triggerJump();
        }

        myAnimator.ResetTrigger("Jump");
        myAnimator.SetTrigger("Jump");
        myAnimator.SetBool("Idle Bounce Allowed", false);
        myAnimator.SetBool("Calm", false);
        calm = false;
        forceEndEmote();
        forceStopCrouch();
        releaseJumpVFX();
        stopForceFall();

        /*
        if (isGrounded && jumpsLeft != 0)
        {
            isGrounded = false;
            //isRunning = false;
            //isWalking = false;
            focusedAttackActive = false;
            isGliding = false;
            jumpsLeft--;
            canDashAttack = true;
            canRoll = true;

            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].triggerJump();
            }

            myAnimator.SetTrigger("Jump");
            myAnimator.SetBool("Idle Bounce Allowed", false);
            myAnimator.SetBool("Calm", false);
            calm = false;
            forceEndEmote();
            forceStopCrouch();
            releaseJumpVFX();
            stopForceFall();
            
        }
        else
        {
            doubleJump();
            stopForceFall();
        }
        */
    }

    public void doubleJump()
    {
        stopForceFall();
        releaseDoubleJumpVFX();
        /*
        if (isWinged)
        {
            if (jumpsLeft != 0)
            {
                isGliding = false;
                jumpsLeft--;
                if (SFXManager)
                {
                    SFXManager.DoubleJumpWingedSFX();
                }
                for (int i = 0; i < allMonsterParts.Length; i++)
                {
                    allMonsterParts[i].triggerWingFlap();
                }

            }
        }
        else
        {
            if (jumpsLeft != 0)
            {
                isGliding = false;
                jumpsLeft--;

                for (int i = 0; i < allMonsterParts.Length; i++)
                {
                    allMonsterParts[i].triggerRoll(false, false);
                }

                myAnimator.ResetTrigger("Roll");
                myAnimator.SetFloat("Flipping Speed", 1.5f);
                myAnimator.SetTrigger("Roll");
                releaseJumpVFX();
                   
            }
        }
        */

        isGliding = false;
        //jumpsLeft--;

        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].triggerRoll(false, false);
        }

        myAnimator.ResetTrigger("Roll");
        myAnimator.SetFloat("Flipping Speed", 1.5f);
        myAnimator.SetTrigger("Roll");
        releaseJumpVFX();
        stopRunVFX();

    }

    public void releaseDoubleJumpVFX()
    {
        doubleJumpVisual.Stop();
        doubleJumpVisual.Play();
    }

    public void releaseJumpVFX()
    {
        jumpVisual.Stop();
        jumpVisual.Play();
    }

    public void glide()
    {
        if (damageLocked)
        {
            return;
        }

        stopRunVFX();

        if (isGrounded == false && isWinged)
        {
            if (isGliding == false)
            {
                isGliding = true;
                //focusedAttackActive = true;
                glideVisual.Play();
                myAnimator.SetBool("Gliding", true);
                getOutOfLaunch();

                for (int i = 0; i < allMonsterParts.Length; i++)
                {
                    allMonsterParts[i].triggerGlide();
                }
            }
            else
            {
                glideToFall();
            }
        }
    }

    private void glideToFall()
    {
        focusedAttackActive = false;
        isGliding = false;
        glideVisual.Stop();
        myAnimator.ResetTrigger("Glide to Attack");
        myAnimator.SetBool("Gliding", false);

        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].triggerFall();
        }
    }

    private void glideToAttack()
    {
        isGliding = false;
        glideVisual.Stop();
        myAnimator.SetTrigger("Glide to Attack");
        myAnimator.SetBool("Gliding", false);
    }

    public void walkToFall()
    {

        if (isGrounded)
        {
            isGrounded = false;
            //isRunning = false;
            //isWalking = false;
            focusedAttackActive = false;
            isGliding = false;

            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].triggerFall();
            }

            myAnimator.SetBool("Idle Bounce Allowed", false);
            myAnimator.SetBool("Calm", false);
            calm = false;
            forceEndEmote();
            forceStopCrouch();
            stopRunVFX();
        }
    }

    public void goThroughPlatform()
    {
        if (isGrounded)
        {
            isGrounded = false;
            //isRunning = false;
            isWalking = false;
            focusedAttackActive = false;

            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].triggerRoll(false, false);
            }

            myAnimator.SetFloat("Flipping Speed", 1.5f);
            myAnimator.SetTrigger("Roll");
            myAnimator.SetBool("Idle Bounce Allowed", false);
            myAnimator.SetBool("Calm", false);
            calm = false;
            forceEndEmote();
            forceStopCrouch();
            stopRunVFX();
        }
    }

    public void land()
    {
        if(isGrounded == false)
        {
            isGrounded = true;
            focusedAttackActive = false;
            canDashAttack = true;
            canRoll = true;
            glideVisual.Stop();
            myAnimator.SetBool("Gliding", false);
            myAnimator.ResetTrigger("Glide to Attack");
            stopForceFall();

            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].triggerLand();
            }

            isGliding = false;

            myAnimator.SetTrigger("Land");

            if (isRunning == false && isWalking == false)
            {
                myAnimator.SetBool("Idle Bounce Allowed", true);
                myAnimator.SetBool("Calm", false);
                calm = false;
            }

            if (isWinged)
            {
                jumpsLeft = jumpsAllowed_FlightedJump;
            }
            else
            {
                jumpsLeft = jumpsAllowed_DoubleJump;
            }

            landVisual.Stop();
            landVisual.Play();
            
        }
    }

    public void lateLand()
    {
        isGrounded = true;
        //focusedAttackActive = false;
        canDashAttack = true;
        canRoll = true;
        glideVisual.Stop();
        myAnimator.SetBool("Gliding", false);
        myAnimator.ResetTrigger("Glide to Attack");
        stopForceFall();

        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].triggerLateLand();
        }

        requiresFlourishingRoll = true;
        braceForFlourishImpact();
        requiresFlourishingRoll = false;

        isGliding = false;

        if (isWinged)
        {
            jumpsLeft = jumpsAllowed_FlightedJump;
        }
        else
        {
            jumpsLeft = jumpsAllowed_DoubleJump;
        }
    }

    public void forceUngrounded()
    {
        if (isGrounded)
        {
            isGrounded = false;
            //isRunning = false;
            //isWalking = false;
            //focusedAttackActive = false;
            isGliding = false;

            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].triggerSimpleUngrounded();
            }

            myAnimator.SetBool("Idle Bounce Allowed", false);
            myAnimator.SetBool("Calm", false);
            calm = false;
            forceEndEmote();
            forceStopCrouch();
        }
    }

    public void roll()
    {
        if (damageLocked)
        {
            return;
        }

        if (isGrounded && canRoll)
        {
            isGrounded = true;
            isRunning = false;
            isWalking = false;
            focusedAttackActive = false;
            canRoll = false;
            glideVisual.Stop();
            myAnimator.SetBool("Gliding", false);
            myAnimator.ResetTrigger("Glide to Attack");

            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].triggerRoll(true, true);
            }

            myAnimator.SetFloat("Flipping Speed", 1f);
            myAnimator.SetTrigger("Roll");
            myAnimator.SetBool("Idle Bounce Allowed", false);
            myAnimator.SetBool("Calm", false);
            calm = false;
            forceEndEmote();
            forceStopCrouch();
        }
    }

    public void entryTeleportalVFX(bool facingInDirection)
    {
        stopRunVFX();

        if (facingInDirection)
        {
            forwardTeleportal.SetActive(true);
        }
        else
        {
            backwardTeleportal.SetActive(true);
        }

        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].triggerVisualDissappearance();
        }
    }

    public void reEntryTeleportalVFX()
    {
        stopRunVFX();

        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].triggerVisualReappearance();
        }
    }

    public void resetTeleportalsVFX()
    {
        forwardTeleportal.SetActive(false);
        backwardTeleportal.SetActive(false);
    }

    public void toggleCrouch() //there's a separation here just for ease of access and in case the toggle gets weird 
    {
        if (damageLocked)
        {
            return;
        }

        stopRunVFX();
        //crouch can interrupt running, walking, landing, and emotes
        //crouch cannout interrupt neutral or heavy attacks, windups, rolls, and jumps
        if (focusedAttackActive == false && isGrounded)
        {
            if (isCrouching)
            {
                stopCrouching();
            }
            else
            {
                isCrouching = true;
                isRunning = false;
                isWalking = false;
                //emoteActive = false;
                calm = false;
                myAnimator.SetBool("Idle Bounce Allowed", false);
                myAnimator.SetBool("Calm", false);

                for (int i = 0; i < allMonsterParts.Length; i++)
                {
                    allMonsterParts[i].triggerCrouch();
                }

                forceEndEmote();
                //gasVisual.Stop();
                //musicVisual.Stop();
            }
        }

        getOutOfLaunch();
    }

    public void crouch()
    {
        if (damageLocked)
        {
            return;
        }

        stopRunVFX();
        //crouch can interrupt running, walking, landing, and emotes
        //crouch cannout interrupt neutral or heavy attacks, windups, rolls, and jumps
        if (focusedAttackActive == false && isGrounded)
        {
            isCrouching = true;
            isRunning = false;
            isWalking = false;
            //emoteActive = false;
            calm = false;
            myAnimator.SetBool("Idle Bounce Allowed", false);
            myAnimator.SetBool("Calm", false);

            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].triggerCrouch();
            }

            forceEndEmote();
            //gasVisual.Stop();
            //musicVisual.Stop();
        }

        getOutOfLaunch();
    }

    public void stopCrouching()
    {
        if (damageLocked)
        {
            return;
        }

        isCrouching = false;

        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].triggerCrouchStop();
        }

        if (isRunning == false && isWalking == false)
        {
            myAnimator.SetBool("Idle Bounce Allowed", true);
        }
    }

    private void forceStopCrouch()
    {
        isCrouching = false;

        for (int i = 0; i < allMonsterParts.Length; i++) //wrap this in a bool
        {
            allMonsterParts[i].triggerCrouchStop();
        }
    }

    public void forceFallToggle()
    {
        if (damageLocked)
        {
            return;
        }

        stopRunVFX();

        if (isGrounded == false && focusedAttackActive == false)
        {
            if (forceFallingActivated)
            {

                for (int i = 0; i < allMonsterParts.Length; i++)
                {
                    allMonsterParts[i].triggerForceFallStop();
                }

                forceFallingActivated = false;
            }
            else
            {

                for (int i = 0; i < allMonsterParts.Length; i++)
                {
                    allMonsterParts[i].triggerForceFall();
                }

                forceFallingActivated = true;
            }

            getOutOfLaunch();
        }
    }

    public void forceFall()
    {
        if (damageLocked)
        {
            return;
        }

        if (isGrounded == false && forceFallingActivated == false && focusedAttackActive == false)
        {
            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].triggerForceFall();
            }

            forceFallingActivated = true;
        }

        getOutOfLaunch();
        stopRunVFX();
    }

    public void stopForceFall()
    {
        if (damageLocked)
        {
            return;
        }

        if (isGrounded == false && forceFallingActivated)
        {
            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].triggerForceFallStop();
            }

            forceFallingActivated = false;
        }

        getOutOfLaunch();
    }

    #endregion

    #region Reactions
    public void braceForRightImpact()
    {
        myAnimator.SetBool("Idle Bounce Allowed", false);

        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].triggerRightAttackStance();
        }
    }

    public void braceForLeftImpact()
    {
        myAnimator.SetBool("Idle Bounce Allowed", false);

        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].triggerLeftAttackStance();
        }
    }

    public void braceForForwardImpact()
    {
        myAnimator.SetBool("Idle Bounce Allowed", false);

        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].triggerForwardStance();
        }
    }

    public void braceForBackwardImpact()
    {
        myAnimator.SetBool("Idle Bounce Allowed", false);

        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].triggerBackwardStance();
        }
    }

    public void braceForFlourishImpact()
    {
        /*
        if (requiresFlourishingTwirl)
        {
            myAnimator.SetTrigger("Flourish Twirl");
        }
        else
        {
            myAnimator.SetTrigger("Flourish Roll");
        }
        */

        myAnimator.SetBool("Idle Bounce Allowed", false);

        if (requiresFlourishingTwirl)
        {
            myAnimator.SetTrigger("Flourish Twirl");
        }
        else if (requiresFlourishingRoll)
        {
            myAnimator.SetTrigger("Flourish Roll");
        }

        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].triggerFlourishStance();
        }
    }

    public void switchBraceStance()
    {
        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].triggerHeavyLegStance();
        }
    }

    public void endBracing()
    {
        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].triggerUnbrace();
        }

        myAnimator.ResetTrigger("Glide to Attack");

        if (requiresFlourishingTwirl)
        {
            myAnimator.ResetTrigger("Flourish Twirl");
        }
        else if(requiresFlourishingRoll)
        {
            myAnimator.ResetTrigger("Flourish Roll");
        }
    }

    public void grappleToTarget()
    {
        stopRunVFX();

        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            //allMonsterParts[i].triggerUnbrace();
            allMonsterParts[i].reelBackReaction();
            //tell attacking limb to pull in
            //tell everything else to jump I guess?
        }
    }

    public void lateGrappleCorrections()
    {
        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            //allMonsterParts[i].triggerUnbrace();
            //tell all parts to unbrace or whatever
        }
    }

    public void grabbingActivated(monsterAttackSystem grabbedMonster, Transform reelBone ,Vector3 pointOfContact)
    {
        grabActivated = true;
        foreignMonster = grabbedMonster;
        foreignReel = foreignMonster.nativeReel;
        foreignReel.parent = null;
        foreignReel.position = pointOfContact;
        nativeReel.position = pointOfContact;
        nativeReel.parent = reelBone;
        foreignMonster.transform.parent = foreignReel;
    }

    public void grabbingStabilized()
    {
        nativeReel.parent = null;
    }

    public void grabbingCanceled()
    {
        // /*
        if (grabActivated)
        {
            foreignMonster.transform.parent = null;
            foreignReel.parent = foreignMonster.transform;
            foreignReel = null;
            foreignMonster = null;
            nativeReel.parent = this.transform;
            nativeReel.position = Vector3.zero;
        }

        grabActivated = false;
        // */
    }

    public void attackFocusOn()
    {
        focusedAttackActive = true;
        myAnimator.SetBool("Idle Bounce Allowed", false);
        myAnimator.SetBool("Calm", false);
        calm = false;
        forceEndEmote();
        forceStopCrouch();
        stopForceFall();
        stopRunVFX();
        myAnimator.ResetTrigger("Back to Prior State");
        //myAnimator.SetTrigger("Back to Prior State");

        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].bounceCorrections(false);
        }

        if (myPlayer != null)
        {
            myPlayer.lockPlayerController();
        }
    }

    public void attackFocusOff()
    {
        focusedAttackActive = false;
        heavyAttackActive = false;
        if (isGrounded && isRunning == false)
        {
            myAnimator.SetBool("Idle Bounce Allowed", true);
            myAnimator.SetBool("Calm", false);
            calm = false;
        }
        myAnimator.ResetTrigger("Right Attack Release");
        myAnimator.ResetTrigger("Left Attack Release");
        myAnimator.SetTrigger("Back to Prior State");
        stopRunVFX();


        for (int i = 0; i < allMonsterParts.Length; i++)
        {
           allMonsterParts[i].resetBracing();
        }

        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].bounceCorrections(true);
        }

        if (myPlayer != null)
        {
            myPlayer.unlockPlayerController();
        }

    }

    public void heavyAttackActivated()
    {
        heavyAttackActive = true;
    }

    public void teeterCheck()
    {
        if (isRunning || isWalking)
        {
            return;
        }

        if (onPlatformEdge)
        {
            calm = true;
            myAnimator.SetBool("Calm", true);
            myAnimator.SetBool("Idle Bounce Allowed", false);
            stopRunVFX();

            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].teeterOnEdge();
            }
        }

    }

    public void calmedDown()
    {
        if (isRunning || isWalking)
        {
            return;
        }

        if (fullyFlighted)
        {
            if(isGrounded)
            {
                myAnimator.SetBool("Idle Bounce Allowed", true);
                myAnimator.SetBool("Calm", false);
                calm = false;
            }
            return;
        }

        if (calm == false && isGrounded && emoteActive == false)
        {

            if (onPlatformEdge == false)
            {
                calm = true;
                myAnimator.SetBool("Calm", true);
                myAnimator.SetBool("Idle Bounce Allowed", false);

                for (int i = 0; i < allMonsterParts.Length; i++)
                {
                    allMonsterParts[i].calmedDown();
                }
            }
        }
    }

    public void activeBounce()
    {
        calm = false;
        myAnimator.SetBool("Calm", false);
        myAnimator.SetBool("Idle Bounce Allowed", true);

        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].bounceCorrections(true);
        }
    }

    public void enteredPlatformEdge()
    {
        onPlatformEdge = true;
        myAnimator.SetBool("Idle Bounce Allowed", false);
        //tell all parts we're at the platform edge
        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].isAtEdge();
        }
    }

    public void exitedPlatformEdge()
    {
        onPlatformEdge = false;
        //tell all parts we're no longer at the platform edge
        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].notAtEdge();
        }
    }

    public void runIntoAWall()
    {
        if (isGrounded && isRunning)
        {
            isRunning = false;
            isWalking = false;
            focusedAttackActive = false;
            calm = false;
            myAnimator.SetTrigger("Neutral Damage");
            myAnimator.SetBool("Idle Bounce Allowed", false);
            myAnimator.SetBool("Calm", false);
            myAnimator.ResetTrigger("Back to Prior State");
            myAnimator.ResetTrigger("Right Attack Release");
            myAnimator.ResetTrigger("Left Attack Release");
            isGliding = false;
            glideVisual.Stop();
            myAnimator.ResetTrigger("Glide to Attack");
            myAnimator.SetBool("Gliding", false);

            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].triggerNeutralDamage(false);
                allMonsterParts[i].resetBracing();
                allMonsterParts[i].bounceCorrections(true);
            }

            forceEndEmote();
            forceStopCrouch();
            StartCoroutine(neutralDamageRecoveryTimer());
        }
    }

    public void chargeForward()
    {
        myPlayer.nonStopChargeForward();
        releaseRunVFX();
    }

    #endregion

    #region Health

    public void neutralDamage()
    {
        //this can interrupt running, walking, screeching turns, jumps, double jumps, wing flaps, lands, gliding, falling, neutral attacks, wind ups, emotes
        //idles (active and calm), launching, other damage intakes, leaping upwards attacks, rolling upwards and downwards attacks, stomp attacks, teetering

        //what isn't interrupted: heavy attacks, dash attacks, rolling

        if (heavyAttackActive)
        {
            return;
        }

        damageLocked = true;
        isRunning = false;
        isWalking = false;
        focusedAttackActive = false;
        calm = false;
        myAnimator.SetTrigger("Neutral Damage");
        myAnimator.SetBool("Idle Bounce Allowed", false);
        myAnimator.SetBool("Calm", false);
        myAnimator.ResetTrigger("Back to Prior State");
        myAnimator.ResetTrigger("Right Attack Release");
        myAnimator.ResetTrigger("Left Attack Release");
        isGliding = false;
        glideVisual.Stop();
        myAnimator.ResetTrigger("Glide to Attack");
        myAnimator.SetBool("Gliding", false);
        myAnimator.ResetTrigger("Recover");
        forceEndEmote();
        forceStopCrouch();
        stopForceFall();

        
        damageComboCounter++;

        if (timeSinceLastDamage <= 0.5f)
        {
            /*
            if (damageComboCounter >= 3)
            {
                //if not a neutral projectile, neutral beam, or reel
                //sent launching
                knockback();
                damageComboCounter = 0; //we're going to have to pass in a bool or int about what attack type it is to see if launching is appropriate
                return;
            }
            */

            if (damageAnimationAltNeeded)
            {
                damageAnimationAltNeeded = false;
            }
            else
            {
                damageAnimationAltNeeded = true;
            }
        }
        else
        {
            damageComboCounter = 1;
            damageAnimationAltNeeded = false;
        }
        timeSinceLastDamage = 0;
        

        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].triggerNeutralDamage(damageAnimationAltNeeded);//pass in a different int for what torso animation to play
            allMonsterParts[i].resetBracing();
            allMonsterParts[i].bounceCorrections(true);
        }

        StartCoroutine(neutralDamageRecoveryTimer());
    }

    IEnumerator neutralDamageRecoveryTimer()
    {
        //this entire section should be controlled by player controller, not monster attack/animation system
        yield return new WaitForSeconds(0.1f);
        myAnimator.SetBool("Idle Bounce Allowed", isGrounded);
        damageLocked = false;

        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].triggerNeutralDamageRecovery();
        }
    }

    public void heavyDamage(bool hasKnockBack)
    {
        //this can interrupt running, walking, screeching turns, jumps, double jumps, wing flaps, lands, gliding, falling, neutral attacks, wind ups, emotes
        //idles (active and calm), launching, other damage intakes, leaping upwards attacks, rolling upwards and downwards attacks, stomp attacks, teetering

        //what isn't interrupted: heavy attacks, dash attacks, rolling

        if (heavyAttackActive)
        {
            return;
        }

        if (damageAnimationAltNeeded)
        {
            damageAnimationAltNeeded = false;
        }
        else
        {
            damageAnimationAltNeeded = true;
        }

        damageLocked = true;
        isRunning = false;
        isWalking = false;
        focusedAttackActive = false;
        calm = false;
        myAnimator.SetTrigger("Heavy Damage");
        myAnimator.SetBool("Idle Bounce Allowed", false);
        myAnimator.SetBool("Calm", false);
        myAnimator.ResetTrigger("Back to Prior State");
        myAnimator.ResetTrigger("Right Attack Release");
        myAnimator.ResetTrigger("Left Attack Release");
        isGliding = false;
        glideVisual.Stop();
        myAnimator.ResetTrigger("Glide to Attack");
        myAnimator.SetBool("Gliding", false);
        myAnimator.ResetTrigger("Recover");

        if (hasKnockBack)
        {
            knockback();
        }
        else
        {
            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].triggerNeutralDamage(damageAnimationAltNeeded);
                allMonsterParts[i].resetBracing();
                allMonsterParts[i].bounceCorrections(true);
            }
        }

        forceEndEmote();
        forceStopCrouch();
        stopForceFall();
        StartCoroutine(heavyDamageRecoveryTimer());
    }

    IEnumerator heavyDamageRecoveryTimer()
    {
        yield return new WaitForSeconds(0.15f);
        damageLocked = false;
    }

    public void knockback()
    {
        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].triggerLaunch();
            allMonsterParts[i].resetBracing();
            allMonsterParts[i].bounceCorrections(true);
        }

        isGrounded = false;
        isLaunching = true;
        myAnimator.SetTrigger("Launch");
        StartCoroutine(spinTimer());
    }

    IEnumerator spinTimer()
    {
        yield return new WaitForSeconds(0.2f);
        myAnimator.SetTrigger("Spin");
    }

    public void getOutOfLaunch()
    {
        if (isLaunching)
        {
            isLaunching = false;
            myAnimator.SetTrigger("Recover");

            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].triggerHeavyDamageRecovery();
            }
        }
    }

    public void popOffMonsterPart(monsterPart partRemoved)
    {
        for (int i = 0; i < attackSlotMonsterParts.Length; i++)
        {
            if (attackSlotMonsterParts[i] != null)
            {
                if (attackSlotMonsterParts[i] == partRemoved)
                {
                    partRemoved.disconnectThisPart();
                    destructionPhysicsHelper.SetActive(true);
                    StartCoroutine(removeMonsterPartFromStage(attackSlotMonsterParts[i].gameObject));
                    //store some sort of parental and location data
                    //bool saying disconnect
                    //blank animation for part
                    //remove part's parent
                    //turn on its rigidbody
                    //while we're at it, maybe just turn off the monster part script
                }
            }
        }

        healthCheck();
    }

    private void healthCheck()
    {
        int limbsStillActive = 0;
        for (int i = 0; i < attackSlotMonsterParts.Length; i++)
        {
            if (attackSlotMonsterParts[i] != null)
            {
                if (attackSlotMonsterParts[i].connected)
                {
                    limbsStillActive++;
                }
            }
        }

        if (limbsStillActive == 0)
        {
            totalDestruction();
        }
        else
        {
            partDestructionVisual.Play();
        }
    }

    public void totalDestruction()
    {

        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            if (allMonsterParts[i].connected)
            {
                nonMappedMonsterParts.Add(allMonsterParts[i]);
            }
        }

        for (int i = 0; i < nonMappedMonsterParts.Count; i++)
        {
            nonMappedMonsterParts[i].disconnectThisPart();
        }

        StartCoroutine(removeAllMonsterPartsFromStage());

        destructionVisual.Play();
        //spawn goo
    }

    private void turnOffAllMonsterPartPhysics()
    {

        for (int i = 0; i < nonMappedMonsterParts.Count; i++)
        {
            nonMappedMonsterParts[i].turnOffPhysics();
        }
    }


    private void deactivateAllMonsterParts()
    {

        for (int i = 0; i < nonMappedMonsterParts.Count; i++)
        {
            nonMappedMonsterParts[i].gameObject.SetActive(false);
        }
    }

    private void reactivateAllMonsterParts()
    {

        for (int i = 0; i < nonMappedMonsterParts.Count; i++)
        {
            nonMappedMonsterParts[i].gameObject.SetActive(true);
        }
    }

    IEnumerator removeMonsterPartFromStage(GameObject partToDisappear)
    {
        yield return new WaitForSeconds(0.1f);
        destructionPhysicsHelper.SetActive(false);
        yield return new WaitForSeconds(3);
        partToDisappear.gameObject.SetActive(false);
        partToDisappear.GetComponent<Rigidbody>().isKinematic = true;
        yield return new WaitForSeconds(0.2f);
        partToDisappear.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        partToDisappear.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        partToDisappear.SetActive(true);
        yield return new WaitForSeconds(0.4f);
        partToDisappear.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        partToDisappear.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        partToDisappear.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        partToDisappear.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        partToDisappear.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        partToDisappear.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        partToDisappear.SetActive(false);

    }

    IEnumerator removeAllMonsterPartsFromStage()
    {
        yield return new WaitForSeconds(0.1f);
        destructionPhysicsHelper.SetActive(false);
        yield return new WaitForSeconds(8);
        deactivateAllMonsterParts();
        turnOffAllMonsterPartPhysics();
        yield return new WaitForSeconds(0.2f);
        reactivateAllMonsterParts();
        yield return new WaitForSeconds(0.5f);
        deactivateAllMonsterParts();
        yield return new WaitForSeconds(0.2f);
        reactivateAllMonsterParts();
        yield return new WaitForSeconds(0.4f);
        deactivateAllMonsterParts();
        yield return new WaitForSeconds(0.3f);
        reactivateAllMonsterParts();
        yield return new WaitForSeconds(0.1f);
        deactivateAllMonsterParts();
        yield return new WaitForSeconds(0.1f);
        reactivateAllMonsterParts();
        yield return new WaitForSeconds(0.1f);
        deactivateAllMonsterParts();
        yield return new WaitForSeconds(0.1f);
        reactivateAllMonsterParts();
        yield return new WaitForSeconds(0.1f);
        deactivateAllMonsterParts();
    }

    #endregion

    #region VFX
    public void releaseFullHeavyVisual()
    {
        fullHeavyVFX.Stop();
        fullHeavyVFX.Play();
    }
    #endregion

    #region Emotes
    public void fierceEmote()
    {
        if (damageLocked)
        {
            return;
        }

        if (focusedAttackActive == false && isGrounded && emoteActive == false && isRunning == false && isWalking == false && isCrouching == false)
        {
            emoteActive = true;
            calm = false;
            myAnimator.SetBool("Idle Bounce Allowed", false);

            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].fierceEmote();
            }

            forceStopCrouch();
        }
    }

    public void gasEmote()
    {
        if (damageLocked)
        {
            return;
        }

        if (focusedAttackActive == false && isGrounded && emoteActive == false && isRunning == false && isWalking == false && isCrouching == false)
        {
            emoteActive = true;
            calm = false;
            myAnimator.SetBool("Idle Bounce Allowed", false);
            gasVisual.Stop();
            gasVisual.Play();

            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].gasEmote();
            }

            forceStopCrouch();
        }
    }

    public void mockingEmote()
    {
        if (damageLocked)
        {
            return;
        }

        if (focusedAttackActive == false && isGrounded && emoteActive == false && isRunning == false && isWalking == false && isCrouching == false)
        {
            emoteActive = true;
            calm = false;
            myAnimator.SetBool("Idle Bounce Allowed", false);

            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].mockingEmote();
            }

            forceStopCrouch();
        }
    }

    public void danceEmote()
    {
        if (damageLocked)
        {
            return;
        }

        if (focusedAttackActive == false && isGrounded && emoteActive == false && isRunning == false && isWalking == false && isCrouching == false)
        {
            emoteActive = true;
            calm = false;
            myAnimator.SetBool("Idle Bounce Allowed", false);
            musicVisual.Stop();
            musicVisual.Play();

            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].danceEmote();
            }

            forceStopCrouch();
        }
    }

    public void jackEmote()
    {
        if (damageLocked)
        {
            return;
        }

        if (focusedAttackActive == false && isGrounded && emoteActive == false && isRunning == false && isWalking == false && isCrouching == false)
        {
            emoteActive = true;
            calm = false;
            myAnimator.SetBool("Idle Bounce Allowed", false);

            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].jackEmote();
            }

            forceStopCrouch();
        }
    }

    public void thinkingEmote()
    {
        if (damageLocked)
        {
            return;
        }

        if (focusedAttackActive == false && isGrounded && emoteActive == false && isRunning == false && isWalking == false && isCrouching == false)
        {
            emoteActive = true;
            calm = false;
            myAnimator.SetBool("Idle Bounce Allowed", false);

            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].thinkingEmote();
            }

            forceStopCrouch();
        }
    }

    public void booEmote()
    {
        if (damageLocked)
        {
            return;
        }

        if (focusedAttackActive == false && isGrounded && emoteActive == false && isRunning == false && isWalking == false && isCrouching == false)
        {
            emoteActive = true;
            calm = false;
            spookyVisual.Stop();
            spookyVisual.Play();
            myAnimator.SetBool("Idle Bounce Allowed", false);

            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].booEmote();
            }

            forceStopCrouch();
        }

    }

    public void excerciseEmote()
    {
        if (damageLocked)
        {
            return;
        }

        if (focusedAttackActive == false && isGrounded && emoteActive == false && isRunning == false && isWalking == false && isCrouching == false)
        {
            emoteActive = true;
            calm = false;
            myAnimator.SetBool("Idle Bounce Allowed", false);

            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].excerciseEmote();
            }

            forceStopCrouch();
        }
    }

    public void hulaEmote()
    {
        if (damageLocked)
        {
            return;
        }

        if (focusedAttackActive == false && isGrounded && emoteActive == false && isRunning == false && isWalking == false && isCrouching == false)
        {
            emoteActive = true;
            calm = false;
            hulaHoopVisual.Stop();
            hulaHoopVisual.Play();
            myAnimator.SetBool("Idle Bounce Allowed", false);

            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].hulaEmote();
            }

            forceStopCrouch();
        }
    }

    public void vomitEmote()
    {
        if (damageLocked)
        {
            return;
        }

        if (focusedAttackActive == false && isGrounded && emoteActive == false && isRunning == false && isWalking == false && isCrouching == false)
        {
            emoteActive = true;
            calm = false;
            vomitVisual.Stop();
            vomitVisual.Play();
            myAnimator.SetBool("Idle Bounce Allowed", false);

            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].vomitEmote();
            }

            forceStopCrouch();
        }
    }

    public void brianEmote()
    {
        if (damageLocked)
        {
            return;
        }

        if (focusedAttackActive == false && isGrounded && emoteActive == false && isRunning == false && isWalking == false && isCrouching == false)
        {
            emoteActive = true;
            calm = false;
            myAnimator.SetBool("Idle Bounce Allowed", false);

            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].brianEmote();
            }

            forceStopCrouch();
        }
    }

    public void sleepEmote()
    {
        if (damageLocked)
        {
            return;
        }

        if (focusedAttackActive == false && isGrounded && emoteActive == false && isRunning == false && isWalking == false && isCrouching == false)
        {
            emoteActive = true;
            calm = false;

            if (facingRight)
            {
                //sleepVisual_Right.Stop();
                //sleepVisual_Right.Play();
                sleepVisual.Stop();
                sleepVisual.Play();
            }
            else
            {
                sleepVisual.Stop();
                sleepVisual.Play();
            }

            myAnimator.SetBool("Idle Bounce Allowed", false);

            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].sleepEmote();
            }

            forceStopCrouch();
        }
    }

    public void explosiveEmote()
    {
        if (damageLocked)
        {
            return;
        }

        if (focusedAttackActive == false && isGrounded && emoteActive == false && isRunning == false && isWalking == false && isCrouching == false)
        {
            emoteActive = true;
            calm = false;
            fieryVisual.Stop();
            fieryVisual.Play();
            myAnimator.SetBool("Idle Bounce Allowed", false);

            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].explosiveEmote();
            }

            forceStopCrouch();
        }
    }

    public void laughingEmote()
    {
        if (damageLocked)
        {
            return;
        }

        if (focusedAttackActive == false && isGrounded && emoteActive == false && isRunning == false && isWalking == false && isCrouching == false)
        {
            emoteActive = true;
            calm = false;
            myAnimator.SetBool("Idle Bounce Allowed", false);

            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].laughingEmote();
            }

            forceStopCrouch();
        }
    }

    public void sneezingEmote()
    {
        if (damageLocked)
        {
            return;
        }

        if (focusedAttackActive == false && isGrounded && emoteActive == false && isRunning == false && isWalking == false && isCrouching == false)
        {
            emoteActive = true;
            calm = false;
            sneezeVisual.Stop();
            sneezeVisual.Play();
            myAnimator.SetBool("Idle Bounce Allowed", false);

            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].sneezingEmote();
            }

            forceStopCrouch();
        }
    }

    public void emoteEnded()
    {
        emoteActive = false;
        musicVisual.Stop();
        spookyVisual.Stop();
        sleepVisual.Stop();
        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].emoteCorrections();
        }
        calmedDown();
    }

    public void forceEndEmote()
    {
        if (emoteActive)
        {
            emoteActive = false;
            myAnimator.SetBool("Calm", false);
            fieryVisual.Stop();
            gasVisual.Stop();
            musicVisual.Stop();
            spookyVisual.Stop();
            vomitVisual.Stop();
            hulaHoopVisual.Stop();
            sleepVisual.Stop();
            sneezeVisual.Stop();
            for (int i = 0; i < allMonsterParts.Length; i++)
            {
                allMonsterParts[i].triggerForceStopEmote();
            }
        }
    }
    #endregion

    #region Corrections

    public void correctAttackDirection(int limbPlacement)
    {
        if (limbPlacement == -1) //punch sent from left limb while facing left --> full turn; punch sent from left limb while facing right --> half turn
        {
            //myAnimator.SetTrigger("Left Attack Release");
            myAnimator.SetTrigger("Forward Attack Release");
        }
        else if(limbPlacement == 1) //punch sent from right limb while facing right --> full turn; punch sent from right limb while facing left --> half turn
        {
            //myAnimator.SetTrigger("Right Attack Release");
            myAnimator.SetTrigger("Forward Attack Release");
        }
        else
        {
            myAnimator.SetTrigger("Forward Attack Release");
        }
    }

    public void correctWalkingAttackAnimations()
    {
        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].walkToAttackCorrections();
        }
    }

    public void correctRunningAttackAnimations()
    {
        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].runToAttackCorrections();
        }
    }

    public void correctRollControl()
    {
        canRoll = true;
        if (isGrounded && isRunning == false && focusedAttackActive == false)
        {
            myAnimator.SetBool("Idle Bounce Allowed", true);
        }
    }

    public void correctLeapingAttackData()
    {
        if (isGrounded && isRunning == false && focusedAttackActive == false)
        {
            myAnimator.SetBool("Idle Bounce Allowed", true);
        }
    }

    #endregion

    #region Cutscene and Cinematic Specifics

    public void getHeadAttention()
    {
        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].headTurnToTarget();
        }
    }

    public void loseHeadAttention()
    {
        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].returnHeadToNormalState();
        }
    }

    public void getTorsoAttention()
    {
        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].torsoTurnToTarget();
        }
    }

    public void loseTorsoAttention()
    {
        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].returnTorsoToNormalState();
        }
    }

    #endregion
}

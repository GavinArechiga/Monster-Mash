using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System.Linq;
using Random = UnityEngine.Random;

public class HealthEventArgs : EventArgs
{
    public int MaxHealth;
    public int CurrentHealth;
    public int SegmentHealth;
}


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
    public bool isCrouching = false;
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
    public bool calm = false;
    public bool emoteActive = false;
    public bool onPlatformEdge;
    public bool damageLocked = false;
    private bool forceFallingActivated = false;
    private bool isLaunching = false;

    public playerController myPlayer;
    public Animator myAnimator;
    private Animator mainTorso;

    public NewMonsterPart[] attackSlotMonsterParts = new NewMonsterPart[8];
    private int[] attackSlotMonsterID = new int[8];
    private List<monsterPartReference> listOfInternalReferences = new List<monsterPartReference>();
    public NewMonsterPart[] allMonsterParts;
    private List<NewMonsterPart> nonMappedMonsterParts = new List<NewMonsterPart>();

    [Header("Damage and Status Effects")]
    [SerializeField] private int maxHealth = 800;
    public event EventHandler OnMonsterPartRemoved;
    public event EventHandler<HealthEventArgs> OnHealthUpdated;
    public event EventHandler OnMonsterDeath;

    private int segmentHealth;
    private int currentHealth;
    private HealthEventArgs healthEventArgs = new();

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

    // Emotes
    [Header("Emotes")]
    public EmoteManager emoteManager;

    private void OnDestroy()
    {
        // the events are already unsubscribed in popoffmonsterpart() but this is just for safety in case the object is suddenely distroyed for some reason.
        foreach (NewMonsterPart monsterPart in attackSlotMonsterParts)
        {
            if (monsterPart != null)
            {
                // if you enter player mode and immeditely exit this will throw an error because the event was never subscribed to.
                // We can't check events for null so this is a simple work arround to prevent the error.
                try
                {
                    monsterPart.neutralAttack.OnAttackRelease -= myPlayer.ApplyMovementModifier;
                    monsterPart.heavyAttack.OnAttackRelease -= myPlayer.ApplyMovementModifier;
                }
                catch
                {
                    
                }
            }
        }
    }


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
        NewMonsterPart[] monsterPartList = GetComponentsInChildren<NewMonsterPart>();
        for (int i = 0; i < monsterPartList.Length; i++)
        {
            if (monsterPartList[i].PartType is not MonsterPartType.Torso && monsterPartList[i].PartType is not MonsterPartType.Head && monsterPartList[i].monsterPartID == 1)
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
        myAnimator = GetComponent<Animator>();
        emoteManager.Initilize(this, myPlayer);
        floorCheck.SetActive(false);
        grabAttackSlotInfo();
        myAnimator.SetBool("Facing Right", facingRight);

        allMonsterParts = GetComponentsInChildren<NewMonsterPart>();

        #region Figuring out if this Monster has one leg, many legs, or is a legless guy

        bool hasLeftGroundedLegs = false;
        bool hasRightGroundedLegs = false;
        bool hasWings = false;
        List<NewMonsterPart> allGroundedRightLegs = new List<NewMonsterPart>();
        List<NewMonsterPart> allGroundedLeftLegs = new List<NewMonsterPart>();
        List<NewMonsterPart> allWings = new List<NewMonsterPart>();

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

            if (allMonsterParts[i].isGroundedLimb == false && allMonsterParts[i].PartType is MonsterPartType.Leg)
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

            if (allMonsterParts[i].PartType is MonsterPartType.Wing)
            {
                allWings.Add(allMonsterParts[i]);
                hasWings = true;
                isWinged = true;
            }

            if (allMonsterParts[i].PartType is MonsterPartType.Torso)
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
                        if (allMonsterParts[i].PartType is MonsterPartType.Torso)
                        {
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
            allMonsterParts[i].AttackSetup();
            allMonsterParts[i].triggerAnimationSetUp();
            allMonsterParts[i].triggerAnimationOffsets();
            allMonsterParts[i].triggerCollisionLogic(); //collision logic must come after animation set up because animation set up includes projectile set up 
            allMonsterParts[i].triggerIdle();
        }

       
        // connects all the attacking monster parts OnAttackrelease event to the player controller
        foreach (NewMonsterPart monsterPart in attackSlotMonsterParts)
        {
            if (monsterPart != null)
            {
                // if we don't unsubscribe first then ApplyMovemnetModifer can be called multiple times for one action
                monsterPart.neutralAttack.OnAttackRelease -= myPlayer.ApplyMovementModifier;
                monsterPart.heavyAttack.OnAttackRelease -= myPlayer.ApplyMovementModifier;

                monsterPart.neutralAttack.OnAttackRelease += myPlayer.ApplyMovementModifier;
                monsterPart.heavyAttack.OnAttackRelease += myPlayer.ApplyMovementModifier;
            }
        }

        myAnimator.SetBool("Idle Bounce Allowed", true);
        StartCoroutine(spawnRenactment());
        myAnimator.SetBool("Calm", false);
        calm = false;

        SFXManager = FindObjectOfType<SFXManager>();

        // set default emotes
        myPlayer.playerControlsMap.Emotes.Enable();

        CalculateStartHealth();
    }

    public void AssignMyPlayer(playerController controller)
    {
        myPlayer = controller;
    }

    /*
    public void AttackListSetUp()
    {
        monsterAttackInformation = new List<MonsterPartAttackData>();
    }

    public void AddMonsterAttackInformation(MonsterPartData.Button assignedButton, monsterPart assignedPart)
    {
        MonsterPartAttackData data = new MonsterPartAttackData(assignedButton, assignedPart);

        monsterAttackInformation.Add(data);
    }

    */

    public void AssignMonsterPartAttackInfo(MonsterPartData.Button assignedButton, NewMonsterPart part)
    {
        attackSlotMonsterParts[(int)assignedButton] = part;
    }


    IEnumerator spawnRenactment()
    {
        myAnimator.SetBool("Spawning In", true);
        myAnimator.SetTrigger("Spawn In");
        yield return new WaitForSeconds(0.2f);

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

            if (allMonsterParts[i].PartType is MonsterPartType.Wing)
            {
                allMonsterParts[i].hasFlightedIdle = false;
            }

            if (allMonsterParts[i].PartType is MonsterPartType.Leg)
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

    public List<NewMonsterPart> GetActiveAttackSlots()
    {
        List<NewMonsterPart> activeAttackSlots = attackSlotMonsterParts.Where(part => part != null && part.connected).ToList();
        return activeAttackSlots;
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
                    /*
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
                    */
                }
                else if (attackSlotMonsterParts[attackSlot].attackAnimationID == 1)
                {
                    //requiresFlourishingTwirl = true;
                    //requiresFlourishingRoll = false;
                    /*
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
                    */
                }
                else if (attackSlotMonsterParts[attackSlot].attackAnimationID == -1)
                {
                    /*
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
                    */
                }
                else if (attackSlotMonsterParts[attackSlot].attackAnimationID == 2)
                {
                    /*
                    requiresFlourishingTwirl = false;
                    requiresFlourishingRoll = false;
                    */
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
                allMonsterParts[i].PartVisual.triggerRoll(isGrounded, true);
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
            allMonsterParts[i].PartVisual.triggerRoll(isGrounded, true);
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
            allMonsterParts[i].PartVisual.triggerRoll(isGrounded, true);
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
        if (damageLocked || !facingRight)
        {
            return;
        }

        myAnimator.ForceResetTriggers();
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
        if (damageLocked || facingRight)
        {
            return;
        }

        myAnimator.ForceResetTriggers();
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
                allMonsterParts[i].PartVisual.triggerWalk();
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
                allMonsterParts[i].PartVisual.triggerWalk();
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
                    allMonsterParts[i].PartVisual.triggerStopWalking();
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
                allMonsterParts[i].PartVisual.triggerRun();
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
                allMonsterParts[i].PartVisual.triggerRun();
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
                    allMonsterParts[i].PartVisual.triggerStopRunning();
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
                    allMonsterParts[i].PartVisual.triggerStopRunning();
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
                allMonsterParts[i].PartVisual.triggerScreechingStop();
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
            allMonsterParts[i].PartVisual.triggerJump();
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
            allMonsterParts[i].PartVisual.triggerRoll(false, false);
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
                    allMonsterParts[i].PartVisual.triggerGlide();
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
            allMonsterParts[i].PartVisual.triggerFall();
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
                allMonsterParts[i].PartVisual.triggerFall();
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
                allMonsterParts[i].PartVisual.triggerRoll(false, false);
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
                allMonsterParts[i].PartVisual.triggerLand();
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
            allMonsterParts[i].PartVisual.triggerLateLand();
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
                allMonsterParts[i].PartVisual.triggerSimpleUngrounded();
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
                allMonsterParts[i].PartVisual.triggerRoll(true, true);
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
                    allMonsterParts[i].PartVisual.triggerCrouch();
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
                allMonsterParts[i].PartVisual.triggerCrouch();
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
            allMonsterParts[i].PartVisual.triggerCrouchStop();
        }

        if (isRunning == false && isWalking == false)
        {
            myAnimator.SetBool("Idle Bounce Allowed", true);
        }
    }

    public void forceStopCrouch()
    {
        isCrouching = false;

        for (int i = 0; i < allMonsterParts.Length; i++) //wrap this in a bool
        {
            allMonsterParts[i].PartVisual.triggerCrouchStop();
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
                    allMonsterParts[i].PartVisual.triggerForceFallStop();
                }

                forceFallingActivated = false;
            }
            else
            {

                for (int i = 0; i < allMonsterParts.Length; i++)
                {
                    allMonsterParts[i].PartVisual.triggerForceFall();
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
                allMonsterParts[i].PartVisual.triggerForceFall();
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
                allMonsterParts[i].PartVisual.triggerForceFallStop();
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
            allMonsterParts[i].PartVisual.triggerRightAttackStance();
        }
    }

    public void braceForLeftImpact()
    {
        myAnimator.SetBool("Idle Bounce Allowed", false);

        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].PartVisual.triggerLeftAttackStance();
        }
    }

    public void braceForForwardImpact()
    {
        myAnimator.SetBool("Idle Bounce Allowed", false);

        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].PartVisual.triggerForwardStance();
        }
    }

    public void braceForBackwardImpact()
    {
        myAnimator.SetBool("Idle Bounce Allowed", false);

        for (int i = 0; i < allMonsterParts.Length; i++)
        {
            allMonsterParts[i].PartVisual.triggerBackwardStance();
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
            allMonsterParts[i].PartVisual.triggerFlourishStance();
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
            allMonsterParts[i].PartVisual.triggerUnbrace();
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
            //myPlayer.lockPlayerController();
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

        for (int i = 0; i < GetActiveAttackSlots().Count; i++)
        {
            GetActiveAttackSlots()[i].forceTriggerJabOrSlashCollisionsOff();
        }

        if (myPlayer != null)
        {
            //myPlayer.unlockPlayerController();
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


    private void CalculateStartHealth()
    {
        currentHealth = maxHealth;
        List<NewMonsterPart> activeAttackSlots = GetActiveAttackSlots();
        segmentHealth = maxHealth / activeAttackSlots.Count;

        healthEventArgs.MaxHealth = maxHealth;
        healthEventArgs.CurrentHealth = currentHealth;
        healthEventArgs.SegmentHealth = segmentHealth;
    }

    public void DecreaseHealth(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        healthEventArgs.CurrentHealth = currentHealth;
        OnHealthUpdated?.Invoke(this, healthEventArgs);

        while (maxHealth - currentHealth >= segmentHealth)
        {
            var activeAttackSlots = GetActiveAttackSlots();
            if (activeAttackSlots.Count > 0)
            {
                popOffMonsterPart(activeAttackSlots[Random.Range(0, activeAttackSlots.Count)]);

                if (GetActiveAttackSlots().Count == 0)
                {
                    totalDestruction();
                    return;
                }
            }

            currentHealth += segmentHealth; // Increase current health by one segment untill it reaches the max health

            // Remove the reaminder if it is not enouth for a full segment
            if ((maxHealth - currentHealth) < segmentHealth)
            {
                currentHealth = maxHealth;
            }

            healthEventArgs.CurrentHealth = currentHealth;
            OnHealthUpdated?.Invoke(this, healthEventArgs);
        }
    }



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
        //StartCoroutine(spinTimer());
    }

    /*IEnumerator spinTimer()
    {
        yield return new WaitForSeconds(0.2f);
        myAnimator.SetTrigger("Spin");
    }*/

    public IEnumerator SpinTimer()
    {
        myAnimator.SetTrigger("Flourish Twirl");
        yield return new WaitForSeconds(1f);
        myAnimator.ResetTrigger("Flourish Twirl");
        myAnimator.SetTrigger("Land");

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

    public void popOffMonsterPart(NewMonsterPart partRemoved)
    {
        for (int i = 0; i < attackSlotMonsterParts.Length; i++)
        {
            if (attackSlotMonsterParts[i] != null)
            {
                if (attackSlotMonsterParts[i] == partRemoved)
                {
                    partRemoved.disconnectThisPart();
                    partRemoved.neutralAttack.OnAttackRelease -= myPlayer.ApplyMovementModifier;
                    partRemoved.heavyAttack.OnAttackRelease -= myPlayer.ApplyMovementModifier;
                    destructionPhysicsHelper.SetActive(true);
                    OnMonsterPartRemoved?.Invoke(this, EventArgs.Empty);
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

        StartCoroutine(RemoveAllPartsAndDestroyPlayer());
        //spawn goo
        destructionVisual.Play();
        OnMonsterDeath?.Invoke(this, EventArgs.Empty);
    }

    private IEnumerator RemoveAllPartsAndDestroyPlayer()
    {
        int partsCount = nonMappedMonsterParts.Count;
        int finishedCount = 0;

        // Use a local function to track completion
        IEnumerator RemovePartCoroutine(GameObject part)
        {
            yield return StartCoroutine(removeMonsterPartFromStage(part));
            finishedCount++;
        }

        // Start all coroutines
        for (int i = 0; i < nonMappedMonsterParts.Count; i++)
        {
            StartCoroutine(RemovePartCoroutine(nonMappedMonsterParts[i].gameObject));
        }

        // Wait until all are finished
        while (finishedCount < partsCount)
        {
            yield return null;
        }

        if (playerManager.Instance != null)
        {
            playerManager.Instance.RemovePlayer(myPlayer);
        }
    }

    IEnumerator removeMonsterPartFromStage(GameObject partToDisappear)
    {
        // Initial delays and setup
        yield return new WaitForSeconds(0.1f);
        destructionPhysicsHelper.SetActive(false);

        yield return new WaitForSeconds(3f);
        partToDisappear.SetActive(false);
        partToDisappear.GetComponent<Rigidbody>().isKinematic = true;

        // Toggle part on/off
        float toggleInterval = Random.Range(0.2f, 0.5f);
        int toggleCount = 8;
        bool activeState = true;
        for (int i = 0; i < toggleCount; i++)
        {
            yield return new WaitForSeconds(toggleInterval);
            partToDisappear.SetActive(activeState);
            activeState = !activeState;
        }

        Destroy(partToDisappear);
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

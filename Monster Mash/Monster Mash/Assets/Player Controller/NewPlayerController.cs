using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class NewPlayerController : MonoBehaviour
{
    public int playerIndex;
    public monsterAttackSystem myMonster;
    private playerAudioManager myAudioSystem;
    public CapsuleCollider2D bodyCollider;
    public CircleCollider2D smallBodyCollider;
    public BoxCollider2D groundFrictionCollider;
    //public brainSFX mySFXBrain;
    //public MeshRenderer standingVisual;
    //public MeshRenderer ballVisual;
    public Transform groundCheck;
    public Transform headCheck;
    public LayerMask solidGroundLayer;
    public LayerMask semiSolidGroundLayer;
    public PlayerInput playerInput;
    public PlayerControls playerControlsMap;
    private InputActionMap startingActionMap;
    private InputActionMap UIcontrols;
    //private bool UIcontrolsNeeded = true;
    private InputActionMap monsterControls;
    [SerializeField] private Collider2D currentPlatformCollider;
    //private bool monsterControlsNeeded = false;
    bool monsterControllerActive = false;
    public bool facingRight;
    Vector2 leftJoystickVector; //gives us direction on x axis
    float leftJoystickValue; //gives us nuance of input between magnitudes
    public bool isAttacking = false;
    // Normal is attacking only works when player is on the ground and we need it to work while in the air for the left stick jump
    private bool leftStickIsAttacking = false;

    private float walkSpeed = 5f;
    private float runSpeed = 25f;
    private float groundedModifer = 1;
    private float airbornModifer = 0.75f;
    private float currentGroundedStateModifier = 1;
    private bool isDamageLaunching;
    public bool canMove = true;
    public bool isWalking = false;
    public bool isRunning = false;

    public bool grounded = false;
    private bool atPlatformEdge = false;
    private Vector2 platformEdgeCooridinates;
    private bool requiresLateLand = false;
    public bool landDetectionReady = true;
    //public bool onSemiSolid = false;
    private bool isCrouching = false;
    public bool isPhasingThroughPlatform;
    private bool isFastFalling = false;
    public bool canJump = true;
    bool jumpButtonReset = false;
    //public bool primedForBigJump = false;
    public int numberOfJumps = 2;
    public int numberOfJumpsLeft = 2;
    /// <summary>
    /// When holding the left stick up how many secounds before the jump actually activates
    /// </summary>
    private float LeftStickJumpDelayTime = 0.1f;
    /// <summary>
    /// How many secounds the player has been holding the left stick in the upwards direction
    /// </summary>
    private float leftStickElapsedJumpTime;
    private Vector2 jumpValue;
    private bool jumpValueHasBeenRead = true;
    private float bigJumpPower = 65;//80
    private float littleJumpPower = 45;//60
    private bool slowFallBlocked = false;
    private float slowFallGravityPower = 6;
    private float gravityPower;
    private bool insideFloor = false;

    public bool isGooball; //flings self through air, splats on walls, ball rolls on ground, double jump, medium movement, limited air control
    public bool isGlider; //glides through air, grabs opponents, slow fall flying while holding A, many more jumps, slower ground movement
    public bool isHunter; //zips through opponents, in air connects to nearest opponent in radius, faster ground movement, triple jump
    public bool isTechnoid; //places or destroys portals, can attack through portals, in air can place platforms that destroy upon exit

    private float rollSpeed = 50f;//50
    Vector2 rightJoystickVector; //gives us direction on x axis for roll
    public bool isRolling = false;
    public bool canRoll = true;

    private float dashSpeed = 60f;
    public bool isDashing = false;
    public bool canDash = true;

    private bool ledgeHopAvailable = true;

    public bool grappling = false;
    public bool grapplingPlayer = false;
    public bool grapplingWall = false;
    private float grappleSpeed = 300;
    private float endPlayerGrappleDistance = 8f;
    private float endWallPlayerGrappledistance = 5f;
    public NewPlayerController grapplePlayerTarget;
    public Vector3 wallGrapplePoint;

    public bool chargingForward = false;

    public bool grabbingWall = false;
    private float wallGrabbingGravityPower = 0.2f;
    private float wallJumpPower = 28f;

    private bool buttonA_Pressed = false;
    private bool buttonB_Pressed = false;
    private bool buttonX_Pressed = false;
    private bool buttonY_Pressed = false;

    private int neutralAttackSFXIndex = 1;
    private int heavyAttackSFXIndex = 1;

    [SerializeField]
    private Rigidbody2D myRigidbody;
    Vector2 lastInputDirectionVector;
    float directionThreshold = 0.2f;
    private enum InputDirection
    {
        Forward = 1,
        Backward = -1,
        Up = 2,
        Down = 0
    }

    private InputDirection lastInputDirection = InputDirection.Forward;

    [Header("Damage Launching")]
    [SerializeField] private AnimationCurve damageToForceCurve;
    [SerializeField, Tooltip("Controls how much of an arc the launch has for left and right")] private float yMultiplier = 1.5f;


    // damage timer
    float lastAttackTime = -Mathf.Infinity;

    public List<NewMonsterPart> allParts;
    public List<NewMonsterPart> legs;

    private List<NewMonsterPart> GetAllPartsInRoot()
    {
        var allParts = new List<NewMonsterPart>(transform.root.GetComponentsInChildren<NewMonsterPart>(true));
        return allParts;
    }

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerControlsMap = new PlayerControls();
        myAudioSystem = GetComponentInChildren<playerAudioManager>();
        gravityPower = myRigidbody.gravityScale;

        if(playerInput != null)
        {
            startingActionMap = playerInput.actions.FindActionMap("Starting Action Map");
            UIcontrols = playerInput.actions.FindActionMap("UI Controls");
            monsterControls = playerInput.actions.FindActionMap("Monster Controls");

            if (UIcontrols != null)
            {
                playerInput.SwitchCurrentActionMap("UI Controls");
                playerControlsMap.StartingActionMap.Disable();
                //print("New Action Map: " + playerInput.currentActionMap);
            }
        }
    }
    private void Start()
    {
        allParts = GetAllPartsInRoot();
        legs = allParts.Where(part => part.PartType == MonsterPartType.Leg).ToList();
    }

    private void OnDestroy()
    {
        UnsubscribeActionMap();
    }

    public void PlayerInputSetUp()
    {
        startingActionMap = playerInput.actions.FindActionMap("Starting Action Map");
        UIcontrols = playerInput.actions.FindActionMap("UI Controls");
        monsterControls = playerInput.actions.FindActionMap("Monster Controls");

        if (UIcontrols != null)
        {
            playerInput.SwitchCurrentActionMap("UI Controls");
            playerControlsMap.StartingActionMap.Disable();
            //print("New Action Map: " + playerInput.currentActionMap);
        }

        switchActionMap("Monster Controls");

        SubscribeActionMap();
    }

    private void SubscribeActionMap()
    {
        playerInput.actions.FindActionMap("Monster Controls").FindAction("Left Stick").performed += OnLeftStick;

        playerInput.actions.FindActionMap("Monster Controls").FindAction("Left Stick").canceled += OnLeftStick;


        playerInput.actions.FindActionMap("Monster Controls").FindAction("Right Stick").performed += OnRightStick;

        playerInput.actions.FindActionMap("Monster Controls").FindAction("Right Stick").canceled += OnRightStick;

        playerInput.actions.FindActionMap("Monster Controls").FindAction("A").started += onButtonA;
        playerInput.actions.FindActionMap("Monster Controls").FindAction("A").canceled += onButtonA;

        playerInput.actions.FindActionMap("Monster Controls").FindAction("B").started += onButtonB;
        playerInput.actions.FindActionMap("Monster Controls").FindAction("B").canceled += onButtonB;

        playerInput.actions.FindActionMap("Monster Controls").FindAction("X").started += onButtonX;
        playerInput.actions.FindActionMap("Monster Controls").FindAction("X").canceled += onButtonX;

        playerInput.actions.FindActionMap("Monster Controls").FindAction("Y").started += onButtonY;
        playerInput.actions.FindActionMap("Monster Controls").FindAction("Y").canceled += onButtonY;

        playerInput.actions.FindAction("ShowMenu").Enable();
        playerInput.actions.FindAction("ShowMenu").performed += ShowRemappingMenu;
    }

    private void UnsubscribeActionMap()
    {
        if (playerInput == null) {  return; }

        playerInput.actions.FindActionMap("Monster Controls").FindAction("Left Stick").performed -= OnLeftStick;

        playerInput.actions.FindActionMap("Monster Controls").FindAction("Left Stick").canceled -= OnLeftStick;


        playerInput.actions.FindActionMap("Monster Controls").FindAction("Right Stick").performed -= OnRightStick;

        playerInput.actions.FindActionMap("Monster Controls").FindAction("Right Stick").canceled -= OnRightStick;

        playerInput.actions.FindActionMap("Monster Controls").FindAction("A").started -= onButtonA;
        playerInput.actions.FindActionMap("Monster Controls").FindAction("A").canceled -= onButtonA;

        playerInput.actions.FindActionMap("Monster Controls").FindAction("B").started -= onButtonB;
        playerInput.actions.FindActionMap("Monster Controls").FindAction("B").canceled -= onButtonB;

        playerInput.actions.FindActionMap("Monster Controls").FindAction("X").started -= onButtonX;
        playerInput.actions.FindActionMap("Monster Controls").FindAction("X").canceled -= onButtonX;

        playerInput.actions.FindActionMap("Monster Controls").FindAction("Y").started -= onButtonY;
        playerInput.actions.FindActionMap("Monster Controls").FindAction("Y").canceled -= onButtonY;

        playerInput.actions.FindAction("ShowMenu").Disable();
        playerInput.actions.FindAction("ShowMenu").performed -= ShowRemappingMenu;
    }

    private void ShowRemappingMenu(InputAction.CallbackContext ctx)
    {
        if (InputRemapper.Instance == null) {  return; }

        InputRemapper.Instance.ShowMenu(playerInput);
    }

    

    public void switchActionMap(string newActionMap)
    {
        //might cut this first section, its a bit unecessary
        if (UIcontrols != null)
        {
            if (newActionMap == "UI Controls" && playerInput.currentActionMap == UIcontrols)
            {
                return;
            }
        }

        if (playerInput != null)
        {
            playerInput.SwitchCurrentActionMap(newActionMap);
        }

        if (newActionMap == "Monster Controls")
        {
            myRigidbody.bodyType = RigidbodyType2D.Dynamic;
            monsterControllerActive = true;
        }
        else
        {
            myRigidbody.bodyType = RigidbodyType2D.Kinematic;
            monsterControllerActive = false;
        }
    }

    private void Update()
    {
        //This section moves the x axis of the player
        //For moving the y axis of the player, check out the jumping category of the movement section
        //chances are we'll be moving most of this movement to a seperate script so that we can enable or disable with ease and not have all this running all the time
        if (monsterControllerActive)
        {

            if (!isPhasingThroughPlatform && !isGrounded())
            {
                Collider2D platform = GetClosestPlatform();
                if (platform != null)
                {
                    currentPlatformCollider = platform;
                }   
            }

            if (isPhasingThroughPlatform)
            {
                phase();
            }
            else
            {
                antiPhase(true);
            }

            if (isPhasingThroughPlatform)
            {
                // Only check for re-enabling if we are currently phasing
                bool touching = false;
                if (bodyCollider.IsTouchingLayers(semiSolidGroundLayer)) touching = true;
                if (smallBodyCollider.IsTouchingLayers(semiSolidGroundLayer)) touching = true;
                if (groundFrictionCollider.IsTouchingLayers(semiSolidGroundLayer)) touching = true;

                if (!touching)
                {
                    currentPlatformCollider = null;
                    isPhasingThroughPlatform = false;
                }
            }


            if (isGrounded() && (myRigidbody.velocity.y < 0f || myRigidbody.velocity.y == 0f))
            {
                if (grounded == false)
                {
                    land();
                }
            }
            else if (isSemiGrounded())
            {
                if (grounded == false && (myRigidbody.velocity.y < 0f || myRigidbody.velocity.y == 0f) && isPhasingThroughPlatform == false && landDetectionReady)
                {
                    land();
                }

            }
            else if ((myRigidbody.velocity.y < 0f || myRigidbody.velocity.y == 0f) && myRigidbody.gravityScale != slowFallGravityPower && canMove && slowFallBlocked == false)
            {
                //falling
                activateSlowFall();
            }

            if(isWalking == false && myMonster.isWalking)
            {
                stopWalkingVisual();
            }

            if(isRunning == false && myMonster.isRunning)
            {
                stopRunningVisual();
            }
            else if (isRunning && myMonster.isRunning == false)
            {
                startRunningVisual();
            }

            if (canMove)
            {

                if (isGrounded() || isSemiGrounded())
                {

                    if (isWalking == false && isRunning == false && isPhasingThroughPlatform == false && groundFrictionCollider.enabled == false && grounded)
                    {
                        turnOnFriction();
                    }
                     

                    if ((leftJoystickVector.x > 0.9f || leftJoystickVector.x < -0.9f))
                    {
                        //run
                        if (leftJoystickVector.x > 0.9f)
                        {
                            //right
                            if (isRunning == false)
                            {
                                isRunning = true;
                                isWalking = false;
                                stopWalkingVisual();
                                startRunningVisual();
                            }

                            myRigidbody.velocity = new Vector2(1 * runSpeed, myRigidbody.velocity.y);
                        }
                        else
                        {
                            //left
                            if (isRunning == false)
                            {
                                isRunning = true;
                                isWalking = false;
                                stopWalkingVisual();
                                startRunningVisual();
                            }

                            myRigidbody.velocity = new Vector2(-1 * runSpeed, myRigidbody.velocity.y);
                        }
                    }
                    else if ((leftJoystickVector.x > 0.2f || leftJoystickVector.x < -0.2f))
                    {

                        if (leftJoystickVector.x > 0.2f)
                        {
                            myRigidbody.velocity = new Vector2(1 * walkSpeed, myRigidbody.velocity.y);
                        }
                        else
                        {
                            myRigidbody.velocity = new Vector2(-1 * walkSpeed, myRigidbody.velocity.y);
                        }
                    }
                    else if ((leftJoystickVector.x < 0.1f && leftJoystickVector.x > -0.1f))
                    {
                        myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);

                        if (isWalking && isPhasingThroughPlatform == false && grounded)
                        {
                            isWalking = false;
                            isRunning = false;
                            stopWalkingVisual();
                            startMiscIdleAnimations();
                            turnOnFriction();
                        }

                        if (isRunning && isPhasingThroughPlatform == false && grounded)
                        {
                            isRunning = false;
                            isWalking = false;
                            stopRunningVisual();
                            startMiscIdleAnimations();
                        }
                    }
                    

                }
                else
                {
                    if (leftJoystickVector.x > 0.9f || leftJoystickVector.x < -0.9f)
                    {

                        if (leftJoystickVector.x > 0.9f)
                        {
                            //right
                            myRigidbody.velocity = new Vector2(1 * runSpeed / 1.8f, myRigidbody.velocity.y);
                        }
                        else
                        {
                            //left
                            myRigidbody.velocity = new Vector2(-1 * runSpeed / 1.8f, myRigidbody.velocity.y);
                        }
                    }
                    else if (leftJoystickVector.x > 0.2f || leftJoystickVector.x < -0.2f)
                    {

                        if (leftJoystickVector.x > 0.2f)
                        {
                            //right
                            myRigidbody.velocity = new Vector2(1 * walkSpeed / 2, myRigidbody.velocity.y);
                        }
                        else
                        {
                            //left
                            myRigidbody.velocity = new Vector2(-1 * walkSpeed / 2, myRigidbody.velocity.y);
                        }
                    }
                    else if ((leftJoystickVector.x < 0.1f && leftJoystickVector.x > -0.1f))
                    {
                        myRigidbody.velocity = new Vector2(myRigidbody.velocity.x - 0.001f, myRigidbody.velocity.y);

                        if (isWalking && isPhasingThroughPlatform == false)
                        {
                            isWalking = false;
                            isRunning = false;
                            stopWalkingVisual();
                            stopRunningVisual();
                            startMiscIdleAnimations();
                        }

                        if (isRunning && isPhasingThroughPlatform == false)
                        {
                            isRunning = false;
                            isWalking = false;
                            stopWalkingVisual();
                            stopRunningVisual();
                            startMiscIdleAnimations();
                        }
                    }
                }

                if (isBelowSemiGround() && isGrounded() == false)
                {
                    if (isPhasingThroughPlatform == false)
                    {
                        phase();
                        isPhasingThroughPlatform = true;
                        isCrouching = false;
                        isFastFalling = false;
                    }
                }

                if (buttonA_Pressed == false && buttonB_Pressed == false && buttonX_Pressed == false && buttonY_Pressed == false && isAttacking == false)
                {
                    if (leftJoystickVector.y > 0.4f && Mathf.Abs(leftJoystickVector.x) < 0.4f)
                    {
                        // Cancels the jump if you attack during the delay window. I don't like using a seperate bool for this but isAttacking does not work reliably 
                        if (leftStickIsAttacking)
                        {
                            return;
                        }
                       
                        leftStickElapsedJumpTime += Time.deltaTime;
                        if (canJump && numberOfJumpsLeft > 0 && jumpButtonReset)
                        {
                            // Adds a delay so that the movement modifiers have time to activate
                            if (leftStickElapsedJumpTime >= LeftStickJumpDelayTime)
                            {
                                bigJump();
                            }
                        }
                    }
                    else
                    {
                        if (leftJoystickValue < 0.1f)
                        {
                            leftStickElapsedJumpTime = 0;
                            leftStickIsAttacking = false;
                        }
                        
                    }

                    if (leftJoystickVector.y < 0.05f && jumpButtonReset == false)
                    {
                        jumpButtonReset = true;
                    }
                    



                    if (leftJoystickVector.y < -0.6f && (leftJoystickVector.x < 0.1f && leftJoystickVector.x > -0.1f))
                    {
                        //down stick -> either crouch or go through semi solid or fast fall
                        if (isGrounded())
                        {
                            //crouch
                            if (isCrouching == false)
                            {
                                startCrouchVisual();
                                isCrouching = true;
                                isPhasingThroughPlatform = false;
                                isFastFalling = false;

                                if ((isRunning || isWalking))
                                {
                                    if (isWalking)
                                    {
                                        stopWalkingVisual();
                                    }

                                    if (isRunning)
                                    {
                                        stopRunningVisual();
                                    }

                                    isRunning = false;
                                    isWalking = false;
                                    startMiscIdleAnimations();
                                }
                                else
                                {
                                    turnOnFriction();
                                }
                            }
                        }
                        else if (isSemiGrounded())
                        {
                            //fall through platform
                            if (isPhasingThroughPlatform == false)
                            {
                                phase();
                                phaseThroughPlatformVisual();
                                grounded = false;
                                isCrouching = false;
                                isFastFalling = false;
                                landDetectionReady = false;
                            }
                        }
                    }
                    else
                    {
                        if (isCrouching)
                        {
                            endCrouchVisual();
                            isCrouching = false;
                        }
                    }
                    
                }

            }

            if (isAttacking == false)
            {
                if (isRolling)
                {

                    if (rightJoystickVector.x > 0.2f)
                    {
                        myRigidbody.velocity = new Vector2(1 * rollSpeed, myRigidbody.velocity.y);
                    }
                    else if (rightJoystickVector.x < -0.2f)
                    {
                        myRigidbody.velocity = new Vector2(-1 * rollSpeed, myRigidbody.velocity.y);
                    }
                    
                }
            }

            if (chargingForward)
            {
                if (facingRight)
                {
                    myRigidbody.velocity = new Vector2(1 * (runSpeed * 2), myRigidbody.velocity.y);
                }
                else
                {
                    myRigidbody.velocity = new Vector2(-1 * (runSpeed * 2), myRigidbody.velocity.y);
                }
            }
        }
    }


    public void OnLeftStick(InputAction.CallbackContext context)
    {
        leftJoystickVector = context.ReadValue<Vector2>();
        leftJoystickValue = context.ReadValue<Vector2>().magnitude;

        if (context.canceled)
        {
            jumpButtonReset = true;
            lastInputDirection = facingRight ? InputDirection.Forward : InputDirection.Backward;
            return;
        }

        if (context.performed)
        {
            if (Mathf.Abs(leftJoystickVector.x) > directionThreshold || Mathf.Abs(leftJoystickVector.y) > directionThreshold)
            {
                lastInputDirectionVector = leftJoystickVector.normalized;
                UpdateInputDirection(lastInputDirectionVector);


                if (grabbingWall == false && isDashing == false && isRolling == false && canMove)
                {
                    if (lastInputDirection is InputDirection.Forward)
                    {
                        flipRightVisual();
                    }
                    else if (lastInputDirection is InputDirection.Backward)
                    {
                        flipLeftVisual();
                    }
                }

                //Debug.Log($"Last Input Direction:{lastInputDirection} Vector: {lastInputDirectionVector}");
            }

            if (buttonA_Pressed || buttonB_Pressed || buttonX_Pressed || buttonY_Pressed || canMove == false)
            {
                return;
            }

            if (isPhasingThroughPlatform == false && groundFrictionCollider.enabled && isCrouching == false && canMove && isAttacking == false)
            {
                turnOffFriction();
            }

            if (canMove && isAttacking == false)
            {
                if (leftJoystickVector.x > 0.9f || leftJoystickVector.x < -0.9f)
                {
                    //run

                    if (isRunning == false)
                    {
                        isRunning = true;
                        isWalking = false;
                        startRunningVisual();
                        //stopWalkingVisual();
                        turnOffFriction();
                    }
                }
                else if (leftJoystickVector.x > 0.2f || leftJoystickVector.x < -0.2f || leftJoystickVector.x == 0.2f || leftJoystickVector.x == -0.2f)
                {
                    //walk

                    if (isWalking == false)
                    {
                        isWalking = true;
                        isRunning = false;
                        startWalkingVisual();
                        stopRunningVisual();
                        turnOffFriction();
                    }
                }
            }
        }
    }


    private void UpdateInputDirection(Vector2 directionVector)
    {
        if (Mathf.Abs(directionVector.x) > Mathf.Abs(directionVector.y))
        {
            if (directionVector.x > directionThreshold)
            {
                lastInputDirection = InputDirection.Forward;
            }
            else if (directionVector.x < -directionThreshold)
            {
                lastInputDirection = InputDirection.Backward;
            }
        }
        else if (Mathf.Abs(directionVector.y) > directionThreshold)
        {
            if (directionVector.y > directionThreshold)
            {
                lastInputDirection = InputDirection.Up;
            }
            else if (directionVector.y < -directionThreshold)
            {
                lastInputDirection = InputDirection.Down;
            }
        }
    }

    private bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.5f, solidGroundLayer);
    }

    private bool isSemiGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.5f, semiSolidGroundLayer);
    }

    private bool isBelowSemiGround()
    {
        return Physics2D.OverlapCircle(headCheck.position, 1f, semiSolidGroundLayer);
    }

    private Collider2D GetClosestPlatform()
    {
        return Physics2D.OverlapCircle(headCheck.position, 1f, semiSolidGroundLayer);
    }

    private void land()
    {
        isDamageLaunching = false;
        grounded = true;
        numberOfJumpsLeft = numberOfJumps;
        StopCoroutine(jumpRecharge());
        isFastFalling = false;
        landDetectionReady = true;
        insideFloor = false;

        if (grabbingWall)
        {
            endWallGrabVisual();
        }
        grabbingWall = false;
        if (isAttacking == false)
        {
            landVisual();
            canMove = true;
            myRigidbody.gravityScale = gravityPower;
        }
        else
        {
            myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
            turnOnFriction();
            myRigidbody.gravityScale = gravityPower;
            return;
        }

        if ((leftJoystickVector.x < 0.1f && leftJoystickVector.x > -0.1f))
        {
            myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);

            if (isWalking && isPhasingThroughPlatform == false && grounded)
            {
                isWalking = false;
                isRunning = false;
                stopWalkingVisual();
                startMiscIdleAnimations();
                turnOnFriction();
            }

            if (isRunning && isPhasingThroughPlatform == false && grounded)
            {
                isRunning = false;
                isWalking = false;
                stopRunningVisual();
                startMiscIdleAnimations();
            }
        }

        if ((isRunning || isWalking) && isAttacking == false && canMove)
        {
            turnOffFriction();
        }
        else if (leftJoystickVector.x < 0.1f && leftJoystickVector.x > -0.1f)
        {
            turnOnFriction();
        }
    }

    private void bigJump()
    {

        if (numberOfJumpsLeft > 0)
        {
            if (isWalking)
            {
                stopWalkingVisual();
            }

            if (isRunning)
            {
                stopRunningVisual();
            }

            if (numberOfJumpsLeft == numberOfJumps)
            {
                bigJumpVisual();

            }
            else
            {
                doubleJumpVisual();
            }

            numberOfJumpsLeft = numberOfJumpsLeft - 1;
            grounded = false;
            jumpButtonReset = false;
            slowFallBlocked = false;
            myRigidbody.gravityScale = gravityPower;
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, bigJumpPower);
            StartCoroutine(jumpRecharge());
        }
    }

    IEnumerator jumpRecharge()
    {
        canJump = false;
        yield return new WaitForSeconds(0.2f);
        canJump = true;
    }

    public void OnRightStick(InputAction.CallbackContext context)
    {
        rightJoystickVector = context.ReadValue<Vector2>();


        if (context.performed && grabbingWall == false && isDashing == false && isRolling == false)
        {
            if (facingRight == false && rightJoystickVector.x > 0.1f)
            {
                //face right
                facingRight = true;
                flipRightVisual();
            }
            else if (facingRight && rightJoystickVector.x < -0.1f)
            {
                //face left
                facingRight = false;
                flipLeftVisual();
            }
        }

        if (context.performed && (canDash || canRoll) && grabbingWall == false)
        {

            if (isGrounded() || isSemiGrounded())
            {

                if (canRoll && isRolling == false)
                {

                    if (rightJoystickVector.x > 0.1f || rightJoystickVector.x < -0.1f)
                    {
                        
                        if (facingRight == false && rightJoystickVector.x > 0.1f)
                        {
                            backwardEntryTeleportalVisual();
                        }
                        else if (facingRight && rightJoystickVector.x < -0.1f)
                        {
                            backwardEntryTeleportalVisual();
                        }
                        else
                        {
                            forwardEntryTeleportalVisual();
                        }
                        StartCoroutine(rollTime());
                    }
                }
            }
            else
            {
                if (canDash && isDashing == false && isRolling == false && wallToFloorCheck() == false)
                {
                    if (rightJoystickVector.x > 0.1f || rightJoystickVector.x < -0.1f)
                    {

                        if (facingRight == false && rightJoystickVector.x > 0.1f)
                        {
                            //face right
                            flipRightVisual();
                        }
                        else if (facingRight && rightJoystickVector.x < -0.1f)
                        {
                            //face left
                            flipLeftVisual();
                        }

                        StartCoroutine(dashTime());
                    }
                }
            }

        }
    }

    IEnumerator rollTime()
    {
        turnOffFriction();
        myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
        isRolling = true;
        canMove = false;
        canRoll = false;
        canDash = false;
        if (isWalking)
        {
            stopWalkingVisual();
        }

        if (isRunning)
        {
            stopRunningVisual();
        }

        isWalking = false;
        isRunning = false;
        rollVisual();
        yield return new WaitForSeconds(0.2f);
        reEntryTeleportalVisual();
        myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
        isRolling = false;
        isDashing = false;
        canMove = true;
        isRunning = false;
        isWalking = false;
        StartCoroutine(rollRecharge());
    }

    IEnumerator rollRecharge()
    {
        yield return new WaitForSeconds(1f);
        resetTeleportalVisual();
        canRoll = true;
    }

    IEnumerator dashTime()
    {
        turnOffFriction();
        isDashing = true;
        grabbingWall = false;
        canMove = false;
        canDash = false;
        canRoll = false;
        myRigidbody.gravityScale = 0;
        if (isWalking)
        {
            stopWalkingVisual();
        }

        if (isRunning)
        {
            stopRunningVisual();
        }

        isRunning = false;
        isWalking = false;
        startDashAttackVisual();
        yield return new WaitForSeconds(0.25f);
        isRunning = false;
        isWalking = false;
    }

    private bool wallToFloorCheck()
    {
        return Physics2D.Raycast(transform.position, -transform.up, 2f, solidGroundLayer);
    }

    private void wallJump(int direction)
    {
        myRigidbody.gravityScale = gravityPower;
        grabbingWall = false;
        jumpButtonReset = false;
        numberOfJumpsLeft = numberOfJumps - 1;
        myRigidbody.velocity = new Vector2(direction * wallJumpPower, bigJumpPower);
        StartCoroutine(jumpRecharge());
        endWallGrabVisual();
        isDashing = false;
        isRolling = false;
        canRoll = true;
        canDash = true;
        canMove = true;
    }

    public void onButtonA(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (grabbingWall)
            {
                if (facingRight)
                {
                    wallJump(-1);
                }
                else
                {
                    wallJump(1);
                }

                bigJumpVisual();
            }
            else
            {
                if (canJump && numberOfJumpsLeft > 0 && jumpButtonReset)
                {
                    bigJump();
                }
            }
        }

        if (context.canceled)
        {
            jumpButtonReset = true;
        }
    }

    public void onButtonB(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            myMonster.attackCancel(1);
            buttonB_Pressed = false;
        }

        if (context.started)
        {
            if (myMonster.attackSlotMonsterParts[1] == null) { return; }
            myMonster.attackSlotMonsterParts[1].attackAnimationID = (int)lastInputDirection;
            myMonster.attack(1, (int)lastInputDirection);
            buttonB_Pressed = true;
        }
    }

    public void onButtonX(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            myMonster.attackCancel(2);
            buttonX_Pressed = false;
        }

        if (context.started)
        {
            if (myMonster.attackSlotMonsterParts[2] == null) { return; }
            myMonster.attackSlotMonsterParts[2].attackAnimationID = (int)lastInputDirection;
            myMonster.attack(2, (int)lastInputDirection);
            buttonX_Pressed = true;
        }
    }

    public void onButtonY(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            myMonster.attackCancel(3);
            buttonY_Pressed = false;
        }

        if (context.started)
        {
            if (myMonster.attackSlotMonsterParts[3] == null) { return; }
            myMonster.attackSlotMonsterParts[3].attackAnimationID = (int)lastInputDirection;
            myMonster.attack(3, (int)lastInputDirection);
            buttonY_Pressed = true;
        }
    }

    private void flipLeftVisual()
    {
        myMonster.flipLeft();
        facingRight = false;
    }

    private void flipRightVisual()
    {
        myMonster.flipRight();
        facingRight = true;
    }

    private void startMiscIdleAnimations()
    {
        canMove = true;
        myMonster.focusedAttackActive = false;

        if (atPlatformEdge)
        {
            myMonster.teeterCheck();
        }
        else
        {
            myMonster.activeBounce();
        }
    }

    private void startRunningVisual()
    {
        myMonster.run();
    }

    private void stopRunningVisual()
    {
        myMonster.stopRunning();
    }

    private void startWalkingVisual()
    {
        myMonster.walk();
    }

    private void stopWalkingVisual()
    {
        myMonster.stopWalking();
    }

    private void startTeeterVisual()
    {
        myMonster.enteredPlatformEdge();
    }

    private void stopTeeterVisual()
    {
        myMonster.exitedPlatformEdge();
    }

    private void bigJumpVisual()
    {
        myMonster.jump();
        playJumpSound();
    }

    private void doubleJumpVisual()
    {
        myMonster.doubleJump();
        playDoubleJumpSound();
    }

    private void phaseThroughPlatformVisual()
    {
        myMonster.goThroughPlatform();
    }

    private void landVisual()
    {
        myMonster.land();
    }

    private void lateAttackReleaseVisualCorrections()
    {
        myMonster.forceUngrounded();
    }

    private void startCrouchVisual()
    {
        myMonster.crouch();
    }

    private void endCrouchVisual()
    {
        myMonster.stopCrouching();
    }

    private void startDashAttackVisual()
    {
        myMonster.dashAttack();
    }

    private void endWallGrabVisual()
    {
        myMonster.endWallGrab();
    }

    private void rollVisual()
    {
        myMonster.roll();
    }

    private void forwardEntryTeleportalVisual()
    {
        myMonster.entryTeleportalVFX(true);
    }

    private void backwardEntryTeleportalVisual()
    {
        myMonster.entryTeleportalVFX(false);
    }

    private void reEntryTeleportalVisual()
    {
        myMonster.reEntryTeleportalVFX();
    }

    private void resetTeleportalVisual()
    {
        myMonster.resetTeleportalsVFX();
    }
    

    public void playJumpSound()
    {
        myAudioSystem.playJumpSound();
    }

    public void playDoubleJumpSound()
    {
        myAudioSystem.playDoubleJumpSound();
    }

    public void turnOnFriction()
    {
        if (isRunning || isWalking || isPhasingThroughPlatform)
        {
            return;
        }
        else
        {
            groundFrictionCollider.enabled = true;
        }
    }

    public void turnOffFriction()
    {
        groundFrictionCollider.enabled = false;
    }

    private void phase()
    {
        if (currentPlatformCollider != null)
        {
            Physics2D.IgnoreCollision(bodyCollider, currentPlatformCollider, true);
            Physics2D.IgnoreCollision(smallBodyCollider, currentPlatformCollider, true);
            Physics2D.IgnoreCollision(groundFrictionCollider, currentPlatformCollider, true);
            isPhasingThroughPlatform = true;
        }    
    }

    private void antiPhase(bool hasOverride)
    {
        if (hasOverride || insideFloor == false)
        {
            if (currentPlatformCollider != null)
            {
                Physics2D.IgnoreCollision(bodyCollider, currentPlatformCollider, false);
                Physics2D.IgnoreCollision(smallBodyCollider, currentPlatformCollider, false);
                Physics2D.IgnoreCollision(groundFrictionCollider, currentPlatformCollider, false);
                isPhasingThroughPlatform = false;
            }
        }
    }

    private void activateSlowFall()
    {
        myRigidbody.gravityScale = slowFallGravityPower;
    }

    public void lockPlayerController()
    {
        //Debug.Log("Player Controller Locked");
        canMove = false;
        canJump = false;
        isRunning = false;
        isWalking = false;

        if (grounded)
        {
            myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
        }
    }

    public void unlockPlayerController()
    {
        //Debug.Log("Player Controller Unlocked");
        canMove = true;
        canJump = true;
        isRunning = false;
        isWalking = false;
        isAttacking = false;
        myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);

        
        if (isDashing == false && isRolling == false)
        {
            myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
            turnOnFriction();
            landDetectionReady = true;
        }
    }

    public void nonStopChargeForward()
    {
        chargingForward = true;
        turnOffFriction();
    }

    public void endChargeForward()
    {
        chargingForward = false;
        turnOnFriction();
        myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
    }

    // Listens for when an attack calls Trigger Attack Release
    public void ApplyMovementModifier(object sender, TriggerAttackReleaseEventArgs eventArgs)
    {
        leftStickIsAttacking = true;

        Vector2 currentMovementModifier = Vector2.zero;
        switch (lastInputDirection)
        {
            case InputDirection.Forward:
                currentMovementModifier = eventArgs.MovementModifier.Right;
                break;
            case InputDirection.Backward:
                currentMovementModifier = eventArgs.MovementModifier.Left;
                break;
            case InputDirection.Up:
                currentMovementModifier = eventArgs.MovementModifier.Up;
                break;
            case InputDirection.Down:
                currentMovementModifier = eventArgs.MovementModifier.Down;
                break;
        }

        StartCoroutine(ApplySmoothedMovementModifier(currentMovementModifier, eventArgs.ClipLength));
    }

    // smooths out the movement so that it is not instant and it looks better
    private IEnumerator ApplySmoothedMovementModifier(Vector2 totalOffset, float duration)
    {
        float elapsed = 0f;
        // Clamp the Y offset to a reasonable value (e.g., 5 units)
        float maxY = totalOffset.y;
        float modifierX = totalOffset.x / duration;
        float modifierY = Mathf.Clamp(totalOffset.y / duration, -maxY, maxY);

        while (elapsed < duration)
        {
            myRigidbody.velocity = new Vector2(modifierX, myRigidbody.velocity.y + modifierY);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    public IEnumerator DisableJumping(float seconds)
    {
        lockPlayerController(); 
        yield return new WaitForSeconds(seconds);
        unlockPlayerController(); 
    }

    public void SetGroundedState(bool isGrounded)
    {
        foreach (var part in legs)
        {
            part.isGroundedLimb = isGrounded;
        }
    }

    public void ResetLegAnimations()
    {
        foreach (var part in legs)
        {
            if (part.myAnimator != null)
            {
                part.myAnimator.SetBool("Running", false);
                part.myAnimator.SetBool("Walking", false);
                part.myAnimator.SetBool("Teeter", false);
                part.myAnimator.SetTrigger("Idle");
            }
        }
    }

    public void ResetAttackColliders()
    {
        foreach (var part in allParts)
        {
            part.OnLandedDuringAttack();
        }
    }    

    //damage as to how it relates to the initial strike and the knockback effect
    public void damaged(int damageRecieved, bool markedForHeavyAttack, Vector3 attackerPosition, Vector3 contactPoint)
    {
        // Prevents dammage being applied multiple times for one attack
        if (Time.time - lastAttackTime <= 0.5f) { return; }
        lastAttackTime = Time.time;

        myMonster.DecreaseHealth(damageRecieved);
        isRunning = false;
        isWalking = false;

        bool facingPunch;

        if ((contactPoint.x < transform.position.x && facingRight == false) || (contactPoint.x > transform.position.x && facingRight) || (contactPoint.x == transform.position.x)) //facing punch
        {
            facingPunch = true;
        }
        else
        {
            facingPunch = false;
        }


        if (facingPunch == false) //flip to face attack
        {
            if (facingRight == false)
            {
                //face right
                flipRightVisual();
            }
            else if (facingRight)
            {
                //face left
                flipLeftVisual();
            }
        }

        if (markedForHeavyAttack)
        {
            myMonster.neutralDamage();
            myAudioSystem.playHeavyDamageSound();
            StartCoroutine(damageRecoveryTime(0.1f));
        }
        else
        {
            myMonster.neutralDamage();
            myAudioSystem.playNeutralDamageSound();
            StartCoroutine(damageRecoveryTime(0.1f));
        }

        DammageLaunch(damageRecieved, attackerPosition);
    }

    private void DammageLaunch(int damage, Vector3 attackerPosition)
    {
        isDamageLaunching = true;

        Vector2 diff = attackerPosition - transform.position;
        Vector2 launchDir;

        // Sets the launch direction to the opposite of where the player was hit.
        // Uses the abs value to check if the attack direction is more horizontal or vertical
        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
        {
            // if attacked from the left launch right. if attacked from the left launch right;
            launchDir = diff.x > 0 ? Vector2.left : Vector2.right;
            // Adds a bit of y to give the left and rigt launch an arc
            launchDir.y = yMultiplier;
        }
        else
        {
            // if hit from the top launch down. if hit from the bottom launch up
            launchDir = diff.y > 0 ? Vector2.down: Vector2.up;
        }


        launchDir.Normalize();

        
        Vector2 finalLaunchVector = launchDir * damageToForceCurve.Evaluate(damage);

        float launchDuration = 0.1f;
        myRigidbody.velocity = Vector2.zero;
        StartCoroutine(SmoothLaunch(finalLaunchVector, launchDuration));
    }

    // Makes the launch happen over multiple frames instead of all at once which makes the movement less choppy.
    IEnumerator SmoothLaunch(Vector2 totalForce, float duration)
    {
        if (duration <= 0f)
        {
            myRigidbody.AddForce(totalForce, ForceMode2D.Impulse);
            yield break;
        }

        float elapsed = 0f;

        // Divides the total force by the mass to get the total velocity change for the current hit.
        Vector2 targetVelocityChange = totalForce / myRigidbody.mass;
        Vector2 startVelocity = myRigidbody.velocity;

        while (elapsed < duration)
        {
            // Get a percentage (0 -> 1) of how far we are through the launch
            float t = elapsed / duration;
            // based on the percentage lerp the current velocity towards the total velocity change
            Vector2 desiredVelocity = Vector2.Lerp(startVelocity, startVelocity + targetVelocityChange, t);
            myRigidbody.velocity = desiredVelocity;
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // Ensure final velocity equals what the impulse would’ve done
        myRigidbody.velocity = startVelocity + targetVelocityChange;
    }

    IEnumerator damageRecoveryTime(float recoveryTime)
    {
        //check to make sure that i didnt just land
        yield return new WaitForSeconds(recoveryTime);

        canMove = true;
        isRunning = false;
        isWalking = false;
        if (!isDamageLaunching)
        {
            myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
        }
        

        if (isGrounded())
        {
            myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
            turnOnFriction();
            landDetectionReady = true;
        }

    }

    //damage and effects over time
    public void statusGiven(bool hasBurning, bool hasElectrified, bool hasConfusion, bool hasStink, bool hasCurse, bool hasSlowing, bool hasPoison, 
                            bool hasFreezing, bool hasSlime, int tickDamageRecieved, float timeBetweenTick, float tickDuration)
    {
        if (hasBurning)
        {

        }

        if (hasElectrified)
        {

        }

        if (hasConfusion)
        {

        }

        if (hasStink)
        {

        }

        if (hasCurse)
        {

        }

        if (hasSlowing)
        {

        }

        if (hasPoison)
        {

        }

        if (hasFreezing)
        {

        }

        if (hasSlime)
        {

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Solid")
        {
            if (isPhasingThroughPlatform)
            {
                isPhasingThroughPlatform = false;
                antiPhase(true);
                
            }
        }

        if (collision.gameObject.tag == "Semi Solid")
        {
            currentPlatformCollider = collision;
            insideFloor = true;
        }

        if (collision.gameObject.tag == "Platform Edge")
        {
            startTeeterVisual();
            atPlatformEdge = true;
            platformEdgeCooridinates = new Vector2(collision.gameObject.transform.position.x, collision.gameObject.transform.position.y);

            if (isAttacking && (isGrounded() || isSemiGrounded()) && isDashing == false && isRolling == false)
            {
                myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Semi Solid")
        {
            insideFloor = false;
            landDetectionReady = true;
            isPhasingThroughPlatform = false;
        }

        if (collision.gameObject.tag == "Platform Edge")
        {
            stopTeeterVisual();
            atPlatformEdge = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if(collision.gameObject.tag == "Player")
        {
            if (collision.transform.parent.gameObject.GetComponent<NewPlayerController>() != null)
            {
                NewPlayerController fellowPlayer = collision.transform.parent.gameObject.GetComponent<NewPlayerController>();
            }
        }
        
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerController : MonoBehaviour
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
    private PlayerControls playerControlsMap;
    private InputActionMap startingActionMap;
    private InputActionMap UIcontrols;
    //private bool UIcontrolsNeeded = true;
    private InputActionMap monsterControls;
    //private bool monsterControlsNeeded = false;

    //
    bool monsterControllerActive = false;
    public bool facingRight;
    Vector2 leftJoystickVector; //gives us direction on x axis
    float leftJoystickValue; //gives us nuance of input between magnitudes
    public bool isAttacking = false;
    //
    private float walkSpeed = 5f;
    private float runSpeed = 25f;
    private float groundedModifer = 1;
    private float airbornModifer = 0.75f;
    private float currentGroundedStateModifier = 1;
    public bool canMove = true;
    public bool isWalking = false;
    public bool isRunning = false;
    private int directionModifier = -1;
    //
    public bool grounded = false;
    private bool atPlatformEdge = false;
    private Vector2 platformEdgeCooridinates;
    private bool requiresLateLand = false;
    public bool landDetectionReady = true;
    //public bool onSemiSolid = false;
    private bool isCrouching = false;
    public bool isPhasingThroughPlatform;
    private bool isFastFalling = false;
    bool canJump = true;
    bool jumpButtonReset = false;
    //public bool primedForBigJump = false;
    public int numberOfJumps = 2;
    public int numberOfJumpsLeft = 2;
    private Vector2 jumpValue;
    private bool jumpValueHasBeenRead = true;
    private float bigJumpPower = 65;//80
    private float littleJumpPower = 45;//60
    private bool slowFallBlocked = false;
    private float slowFallGravityPower = 6;
    private float gravityPower;
    private bool insideFloor = false;
    //
    public bool isGooball; //flings self through air, splats on walls, ball rolls on ground, double jump, medium movement, limited air control
    public bool isGlider; //glides through air, grabs opponents, slow fall flying while holding A, many more jumps, slower ground movement
    public bool isHunter; //zips through opponents, in air connects to nearest opponent in radius, faster ground movement, triple jump
    public bool isTechnoid; //places or destroys portals, can attack through portals, in air can place platforms that destroy upon exit
    //
    private float rollSpeed = 50f;//50
    Vector2 rightJoystickVector; //gives us direction on x axis for roll
    public bool isRolling = false;
    public bool canRoll = true;
    //
    private float dashSpeed = 60f;//60
    public bool isDashing = false;
    public bool canDash = true;
    //
    private bool ledgeHopAvailable = true;
    //
    public bool grappling = false;
    public bool grapplingPlayer = false;
    public bool grapplingWall = false;
    private float grappleSpeed = 300;
    private float endPlayerGrappleDistance = 8f;
    private float endWallPlayerGrappledistance = 5f;
    public playerController grapplePlayerTarget;
    public Vector3 wallGrapplePoint;
    //
    public bool chargingForward = false;
    //
    public bool grabbingWall = false;
    private float wallGrabbingGravityPower = 0.2f;
    private float wallJumpPower = 28f;
    //
    private bool buttonA_Pressed = false;
    private bool buttonB_Pressed = false;
    private bool buttonX_Pressed = false;
    private bool buttonY_Pressed = false;

    //

    private int neutralAttackSFXIndex = 1;
    private int heavyAttackSFXIndex = 1;

    [SerializeField]
    private Rigidbody2D myRigidbody;
    private int inputModifier = 1;

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

   

    private void OnDisable()
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

    void SubscribeActionMap()
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
    }

    void UnsubscribeActionMap()
    {

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
            if (isGrounded() && (myRigidbody.velocity.y < 0f || myRigidbody.velocity.y == 0f))
            {
                if (grounded == false)
                {
                    land();
                    /*
                    if (isAttacking)
                    {
                        if (myRigidbody.gravityScale != 0)
                        {
                            myRigidbody.gravityScale = 0;
                        }
                    }
                    else
                    {
                        land();
                    }
                    */
                }
            }
            else if (isSemiGrounded())
            {
                if (grounded == false && (myRigidbody.velocity.y < 0f || myRigidbody.velocity.y == 0f) && isPhasingThroughPlatform == false && landDetectionReady)
                {
                    land();
                    /*
                    if (isAttacking)
                    {
                        if (myRigidbody.gravityScale != 0)
                        {
                            myRigidbody.gravityScale = 0;
                        }
                    }
                    else
                    {
                        land();
                    }
                    */
                }
                
            }else if ((myRigidbody.velocity.y < 0f || myRigidbody.velocity.y == 0f) && myRigidbody.gravityScale != slowFallGravityPower && canMove && slowFallBlocked == false)
            {
                //falling
                activateSlowFall();
            }

            if(isWalking == false && myMonster.isWalking)
            {
                stopWalkingVisual();
            }
            else if (isWalking && myMonster.isWalking == false)
            {
                //startWalkingVisual();
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
                     
                    #region Run and Walk on Ground
                    if ((leftJoystickVector.x > 0.9f || leftJoystickVector.x < -0.9f))
                    {
                        //run

                        /*
                        if (isRunning == false)
                        {
                            isRunning = true;
                            isWalking = false;
                            stopWalkingVisual();
                            startRunningVisual();
                        }
                        */

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
                        //walk

                        /*
                        if (isWalking == false)
                        {
                            isWalking = true;
                            isRunning = false;
                            stopRunningVisual();
                            startWalkingVisual();
                        }
                        */

                        if (leftJoystickVector.x > 0.2f)
                        {
                            //right
                            if (isWalking == false)
                            {
                                //isWalking = true;
                                //isRunning = false;
                                //stopRunningVisual();
                                //startWalkingVisual();
                            }

                            myRigidbody.velocity = new Vector2(1 * walkSpeed, myRigidbody.velocity.y);
                        }
                        else
                        {
                            //left
                            if (isWalking == false)
                            {
                                //isWalking = true;
                                //isRunning = false;
                                //stopRunningVisual();
                                //startWalkingVisual();
                            }

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
                            //forceCalmEffect();
                            startMiscIdleAnimations();
                            turnOnFriction();
                        }

                        if (isRunning && isPhasingThroughPlatform == false && grounded)
                        {
                            isRunning = false;
                            isWalking = false;
                            stopRunningVisual();
                            //forceCalmEffect();
                            startMiscIdleAnimations();
                            //turnOnFriction();
                            StartCoroutine(slideToStop());
                        }
                    }
                    #endregion

                }
                else
                {
                    if (grounded)
                    {
                        if (isWalking)
                        {
                            //stopWalkingVisual();
                        }

                        if (isRunning)
                        {
                            //stopRunningVisual();
                        }

                        grounded = false;
                        //isRunning = false;
                        //isWalking = false;
                        fallVisual();
                    }

                    #region Run and Walk in Air
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
                            //forceCalmEffect();
                            startMiscIdleAnimations();
                        }

                        if (isRunning && isPhasingThroughPlatform == false)
                        {
                            isRunning = false;
                            isWalking = false;
                            stopWalkingVisual();
                            stopRunningVisual();
                            //forceCalmEffect();
                            startMiscIdleAnimations();
                        }
                    }
                    #endregion
   
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

                    #region Jumping
                    /*
                    if (leftJoystickVector.y > 0.25f && leftJoystickValue > 0.25f)
                    {
                        //big jump
                        if (canJump && numberOfJumpsLeft > 0 && jumpButtonReset)
                        {
                            bigJump();
                        }
                    }
                    else if (leftJoystickVector.y > 0.1f)
                    {
                        //little jump
                        if (canJump && numberOfJumpsLeft > 0 && jumpButtonReset)
                        {
                            if (primedForBigJump)
                            {
                                bigJump();
                            }
                            else
                            {
                                littleJump();
                            }
                        }
                    }
                    */

                    if (leftJoystickVector.y > 0.2f && (leftJoystickVector.x < 0.2f && leftJoystickVector.x > -0.2f))
                    {
                        //big jump
                        if (canJump && numberOfJumpsLeft > 0 && jumpButtonReset)
                        {
                            bigJump();
                        }
                    }

                    if (leftJoystickVector.y < 0.05f && jumpButtonReset == false)
                    {
                        jumpButtonReset = true;
                    }
                    #endregion

                    #region Crouching, Phasing through Platforms, and Fast Falling

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
                                    StartCoroutine(slideToStop());
                                    //forceCalmEffect();
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
                                isPhasingThroughPlatform = true;
                                isCrouching = false;
                                isFastFalling = false;
                                landDetectionReady = false;
                            }
                        }
                        else
                        {
                            //fast fall

                            /*

                            if (isFastFalling == false)
                            {
                                isFastFalling = true;
                                isPhasingThroughPlatform = false;
                                isCrouching = false;
                            }
                            */
                        }
                    }
                    else
                    {
                        if (isCrouching)
                        {
                            endCrouchVisual();
                            isCrouching = false;
                        }

                        if (isPhasingThroughPlatform && isSemiGrounded())
                        {
                            isPhasingThroughPlatform = false;

                        }
                    }
                    #endregion
                }

            }


            if (isAttacking == false)
            {
                if (isRolling)
                {
                    #region Rolling Momentum
                    if (rightJoystickVector.x > 0.2f)
                    {
                        myRigidbody.velocity = new Vector2(1 * rollSpeed, myRigidbody.velocity.y);
                    }
                    else if (rightJoystickVector.x < -0.2f)
                    {
                        myRigidbody.velocity = new Vector2(-1 * rollSpeed, myRigidbody.velocity.y);
                    }
                    #endregion
                }

                if (isDashing && grabbingWall == false)
                {
                    #region Dash Momentum and Entering Wall Grab
                    if (rightJoystickVector.x > 0.1f)
                    {
                        myRigidbody.velocity = new Vector2(1 * dashSpeed, 0);

                        if (wallCheck(1) && grabbingWall == false && wallToFloorCheck() == false)
                        {
                            wallGrab(1);
                        }
                    }
                    else if (rightJoystickVector.x < -0.1f)
                    {
                        myRigidbody.velocity = new Vector2(-1 * dashSpeed, 0);

                        if (wallCheck(-1) && grabbingWall == false && wallToFloorCheck() == false)
                        {
                            wallGrab(-1);
                        }
                    }

                    #endregion
                }
                else if (grabbingWall)
                {
                    #region Exiting Wall Grab by land, slipping, or jumping
                    if ((isGrounded() || isSemiGrounded()) && grounded == false)
                    {
                        land();
                    }
                    else if (wallCheck(1) == false && wallCheck(-1) == false && grabbingWall)
                    {
                        slippedOffWall();
                    }

                    if (leftJoystickVector.x < -0.1f)
                    {
                        //jump left
                        wallJump(-1);
                        bigJumpVisual();
                    }
                    else if (leftJoystickVector.x > 0.1f)
                    {
                        //jump right
                        wallJump(1);
                        bigJumpVisual();
                    }
                    #endregion
                }
            }

            #region Grappling
            if (grappling)
            {
                if (grapplePlayerTarget != null && grapplingPlayer)
                {
                    float distanceFromTarget = Vector3.Distance(grapplePlayerTarget.transform.position, transform.position);

                    if (distanceFromTarget > endPlayerGrappleDistance)
                    {
                        Vector3 directionOfGrapple = grapplePlayerTarget.transform.position - transform.position;
                        directionOfGrapple.Normalize();
                        myRigidbody.MovePosition(transform.position + (directionOfGrapple * grappleSpeed * Time.deltaTime));
                    }
                    else
                    {
                        grappling = false;
                        grapplePlayerTarget = null;
                        wallGrapplePoint = new Vector3(0, 0, 0);
                        myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
                    }
                }
                else if (wallGrapplePoint != new Vector3(0,0,0) && grapplingWall)
                {
                    float distanceFromTarget = Vector3.Distance(wallGrapplePoint, transform.position);

                    if (distanceFromTarget > endWallPlayerGrappledistance)
                    {
                        Vector3 directionOfGrapple = wallGrapplePoint - transform.position;
                        directionOfGrapple.Normalize();
                        myRigidbody.MovePosition(transform.position + (directionOfGrapple * grappleSpeed * Time.deltaTime));
                    }
                    else
                    {
                        grappling = false;
                        grapplePlayerTarget = null;
                        wallGrapplePoint = new Vector3(0, 0, 0);
                        myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
                    }
                }
                else
                {
                    grappling = false;
                }
            }
            #endregion

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

    #region Left Stick - Walk, Run, Big Jump, Little Jump
    public void OnLeftStick(InputAction.CallbackContext context)
    {
        leftJoystickVector = context.ReadValue<Vector2>();
        leftJoystickValue = context.ReadValue<Vector2>().magnitude;

        if (context.canceled)
        {
            jumpButtonReset = true;
            inputModifier = 1;
        }

        /*
        if (context.performed && grabbingWall == false && isDashing == false && isRolling == false)
        {
            if (facingRight == false && leftJoystickVector.x > 0.1f)
            {
                //face right
                facingRight = true;
                directionModifier = 1;
                flipRightVisual();
            }
            else if (facingRight && leftJoystickVector.x < -0.1f)
            {
                //face left
                facingRight = false;
                directionModifier = -1;
                flipLeftVisual();
            }
        }
        */

        if (context.performed)
        {
            if ((leftJoystickVector.x > 0.9f && facingRight) || (leftJoystickVector.x < -0.9f && facingRight == false))
            {
                inputModifier = 1; //they'll be attacking forward
            }
            else if (leftJoystickVector.y > 0.2f)
            {
                inputModifier = 2;
            }
            else if (leftJoystickVector.y < -0.2f)
            {
                inputModifier = 0;
            }
            else if ((leftJoystickVector.x > 0.9f && facingRight == false) || (leftJoystickVector.x < -0.9f && facingRight))
            {
                inputModifier = -1;
            }

            if (grabbingWall == false && isDashing == false && isRolling == false && canMove)
            {
                if (facingRight == false && leftJoystickVector.x > 0.1f)
                {
                    //face right
                    facingRight = true;
                    directionModifier = 1;
                    flipRightVisual();
                }
                else if (facingRight && leftJoystickVector.x < -0.1f)
                {
                    //face left
                    facingRight = false;
                    directionModifier = -1;
                    flipLeftVisual();
                }
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

    private void land()
    {
        grounded = true;
        numberOfJumpsLeft = numberOfJumps;
        StopCoroutine(jumpRecharge());
        bodyCollider.enabled = true;
        smallBodyCollider.enabled = true;
        isPhasingThroughPlatform = false;
        isFastFalling = false;
        landDetectionReady = true;
        canJump = true;
        canDash = true;
        insideFloor = false;
        //canRoll = true;
        if (grabbingWall)
        {
            endWallGrabVisual();
        }
        grabbingWall = false;
        /*
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
        */
        //primedForBigJump = false;
        if (isAttacking == false)
        {
            landVisual();
            canMove = true;
            myRigidbody.gravityScale = gravityPower;
        }
        else
        {
            lateLandVisualCorrections();
            myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
            turnOnFriction();
            requiresLateLand = true;
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
                //forceCalmEffect();
                startMiscIdleAnimations();
                turnOnFriction();
            }

            if (isRunning && isPhasingThroughPlatform == false && grounded)
            {
                isRunning = false;
                isWalking = false;
                stopRunningVisual();
                //forceCalmEffect();
                startMiscIdleAnimations();
                //turnOnFriction();
                StartCoroutine(slideToStop());
            }
        }

        /*
        if (isWalking || isRunning)
        {
            
        }
        else
        {
            stopWalkingVisual();
            stopRunningVisual();
        }
        */

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
            //isRunning = false;
            //isWalking = false;
            //primedForBigJump = true;
            slowFallBlocked = false;
            myRigidbody.gravityScale = gravityPower;
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, bigJumpPower);
            StartCoroutine(jumpRecharge());
            //StartCoroutine(landDetectionDelay());

            if (isBelowSemiGround())
            {
                phase();
                isPhasingThroughPlatform = true;
                isCrouching = false;
                isFastFalling = false;
            }
        }
    }

    /*
    private void littleJump()
    {
        if (numberOfJumpsLeft > 0)
        {
            if (numberOfJumpsLeft == numberOfJumps)
            {
                playJumpSound();
            }
            else
            {
                playDoubleJumpSound();
            }

            numberOfJumpsLeft--;
            grounded = false;
            jumpButtonReset = false;
            primedForBigJump = true;
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, littleJumpPower);
            StartCoroutine(jumpRecharge());
            littleJumpVisual();
            playJumpSound();

            if (isBelowSemiGround())
            {
                phase();
                isPhasingThroughPlatform = true;
                isCrouching = false;
                isFastFalling = false;
            }
        }

    }
    */

    IEnumerator jumpRecharge()
    {
        canJump = false;
        yield return new WaitForSeconds(0.2f);
        canJump = true;
    }

    #endregion

    #region Right Stick - Dash and Roll
    public void OnRightStick(InputAction.CallbackContext context)
    {
        rightJoystickVector = context.ReadValue<Vector2>();

        /*
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
        */

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
                            //face right
                            //facingRight = true;
                            // flipRightVisual();
                            backwardEntryTeleportalVisual();
                        }
                        else if (facingRight && rightJoystickVector.x < -0.1f)
                        {
                            //face left
                            //facingRight = false;
                            //flipLeftVisual();
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
                            facingRight = true;
                            directionModifier = 1;
                            flipRightVisual();
                        }
                        else if (facingRight && rightJoystickVector.x < -0.1f)
                        {
                            //face left
                            facingRight = false;
                            directionModifier = -1;
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
        //start
        turnOffFriction();
        myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
        isRolling = true;
        canMove = false;
        canRoll = false;
        canDash = false;
        bodyCollider.enabled = false;
        smallBodyCollider.enabled = true;
        //standingVisual.enabled = false;
        //ballVisual.enabled = true;
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
        bodyCollider.enabled = true;
        smallBodyCollider.enabled = true;
        //turnOnFriction();
        //standingVisual.enabled = true;
        //ballVisual.enabled = false;
        isRolling = false;
        isDashing = false;
        canMove = true;
        //canDash = true;
        isRunning = false;
        isWalking = false;
        StartCoroutine(rollRecharge());
        //stop
    }

    IEnumerator rollRecharge()
    {
        yield return new WaitForSeconds(1f);
        resetTeleportalVisual();
        canRoll = true;
    }

    IEnumerator dashTime()
    {
        //start
        turnOffFriction();
        isDashing = true;
        grabbingWall = false;
        canMove = false;
        canDash = false;
        canRoll = false;
        isPhasingThroughPlatform = false;
        bodyCollider.enabled = false;
        smallBodyCollider.enabled = true;
        groundFrictionCollider.enabled = true;
        //standingVisual.enabled = false;
        //ballVisual.enabled = true;
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

        
        if (wallCheck(1) && grabbingWall == false && wallToFloorCheck() == false)
        {
            wallGrab(1);
        }
        else if (wallCheck(-1) && grabbingWall == false && wallToFloorCheck() == false)
        {
            wallGrab(-1);
        }
        else if (grabbingWall == false)
        {

            myRigidbody.gravityScale = slowFallGravityPower;
            myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
            bodyCollider.enabled = true;
            smallBodyCollider.enabled = true;
            turnOnFriction();
            //standingVisual.enabled = true;
            //ballVisual.enabled = false;
            isDashing = false;
            isRolling = false;
            canRoll = true;
            canMove = true;
            isPhasingThroughPlatform = false;
            endDashAttackVisual();

            if (isSemiGrounded())
            {
                land();
            }

            /*
            if (grabbingWall == false)
            {
                myRigidbody.gravityScale = gravityPower;
                myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
                bodyCollider.enabled = true;
                smallBodyCollider.enabled = true;
                turnOnFriction();
                //standingVisual.enabled = true;
                //ballVisual.enabled = false;
                isDashing = false;
                isRolling = false;
                canRoll = true;
                canMove = true;
                endDashAttackVisual();
            }
            */
        }
        

        /*
        if (grabbingWall == false)
        {
            myRigidbody.gravityScale = gravityPower;
            myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
            bodyCollider.enabled = true;
            smallBodyCollider.enabled = true;
            //turnOnFriction();
            //standingVisual.enabled = true;
            //ballVisual.enabled = false;
            isDashing = false;
            isRolling = false;
            canRoll = true;
            canMove = true;
            endDashAttackVisual();
        }
        */
    }

    private bool wallCheck(int direction)
    {
        return Physics2D.Raycast(transform.position, direction * transform.right, 1f, solidGroundLayer);
    }

    private bool wallToFloorCheck()
    {
        return Physics2D.Raycast(transform.position, -transform.up, 2f, solidGroundLayer);
    }

    private void wallGrab(int direction)
    {
        wallGrabVisual();
        StopCoroutine(dashTime());
        //endDashAttackVisual();
        canMove = false;
        grabbingWall = true;
        canRoll = false;
        canDash = false;
        isRolling = false;
        isDashing = false;
        myRigidbody.gravityScale = wallGrabbingGravityPower;
        myRigidbody.velocity = new Vector2(0, 0);
        //print("Grabbing Wall");
        //change visual based on direction
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
        bodyCollider.enabled = true;
        //standingVisual.enabled = true;
        //ballVisual.enabled = false;
        isDashing = false;
        isRolling = false;
        canRoll = true;
        canDash = true;
        canMove = true;
    }

    private void slippedOffWall()
    {
        myRigidbody.gravityScale = slowFallGravityPower;
        myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
        bodyCollider.enabled = true;
        smallBodyCollider.enabled = true;
        endWallGrabVisual();
        //standingVisual.enabled = true;
        //ballVisual.enabled = false;
        isDashing = false;
        isRolling = false;
        canRoll = true;
        canDash = true;
        grabbingWall = false;
        canMove = true;
    }


    #endregion

    #region Face Buttons, Triggers, and Buttons
    public void onButtonA(InputAction.CallbackContext context)
    {
        /*
        if (context.canceled)
        {
            myMonster.attackCancel(0);
            buttonA_Pressed = false;
        }

        if (isRolling || isDashing || grabbingWall || canMove == false)
        {
            return; //in the future we'll develop some sort of queuing system that plays next attack after finished doing the above action within an amount of time
        }

        if (context.started)
        {
            myMonster.attackSlotMonsterParts[0].attackAnimationID = inputModifier;
            myMonster.attack(0, inputModifier);
            canMove = false;
            buttonA_Pressed = true;
        }
        */


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

        if (isRolling || isDashing || grabbingWall || canMove == false)
        {
            return; //in the future we'll develop some sort of queuing system that plays next attack after finished doing the above action within an amount of time
        }

        if (context.started)
        {
            myMonster.attackSlotMonsterParts[1].attackAnimationID = inputModifier;
            myMonster.attack(1, inputModifier);
            canMove = false;
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

        if (isRolling || isDashing || grabbingWall || canMove == false)
        {
            return; //in the future we'll develop some sort of queuing system that plays next attack after finished doing the above action within an amount of time
        }

        if (context.started)
        {
            myMonster.attackSlotMonsterParts[2].attackAnimationID = inputModifier;
            myMonster.attack(2, inputModifier);
            canMove = false;
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

        if (isRolling || isDashing || grabbingWall || canMove == false)
        {
            return; //in the future we'll develop some sort of queuing system that plays next attack after finished doing the above action within an amount of time
        }

        if (context.started)
        {
            myMonster.attackSlotMonsterParts[3].attackAnimationID = inputModifier;
            myMonster.attack(3, inputModifier);
            canMove = false;
            buttonY_Pressed = true;
        }
    }

    #endregion

    #region Monster Attack System Visual Communication

    private void flipLeftVisual()
    {
        myMonster.flipLeft();
    }

    private void flipRightVisual()
    {
        myMonster.flipRight();
    }

    /*
    private void forceCalmEffect()
    {
        myMonster.teeterCheck();
    }
    */

    private void startMiscIdleAnimations()
    {
        canMove = true;
        isAttacking = false;
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

    private void fallVisual()
    {
        myMonster.walkToFall();
    }

    private void landVisual()
    {
        myMonster.land();
    }

    private void lateLandVisualCorrections()
    {
        myMonster.lateLand();
    }

    private void lateAttackReleaseVisualCorrections()
    {
        myMonster.forceUngrounded();
    }

    private void grappleTowardsVisual()
    {
        myMonster.grappleToTarget();
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

    private void endDashAttackVisual()
    {
        myMonster.endDashAttack();
    }

    private void wallGrabVisual()
    {
        myMonster.wallGrab();
        myMonster.wallGrabbedCorrections();
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

    private void crouchVisual()
    {
        myMonster.crouch();
    }
    #endregion

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
        bodyCollider.enabled = false;
        smallBodyCollider.enabled = false;
        groundFrictionCollider.enabled = false;
        isPhasingThroughPlatform = true;
    }

    private void antiPhase(bool hasOverride)
    {
        if (hasOverride || insideFloor == false)
        {
            bodyCollider.enabled = true;
            smallBodyCollider.enabled = true;
            //groundFrictionCollider.enabled = true;
            isPhasingThroughPlatform = false;
        }
    }

    IEnumerator slideToStop()
    {
        yield return new WaitForSeconds(0.1f);

        if ((leftJoystickVector.x < 0.2f && leftJoystickVector.x > -0.2f && isPhasingThroughPlatform == false))
        {
            //turnOnFriction();
            //myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
        }
    }

    private void activateSlowFall()
    {
        myRigidbody.gravityScale = slowFallGravityPower;
    }

    #region Attack Based Movement

    public void damageLockPlayercontroller()
    {
        canMove = false;
        isRunning = false;
        isWalking = false;
        antiPhase(false);

        if (grounded)
        {
            myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
        }

        //temp code
        if (grounded)
        {
            turnOffFriction();
            myRigidbody.velocity = new Vector2(1 * -directionModifier, myRigidbody.velocity.y);
        }
        else
        {
            myRigidbody.velocity = new Vector2(20 * -directionModifier, myRigidbody.velocity.y);
        }
    }

    public void damageUnlockPlayerController()
    {
        canMove = true;
        isRunning = false;
        isWalking = false;
        isAttacking = false;
        //myRigidbody.gravityScale = gravityPower;
        myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);

        if (isDashing == false && isRolling == false)
        {
            myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
            turnOnFriction();
            landDetectionReady = true;
        }

        if (requiresLateLand)
        {
            requiresLateLand = false;
            //lateLandVisualCorrections();
        }



        /*
        if (grounded)
        {
            myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
        }
        */
    }

    public void lockPlayerController()
    {
        canMove = false;
        isRunning = false;
        isWalking = false;

        if (grounded)
        {
            myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
        }

        if (isDashing == false && isRolling == false)
        {
            //myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
            isAttacking = true;
            antiPhase(false);
            //heavyActivated();
            midAirWindUp();
        }
        //myRigidbody.gravityScale = slowedGravityPower;

        /*
        if (grounded)
        {
            myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
        }
        else
        {
            myRigidbody.gravityScale = slowedGravityPower;
        }
        */
    }



    public void unlockPlayerController()
    {
        canMove = true;
        isRunning = false;
        isWalking = false;
        isAttacking = false;
        //myRigidbody.gravityScale = gravityPower;
        myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);

        if (isDashing == false && isRolling == false)
        {
            myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
            turnOnFriction();
            landDetectionReady = true;
            heavyDeactivated();
        }

        if (requiresLateLand)
        {
            requiresLateLand = false;
            //lateLandVisualCorrections();
        }

        

        /*
        if (grounded)
        {
            myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
        }
        */
    }

    public void midAirWindUp()
    {
        if (grounded == false && slowFallBlocked == false)
        {
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, 0);
            myRigidbody.gravityScale = 4;
        }
    }

    public void heavyActivated()
    {
        if (grounded == false)
        {
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, 0);
            myRigidbody.gravityScale = 0;
        }
    }

    public void heavyDeactivated()
    {
        /*
        if (grounded == false)
        {
            myRigidbody.gravityScale = slowFallGravityPower;
        }
        else
        {
            myRigidbody.gravityScale = gravityPower;
        }
        */
        myRigidbody.gravityScale = gravityPower;
        slowFallBlocked = true;
    }

    public void playerGrapple(playerController target)
    {
        if (grappling == false)
        {
            grapplePlayerTarget = target;
            grappling = true;
            grapplingPlayer = true;
            grapplingWall = false;
            grappleTowardsVisual();
        }

    }

    public void wallGrapple(Vector3 contactPoint)
    {
        if (grappling == false)
        {
            wallGrapplePoint = contactPoint;
            grappling = true;
            grapplingWall = true;
            grapplingPlayer = false;
            grappleTowardsVisual();
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

    public void smallLeapAttackForward()
    {
        //turnOffFriction();
        //myRigidbody.velocity = new Vector2(1 * directionModifier, myRigidbody.velocity.y);
        /*
        if (isFacingEdge() == false)
        {
            if (grounded)
            {
                turnOffFriction();
                myRigidbody.velocity = new Vector2(1 * directionModifier, myRigidbody.velocity.y);
            }
            else
            {
                myRigidbody.velocity = new Vector2(20 * directionModifier, myRigidbody.velocity.y);
            }
        }
        */
        /*
        if (grounded)
        {
            myRigidbody.velocity = new Vector2(15 * directionModifier, myRigidbody.velocity.y);
        }
        else
        {
            myRigidbody.velocity = new Vector2(5 * directionModifier, myRigidbody.velocity.y);
        }
        */
    }

    public void smallLeapAttackBackward()
    {

        /*
        if (isFacingEdge() || atPlatformEdge == false)
        {
            turnOffFriction();
            myRigidbody.velocity = new Vector2(1 * -directionModifier, myRigidbody.velocity.y);
        }
        */
        /*
        if (grounded)
        {
            myRigidbody.velocity = new Vector2(15 * -directionModifier, myRigidbody.velocity.y);
        }
        else
        {
            myRigidbody.velocity = new Vector2(5 * -directionModifier, myRigidbody.velocity.y);
        }
        */
    }

    public void smallLeapAttackUpward()
    {
        /*
        if (grounded)
        {
            bigJumpVisual();
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, 30);
            grounded = false;
            isRunning = false;
            isWalking = false;
        }
        else
        {
            if (numberOfJumpsLeft > 0)
            {
                numberOfJumpsLeft--;
                bigJumpVisual();
                myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, 30);
                grounded = false;
                isRunning = false;
                isWalking = false;
            }
        }
        */
    }

    public void smallLeapAttackDownward()
    {
        /*
        if (grounded)
        {

        }
        else
        {

        }
        */
    }

    public void leapAttackForward()
    {
        //turnOffFriction();

        if (isFacingEdge() == false)
        {
            if (grounded)
            {
                turnOnFriction();
                myRigidbody.velocity = new Vector2(0, 0);
                myRigidbody.velocity = new Vector2(90 * directionModifier, myRigidbody.velocity.y);
            }
            else
            {
                heavyActivated();
                myRigidbody.velocity = new Vector2(0, 0);
                myRigidbody.velocity = new Vector2(90 * directionModifier, myRigidbody.velocity.y);
                StartCoroutine(leapAttackForwardControl());
            }
        }

        /*
        myRigidbody.velocity = new Vector2(20 * directionModifier, myRigidbody.velocity.y);
        */

        /*
        if (grounded)
        {
            myRigidbody.velocity = new Vector2(60 * directionModifier, myRigidbody.velocity.y);
        }
        else
        {
            myRigidbody.velocity = new Vector2(20 * directionModifier, myRigidbody.velocity.y);
        }
        */
    }

    // Listens for when an attack calls Trigger Attack Release
    public void ApplyMovementModifier(object sender, TriggerAttackReleaseEventArgs eventArgs)
    {
        Vector2 movementModifier = eventArgs.MovementModifier;

        if (!facingRight)
            movementModifier.x *= -1;

        // using the animation clip length so that the movement duration matches the animation
        StartCoroutine(ApplySmoothedMovementModifier(movementModifier, eventArgs.ClipLength));
    }

    // smooths out the movement so that it is not instant and it looks a bit better
    private IEnumerator ApplySmoothedMovementModifier(Vector2 totalOffset, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float delta = Time.fixedDeltaTime;
            float previousT = elapsed / duration;
            elapsed += delta;
            float currentT = Mathf.Clamp01(elapsed / duration);

            
            Vector2 frameOffset = (currentT - previousT) * totalOffset;

            myRigidbody.MovePosition(myRigidbody.position + frameOffset);
            yield return new WaitForFixedUpdate();
        }
    }


        IEnumerator leapAttackForwardControl()
    {
        yield return new WaitForSeconds(0.1f);
        myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
    }

    public void forceStopLeap()
    {
        myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
    }

    public void leapAttackBackward()
    {
        if (isFacingEdge() || atPlatformEdge == false)
        {
            myRigidbody.velocity = new Vector2(45 * -directionModifier, myRigidbody.velocity.y);
        }

        /*
        if (grounded)
        {
            myRigidbody.velocity = new Vector2(60 * -directionModifier, myRigidbody.velocity.y);
        }
        else
        {
            myRigidbody.velocity = new Vector2(20 * -directionModifier, myRigidbody.velocity.y);
        }
        */
    }

    public void leapAttackUpward()
    {

        if (grounded)
        {
            heavyActivated();
            myRigidbody.velocity = new Vector2(0, 60);
            grounded = false;
            //
            lateAttackReleaseVisualCorrections();
        }
        else
        {
            heavyActivated();
            myRigidbody.velocity = new Vector2(0, 60);
            grounded = false;
        }

        StartCoroutine(leapAttackUpwardControl());
        /*
        if (grounded)
        {
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, 60);
            grounded = false;
            isRunning = false;
            isWalking = false;
        }
        else
        {
            if (numberOfJumpsLeft > 0)
            {
                numberOfJumpsLeft--;
                myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, 20);
                grounded = false;
                isRunning = false;
                isWalking = false;
            }
        }
        */
    }

    IEnumerator leapAttackUpwardControl()
    {
        yield return new WaitForSeconds(0.1f);
        myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, 0);
        myRigidbody.gravityScale = 0;
    }

    public void leapAttackDownward()
    {
        /*
        if (grounded)
        {

        }
        else
        {

        }
        */
    }

    #endregion

    #region Health

    //damage as to how it relates to the initial strike and the knockback effect
    public void damaged(int damageRecieved, bool markedForHeavyAttack, int attackDirection, Vector3 contactPoint)
    {
        canMove = false;
        isRunning = false;
        isWalking = false;
        antiPhase(false);

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
                facingRight = true;
                directionModifier = 1;
                flipRightVisual();
            }
            else if (facingRight)
            {
                //face left
                facingRight = false;
                directionModifier = -1;
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

    }

    IEnumerator damageRecoveryTime(float recoveryTime)
    {
        //check to make sure that i didnt just land
        yield return new WaitForSeconds(recoveryTime);

        canMove = true;
        isRunning = false;
        isWalking = false;
        isAttacking = false;
        //myRigidbody.gravityScale = gravityPower;
        myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);

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

    IEnumerator tickDamageTimer(float tickDuration)
    {
        yield return new WaitForSeconds(tickDuration);
        //turn off tick machine
        StopCoroutine(tickDamageMachine(0,0));
    }

    IEnumerator tickDamageMachine(float timeBetweenTick, int tickDamage)
    {
        
        yield return new WaitForSeconds(timeBetweenTick);
        //apply damage
        //restart machine
    }

    #endregion

    private bool isFacingEdge()
    {
        if (atPlatformEdge && ((facingRight && transform.position.x < platformEdgeCooridinates.x) || (facingRight == false && transform.position.x > platformEdgeCooridinates.x)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Solid")
        {
            if (isPhasingThroughPlatform)
            {
                antiPhase(true);
            }
        }

        if (collision.gameObject.tag == "Semi Solid")
        {
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
            if (collision.transform.parent.gameObject.GetComponent<playerController>() != null)
            {
                playerController fellowPlayer = collision.transform.parent.gameObject.GetComponent<playerController>();

                if (grappling && grapplePlayerTarget != null)
                {
                    if (grapplePlayerTarget == fellowPlayer)
                    {
                        grappling = false;
                        myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
                        grapplePlayerTarget = null;
                        //belly bounce
                    }
                    else
                    {
                        //tackle out of the way
                    }
                }
            }
        }
        
    }
}

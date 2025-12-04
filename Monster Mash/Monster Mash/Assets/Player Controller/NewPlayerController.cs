using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class NewPlayerController : MonoBehaviour
{

    // Components
    public monsterAttackSystem myMonster;
    private playerAudioManager myAudioSystem;
    public CapsuleCollider2D bodyCollider;
    public CircleCollider2D smallBodyCollider;
    public BoxCollider2D groundFrictionCollider;
    public Transform groundCheck;
    public Transform headCheck;
    public PlayerInput playerInput;
    public PlayerControls playerControlsMap;
    [SerializeField] public Rigidbody2D myRigidbody;

    // Input
    public PlayerInputHandler inputHandler = new PlayerInputHandler();
    private InputActionMap startingActionMap;
    private InputActionMap UIcontrols;
    private InputActionMap monsterControls;
    public int playerIndex;
    public Vector2 lastInputDirectionVector;
    public float directionThreshold = 0.2f;
    public enum InputDirection { Forward = 1, Backward = -1, Up = 2, Down = 0 }
    public InputDirection lastInputDirection = InputDirection.Forward;
    public Vector2 rightJoystickVector;
    Vector2 leftJoystickVector;
    public float leftJoystickValue;

    // State Machine
    private PlayerState currentState;

    // Movement
    public float walkSpeed = 5f;
    public float runSpeed = 25f;
    public bool canMove = true;
    public bool isWalking = false;
    public bool isRunning = false;
    public bool grounded = false;
    public bool isCrouching = false;
    public bool isPhasingThroughPlatform;
    public bool isFastFalling = false;
    public bool slowFallBlocked = false;
    public float slowFallGravityPower = 6;
    private float gravityPower;
    public bool monsterControllerActive = false;
    public bool facingRight;
    public bool grabbingWall = false;
    public bool chargingForward = false;
    public bool landDetectionReady = true;
    public Collider2D currentPlatformCollider;
    private bool isDamageLaunching;
    private bool atPlatformEdge = false;
    private Vector2 platformEdgeCooridinates;
    private bool insideFloor = false;

    // Jumping
    public bool canJump = true;
    public bool jumpButtonReset = false;
    public int numberOfJumps = 2;
    public int numberOfJumpsLeft = 2;
    public float LeftStickJumpDelayTime = 0.1f;
    public float leftStickElapsedJumpTime;
    private float wallJumpPower = 28f;
    private float bigJumpPower = 65;

    // Attacks
    public bool isAttacking = false;
    public bool leftStickIsAttacking = false;
    public bool buttonA_Pressed = false;
    public bool buttonB_Pressed = false;
    public bool buttonX_Pressed = false;
    public bool buttonY_Pressed = false;
    float lastAttackTime = -Mathf.Infinity;

    // Platform
    public LayerMask solidGroundLayer;
    public LayerMask semiSolidGroundLayer;

    // Abilities
    public float rollSpeed = 50f;
    public bool isRolling = false;
    public bool canRoll = true;
    public bool isDashing = false;
    public bool canDash = true;
    public bool grappling = false;
    public bool grapplingPlayer = false;
    public bool grapplingWall = false;
    public NewPlayerController grapplePlayerTarget;
    public Vector3 wallGrapplePoint;

    // Animation
    public List<NewMonsterPart> allParts;
    public List<NewMonsterPart> legs;

    // Damage Launching
    [Header("Damage Launching")]
    [SerializeField] private AnimationCurve damageToForceCurve;
    [SerializeField, Tooltip("Controls how much of an arc the launch has for left and right")] private float yMultiplier = 1.5f;

    private List<NewMonsterPart> GetAllPartsInRoot()
    {
        var allParts = new List<NewMonsterPart>(transform.root.GetComponentsInChildren<NewMonsterPart>(true));
        return allParts;
    }

    private void Awake()
    {

        inputHandler.OnButtonB_Canceled += () =>
        {
            myMonster.attackCancel(1);
        };

        inputHandler.OnButtonX_Canceled += () =>
        {
            myMonster.attackCancel(2);
        };

        inputHandler.OnButtonY_Canceled += () =>
        {
            myMonster.attackCancel(3);
        };

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
        ChangeState(new IdleState(this));
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
        playerInput.actions.FindActionMap("Monster Controls").FindAction("Left Stick").performed += inputHandler.OnLeftStick;
        playerInput.actions.FindActionMap("Monster Controls").FindAction("Left Stick").canceled += inputHandler.OnLeftStick;

        playerInput.actions.FindActionMap("Monster Controls").FindAction("Right Stick").performed += inputHandler.OnRightStick;
        playerInput.actions.FindActionMap("Monster Controls").FindAction("Right Stick").canceled += inputHandler.OnRightStick;

        playerInput.actions.FindActionMap("Monster Controls").FindAction("A").started += inputHandler.OnButtonA;
        playerInput.actions.FindActionMap("Monster Controls").FindAction("A").canceled += inputHandler.OnButtonA;

        playerInput.actions.FindActionMap("Monster Controls").FindAction("B").started += inputHandler.OnButtonB;
        playerInput.actions.FindActionMap("Monster Controls").FindAction("B").canceled += inputHandler.OnButtonB;

        playerInput.actions.FindActionMap("Monster Controls").FindAction("X").started += inputHandler.OnButtonX;
        playerInput.actions.FindActionMap("Monster Controls").FindAction("X").canceled += inputHandler.OnButtonX;

        playerInput.actions.FindActionMap("Monster Controls").FindAction("Y").started += inputHandler.OnButtonY;
        playerInput.actions.FindActionMap("Monster Controls").FindAction("Y").canceled += inputHandler.OnButtonY;

        playerInput.actions.FindAction("ShowMenu").Enable();
        playerInput.actions.FindAction("ShowMenu").performed += ShowRemappingMenu;
    }

    private void UnsubscribeActionMap()
    {
        if (playerInput == null) {  return; }

        playerInput.actions.FindActionMap("Monster Controls").FindAction("Left Stick").performed -= inputHandler.OnLeftStick;
        playerInput.actions.FindActionMap("Monster Controls").FindAction("Left Stick").canceled -= inputHandler.OnLeftStick;

        playerInput.actions.FindActionMap("Monster Controls").FindAction("Right Stick").performed -= inputHandler.OnRightStick;
        playerInput.actions.FindActionMap("Monster Controls").FindAction("Right Stick").canceled -= inputHandler.OnRightStick;

        playerInput.actions.FindActionMap("Monster Controls").FindAction("A").started -= inputHandler.OnButtonA;
        playerInput.actions.FindActionMap("Monster Controls").FindAction("A").canceled -= inputHandler.OnButtonA;

        playerInput.actions.FindActionMap("Monster Controls").FindAction("B").started -= inputHandler.OnButtonB;
        playerInput.actions.FindActionMap("Monster Controls").FindAction("B").canceled -= inputHandler.OnButtonB;

        playerInput.actions.FindActionMap("Monster Controls").FindAction("X").started -= inputHandler.OnButtonX;
        playerInput.actions.FindActionMap("Monster Controls").FindAction("X").canceled -= inputHandler.OnButtonX;

        playerInput.actions.FindActionMap("Monster Controls").FindAction("Y").started -= inputHandler.OnButtonY;
        playerInput.actions.FindActionMap("Monster Controls").FindAction("Y").canceled -= inputHandler.OnButtonY;

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

    public void ChangeState(PlayerState newState)
    {
        if (currentState != null) currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    private void Update()
    {
        if (currentState != null)
        {
            currentState.HandleInput();
            currentState.Update();
        }

        if (inputHandler.ButtonA_Pressed)
        {
            if (grabbingWall)
            {
                if (facingRight)
                    wallJump(-1);
                else
                    wallJump(1);

                bigJumpVisual();
            }
            else
            {
                if (canJump && numberOfJumpsLeft > 0 && jumpButtonReset)
                    bigJump();
            }
            inputHandler.ButtonA_Pressed = false;
        }


        if (inputHandler.ButtonB_Pressed)
        {
            if (myMonster.attackSlotMonsterParts[1] != null)
            {
                myMonster.attackSlotMonsterParts[1].attackAnimationID = (int)lastInputDirection;
                myMonster.attack(1, (int)lastInputDirection);
            }
            inputHandler.ButtonB_Pressed = false;
        }

        if (inputHandler.ButtonX_Pressed)
        {
            if (myMonster.attackSlotMonsterParts[2] != null)
            {
                myMonster.attackSlotMonsterParts[2].attackAnimationID = (int)lastInputDirection;
                myMonster.attack(2, (int)lastInputDirection);
            }
            inputHandler.ButtonB_Pressed = false;
        }

        if (inputHandler.ButtonY_Pressed)
        {
            if (myMonster.attackSlotMonsterParts[3] != null)
            {
                myMonster.attackSlotMonsterParts[3].attackAnimationID = (int)lastInputDirection;
                myMonster.attack(3, (int)lastInputDirection);
            }
            inputHandler.ButtonB_Pressed = false;
        }
    }
    public void UpdateInputDirection(Vector2 directionVector)
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

    public bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.5f, solidGroundLayer);
    }

    public bool isSemiGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.5f, semiSolidGroundLayer);
    }

    public bool isBelowSemiGround()
    {
        return Physics2D.OverlapCircle(headCheck.position, 1f, semiSolidGroundLayer);
    }

    public Collider2D GetClosestPlatform()
    {
        return Physics2D.OverlapCircle(headCheck.position, 1f, semiSolidGroundLayer);
    }

    public void land()
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
    public void bigJump()
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

                        // air dash

                        //StartCoroutine(dashTime());
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

    public void flipLeftVisual()
    {
        myMonster.flipLeft();
        facingRight = false;
    }

    public void flipRightVisual()
    {
        myMonster.flipRight();
        facingRight = true;
    }

    public void startMiscIdleAnimations()
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

    public void startRunningVisual()
    {
        myMonster.run();
    }

    public void stopRunningVisual()
    {
        myMonster.stopRunning();
    }

    public void startWalkingVisual()
    {
        myMonster.walk();
    }

    public void stopWalkingVisual()
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

    public void phaseThroughPlatformVisual()
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

    public void startCrouchVisual()
    {
        myMonster.crouch();
    }

    public void endCrouchVisual()
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

    public void phase()
    {
        if (currentPlatformCollider != null)
        {
            Physics2D.IgnoreCollision(bodyCollider, currentPlatformCollider, true);
            Physics2D.IgnoreCollision(smallBodyCollider, currentPlatformCollider, true);
            Physics2D.IgnoreCollision(groundFrictionCollider, currentPlatformCollider, true);
            isPhasingThroughPlatform = true;
        }    
    }

    public void antiPhase()
    {
        if (insideFloor == false)
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

    public void activateSlowFall()
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
                antiPhase();
                
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

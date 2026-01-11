using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementState : PlayerState
{
    public MovementState(NewPlayerController controller) : base(controller) { }

    private float walkSpeed = 5f;
    private float runSpeed = 25f;

    private bool isCrouching = false;
    private bool isFastFalling = false;

    private float slowFallGravityPower = 6;

    private float LeftStickJumpDelayTime = 0.1f;
    private float leftStickElapsedJumpTime;

    private float wallJumpPower = 28f;
    private float bigJumpPower = 65;
    private int numberOfJumps = 2;
    private int numberOfJumpsLeft = 2;

    private float rollSpeed = 50f;
    private bool canRoll = true;
    private bool canDash = true;

    public bool jumpButtonReset = false;
    private bool slowFallBlocked = false;

    public override void Enter()
    {

    }

    public override void HandleInput()
    {

    }

    public override void Update()
    {

        #region Attack Logic

        PlayerInputHandler.InputDirection lastInputDirection = controller.inputHandler.LastInputDirection; 
        PlayerInputHandler inputHandler = controller.inputHandler;

        
        if (controller.inputHandler.ButtonA_Pressed)
        {
            if (controller.canJump && numberOfJumpsLeft > 0 && jumpButtonReset)
                bigJump();

            controller.inputHandler.ButtonA_Pressed = false;
        }

        if (controller.inputHandler.ButtonB_Pressed)
        {
            if (controller.myMonster.attackSlotMonsterParts[1] != null)
            {
                controller.myMonster.attackSlotMonsterParts[1].attackAnimationID = 
                    controller.inputHandler.ConvertInputDirectionToAnimationID(lastInputDirection);
                controller.myMonster.attack(1, inputHandler.ConvertInputDirectionToAnimationID(lastInputDirection));
            }
            controller.inputHandler.ButtonB_Pressed = false;
        }

        if (controller.inputHandler.ButtonX_Pressed)
        {
            if (controller.myMonster.attackSlotMonsterParts[2] != null)
            {
                controller.myMonster.attackSlotMonsterParts[2].attackAnimationID = 
                    controller.inputHandler.ConvertInputDirectionToAnimationID(lastInputDirection);
                controller.myMonster.attack(2, inputHandler.ConvertInputDirectionToAnimationID(lastInputDirection));
            }
            controller.inputHandler.ButtonB_Pressed = false;
        }

        if (controller.inputHandler.ButtonY_Pressed)
        {
            if (controller.myMonster.attackSlotMonsterParts[3] != null)
            {
                controller.myMonster.attackSlotMonsterParts[3].attackAnimationID = 
                    controller.inputHandler.ConvertInputDirectionToAnimationID(lastInputDirection);
                controller.myMonster.attack(3, inputHandler.ConvertInputDirectionToAnimationID(lastInputDirection));
            }
            controller.inputHandler.ButtonB_Pressed = false;
        }
        #endregion

        // --- Movement logic using inputHandler.LeftStick ---
        Vector2 moveInput = controller.inputHandler.LeftStick;
        float moveValue = moveInput.magnitude;

        if (Mathf.Abs(moveInput.x) > controller.inputHandler.directionThreshold || Mathf.Abs(moveInput.y) > controller.inputHandler.directionThreshold)
        {
            controller.inputHandler.lastInputDirectionVector = moveInput.normalized;
            controller.inputHandler.UpdateInputDirection(controller.inputHandler.lastInputDirectionVector);

            if (!controller.isDashing && !controller.isRolling && controller.canMove)
            {
                if (controller.inputHandler.LastInputDirection == PlayerInputHandler.InputDirection.Forward)
                    controller.flipRightVisual();
                else if (controller.inputHandler.LastInputDirection == PlayerInputHandler.InputDirection.Backward)
                    controller.flipLeftVisual();
            }
        }

        if (controller.inputHandler.ButtonA_Pressed || controller.inputHandler.ButtonB_Pressed || controller.inputHandler.ButtonX_Pressed || controller.inputHandler.ButtonY_Pressed || !controller.canMove)
            return;

        if (!controller.isPhasingThroughPlatform && controller.groundFrictionCollider.enabled && !isCrouching && controller.canMove && !controller.isAttacking)
            controller.turnOffFriction();

        if (controller.canMove && !controller.isAttacking)
        {
            if (moveInput.x > 0.9f || moveInput.x < -0.9f)
            {
                // Run
                if (!controller.isRunning)
                {
                    controller.isRunning = true;
                    controller.isWalking = false;
                    startRunningVisual();
                    controller.turnOffFriction();
                }
            }
            else if (moveInput.x > 0.2f || moveInput.x < -0.2f)
            {
                // Walk
                if (!controller.isWalking)
                {
                    controller.isWalking = true;
                    controller.isRunning = false;
                    startWalkingVisual();
                    stopRunningVisual();
                    controller.turnOffFriction();
                }
            }
        }

        if (controller.monsterControllerActive)
        {
            if (!controller.isPhasingThroughPlatform && !controller.isGrounded())
            {
                Collider2D platform = GetClosestPlatform();
                if (platform != null)
                {
                    controller.currentPlatformCollider = platform;
                }
            }

            if (controller.isPhasingThroughPlatform)
            {
                controller.phase();
            }
            else
            {
                controller.antiPhase();
            }

            if (controller.isPhasingThroughPlatform)
            {
                // Only check for re-enabling if we are currently phasing
                bool touching = false;
                if (controller.bodyCollider.IsTouchingLayers(controller.semiSolidGroundLayer)) touching = true;
                if (controller.smallBodyCollider.IsTouchingLayers(controller.semiSolidGroundLayer)) touching = true;
                if (controller.groundFrictionCollider.IsTouchingLayers(controller.semiSolidGroundLayer)) touching = true;

                if (!touching)
                {
                    controller.currentPlatformCollider = null;
                    controller.isPhasingThroughPlatform = false;
                }
            }

            if (controller.isGrounded() && (controller.myRigidbody.velocity.y < 0f || controller.myRigidbody.velocity.y == 0f))
            {
                if (controller.grounded == false)
                {
                    land();
                }
            }
            else if (controller.isSemiGrounded())
            {
                if (controller.grounded == false && (controller.myRigidbody.velocity.y < 0f || controller.myRigidbody.velocity.y == 0f) && controller.isPhasingThroughPlatform == false && controller.landDetectionReady)
                {
                    land();
                }
            }
            else if ((controller.myRigidbody.velocity.y < 0f || controller.myRigidbody.velocity.y == 0f) && controller.myRigidbody.gravityScale != slowFallGravityPower && controller.canMove && slowFallBlocked == false)
            {
                //falling
                activateSlowFall();
            }

            if (controller.isWalking == false && controller.myMonster.isWalking)
            {
                stopWalkingVisual();
            }

            if (controller.isRunning == false && controller.myMonster.isRunning)
            {
                stopRunningVisual();
            }
            else if (controller.isRunning && controller.myMonster.isRunning == false)
            {
                startRunningVisual();
            }

            if (controller.canMove)
            {
                if (controller.isGrounded() || controller.isSemiGrounded())
                {
                    if (controller.isWalking == false && controller.isRunning == false && controller.isPhasingThroughPlatform == false && controller.groundFrictionCollider.enabled == false && controller.grounded)
                    {
                        controller.turnOnFriction();
                    }

                    if ((controller.inputHandler.LeftStick.x > 0.9f || controller.inputHandler.LeftStick.x < -0.9f))
                    {
                        //run
                        if (controller.inputHandler.LeftStick.x > 0.9f)
                        {
                            //right
                            if (controller.isRunning == false)
                            {
                                controller.isRunning = true;
                                controller.isWalking = false;
                                stopWalkingVisual();
                                startRunningVisual();
                            }

                            controller.myRigidbody.velocity = new Vector2(1 * runSpeed, controller.myRigidbody.velocity.y);
                        }
                        else
                        {
                            //left
                            if (controller.isRunning == false)
                            {
                                controller.isRunning = true;
                                controller.isWalking = false;
                                stopWalkingVisual();
                                startRunningVisual();
                            }

                            controller.myRigidbody.velocity = new Vector2(-1 * runSpeed, controller.myRigidbody.velocity.y);
                        }
                    }
                    else if ((controller.inputHandler.LeftStick.x > 0.2f || controller.inputHandler.LeftStick.x < -0.2f))
                    {
                        if (controller.inputHandler.LeftStick.x > 0.2f)
                        {
                            controller.myRigidbody.velocity = new Vector2(1 * walkSpeed, controller.myRigidbody.velocity.y);
                        }
                        else
                        {
                            controller.myRigidbody.velocity = new Vector2(-1 * walkSpeed, controller.myRigidbody.velocity.y);
                        }
                    }
                    else if ((controller.inputHandler.LeftStick.x < 0.1f && controller.inputHandler.LeftStick.x > -0.1f))
                    {
                        controller.myRigidbody.velocity = new Vector2(0, controller.myRigidbody.velocity.y);

                        if (controller.isWalking && controller.isPhasingThroughPlatform == false && controller.grounded)
                        {
                            controller.isWalking = false;
                            controller.isRunning = false;
                            stopWalkingVisual();
                            startMiscIdleAnimations();
                            controller.turnOnFriction();
                        }

                        if (controller.isRunning && controller.isPhasingThroughPlatform == false && controller.grounded)
                        {
                            controller.isRunning = false;
                            controller.isWalking = false;
                            stopRunningVisual();
                            startMiscIdleAnimations();
                        }
                    }
                }
                else
                {
                    if (controller.inputHandler.LeftStick.x > 0.9f || controller.inputHandler.LeftStick.x < -0.9f)
                    {
                        if (controller.inputHandler.LeftStick.x > 0.9f)
                        {
                            //right
                            controller.myRigidbody.velocity = new Vector2(1 * runSpeed / 1.8f, controller.myRigidbody.velocity.y);
                        }
                        else
                        {
                            //left
                            controller.myRigidbody.velocity = new Vector2(-1 * runSpeed / 1.8f, controller.myRigidbody.velocity.y);
                        }
                    }
                    else if (controller.inputHandler.LeftStick.x > 0.2f || controller.inputHandler.LeftStick.x < -0.2f)
                    {
                        if (controller.inputHandler.LeftStick.x > 0.2f)
                        {
                            //right
                            controller.myRigidbody.velocity = new Vector2(1 * walkSpeed / 2, controller.myRigidbody.velocity.y);
                        }
                        else
                        {
                            //left
                            controller.myRigidbody.velocity = new Vector2(-1 * walkSpeed / 2, controller.myRigidbody.velocity.y);
                        }
                    }
                    else if ((controller.inputHandler.LeftStick.x < 0.1f && controller.inputHandler.LeftStick.x > -0.1f))
                    {
                        controller.myRigidbody.velocity = new Vector2(controller.myRigidbody.velocity.x - 0.001f, controller.myRigidbody.velocity.y);

                        if (controller.isWalking && controller.isPhasingThroughPlatform == false)
                        {
                            controller.isWalking = false;
                            controller.isRunning = false;
                            stopWalkingVisual();
                            stopRunningVisual();
                            startMiscIdleAnimations();
                        }

                        if (controller.isRunning && controller.isPhasingThroughPlatform == false)
                        {
                            controller.isRunning = false;
                            controller.isWalking = false;
                            stopWalkingVisual();
                            stopRunningVisual();
                            startMiscIdleAnimations();
                        }
                    }
                }

                if (isBelowSemiGround() && controller.isGrounded() == false)
                {
                    if (controller.isPhasingThroughPlatform == false)
                    {
                        controller.phase();
                        controller.isPhasingThroughPlatform = true;
                        isCrouching = false;
                        isFastFalling = false;
                    }
                }

                if (controller.inputHandler.ButtonA_Pressed == false && controller.inputHandler.ButtonB_Pressed == false && controller.inputHandler.ButtonX_Pressed == false && controller.inputHandler.ButtonY_Pressed == false && controller.isAttacking == false)
                {
                    if (controller.inputHandler.LeftStick.y > 0.4f && Mathf.Abs(controller.inputHandler.LeftStick.x) < 0.4f)
                    {
                        // Cancels the jump if you attack during the delay window. I don't like using a seperate bool for this but isAttacking does not work reliably 
                        if (controller.leftStickIsAttacking)
                        {
                            return;
                        }

                        leftStickElapsedJumpTime += Time.deltaTime;
                        if (controller.canJump && numberOfJumpsLeft > 0 && jumpButtonReset)
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
                        if (controller.inputHandler.leftJoystickValue < 0.1f)
                        {
                            leftStickElapsedJumpTime = 0;
                            controller.leftStickIsAttacking = false;
                        }
                    }

                    if (controller.inputHandler.LeftStick.y < 0.05f && jumpButtonReset == false)
                    {
                        jumpButtonReset = true;
                    }

                    if (controller.inputHandler.LeftStick.y < -0.6f && (controller.inputHandler.LeftStick.x < 0.1f && controller.inputHandler.LeftStick.x > -0.1f))
                    {
                        //down stick -> either crouch or go through semi solid or fast fall
                        if (controller.isGrounded())
                        {
                            //crouch
                            if (isCrouching == false)
                            {
                                startCrouchVisual();
                                isCrouching = true;
                                controller.isPhasingThroughPlatform = false;
                                isFastFalling = false;

                                if ((controller.isRunning || controller.isWalking))
                                {
                                    if (controller.isWalking)
                                    {
                                        stopWalkingVisual();
                                    }

                                    if (controller.isRunning)
                                    {
                                        stopRunningVisual();
                                    }

                                    controller.isRunning = false;
                                    controller.isWalking = false;
                                    startMiscIdleAnimations();
                                }
                                else
                                {
                                    controller.turnOnFriction();
                                }
                            }
                        }
                        else if (controller.isSemiGrounded())
                        {
                            //fall through platform
                            if (controller.isPhasingThroughPlatform == false)
                            {
                                controller.phase();
                                phaseThroughPlatformVisual();
                                controller.grounded = false;
                                isCrouching = false;
                                isFastFalling = false;
                                controller.landDetectionReady = false;
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

            if (controller.isAttacking == false)
            {
                if (controller.isRolling)
                {
                    if (controller.inputHandler.rightJoystickVector.x > 0.2f)
                    {
                        controller.myRigidbody.velocity = new Vector2(1 * rollSpeed, controller.myRigidbody.velocity.y);
                    }
                    else if (controller.inputHandler.rightJoystickVector.x < -0.2f)
                    {
                        controller.myRigidbody.velocity = new Vector2(-1 * rollSpeed, controller.myRigidbody.velocity.y);
                    }
                }
            }

            if (controller.chargingForward)
            {
                if (controller.facingRight)
                {
                    controller.myRigidbody.velocity = new Vector2(1 * (runSpeed * 2), controller.myRigidbody.velocity.y);
                }
                else
                {
                    controller.myRigidbody.velocity = new Vector2(-1 * (runSpeed * 2), controller.myRigidbody.velocity.y);
                }
            }
        }
    }

    public override void Exit()
    {

    }

    public bool isBelowSemiGround()
    {
        return Physics2D.OverlapCircle(controller.headCheck.position, 1f, controller.semiSolidGroundLayer);
    }

    public Collider2D GetClosestPlatform()
    {
        return Physics2D.OverlapCircle(controller.headCheck.position, 1f, controller.semiSolidGroundLayer);
    }

    public void land()
    {
        controller.isDamageLaunching = false;
        controller.grounded = true;
        numberOfJumpsLeft = numberOfJumps;
        controller.StopCoroutine(jumpRecharge());
        isFastFalling = false;
        controller.landDetectionReady = true;
        controller.insideFloor = false;

        if (controller.isAttacking == false)
        {
            landVisual();
            controller.canMove = true;
            controller.myRigidbody.gravityScale = controller.gravityPower;
        }
        else
        {
            controller.myRigidbody.velocity = new Vector2(0, controller.myRigidbody.velocity.y);
            controller.turnOnFriction();
            controller.myRigidbody.gravityScale = controller.gravityPower;
            return;
        }

        if ((controller.inputHandler.LeftStick.x < 0.1f && controller.inputHandler.leftJoystickVector.x > -0.1f))
        {
            controller.myRigidbody.velocity = new Vector2(0, controller.myRigidbody.velocity.y);

            if (controller.isWalking && controller.isPhasingThroughPlatform == false && controller.grounded)
            {
                controller.isWalking = false;
                controller.isRunning = false;
                stopWalkingVisual();
                startMiscIdleAnimations();
                controller.turnOnFriction();
            }

            if (controller.isRunning && controller.isPhasingThroughPlatform == false && controller.grounded)
            {
                controller.isRunning = false;
                controller.isWalking = false;
                stopRunningVisual();
                startMiscIdleAnimations();
            }
        }

        if ((controller.isRunning || controller.isWalking) && controller.isAttacking == false && controller.canMove)
        {
            controller.turnOffFriction();
        }
        else if (controller.inputHandler.LeftStick.x < 0.1f && controller.inputHandler.leftJoystickVector.x > -0.1f)
        {
            controller.turnOnFriction();
        }
    }

    private void wallJump(int direction)
    {
        controller.myRigidbody.gravityScale = controller.gravityPower;
        jumpButtonReset = false;
        numberOfJumpsLeft = numberOfJumps - 1;
        controller.myRigidbody.velocity = new Vector2(direction * wallJumpPower, bigJumpPower);
        controller.StartCoroutine(jumpRecharge());
        controller.isDashing = false;
        controller.isRolling = false;
        canRoll = true;
        canDash = true;
        controller.canMove = true;
    }





    public void startMiscIdleAnimations()
    {
        controller.canMove = true;
        controller.myMonster.focusedAttackActive = false;

        if (controller.atPlatformEdge)
        {
            controller.myMonster.teeterCheck();
        }
        else
        {
            controller.myMonster.activeBounce();
        }
    }

    public void phaseThroughPlatformVisual()
    {
        controller.myMonster.goThroughPlatform();
    }

    private void landVisual()
    {
        controller.myMonster.land();
    }

    private void lateAttackReleaseVisualCorrections()
    {
        controller.myMonster.forceUngrounded();
    }

    public void startCrouchVisual()
    {
        controller.myMonster.crouch();
    }

    public void endCrouchVisual()
    {
        controller.myMonster.stopCrouching();
    }

    public void activateSlowFall()
    {
        controller.myRigidbody.gravityScale = slowFallGravityPower;
    }

    private void bigJumpVisual()
    {
        controller.myMonster.jump();
        playJumpSound();
    }

    public void bigJump()
    {

        if (numberOfJumpsLeft > 0)
        {
            if (controller.isWalking)
            {
                stopWalkingVisual();
            }

            if (controller.isRunning)
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
            controller.grounded = false;
            jumpButtonReset = false;
            slowFallBlocked = false;
            controller.myRigidbody.gravityScale = controller.gravityPower;
            controller.myRigidbody.velocity = new Vector2(controller.myRigidbody.velocity.x, bigJumpPower);
            controller.StartCoroutine(jumpRecharge());
        }
    }

    IEnumerator rollTime()
    {
        controller.turnOffFriction();
        controller.myRigidbody.velocity = new Vector2(0, controller.myRigidbody.velocity.y);
        controller.isRolling = true;
        controller.canMove = false;
        canRoll = false;
        canDash = false;
        if (controller.isWalking)
        {
            stopWalkingVisual();
        }

        if (controller.isRunning)
        {
            stopRunningVisual();
        }

        controller.isWalking = false;
        controller.isRunning = false;
        rollVisual();
        yield return new WaitForSeconds(0.2f);
        reEntryTeleportalVisual();
        controller.myRigidbody.velocity = new Vector2(0, controller.myRigidbody.velocity.y);
        controller.isRolling = false;
        controller.isDashing = false;
        controller.canMove = true;
        controller.isRunning = false;
        controller.isWalking = false;
        controller.StartCoroutine(rollRecharge());
    }

    IEnumerator rollRecharge()
    {
        yield return new WaitForSeconds(1f);
        resetTeleportalVisual();
        canRoll = true;
    }

    IEnumerator dashTime()
    {
        controller.turnOffFriction();
        controller.isDashing = true;
        controller.canMove = false;
        canDash = false;
        canRoll = false;
        controller.myRigidbody.gravityScale = 0;
        if (controller.isWalking)
        {
            stopWalkingVisual();
        }

        if (controller.isRunning)
        {
            stopRunningVisual();
        }

        controller.isRunning = false;
        controller.isWalking = false;
        startDashAttackVisual();
        yield return new WaitForSeconds(0.25f);
        controller.isRunning = false;
        controller.isWalking = false;
    }

    private void startDashAttackVisual()
    {
        controller.myMonster.dashAttack();
    }

    private void rollVisual()
    {
        controller.myMonster.roll();
    }

    private void forwardEntryTeleportalVisual()
    {
        controller.myMonster.entryTeleportalVFX(true);
    }

    private void backwardEntryTeleportalVisual()
    {
        controller.myMonster.entryTeleportalVFX(false);
    }

    private void reEntryTeleportalVisual()
    {
        controller.myMonster.reEntryTeleportalVFX();
    }

    private void resetTeleportalVisual()
    {
        controller.myMonster.resetTeleportalsVFX();
    }


    public void playJumpSound()
    {
        controller.myAudioSystem.playJumpSound();
    }

    public void playDoubleJumpSound()
    {
        controller.myAudioSystem.playDoubleJumpSound();
    }

    public IEnumerator jumpRecharge()
    {
        controller.canJump = false;
        yield return new WaitForSeconds(0.2f);
        controller.canJump = true;
    }

    private void doubleJumpVisual()
    {
        controller.myMonster.doubleJump();
        playDoubleJumpSound();
    }

    public void startRunningVisual()
    {
        controller.myMonster.run();
    }

    public void stopRunningVisual()
    {
        controller.myMonster.stopRunning();
    }

    public void startWalkingVisual()
    {
        controller.myMonster.walk();
    }

    public void stopWalkingVisual()
    {
        controller.myMonster.stopWalking();
    }


    public void OnRightStick(InputAction.CallbackContext context)
    {
        controller.inputHandler.OnRightStick(context);
        controller.inputHandler.rightJoystickVector = context.ReadValue<Vector2>();


        if (context.performed && controller.isDashing == false && controller.isRolling == false)
        {
            if (controller.facingRight == false && controller.inputHandler.rightJoystickVector.x > 0.1f)
            {
                //face right
                controller.facingRight = true;
                controller.flipRightVisual();
            }
            else if (controller.facingRight && controller.inputHandler.rightJoystickVector.x < -0.1f)
            {
                //face left
                controller.facingRight = false;
                controller.flipLeftVisual();
            }
        }

        if (context.performed && (canDash || canRoll))
        {

            if (controller.isGrounded() || controller.isSemiGrounded())
            {

                if (canRoll && controller.isRolling == false)
                {

                    if (controller.inputHandler.rightJoystickVector.x > 0.1f || controller.inputHandler.rightJoystickVector.x < -0.1f)
                    {

                        if (controller.facingRight == false && controller.inputHandler.rightJoystickVector.x > 0.1f)
                        {
                            backwardEntryTeleportalVisual();
                        }
                        else if (controller.facingRight && controller.inputHandler.rightJoystickVector.x < -0.1f)
                        {
                            backwardEntryTeleportalVisual();
                        }
                        else
                        {
                            forwardEntryTeleportalVisual();
                        }
                        controller.StartCoroutine(rollTime());
                    }
                }
            }
            else
            {
                if (canDash && controller.isDashing == false && controller.isRolling == false && controller.wallToFloorCheck() == false)
                {
                    if (controller.inputHandler.rightJoystickVector.x > 0.1f || controller.inputHandler.rightJoystickVector.x < -0.1f)
                    {

                        if (controller.facingRight == false && controller.inputHandler.rightJoystickVector.x > 0.1f)
                        {
                            //face right
                            controller.flipRightVisual();
                        }
                        else if (controller.facingRight && controller.inputHandler.rightJoystickVector.x < -0.1f)
                        {
                            //face left
                            controller.flipLeftVisual();
                        }

                        // air dash

                        controller.StartCoroutine(dashTime());
                    }
                }
            }

        }
    }

}

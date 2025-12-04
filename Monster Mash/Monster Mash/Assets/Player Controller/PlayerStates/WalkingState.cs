using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WalkingState : PlayerState
{
    public WalkingState(NewPlayerController controller) : base(controller) { }

    public override void Enter()
    {

    }

    public override void HandleInput()
    {

    }

    public override void Update()
    {
        // --- Movement logic using inputHandler.LeftStick ---
        Vector2 moveInput = controller.inputHandler.LeftStick;
        float moveValue = moveInput.magnitude;

        if (Mathf.Abs(moveInput.x) > controller.directionThreshold || Mathf.Abs(moveInput.y) > controller.directionThreshold)
        {
            controller.lastInputDirectionVector = moveInput.normalized;
            controller.UpdateInputDirection(controller.lastInputDirectionVector);

            if (!controller.grabbingWall && !controller.isDashing && !controller.isRolling && controller.canMove)
            {
                if (controller.lastInputDirection == NewPlayerController.InputDirection.Forward)
                    controller.flipRightVisual();
                else if (controller.lastInputDirection == NewPlayerController.InputDirection.Backward)
                    controller.flipLeftVisual();
            }
        }

        if (controller.buttonA_Pressed || controller.buttonB_Pressed || controller.buttonX_Pressed || controller.buttonY_Pressed || !controller.canMove)
            return;

        if (!controller.isPhasingThroughPlatform && controller.groundFrictionCollider.enabled && !controller.isCrouching && controller.canMove && !controller.isAttacking)
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
                    controller.startRunningVisual();
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
                    controller.startWalkingVisual();
                    controller.stopRunningVisual();
                    controller.turnOffFriction();
                }
            }
        }

        if (controller.monsterControllerActive)
        {
            if (!controller.isPhasingThroughPlatform && !controller.isGrounded())
            {
                Collider2D platform = controller.GetClosestPlatform();
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
                    controller.land();
                }
            }
            else if (controller.isSemiGrounded())
            {
                if (controller.grounded == false && (controller.myRigidbody.velocity.y < 0f || controller.myRigidbody.velocity.y == 0f) && controller.isPhasingThroughPlatform == false && controller.landDetectionReady)
                {
                    controller.land();
                }
            }
            else if ((controller.myRigidbody.velocity.y < 0f || controller.myRigidbody.velocity.y == 0f) && controller.myRigidbody.gravityScale != controller.slowFallGravityPower && controller.canMove && controller.slowFallBlocked == false)
            {
                //falling
                controller.activateSlowFall();
            }

            if (controller.isWalking == false && controller.myMonster.isWalking)
            {
                controller.stopWalkingVisual();
            }

            if (controller.isRunning == false && controller.myMonster.isRunning)
            {
                controller.stopRunningVisual();
            }
            else if (controller.isRunning && controller.myMonster.isRunning == false)
            {
                controller.startRunningVisual();
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
                                controller.stopWalkingVisual();
                                controller.startRunningVisual();
                            }

                            controller.myRigidbody.velocity = new Vector2(1 * controller.runSpeed, controller.myRigidbody.velocity.y);
                        }
                        else
                        {
                            //left
                            if (controller.isRunning == false)
                            {
                                controller.isRunning = true;
                                controller.isWalking = false;
                                controller.stopWalkingVisual();
                                controller.startRunningVisual();
                            }

                            controller.myRigidbody.velocity = new Vector2(-1 * controller.runSpeed, controller.myRigidbody.velocity.y);
                        }
                    }
                    else if ((controller.inputHandler.LeftStick.x > 0.2f || controller.inputHandler.LeftStick.x < -0.2f))
                    {
                        if (controller.inputHandler.LeftStick.x > 0.2f)
                        {
                            controller.myRigidbody.velocity = new Vector2(1 * controller.walkSpeed, controller.myRigidbody.velocity.y);
                        }
                        else
                        {
                            controller.myRigidbody.velocity = new Vector2(-1 * controller.walkSpeed, controller.myRigidbody.velocity.y);
                        }
                    }
                    else if ((controller.inputHandler.LeftStick.x < 0.1f && controller.inputHandler.LeftStick.x > -0.1f))
                    {
                        controller.myRigidbody.velocity = new Vector2(0, controller.myRigidbody.velocity.y);

                        if (controller.isWalking && controller.isPhasingThroughPlatform == false && controller.grounded)
                        {
                            controller.isWalking = false;
                            controller.isRunning = false;
                            controller.stopWalkingVisual();
                            controller.startMiscIdleAnimations();
                            controller.turnOnFriction();
                        }

                        if (controller.isRunning && controller.isPhasingThroughPlatform == false && controller.grounded)
                        {
                            controller.isRunning = false;
                            controller.isWalking = false;
                            controller.stopRunningVisual();
                            controller.startMiscIdleAnimations();
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
                            controller.myRigidbody.velocity = new Vector2(1 * controller.runSpeed / 1.8f, controller.myRigidbody.velocity.y);
                        }
                        else
                        {
                            //left
                            controller.myRigidbody.velocity = new Vector2(-1 * controller.runSpeed / 1.8f, controller.myRigidbody.velocity.y);
                        }
                    }
                    else if (controller.inputHandler.LeftStick.x > 0.2f || controller.inputHandler.LeftStick.x < -0.2f)
                    {
                        if (controller.inputHandler.LeftStick.x > 0.2f)
                        {
                            //right
                            controller.myRigidbody.velocity = new Vector2(1 * controller.walkSpeed / 2, controller.myRigidbody.velocity.y);
                        }
                        else
                        {
                            //left
                            controller.myRigidbody.velocity = new Vector2(-1 * controller.walkSpeed / 2, controller.myRigidbody.velocity.y);
                        }
                    }
                    else if ((controller.inputHandler.LeftStick.x < 0.1f && controller.inputHandler.LeftStick.x > -0.1f))
                    {
                        controller.myRigidbody.velocity = new Vector2(controller.myRigidbody.velocity.x - 0.001f, controller.myRigidbody.velocity.y);

                        if (controller.isWalking && controller.isPhasingThroughPlatform == false)
                        {
                            controller.isWalking = false;
                            controller.isRunning = false;
                            controller.stopWalkingVisual();
                            controller.stopRunningVisual();
                            controller.startMiscIdleAnimations();
                        }

                        if (controller.isRunning && controller.isPhasingThroughPlatform == false)
                        {
                            controller.isRunning = false;
                            controller.isWalking = false;
                            controller.stopWalkingVisual();
                            controller.stopRunningVisual();
                            controller.startMiscIdleAnimations();
                        }
                    }
                }

                if (controller.isBelowSemiGround() && controller.isGrounded() == false)
                {
                    if (controller.isPhasingThroughPlatform == false)
                    {
                        controller.phase();
                        controller.isPhasingThroughPlatform = true;
                        controller.isCrouching = false;
                        controller.isFastFalling = false;
                    }
                }

                if (controller.buttonA_Pressed == false && controller.buttonB_Pressed == false && controller.buttonX_Pressed == false && controller.buttonY_Pressed == false && controller.isAttacking == false)
                {
                    if (controller.inputHandler.LeftStick.y > 0.4f && Mathf.Abs(controller.inputHandler.LeftStick.x) < 0.4f)
                    {
                        // Cancels the jump if you attack during the delay window. I don't like using a seperate bool for this but isAttacking does not work reliably 
                        if (controller.leftStickIsAttacking)
                        {
                            return;
                        }

                        controller.leftStickElapsedJumpTime += Time.deltaTime;
                        if (controller.canJump && controller.numberOfJumpsLeft > 0 && controller.jumpButtonReset)
                        {
                            // Adds a delay so that the movement modifiers have time to activate
                            if (controller.leftStickElapsedJumpTime >= controller.LeftStickJumpDelayTime)
                            {
                                controller.bigJump();
                            }
                        }
                    }
                    else
                    {
                        if (controller.leftJoystickValue < 0.1f)
                        {
                            controller.leftStickElapsedJumpTime = 0;
                            controller.leftStickIsAttacking = false;
                        }
                    }

                    if (controller.inputHandler.LeftStick.y < 0.05f && controller.jumpButtonReset == false)
                    {
                        controller.jumpButtonReset = true;
                    }

                    if (controller.inputHandler.LeftStick.y < -0.6f && (controller.inputHandler.LeftStick.x < 0.1f && controller.inputHandler.LeftStick.x > -0.1f))
                    {
                        //down stick -> either crouch or go through semi solid or fast fall
                        if (controller.isGrounded())
                        {
                            //crouch
                            if (controller.isCrouching == false)
                            {
                                controller.startCrouchVisual();
                                controller.isCrouching = true;
                                controller.isPhasingThroughPlatform = false;
                                controller.isFastFalling = false;

                                if ((controller.isRunning || controller.isWalking))
                                {
                                    if (controller.isWalking)
                                    {
                                        controller.stopWalkingVisual();
                                    }

                                    if (controller.isRunning)
                                    {
                                        controller.stopRunningVisual();
                                    }

                                    controller.isRunning = false;
                                    controller.isWalking = false;
                                    controller.startMiscIdleAnimations();
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
                                controller.phaseThroughPlatformVisual();
                                controller.grounded = false;
                                controller.isCrouching = false;
                                controller.isFastFalling = false;
                                controller.landDetectionReady = false;
                            }
                        }
                    }
                    else
                    {
                        if (controller.isCrouching)
                        {
                            controller.endCrouchVisual();
                            controller.isCrouching = false;
                        }
                    }
                }
            }

            if (controller.isAttacking == false)
            {
                if (controller.isRolling)
                {
                    if (controller.rightJoystickVector.x > 0.2f)
                    {
                        controller.myRigidbody.velocity = new Vector2(1 * controller.rollSpeed, controller.myRigidbody.velocity.y);
                    }
                    else if (controller.rightJoystickVector.x < -0.2f)
                    {
                        controller.myRigidbody.velocity = new Vector2(-1 * controller.rollSpeed, controller.myRigidbody.velocity.y);
                    }
                }
            }

            if (controller.chargingForward)
            {
                if (controller.facingRight)
                {
                    controller.myRigidbody.velocity = new Vector2(1 * (controller.runSpeed * 2), controller.myRigidbody.velocity.y);
                }
                else
                {
                    controller.myRigidbody.velocity = new Vector2(-1 * (controller.runSpeed * 2), controller.myRigidbody.velocity.y);
                }
            }
        }
    }

    public override void Exit()
    {

    }
}

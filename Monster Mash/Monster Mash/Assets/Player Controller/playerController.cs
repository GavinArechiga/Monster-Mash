using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerController : MonoBehaviour
{
    public int playerIndex;
    public monsterAttackSystem myMonster;
    public CapsuleCollider2D bodyCollider;
    //public MeshRenderer standingVisual;
    public MeshRenderer ballVisual;
    public Transform groundCheck;
    public LayerMask solidGroundLayer;
    private PlayerInput playerInput;
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
    //
    private float walkSpeed = 5f;
    private float runSpeed = 25f;
    private float groundedModifer = 1;
    private float airbornModifer = 0.75f;
    private float currentGroundedStateModifier = 1;
    private bool canMove = true;
    public bool isWalking = false;
    public bool isRunning = false;
    //
    public bool grounded = false;
    bool canJump = true;
    bool jumpButtonReset = false;
    public bool primedForBigJump = false;
    public int numberOfJumps = 2;
    public int numberOfJumpsLeft = 2;
    private Vector2 jumpValue;
    private bool jumpValueHasBeenRead = true;
    private float bigJumpPower = 65;//80
    private float littleJumpPower = 45;//60
    private float gravityPower;
    //
    private float rollSpeed = 50f;//50
    Vector2 rightJoystickVector; //gives us direction on x axis for roll
    public bool isRolling = false;
    private bool canRoll = true;
    //
    private float dashSpeed = 60f;//60
    public bool isDashing = false;
    private bool canDash = true;
    //
    private bool ledgeHopAvailable = true;
    //
    public bool grabbingWall = false;
    private float wallGrabbingGravityPower = 0.5f;
    private float wallJumpPower = 28f;

    [SerializeField]
    private Rigidbody2D myRigidbody;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerControlsMap = new PlayerControls();
        gravityPower = myRigidbody.gravityScale;

        startingActionMap = playerInput.actions.FindActionMap("Starting Action Map");
        UIcontrols = playerInput.actions.FindActionMap("UI Controls");
        monsterControls = playerInput.actions.FindActionMap("Monster Controls");

        if (UIcontrols!= null)
        {
            playerInput.SwitchCurrentActionMap("UI Controls");
            playerControlsMap.StartingActionMap.Disable();
            //print("New Action Map: " + playerInput.currentActionMap);
        }
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
            if (canMove)
            {

                if (isGrounded())
                {
                    if (grounded == false)
                    {
                        land();
                    }

                    #region Run and Walk on Ground
                    if (leftJoystickVector.x > 0.9f || leftJoystickVector.x < -0.9f)
                    {
                        //run

                        if (leftJoystickVector.x > 0.9f)
                        {
                            //right
                            myRigidbody.velocity = new Vector2(1 * runSpeed, myRigidbody.velocity.y);
                        }
                        else
                        {
                            //left
                            myRigidbody.velocity = new Vector2(-1 * runSpeed, myRigidbody.velocity.y);
                        }
                    }
                    else if (leftJoystickVector.x > 0.1f || leftJoystickVector.x < -0.1f)
                    {
                        //walk

                        if (leftJoystickVector.x > 0.1f)
                        {
                            //right
                            myRigidbody.velocity = new Vector2(1 * walkSpeed, myRigidbody.velocity.y);
                        }
                        else
                        {
                            //left
                            myRigidbody.velocity = new Vector2(-1 * walkSpeed, myRigidbody.velocity.y);
                        }
                    }
                    else if(leftJoystickValue < 0.1f)
                    {
                        myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);

                        if (isWalking)
                        {
                            isWalking = false;
                            isRunning = false;
                            stopWalkingVisual();
                        }

                        if (isRunning)
                        {
                            isRunning = false;
                            isWalking = false;
                            stopRunningVisual();
                        }
                    }
                    #endregion

                }
                else
                {
                    if (grounded)
                    {
                        grounded = false;
                        fallVisual();
                    }

                    #region Run and Walk in Air
                    if (leftJoystickVector.x > 0.9f || leftJoystickVector.x < -0.9f)
                    {
                        //run

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
                    else if (leftJoystickVector.x > 0.1f || leftJoystickVector.x < -0.1f)
                    {
                        //walk

                        if (leftJoystickVector.x > 0.1f)
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
                    else if(leftJoystickValue < 0.1f)
                    {
                        myRigidbody.velocity = new Vector2(myRigidbody.velocity.x - 0.001f, myRigidbody.velocity.y);

                        if (isWalking)
                        {
                            isWalking = false;
                            isRunning = false;
                            stopWalkingVisual();
                            stopRunningVisual();
                        }

                        if (isRunning)
                        {
                            isRunning = false;
                            isWalking = false;
                            stopWalkingVisual();
                            stopRunningVisual();
                        }
                    }
                    #endregion
   
                }


                #region Jumping
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

                if (leftJoystickVector.y < 0.05f && jumpButtonReset == false)
                {
                    jumpButtonReset = true;
                }
                #endregion

            }


            if (isRolling)
            {
                #region Rolling Momentum
                if (rightJoystickVector.x > 0.1f)
                {
                    myRigidbody.velocity = new Vector2(1 * rollSpeed, myRigidbody.velocity.y);
                }
                else if (rightJoystickVector.x < -0.1f)
                {
                    myRigidbody.velocity = new Vector2(-1 * rollSpeed, myRigidbody.velocity.y);
                }
                #endregion
            }

            if (isDashing && grabbingWall == false)
            {
                #region Entering Wall Grab
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
                if (isGrounded() && grounded == false)
                {
                    land();
                }
                else if (wallCheck(1) == false && wallCheck(-1) == false && grabbingWall)
                {
                    slippedOffWall();
                }

                if (facingRight && leftJoystickVector.x < -0.1f)
                {
                    //jump left
                    wallJump(-1);
                    facingRight = false;
                    flipLeftVisual();
                    littleJumpVisual();
                }
                else if (facingRight == false && leftJoystickVector.x > 0.1f)
                {
                    //jump right
                    wallJump(1);
                    facingRight = true;
                    flipRightVisual();
                    littleJumpVisual();
                }
                #endregion
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
        }

        if (context.performed && grabbingWall == false && isDashing == false && isRolling == false)
        {
            if (facingRight == false && leftJoystickVector.x > 0.1f)
            {
                //face right
                facingRight = true;
                flipRightVisual();
            }
            else if (facingRight && leftJoystickVector.x < -0.1f)
            {
                //face left
                facingRight = false;
                flipLeftVisual();
            }
        }

        if (context.performed)
        {
            if (leftJoystickVector.x > 0.9f || leftJoystickVector.x < -0.9f)
            {
                //run
                if (isRunning == false)
                {
                    isRunning = true;
                    isWalking = false;
                    startRunningVisual();
                }
            }
            else if (leftJoystickVector.x > 0.1f || leftJoystickVector.x < -0.1f)
            {
                //walk
                if (isWalking == false && isRunning == false)
                {
                    isWalking = true;
                    isRunning = false;
                    startWalkingVisual();
                }
            }
        }

    }

    private bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.5f, solidGroundLayer);
    }

    private void land()
    {
        grounded = true;
        numberOfJumpsLeft = numberOfJumps;
        StopCoroutine(jumpRecharge());
        myRigidbody.gravityScale = gravityPower;
        bodyCollider.enabled = true;
        ballVisual.enabled = false;
        canJump = true;
        canDash = true;
        canRoll = true;
        grabbingWall = false;
        canMove = true;
        primedForBigJump = false;
        landVisual();
    }

    private void bigJump()
    {
        if (numberOfJumpsLeft > 0)
        {
            numberOfJumpsLeft--;
            grounded = false;
            jumpButtonReset = false;
            primedForBigJump = true;
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, bigJumpPower);
            StartCoroutine(jumpRecharge());
            bigJumpVisual();
        }
    }

    private void littleJump()
    {
        if (numberOfJumpsLeft > 0)
        {
            numberOfJumpsLeft--;
            grounded = false;
            jumpButtonReset = false;
            primedForBigJump = true;
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, littleJumpPower);
            StartCoroutine(jumpRecharge());
            littleJumpVisual();
        }

    }

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

        if (context.performed && (canDash || canRoll))
        {

            if (isGrounded())
            {
                if (canRoll)
                {
                    if (rightJoystickVector.x > 0.1f || rightJoystickVector.x < -0.1f)
                    {
                        StartCoroutine(rollTime());
                    }
                }
            }
            else
            {
                if (canDash)
                {
                    if (rightJoystickVector.x > 0.1f || rightJoystickVector.x < -0.1f)
                    {
                        StartCoroutine(dashTime());
                    }
                }
            }

        }

    }

    IEnumerator rollTime()
    {
        //start
        isRolling = true;
        canMove = false;
        canRoll = false;
        canDash = false;
        bodyCollider.enabled = false;
        //standingVisual.enabled = false;
        //ballVisual.enabled = true;
        rollVisual();
        yield return new WaitForSeconds(0.125f);
        myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
        bodyCollider.enabled = true;
        //standingVisual.enabled = true;
        //ballVisual.enabled = false;
        isRolling = false;
        isDashing = false;
        canMove = true;
        canDash = true;
        StartCoroutine(rollRecharge());
        //stop
    }

    IEnumerator rollRecharge()
    {
        yield return new WaitForSeconds(0.5f);
        canRoll = true;
    }

    IEnumerator dashTime()
    {
        //start
        isDashing = true;
        canMove = false;
        canDash = false;
        canRoll = false;
        bodyCollider.enabled = false;
        //standingVisual.enabled = false;
        //ballVisual.enabled = true;
        myRigidbody.gravityScale = 0;
        startDashAttackVisual();
        yield return new WaitForSeconds(0.2f);

        if (grabbingWall == false)
        {
            myRigidbody.gravityScale = gravityPower;
            myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
            bodyCollider.enabled = true;
            //standingVisual.enabled = true;
            //ballVisual.enabled = false;
            isDashing = false;
            isRolling = false;
            grabbingWall = false;
            canRoll = true;
            canMove = true;
            endDashAttackVisual();
        }
    }

    private bool wallCheck(int direction)
    {
        return Physics2D.Raycast(transform.position, direction * transform.right, 2f, solidGroundLayer);
    }

    private bool wallToFloorCheck()
    {
        return Physics2D.Raycast(transform.position, -transform.up, 3f, solidGroundLayer);
    }

    private void wallGrab(int direction)
    {
        StopCoroutine(dashTime());
        endDashAttackVisual();
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
        jumpButtonReset = false;
        myRigidbody.velocity = new Vector2(direction * wallJumpPower, littleJumpPower);
        StartCoroutine(jumpRecharge());
        bodyCollider.enabled = true;
        //standingVisual.enabled = true;
        //ballVisual.enabled = false;
        isDashing = false;
        isRolling = false;
        canRoll = true;
        canDash = true;
        grabbingWall = false;
        canMove = true;
    }

    private void slippedOffWall()
    {
        myRigidbody.gravityScale = gravityPower;
        myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
        bodyCollider.enabled = true;
        //standingVisual.enabled = true;
        ballVisual.enabled = false;
        isDashing = false;
        isRolling = false;
        canRoll = true;
        canDash = true;
        grabbingWall = false;
        canMove = true;
    }


    #endregion

    #region Monster Attack System Communication

    private void flipLeftVisual()
    {
        myMonster.flipLeft();
    }

    private void flipRightVisual()
    {
        myMonster.flipRight();
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

    private void littleJumpVisual()
    {
        myMonster.jump();
    }

    private void bigJumpVisual()
    {
        myMonster.jump();
    }

    private void fallVisual()
    {
        myMonster.walkToFall();
    }

    private void landVisual()
    {
        myMonster.land();
    }

    private void startDashAttackVisual()
    {
        myMonster.dashAttack();
    }

    private void endDashAttackVisual()
    {
        myMonster.endDashAttack();
    }

    private void rollVisual()
    {
        myMonster.roll();
    }

    private void crouchVisual()
    {
        myMonster.crouch();
    }
    #endregion
}

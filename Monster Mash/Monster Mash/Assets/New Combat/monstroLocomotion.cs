using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class monstroLocomotion : MonoBehaviour
{

    //Walking and Running
    private CharacterController controller;
    public bool playerLock = true;
    private float moveSpeed = 30f;
    private float walkSpeed = 10f;
    private float runSpeed = 30f;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 inputVector = Vector2.zero;

    //Gravity
    public Transform groundCheck;
    private bool isGrounded;
    private float groundDistance = 1f;//originally 0.4f
    public LayerMask groundMask;
    public LayerMask slopeMask;
    private float gravity = -120f;//was 150
    private float immenseGravity = -400f;
    private float flightedGravity = -20f;
    private bool stuckOnWall;
    private bool isGravityBlind;

    //Jumping and Flight
    private float jumpPower = 10f;
    private float flightedJumpPower = 10f;
    public bool wingedMonster = false;
    private bool isFlying = false;
    private int jumpsLeft = 2;
    private bool isLanded = true;//used to ensure that landing functions only happen once. Also used to tell if players walk or fell off terrain as opposed to jumping
    Vector3 velocity;

    //Hazards and Level Interaction
    public LayerMask trampolineMask;
    private bool isBouncing = false;
    private dontDestroy playerDD;
    private bool onMovingPlatform;
    private Vector3 movingPlatformEnteredPosition;

    //damage launching
    public bool isStunLocked;
    public bool isElectricLocked;
    private Vector3 launchDirection;
    private int launchPower = 50;
    private int lightLaunchPower = 50;
    private int HeavyLaunchPower = 100;

    //attack movement
    private monstroPartHandler monstroVisuals;
    private bool attackLocked;
    private bool rotationLocked;
    private int attackMovementPower = 50;
    private int lightAttackMovement = 50;
    private int heavyAttackMovement = 100;
    private Vector3 attackDirection;
    private Vector3 attackInputVector = Vector2.zero;

    //visual
    public Transform characterRotator;

    private void Awake()
    {
        playerDD = this.GetComponent<dontDestroy>();
    }


    private void OnEnable()
    {
        controller = GetComponent<CharacterController>();
        monstroVisuals = GetComponent<monstroPartHandler>();
        velocity.x = 0f;
        velocity.z = 0f;
        velocity.y = 0f;
        jumpsLeft = 2;
        flightedJumpPower = jumpPower;
        isFlying = false;
        isLanded = true;
        stuckOnWall = false;
        playerLock = true;
        rotationLocked = false;
        attackLocked = false;
        StartCoroutine(movementSpawnDelay());
    }

    IEnumerator movementSpawnDelay()
    {
        yield return new WaitForSeconds(0.5f);
        playerLock = false;
    }

    public void movementInput(Vector2 direction)
    {
        if (playerLock) return;
        if (isStunLocked) return;
        if (isElectricLocked) return;
        if (rotationLocked) return;
        inputVector = direction;


        if (attackLocked) return;

        if ((direction.x > 0.35f || direction.x < -0.35f) || (direction.y > 0.35f || direction.y < -0.35f))
        {
            if (moveSpeed != runSpeed)
            {
                moveSpeed = runSpeed;
                monstroVisuals.run();
            }
        }
        else if((direction.x > 0.1f || direction.x < -0.1f) || (direction.y > 0.1f || direction.y < -0.1f))
        {
            if (moveSpeed != walkSpeed)
            {
                moveSpeed = walkSpeed;
                monstroVisuals.walk();
            }
        }
        else
        {
            if (moveSpeed != 0)
            {
                moveSpeed = 0;
                monstroVisuals.idle();
            }
        }
    }

    public void attackDirectionInput(Vector2 direction)
    {
        attackInputVector = direction;
    }

    public void jumpInput()
    {
        if (jumpsLeft == 0) return;
        if (flightedJumpPower == 0) return;
        if (playerLock) return;
        if (isStunLocked) return;
        if (isElectricLocked) return;
        if (rotationLocked) return;
        if (attackLocked) return;

        isLanded = false;
        isGravityBlind = false;

        if (wingedMonster == false)
        {
            #region Jump and Double Jump - Non Winged Monsters

            if (jumpsLeft == 2)
            {
                monstroVisuals.jump();
            }
            else
            {
                monstroVisuals.doubleJump();
            }

            jumpsLeft = jumpsLeft - 1;
            velocity.y = 0f;
            velocity.y += Mathf.Sqrt(jumpPower * -2f * gravity);
            #endregion
        }
        else
        {
            #region Jump and Flight - Winged Monsters
            if (isGrounded)
            {
                velocity.y = 0f;
                velocity.y += Mathf.Sqrt(jumpPower * -2f * gravity);
            }
            else
            {
                if (isFlying)
                {
                    velocity.y = 0f;
                    flightedJumpPower = flightedJumpPower - 2;
                    velocity.y += Mathf.Sqrt(flightedJumpPower * -2f * gravity);
                }
                else
                {
                    velocity.y = 0f;
                    velocity.y += Mathf.Sqrt(jumpPower * -2f * gravity);
                    isFlying = true;
                }
            }

            #endregion
        }
    }

    void Update()
    {
        /*
        if (playerLock == false && isElectricLocked == false)
        {
            if (isStunLocked == false)
            {
                if (attackLocked)
                {
                    controller.Move(moveDirection * attackMovementPower * Time.deltaTime);
                }
                else
                {
                    moveDirection = new Vector3(inputVector.x, 0, inputVector.y);
                    moveDirection = transform.TransformDirection(moveDirection);
                    moveDirection *= moveSpeed;
                    controller.Move(moveDirection * Time.deltaTime);
                }
            }
            else
            {
                controller.Move(launchDirection * launchPower * Time.deltaTime);
            }
        }
        */
        applyGravity();
        applyCharacterRotation();
        if (playerLock != false) return; //world has asked me to stop all controllers
        if (isElectricLocked != false) return; //I have been electrocuted and im paralyzed

        if (attackLocked && rotationLocked == false) //I am engaging an attack but the player hasn't confirmed a target by letting go of the button
        {
            //attackDirection = -characterRotator.forward;
            //attackDirection = transform.TransformDirection(attackDirection);
            moveDirection = new Vector3(inputVector.x, 0, inputVector.y);
            moveDirection = transform.TransformDirection(moveDirection);
            return;
        }


        if (isStunLocked) //I have been punched and launched in a direction
        {
            controller.Move(launchDirection * launchPower * Time.deltaTime);
            return;
        }

        if (attackLocked && rotationLocked) //I have engaged an attack and have confirmed an attack rotation
        {
            attackDirection = -characterRotator.forward;
            attackDirection = transform.TransformDirection(attackDirection);
            controller.Move(attackDirection * attackMovementPower * Time.deltaTime);
            return;
        }

        //if all that is false and we're just moving around
        moveDirection = new Vector3(inputVector.x, 0, inputVector.y);
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= moveSpeed;
        controller.Move(moveDirection * Time.deltaTime);


    }

    #region Gravity
    private void applyGravity()
    {
        if (isGravityBlind) // when on an elevator or moving platform, we freeze the gravity completely after parenting ourselves to it
        {
            //we do want to keep reapplying the local y position to make sure we dont have any accidental clipping through elevators
            transform.localPosition = new Vector3(transform.localPosition.x, movingPlatformEnteredPosition.y, transform.localPosition.z);
            return;
        } 

        bool groundSphereDetected = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        bool slopeDetected = (Physics.Raycast(groundCheck.position, Vector3.down, groundDistance * 2, slopeMask));
        bool slopeSphereDetected = Physics.CheckSphere(groundCheck.position, groundDistance, slopeMask);
        isBouncing = Physics.CheckSphere(groundCheck.position, groundDistance, trampolineMask);
        bool underTrampoline = (Physics.Raycast(groundCheck.position, Vector3.up, 5, trampolineMask));

        if (groundSphereDetected)
        {
            isGrounded = true;
        }
        else if (slopeDetected) //slopes require an immense downward force in order to avoid juttering
        {
            isGrounded = true;

            if (isLanded)
            {
                //increase downwards forces
                if (velocity.y > -6000)
                {
                    velocity.y += immenseGravity * Time.deltaTime;
                }
                else
                {
                    velocity.y = -6000;
                }
                controller.Move(velocity * Time.deltaTime);
            }
        }
        else
        {
            isGrounded = false;
        }

        
        if (isBouncing && (underTrampoline == false) && ((velocity.y == 0) || (velocity.y < 0)))
        {
            bounce();
            return;
        }
        

        if (isGrounded && ((velocity.y == 0) || (velocity.y < 0)))
        {
            if (isLanded == false)
            {
                land();

                if (onMovingPlatform)
                {
                    isGravityBlind = true;
                    movingPlatformEnteredPosition = this.transform.localPosition;
                }
            }
        }
        else
        {
            //falling

            if (isLanded)
            {
                isLanded = false;
                velocity.y = 0f;
                monstroVisuals.fall();
            }

            if (isFlying && (velocity.y == 0 || velocity.y < 0)) //falling with flight
            {
                //put a velocity cap
                if (velocity.y > -120)
                {
                    velocity.y += flightedGravity * Time.deltaTime;
                }
                else
                {
                    velocity.y = -120;
                }
                controller.Move(velocity * Time.deltaTime);
            }
            else //regular falling
            {

                if (isLanded == false && isGrounded == false && slopeSphereDetected && stuckOnWall == false)//this means we are sticky on the walls
                {
                    stuckOnWall = true;
                }

                if (stuckOnWall)//this will stop the fast fall after becoming sticky
                {
                    velocity.y = -40;
                    stuckOnWall = false;
                }

                //put a velocity cap
                if (velocity.y > -120) 
                {
                    velocity.y += gravity * Time.deltaTime;
                }
                else
                {
                    velocity.y = -120;
                }
                controller.Move(velocity * Time.deltaTime);
            }
        }
    }
    #endregion

    #region Character Rotation
    private void applyCharacterRotation()
    {
        if (moveDirection != Vector3.zero)
        {
            Quaternion intendedRotation = Quaternion.LookRotation(-moveDirection, Vector3.up);

            characterRotator.transform.rotation = Quaternion.RotateTowards(characterRotator.transform.rotation, intendedRotation, 800 * Time.deltaTime);
        }
    }
    #endregion

    private void land()
    {
        velocity.y = 0f;
        jumpsLeft = 2;
        flightedJumpPower = jumpPower;
        isFlying = false;
        isLanded = true;
        stuckOnWall = false;
        isStunLocked = false;
        monstroVisuals.land();
    }

    public void forceRespawn()
    {
        monstroFightManager fightManager = Object.FindFirstObjectByType<monstroFightManager>();
        fightManager.respawnPlayer(this.gameObject);
        velocity.y = 0f;
    }

    #region Level Interactions

    private void bounce() //when the monster lands on a trampoline
    {
        velocity.y = 0f;
        velocity.y += Mathf.Sqrt((jumpPower * 1.5f) * -2f * gravity);
        jumpsLeft = 2;
        flightedJumpPower = jumpPower;
        isFlying = false;
        monstroVisuals.jump();
    }

    private void OnTriggerEnter(Collider other)//Level Triggers
    {
        if (other.gameObject.tag == "Out of Bounds")
        {
            forceRespawn();
        }

        if (other.gameObject.tag == "Animation")
        {
            if (other.gameObject.GetComponent<animationStarter>() != null)
            {
                animationStarter animHandler = other.gameObject.GetComponent<animationStarter>();

                if (animHandler.isTrampolineAnimation)
                {
                    bool nearTrampoline = Physics.CheckSphere(groundCheck.position, groundDistance, trampolineMask);

                    if (nearTrampoline)
                    {
                        animHandler.playAnimation();
                    }
                }
                else
                {
                    animHandler.playAnimation();
                }
            }
            else if (other.gameObject.GetComponentInParent<animationStarter>() != null)
            {
                other.gameObject.GetComponentInParent<animationStarter>().playAnimation();

                animationStarter animHandler = other.gameObject.GetComponentInParent<animationStarter>();

                if (animHandler.isTrampolineAnimation)
                {
                    bool nearTrampoline = Physics.CheckSphere(groundCheck.position, groundDistance, trampolineMask);

                    if (nearTrampoline)
                    {
                        animHandler.playAnimation();
                    }
                }
                else
                {
                    animHandler.playAnimation();
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Moving Platform")
        {
            if (isGravityBlind == false)
            {
                if (onMovingPlatform == false)
                {

                    this.transform.parent = other.gameObject.transform;
                    onMovingPlatform = true;
                }

                if (isGrounded && isLanded)
                {
                    isGravityBlind = true;
                    movingPlatformEnteredPosition = this.transform.localPosition;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Moving Platform")
        {
            this.transform.SetParent(null);
            playerDD.reassignDontDestroyStatus();
            onMovingPlatform = false;
            isGravityBlind = false;
        }
    }

    #endregion

    #region Damage Launching and Effects
    public void damageLaunch(Transform hitPoint, bool isHeavy, bool requiresReverseLaunch)
    {
        StopCoroutine(damageLaunchDelay());
        StopCoroutine(electricLaunchDelay());
        StopCoroutine(attackMovementTimer());

        if (requiresReverseLaunch)
        {
            launchDirection = hitPoint.position - transform.position;
        }
        else
        {
            launchDirection = transform.position - hitPoint.position;
        }

        launchDirection.y = 0;
        launchDirection.Normalize();
        velocity.x = 0;
        velocity.z = 0;
        attackLocked = false;
        rotationLocked = false;


        if (isHeavy)
        {
            launchPower = HeavyLaunchPower;
        }
        else
        {
            launchPower = lightLaunchPower;
        }

        StartCoroutine(damageLaunchDelay());
    }

    IEnumerator damageLaunchDelay()
    {
        isElectricLocked = false;
        isStunLocked = true;

        if (launchPower == HeavyLaunchPower)
        {
            yield return new WaitForSeconds(0.2f);
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
        }
        isStunLocked = false;
    }

    public void electricDamageLaunch(Transform hitPoint, bool isHeavy, bool requiresReverseLaunch)
    {
        StopCoroutine(damageLaunchDelay());
        StopCoroutine(electricLaunchDelay());
        StopCoroutine(attackMovementTimer());

        if (requiresReverseLaunch)
        {
            launchDirection = hitPoint.position - transform.position;
        }
        else
        {
            launchDirection = transform.position - hitPoint.position;
        }

        launchDirection.y = 0;
        launchDirection.Normalize();
        velocity.x = 0;
        velocity.z = 0;
        attackLocked = false;
        rotationLocked = false;

        if (isHeavy)
        {
            launchPower = HeavyLaunchPower;
        }
        else
        {
            launchPower = lightLaunchPower;
        }

        StartCoroutine(electricLaunchDelay());
    }

    IEnumerator electricLaunchDelay()
    {
        isElectricLocked = true;
        yield return new WaitForSeconds(1f);
        isElectricLocked = false;

        isStunLocked = true;

        if (launchPower == HeavyLaunchPower)
        {
            yield return new WaitForSeconds(0.2f);
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
        }
        isStunLocked = false;
    }

    public void antiAirDamageLaunch(Transform hitPoint, bool isHeavy, bool requiresReverseLaunch)
    {
        StopCoroutine(damageLaunchDelay());
        StopCoroutine(electricLaunchDelay());
        StopCoroutine(attackMovementTimer());

        if (requiresReverseLaunch)
        {
            launchDirection = hitPoint.position - transform.position;
        }
        else
        {
            launchDirection = transform.position - hitPoint.position;
        }

        launchDirection.y = 0;
        launchDirection.Normalize();
        velocity.x = 0;
        velocity.z = 0;
        velocity.y = 0;
        velocity.y += Mathf.Sqrt(jumpPower * -2f * gravity);
        isFlying = false;
        jumpsLeft = 0;
        flightedJumpPower = 0;
        attackLocked = false;
        rotationLocked = false;

        if (isHeavy)
        {
            launchPower = HeavyLaunchPower * 2;
        }
        else
        {
            launchPower = lightLaunchPower;
        }

        isStunLocked = true;
    }

    #endregion

    #region Attack Movement

    public void attackEngaged()
    {
        attackLocked = true;
        rotationLocked = false;
        velocity.x = 0;
        velocity.z = 0;
    }

    public void attackMovementConfirmed(bool isHeavy, bool needsMovement)
    {
        velocity.x = 0;
        velocity.z = 0;
        attackLocked = true;
        rotationLocked = true;
        if (needsMovement)
        {
            if (isHeavy)
            {
                attackMovementPower = heavyAttackMovement;
            }
            else
            {
                attackMovementPower = lightAttackMovement;
            }
        }
        else
        {
            attackMovementPower = 0;
        }

        StartCoroutine(attackMovementTimer());
    }

    IEnumerator attackMovementTimer()
    {
        if (attackMovementPower == heavyAttackMovement)
        {
            yield return new WaitForSeconds(0.2f);
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
        }
        attackLocked = false;
        rotationLocked = false;
        velocity.x = 0;
        velocity.z = 0;
        velocity.y = 0;
        moveSpeed = 0;
        inputVector = Vector3.zero;
        monstroVisuals.idle();
    }

    #endregion

}

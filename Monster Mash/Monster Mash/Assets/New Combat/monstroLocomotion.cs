using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monstroLocomotion : MonoBehaviour
{
    //Walking and Running
    private CharacterController controller;
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

    //
    private bool isStunLocked;
    private Vector3 launchDirection;
    private int launchPower = 50;
    private int lightLaunchPower = 50;
    private int HeavyLaunchPower = 100;

    public LayerMask outOfBounds;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        velocity.y = 0f;
        jumpsLeft = 2;
        flightedJumpPower = jumpPower;
        isFlying = false;
        isLanded = true;
    }

    public void movementInput(Vector2 direction)
    {
        inputVector = direction;

        if ((direction.x > 0.35f || direction.x < -0.35f) || (direction.y > 0.35f || direction.y < -0.35f))
        {
            moveSpeed = runSpeed;
        }
        else
        {
            moveSpeed = walkSpeed;
        }
    }

    public void jumpInput()
    {
        if (jumpsLeft == 0) return;
        if (flightedJumpPower == 0) return;

        isLanded = false;

        if (wingedMonster == false)
        {
            #region Jump and Double Jump - Non Winged Monsters

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
        if (isStunLocked == false)
        {
            moveDirection = new Vector3(inputVector.x, 0, inputVector.y);
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= moveSpeed;
            controller.Move(moveDirection * Time.deltaTime);
        }
        else
        {
            controller.Move(launchDirection * launchPower * Time.deltaTime);
        }
        applyGravity();
        
    }

    private void applyGravity()
    {
        bool groundSphereDetected = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        bool slopeDetected = (Physics.Raycast(groundCheck.position, Vector3.down, groundDistance * 2, slopeMask));
        isBouncing = Physics.CheckSphere(groundCheck.position, groundDistance, trampolineMask);

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

        if (isBouncing && ((velocity.y == 0) || (velocity.y < 0)))
        {
            bounce();
            return;
        }

        if (isGrounded && ((velocity.y == 0) || (velocity.y < 0)))
        {
            if (isLanded == false)
            {
                land();
            }
        }
        else
        {
            //falling

            if (isLanded)
            {
                isLanded = false;
                velocity.y = 0f;
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

    private void land()
    {
        print("land");
        velocity.y = 0f;
        jumpsLeft = 2;
        flightedJumpPower = jumpPower;
        isFlying = false;
        isLanded = true;
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
        velocity.y += Mathf.Sqrt(jumpPower * -2f * gravity);
        jumpsLeft = 2;
        flightedJumpPower = jumpPower;
        isFlying = false;
    }

    private void OnTriggerEnter(Collider other)//Level Triggers
    {
        if (other.gameObject.tag == "Out of Bounds")
        {
            forceRespawn();
        }

        if (other.gameObject.tag == "Animation")
        {
            other.gameObject.GetComponent<Animator>().SetTrigger("playAnimation");
        }
    }

    #endregion

    #region Damage Launching
    public void damageLaunch(Collider hitBox, bool isHeavy)
    {
        StopCoroutine(damageLaunchDelay());
        launchDirection = transform.position - hitBox.transform.position;
        launchDirection.y = 0;
        launchDirection.Normalize();
        velocity.x = 0;
        velocity.z = 0;

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

    #endregion

}

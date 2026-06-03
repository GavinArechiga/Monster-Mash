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
    private float groundDistance = 0.4f;
    public LayerMask groundMask;
    private float gravity = -120f;//was 150
    private float normalizedGravity = -80f;
    private float flightedGravity = -20f;

    //Jumping and Flight
    private float jumpPower = 10f;
    private float flightedJumpPower = 10f;
    public bool wingedMonster = false;
    private bool isFlying = false;
    private int jumpsLeft = 2;
    private bool isLanded = true;
    Vector3 velocity;

    //Hazards and Level Interaction
    public LayerMask trampolineMask;
    private bool isBouncing = false;

    public LayerMask outOfBounds;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
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
        moveDirection = new Vector3(inputVector.x, 0, inputVector.y);
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= moveSpeed;

        controller.Move(moveDirection * Time.deltaTime);
        applyGravity();
    }

    private void applyGravity()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        isBouncing = Physics.CheckSphere(groundCheck.position, groundDistance, trampolineMask);

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
            isLanded = false;

            if (isFlying && (velocity.y == 0 || velocity.y < 0))
            {
                velocity.y += flightedGravity * Time.deltaTime;
                controller.Move(velocity * Time.deltaTime);
            }
            else
            {
                velocity.y += gravity * Time.deltaTime;
                controller.Move(velocity * Time.deltaTime);
            }
        }
    }

    private void land()
    {
        velocity.y = 0f;
        jumpsLeft = 2;
        flightedJumpPower = jumpPower;
        isFlying = false;
        isLanded = true;
    }

    #region Level Hazards and Interactions

    private void bounce() //when the monster lands on a trampoline
    {
        velocity.y = 0f;
        velocity.y += Mathf.Sqrt(jumpPower * -2f * gravity);
        jumpsLeft = 2;
        flightedJumpPower = jumpPower;
        isFlying = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Out of Bounds")
        {
            monstroFightManager fightManager = Object.FindFirstObjectByType<monstroFightManager>();
            fightManager.respawnPlayer(this.gameObject);
            velocity.y = 0f;
        }
    }
    #endregion

}

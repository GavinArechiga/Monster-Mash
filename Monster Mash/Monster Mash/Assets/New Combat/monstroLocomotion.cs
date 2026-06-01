using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monstroLocomotion : MonoBehaviour
{
    private CharacterController controller;
    private float moveSpeed = 20f;
    private float walkSpeed = 10f;
    private float runSpeed = 30f;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 inputVector = Vector2.zero;
    private float gravity = -100f;
    private float flightedGravity = -5f;
    private float jumpPower = 10f;
    public bool wingedMonster;
    private bool isFlying = false;
    private int jumpsLeft = 2;
    private bool isLanded = true;
    Vector3 velocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public void setInputVector(Vector2 direction)
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

        if (!wingedMonster)
        {
            jumpsLeft = jumpsLeft - 1;
            velocity.y = 0f;
            velocity.y += Mathf.Sqrt(jumpPower * -2f * gravity);
        }
        else
        {
            if (controller.isGrounded)
            {
                velocity.y = 0f;
                velocity.y += Mathf.Sqrt(jumpPower * -2f * gravity);
            }
            else
            {
                if (isFlying)
                {
                    velocity.y = 0f;
                    isFlying = false;
                    jumpsLeft = 0;
                }
                else
                {
                    velocity.y = 0f;
                    velocity.y += Mathf.Sqrt(jumpPower * -2f * gravity);
                    isFlying = true;
                }
            }
        }
    }

    // Update is called once per frame
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
        if (controller.isGrounded)
        {
            land();
        }
        else
        {
            if (isFlying)
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
        isFlying = false;
        isLanded = true;
    }
}

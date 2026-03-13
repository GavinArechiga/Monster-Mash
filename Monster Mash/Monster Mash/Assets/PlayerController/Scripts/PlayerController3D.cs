 using UnityEngine;
using UnityEngine.InputSystem;
    
    public class PlayerController3D : MonoBehaviour
    {
        [Header("Player")]
        public float MoveSpeed = 2.0f;
        public float RotationSmoothTime = 0.12f;
        public float SpeedChangeRate = 10.0f;

        [Space(10)]
        public float JumpHeight = 1.2f;
        public float Gravity = -15.0f;

        [Space(10)]
        public float JumpTimeout = 0.50f;

        [Header("Player Grounded")]
        public bool Grounded = true;
        public float GroundedOffset = -0.14f; 
        public float GroundedRadius = 0.28f;
        public LayerMask GroundLayers;

        // player
        private float speed;
        private float targetRotation = 0.0f;
        private float rotationVelocity;
        private float verticalVelocity;
        private float terminalVelocity = 53.0f;
        
        private float jumpTimeoutDelta;
        
        private PlayerInput playerInput;
        private CharacterController controller;
        private PlayerInputs input;
        private GameObject mainCamera;

        private void Awake()
        {
            if (mainCamera == null)
            {
                mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            controller = GetComponent<CharacterController>();
            input = GetComponent<PlayerInputs>();
            playerInput = GetComponent<PlayerInput>();
        }

        private void Update()
        {
            JumpAndGravity();
            GroundedCheck();
            Move();
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);
        }

        private void Move()
        {
            float targetSpeed = MoveSpeed;
            
            if (input.move == Vector2.zero) targetSpeed = 0.0f;
            
            float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = input.analogMovement ? input.move.magnitude : 1f;
            
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);
                
                speed = Mathf.Round(speed * 1000f) / 1000f;
            }
            else
            {
                speed = targetSpeed;
            }
            
            Vector3 inputDirection = new Vector3(input.move.x, 0.0f, input.move.y).normalized;
            
            if (input.move != Vector2.zero)
            {
                targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                 mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity,
                    RotationSmoothTime);


                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;
            
            controller.Move(targetDirection.normalized * (speed * Time.deltaTime) +
                             new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                if (verticalVelocity < 0.0f)
                {
                    verticalVelocity = -2f;
                }

                // Jump
                if (input.jump && jumpTimeoutDelta <= 0.0f)
                {
                    verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                }
                
                if (jumpTimeoutDelta >= 0.0f)
                {
                    jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                jumpTimeoutDelta = JumpTimeout;
                input.jump = false;
            }
            
            if (verticalVelocity < terminalVelocity)
            {
                verticalVelocity += Gravity * Time.deltaTime;
            }
        }
    }
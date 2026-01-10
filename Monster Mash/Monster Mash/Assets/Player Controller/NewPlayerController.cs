using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NewPlayerController : MonoBehaviour
{

    // Components
    public monsterAttackSystem myMonster;
    public playerAudioManager myAudioSystem;
    public CapsuleCollider2D bodyCollider;
    public CircleCollider2D smallBodyCollider;
    public BoxCollider2D groundFrictionCollider;
    public Transform groundCheck;
    public Transform headCheck;
    public Rigidbody2D myRigidbody;

    // Input
    public PlayerInputHandler inputHandler = new();
    public int playerIndex;

    // State Machine
    private PlayerState currentState;

    // MISC
    public bool monsterControllerActive = false;
    public bool isDamageLaunching;
    public float gravityPower;
    public bool facingRight;
    public bool landDetectionReady = true;
    public bool atPlatformEdge = false;
    public bool grounded = false;
    public bool isPhasingThroughPlatform;
    private Vector2 platformEdgeCooridinates;
    public Collider2D currentPlatformCollider;

    public bool insideFloor = false;
    public bool isDashing = false;
    public bool isWalking = false;
    public bool isRunning = false;
    public bool canMove = true;
    public bool canJump = true;
    public bool isRolling = false;
    public bool chargingForward = false;

    // Attacks
    public bool isAttacking = false;
    public bool leftStickIsAttacking = false;
    float lastAttackTime = -Mathf.Infinity;

    // Platform
    public LayerMask solidGroundLayer;
    public LayerMask semiSolidGroundLayer;

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
        gravityPower = myRigidbody.gravityScale;
        inputHandler = GetComponent<PlayerInputHandler>();
        myAudioSystem = GetComponentInChildren<playerAudioManager>();
    }

    private void Start()
    {
        ChangeState(new IdleState(this));
        allParts = GetAllPartsInRoot();
        legs = allParts.Where(part => part.PartType == MonsterPartType.Leg).ToList();
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

    public IEnumerator DisableJumping(float seconds)
    {
        lockPlayerController();
        yield return new WaitForSeconds(seconds);
        unlockPlayerController();
    }

    private void startTeeterVisual()
    {
        myMonster.enteredPlatformEdge();
    }

    private void stopTeeterVisual()
    {
        myMonster.exitedPlatformEdge();
    }


    public bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.5f, solidGroundLayer);
    }

    public bool isSemiGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.5f, semiSolidGroundLayer);
    }

    public bool wallToFloorCheck()
    {
        return Physics2D.Raycast(transform.position, -transform.up, 2f, solidGroundLayer);
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


    // Listens for when an attack calls Trigger Attack Release
    public void ApplyMovementModifier(object sender, TriggerAttackReleaseEventArgs eventArgs)
    {
        leftStickIsAttacking = true;

        Vector2 currentMovementModifier = Vector2.zero;
        switch (inputHandler.lastInputDirection)
        {
            case PlayerInputHandler.InputDirection.Forward:
                currentMovementModifier = eventArgs.MovementModifier.Right;
                break;
            case PlayerInputHandler.InputDirection.Backward:
                currentMovementModifier = eventArgs.MovementModifier.Left;
                break;
            case PlayerInputHandler.InputDirection.Up:
                currentMovementModifier = eventArgs.MovementModifier.Up;
                break;
            case PlayerInputHandler.InputDirection.Down:
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

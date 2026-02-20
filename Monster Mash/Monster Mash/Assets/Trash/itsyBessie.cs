using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itsyBessie : MonoBehaviour
{
    #region Old Code
    /*
    #region Old Code
    
    public List<GameObject> players;
    private GameObject playerTarget;
    private Vector2 playerPosition;
    
    public Animator myRotationAnimator;
    public Animator myVisualAnimator;
    //public Rigidbody2D myRigidbody;
    //private bool facingLeft = false;
    //private bool facingRight = false;
    //private float directionModifier = 1;
    //private float runSpeed = 20;
    //private float walkSpeed = 5;
    //private float stoppingDistance = 3;

    //difficulty drop down, right now its just bools
    public bool simpleMode;
    public bool advancedMode;
    public bool complexMode;

    //positioning restraints
    public Transform xMin;
    public Transform xMax;
    public Transform yMin;
    public Transform yMax;

    //detection areas
    public List<GameObject> playersInImmediateArea;

    //Phase 1
    private string[] phase1_AttackCards = new string[6] {"Snip", "Stab", "Spin", "Charge", "Spear", "Tackle" };     //private array of strings containing all attack cards aka our deck of cards to pull from
    private int[] phase1AttackCardAttempts = new int[6] { 3, 2, 1, 2, 1, 1}; //number of times for each card boss will try the attack
    private float[] phase1AttackReactionTime = new float[6] {0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f}; //timer for how long attacks should be spaced out at minimum in each attack card
    private float phase1_IdleDuration = 2f; //timer for how long idle periods should last before a new attack card is pulled


    private string currentAttackCardName;
    private int currentAttackCardInt;
    private int attackAttempts = 0;
    private bool readyForNewAttackCard; //bool saying we're ready for a new attack card
    private bool readyForAttack; //bool saying we're ready for another attack within the same attack card
    
    #endregion

    //Self and Target Stats
    int currentState = 0;
    public GameObject playerTarget;
    public List<GameObject> playersInImmediateArea;
    private Rigidbody2D myRigidbody;
    private float chaseStopDistance = 5f;
    private int directionModifier = 1;
    private float runSpeed = 10f;
    private float walkSpeed = 5f;

    //visuals and animation
    private Animator rotationAnimator;
    public Animator visualsAnimator;
    private Vector2 playerPosition;
    private bool facingRight = false;
    private bool facingLeft = false;

    //positioning and constraints
    public float stoppingDistance = 0.5f;
    private Transform travelGoal;
    public Transform minXPosition;
    public Transform maxXPosition;
    public Transform minYPosition;
    public Transform maxYPosition;

    //basic attack info
    [SerializeField]
    private int attackPhase = 1;
    [SerializeField]
    private int attackAttemptsLeft = 0;
    private float idleDuration;
    [SerializeField]
    private string currentAttackCardName;
    private int currentAttackCardInt;
    public bool simpleMode;
    public bool advancedMode;
    public bool complexMode;

    //Phase 1
    private string[] phase1_AttackCards = new string[6] { "Snip", "Stab", "Spin", "Charge", "Spear", "Tackle" };     //private array of strings containing all attack cards aka our deck of cards to pull from
    private string[] phase1_AttackBehavior = new string[6] { "Close", "Close", "Close", "Side", "Far", "Side"}; //Decides whether to get Close, Far, Stay, or to a Side
    private int[] phase1AttackCardAttempts = new int[6] { 3, 2, 1, 2, 1, 1 }; //number of times for each card boss will try the attack
    private float[] phase1AttackReactionTime = new float[6] { 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f }; //timer for how long attacks should be spaced out at minimum in each attack card
    private int[] phase1_SimpleModeOrder = new int[6] {0, 3, 1, 4, 2, 5}; //order for teaching the new attacks
    private int phase1_SimpleModeCounter = -1;
    private float phase1_IdleDuration = 2f; //timer for how long idle periods should last before a new attack card is pulled


    private void Awake()
    {
        myRigidbody = this.gameObject.GetComponent<Rigidbody2D>();
        rotationAnimator = this.gameObject.GetComponent<Animator>();
    }

    public void startBossFight()
    {
        idle();
    }


    void Update()
    {
        if (playerTarget != null)
        {
            playerPosition = new Vector2(playerTarget.transform.position.x, playerTarget.transform.position.y);
        }

        if (currentState == 0)
        {
            //Idle or Attacking
            myRigidbody.velocity = new Vector2(0, 0);
        }
        else if (currentState == 1)
        { 
            #region Hunt
            if (playersInImmediateArea.Count > 0)
            {
                //aim
                if (playerPosition.x > this.transform.position.x && facingRight == false)
                {
                    //right of me
                    facingRight = true;
                    facingLeft = false;
                    directionModifier = 1;
                    faceRight();
                }
                else if (playerPosition.x < this.transform.position.x && facingLeft == false)
                {
                    //left of me
                    facingRight = false;
                    facingLeft = true;
                    directionModifier = -1;
                    faceLeft();
                }

                //prepare to strike player
                prepStrike();

            }
            else
            {

                if (playerPosition.x > this.transform.position.x && facingRight == false)
                {
                    //right of me
                    facingRight = true;
                    facingLeft = false;
                    directionModifier = 1;
                    turnRight();
                }
                else if (playerPosition.x < this.transform.position.x && facingLeft == false)
                {
                    //left of me
                    facingRight = false;
                    facingLeft = true;
                    directionModifier = -1;
                    turnLeft();
                }

                myRigidbody.velocity = new Vector2(directionModifier * runSpeed, myRigidbody.velocity.y);
            }
            #endregion
        }
        else if (currentState == 2)
        {
            #region Prepare to Strike

            if (playersInImmediateArea.Contains(playerTarget) == false)
            {
                if (playerPosition.x > this.transform.position.x && facingRight == false)
                {
                    //right of me
                    facingRight = true;
                    facingLeft = false;
                    directionModifier = 1;
                    faceRight();
                }
                else if (playerPosition.x < this.transform.position.x && facingLeft == false)
                {
                    //left of me
                    facingRight = false;
                    facingLeft = true;
                    directionModifier = -1;
                    faceLeft();
                }

                myRigidbody.velocity = new Vector2(directionModifier * walkSpeed, myRigidbody.velocity.y);
            }

            #endregion
        }
        else if (currentState == 3)
        {
            //Reposition
            #region Reposition to Screen Side
            float travelDistance = Vector2.Distance(travelGoal.position, this.transform.position);

            if (travelDistance < stoppingDistance)
            {
                startPositionReached();
            }
            else
            {
                if (travelGoal.position.x > this.transform.position.x && facingRight == false)
                {
                    //right of me
                    facingRight = true;
                    facingLeft = false;
                    directionModifier = 1;
                    turnRight();
                }
                else if (travelGoal.position.x < this.transform.position.x && facingLeft == false)
                {
                    //left of me
                    facingRight = false;
                    facingLeft = true;
                    directionModifier = -1;
                    turnLeft();
                }

                myRigidbody.velocity = new Vector2(directionModifier * runSpeed, myRigidbody.velocity.y);
            }
            #endregion
        }
        else if (currentState == 4)
        {
            //charge
            #region Charge to Position

            #endregion
        }
        else if (currentState == 5)
        {
            //go far
            #region Back Up

            #endregion
        }
    }

    #region Animation and Direction Functions
    private void turnRight() //for facing a direction while walking
    {
        rotationAnimator.SetTrigger("Turn Right");
        //some sorta jump or step animation for the visuals
    }

    private void turnLeft() //for facing a direction while walking
    {
        rotationAnimator.SetTrigger("Turn Left");
        //some sorta jump or step animation for the visuals
    }

    private void faceRight() //for looking at players during attacks, more like a 30 degree turn than a full side profile
    {
        rotationAnimator.SetTrigger("Face Right");
        //some sorta jump or step animation for the visuals
    }

    private void faceLeft() //for looking at players during attacks, more like a 30 degree turn than a full side profile
    {
        rotationAnimator.SetTrigger("Face Left");
        //some sorta jump or step animation for the visuals
    }
    #endregion

    #region State 0: Idle
    private void idle()
    {
        currentState = 0;
        facingLeft = false;
        facingRight = false;
        rotationAnimator.SetTrigger("Idle");
        //some sort of idle trigger for visuals
        drawNewAttackCard();
        StartCoroutine(idleDurationTimer());
    }

    private void drawNewAttackCard()
    {
        if (simpleMode)
        {
            //attacks follow a specific order and sequence\

            #region Phase 1

            if (attackPhase == 1)
            {
                if (phase1_SimpleModeCounter == phase1_SimpleModeOrder.Length - 1)
                {
                    advancedMode = true;
                    simpleMode = false;
                }
                else
                {
                    phase1_SimpleModeCounter++;
                    int nextSimpleModeAttack = phase1_SimpleModeOrder[phase1_SimpleModeCounter];
                    currentAttackCardName = phase1_AttackCards[nextSimpleModeAttack];
                    attackAttemptsLeft = phase1AttackCardAttempts[nextSimpleModeAttack];
                    print("new attack card pulled");
                }
            }
            #endregion
        }
        else if (advancedMode)
        {
            //attacks are random but once pulled, the actions are predictable enough to avoid

            #region Phase 1

            if(attackPhase == 1)
            {
                int randomCard = Random.Range(0, phase1_AttackCards.Length);

                if (randomCard != currentAttackCardInt)
                {
                    currentAttackCardInt = randomCard;
                    currentAttackCardName = phase1_AttackCards[currentAttackCardInt];
                    attackAttemptsLeft = phase1AttackCardAttempts[currentAttackCardInt];
                    print("new attack card pulled");
                }
                else
                {
                    drawNewAttackCard();
                    return;
                }
            }

            #endregion
        }
        else if (complexMode)
        {
            //draws from a whole hand that has been optimized for success, 1 attack attempt each so no predictable pattern

            #region Phase 1

            #endregion
        }
    }

    IEnumerator idleDurationTimer()
    {
        yield return new WaitForSeconds(2);
        engageAttackPlan();
    }

    #endregion

    #region State 0: Attack

    private void engageAttackPlan()
    {
        //look at the attack card I've drawn and decide whether I will Hunt, Aim, Prep Attack, or Relocate
        if (phase1_AttackBehavior[currentAttackCardInt] == "Close")
        {
            //find closest target
            currentState = 1; //Hunt
        }
        else if (phase1_AttackBehavior[currentAttackCardInt] == "Far")
        {
            
        }
        else if(phase1_AttackBehavior[currentAttackCardInt] == "Side")
        {
            //find which side is closes to me 
            float leftDistance = Vector2.Distance(minXPosition.position, this.transform.position);
            float rightDistance = Vector2.Distance(maxXPosition.position, this.transform.position);

            if (leftDistance > rightDistance)
            {
                travelGoal = maxXPosition;
            }
            else
            {
                travelGoal = minXPosition;
            }

            currentState = 3;
        }
        else if (phase1_AttackBehavior[currentAttackCardInt] == "Stay")
        {

        }
    }


    private void attack()
    {
        currentState = 0;
        attackAttemptsLeft--;
        print(currentAttackCardName);

        if (attackAttemptsLeft == 0)
        {
            //if it does go idle and set idle timer for new attack card
            StartCoroutine(attackBufferBeforeIdle());
        }
        else
        {
            //go back to aiming
            StartCoroutine(attackBufferBeforeNextAttack());
        }
    }

    IEnumerator attackBufferBeforeIdle()
    {
        yield return new WaitForSeconds(1);
        idle();
        print("Idle");
    }

    IEnumerator attackBufferBeforeNextAttack()
    {
        yield return new WaitForSeconds(2);
        engageAttackPlan();
    }

    #endregion

    #region State 1: Hunt

    #endregion

    #region State 2: Prepare Strike
    private void prepStrike()
    {
        int newTarget = Random.Range(0, playersInImmediateArea.Count);
        playerTarget = playersInImmediateArea[newTarget];
        currentState = 2;
        //trigger whatever aim visual animation for the current attack card
        StartCoroutine(strikeReaction());
    }

    IEnumerator strikeReaction()
    {
        yield return new WaitForSeconds(0.5f);//replace with unique reaction time for each attack card
        attack();
    }

    #endregion

    #region State 3: Reposition

    private void startPositionReached()
    {
        currentState = 0;

        if (phase1_AttackBehavior[currentAttackCardInt] == "Side")
        {
            //turn around
            if (facingRight)
            {
                facingRight = false;
                facingLeft = true;
                directionModifier = -1;
                turnLeft();
            }
            if (facingLeft)
            {
                facingRight = true;
                facingLeft = false;
                directionModifier = 1;
                turnRight();
            }

            //attack

            StartCoroutine(prepCharge());
        }
    }

    IEnumerator prepCharge()
    {
        yield return new WaitForSeconds(3);
        attack();
    }

    #endregion

    #region State 4: Charge

    private void endPositionReached()
    {
        currentState = 0;
    }

    #endregion

    #region Player Detection and Player Fetching

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (playersInImmediateArea.Contains(collision.gameObject) == false)
            {
                playersInImmediateArea.Add(collision.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (playersInImmediateArea.Contains(collision.gameObject))
            {
                playersInImmediateArea.Remove(collision.gameObject);
            }
        }
    }
    #endregion

    #region Old Code
    /*
    public void engagePhase1()
    {
        if (simpleMode)
        {
            currentAttackCardInt = 0;
            currentAttackCardName = phase1_AttackCards[currentAttackCardInt];
        }
    }

    public void attack()
    {
        attackAttempts++;
        //play some sort of attack animation
        if (attackAttempts == phase1AttackCardAttempts[currentAttackCardInt])
        {

        }
    }



    IEnumerator phase1Idle()
    {
        shuffleNewPhase1AttackCard();
        yield return new WaitForSeconds(phase1_IdleDuration);
        //engage

    }

    private void shuffleNewPhase1AttackCard()
    {
        if (simpleMode)
        {
            if (currentAttackCardInt == phase1_AttackCards.Length - 1)
            {
                advancedMode = true;
                simpleMode = false;
            }
            else
            {
                currentAttackCardInt++;
                currentAttackCardName = phase1_AttackCards[currentAttackCardInt];
                attackAttempts = 0;
            }
        }

        if (advancedMode)
        {

            int randomCard = Random.Range(0, phase1_AttackCards.Length);

            if (randomCard != currentAttackCardInt)
            {
                currentAttackCardInt = randomCard;
                currentAttackCardName = phase1_AttackCards[currentAttackCardInt];
                attackAttempts = 0;
            }
            else
            {
                shuffleNewPhase1AttackCard();
                return;
            }
        }
    }

    public void engagePhase2()
    {

    }

    public void engagePhase3()
    {

    }

    public void engageFrenzy()
    {

    }

    private void findCloseRangeTarget()
    {
        if (players.Count > 1)
        {
            GameObject nextTarget = players[0];
            float recordDistance = 1000f;

            for (int i = 0; i < players.Count; i++)
            {
                float playerDistanceToBoss = Vector3.Distance(transform.position, players[i].transform.position);

                if (playerDistanceToBoss < recordDistance)
                {
                    nextTarget = players[i];
                    recordDistance = playerDistanceToBoss;
                }
            }

            playerTarget = nextTarget;
        }
        else
        {
            playerTarget = players[0];
        }
    }

    private void findLongRangeTarget()
    {
        if (players.Count > 1)
        {
            GameObject nextTarget = players[0];
            float recordDistance = 0f;

            for (int i = 0; i < players.Count; i++)
            {
                float playerDistanceToBoss = Vector3.Distance(transform.position, players[i].transform.position);

                if (playerDistanceToBoss > recordDistance)
                {
                    nextTarget = players[i];
                    recordDistance = playerDistanceToBoss;
                }
            }

            playerTarget = nextTarget;
        }
        else
        {
            playerTarget = players[0];
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (playersInImmediateArea.Contains(collision.gameObject) == false)
            {
                playersInImmediateArea.Add(collision.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (playersInImmediateArea.Contains(collision.gameObject))
            {
                playersInImmediateArea.Remove(collision.gameObject);
            }
        }
    }

    

    #endregion

    #region Explanation
    //AI difficulty - simple, advanced, complex
    //simple - built for the first boss introduction, more about storytelling and showing off the ropes
    //advanced - no teaching, just straight into business
    //complex - geared to specifically trip up the player, experiment with combos, prioritize winning combos, and to exploit player behaviors
    //for this complex difficulty, there will be a separate reader who's whole job is making guesses about what the player behaviors we can take advantage of
    //player seeks distance, player seeks close range, player jumps over boss a lot, player uses movement mechanics to try to trip up boss, etc.

    //design the bosses to work well with anywhere between 1-4 players

    //Bessie acts like a bull, devastating charging attacks and a mean close hook
    //She suffers most to aerial combat and long range weaponry

    //phase 1
    //snip close range
    //stab close range
    //pruning twirl close range
    //pruning charge long range
    //pruning spear long range
    //prune tackle long range
    //simple mode order: snip, pruning charge, stab close range, spear long range, pruning twirl close range, prune tackle long range, repeat

    //Phase 2
    //trowel Slam close range
    //trowel multi stab close range
    //trowel twirl close range
    //trowel charge long range
    //trowel dirt catapult long range
    //trowel tackle long range
    //always uses some corm of context mode, so there's no distinct order

    //Phase 3 
    //watering can slam close range
    //watering can multi slam close range 
    //watering can drag close range
    //watering can charge long range
    //watering can rain turret long range
    //watering can distratcion tackle long range

    //Frenzy
    //Carries out same action until animation is complete 
    #endregion

    */

    #endregion
    private Rigidbody2D myRigidbody;
    private Animator myRotationAnimator;
    public Animator myVisualAnimator;
    private bool facingRight;
    private bool facingLeft;
    private int directionModifier = 1;
    public float runSpeed;
    public float spinSpeed;
    public float stopDistance;
    public float greyDistance;
    private bool greyDistanceActivated;
    private bool isAttacking;
    private bool allowedToMove;
    public Transform boundingBox_minX;
    public Transform boundingBox_maxX;
    public Transform boundingBox_minY;
    public Transform boundingBox_maxY;

    public GameObject playerTarget;
    public GameObject[] allPlayersInGame;
    public List<GameObject> playersInTriggerArea;
    private Vector2 playerPosition;
    private float playerDistance;
    private int bossBehavior = 0;

    public GameObject smackTrigger;
    public GameObject stabTrigger;
    public GameObject spinTrigger;
    [SerializeField]
    private int attackAttempts = 3;


    private void Awake()
    {
        myRotationAnimator = this.gameObject.GetComponent<Animator>();
        myRigidbody = this.gameObject.GetComponent<Rigidbody2D>();
        allPlayersInGame = GameObject.FindGameObjectsWithTag("Player");
    }

    private void Update()
    {
        if (playerTarget != null)
        {
            playerPosition = new Vector2(playerTarget.transform.position.x, this.transform.position.y);
            playerDistance = Vector2.Distance(this.transform.position, playerPosition);
        }
        else
        {
            fetchRandomPlayer();
            playerPosition = new Vector2(playerTarget.transform.position.x, this.transform.position.y);
            playerDistance = Vector2.Distance(this.transform.position, playerPosition);
        }

        if(bossBehavior == 1)
        {
            if (allowedToMove)
            {
                fetchRandomTargetInArea();

                if (playerDistance > 1 || playerDistance == 1)
                {
                    float adjustedMoveSpeed = runSpeed - (runSpeed / playerDistance);
                    float adjustedAnimationSpeed = adjustedMoveSpeed / runSpeed;


                    if (playerPosition.x > this.transform.position.x && directionModifier != 1)
                    {
                        //right of me
                        directionModifier = 1;
                    }
                    else if (playerPosition.x < this.transform.position.x && directionModifier != -1)
                    {
                        //left of me
                        directionModifier = -1;
                    }

                    myRigidbody.velocity = new Vector2(directionModifier * adjustedMoveSpeed, myRigidbody.velocity.y);
                    //myVisualAnimator.SetFloat("Run Speed Modifier", adjustedAnimationSpeed);
                }
            }
        }

        if (bossBehavior == 2)
        {
            if (allowedToMove)
            {
                fetchRandomTargetInArea();

                if (playerDistance > stopDistance && greyDistanceActivated == false)
                {
                    if (playerPosition.x > this.transform.position.x && directionModifier != 1)
                    {
                        //right of me
                        directionModifier = 1;
                        moveRight();
                    }
                    else if (playerPosition.x < this.transform.position.x && directionModifier != -1)
                    {
                        //left of me
                        directionModifier = -1;
                        moveLeft();
                    }

                    myRigidbody.velocity = new Vector2(directionModifier * runSpeed, myRigidbody.velocity.y);
                }
                else
                {
                    if (directionModifier != 0)
                    {
                        directionModifier = 0;
                        myRigidbody.velocity = new Vector2(0, 0);
                        greyDistanceActivated = true;
                        idle(2);
                    }

                    if (greyDistanceActivated && playerDistance > greyDistance)
                    {
                        greyDistanceActivated = false;
                    }
                }
            }

        }

        if (bossBehavior == 3)
        {
            if (allowedToMove)
            {
                fetchRandomTargetInArea();

                if (playerDistance > stopDistance && greyDistanceActivated == false)
                {
                    if (playerPosition.x > this.transform.position.x && directionModifier != 1)
                    {
                        //right of me
                        directionModifier = 1;
                        moveRight();
                    }
                    else if (playerPosition.x < this.transform.position.x && directionModifier != -1)
                    {
                        //left of me
                        directionModifier = -1;
                        moveLeft();
                    }

                    myRigidbody.velocity = new Vector2(directionModifier * runSpeed, myRigidbody.velocity.y);
                }
                else
                {
                    if (directionModifier != 0)
                    {
                        directionModifier = 0;
                        myRigidbody.velocity = new Vector2(0, 0);
                        greyDistanceActivated = true;
                        idle(3);
                    }

                    if (greyDistanceActivated && playerDistance > greyDistance)
                    {
                        greyDistanceActivated = false;
                    }
                }
            }

        }

        if (bossBehavior == 30)
        {
            myRigidbody.velocity = new Vector2(directionModifier * spinSpeed, myRigidbody.velocity.y);
        }

        if (bossBehavior == 8) //this behavior keeps Bessie close to the player at all times
        {
            if (playerDistance > stopDistance && greyDistanceActivated == false)
            {
                if (playerPosition.x > this.transform.position.x && directionModifier != 1)
                {
                    //right of me
                    directionModifier = 1;
                    moveRight();
                }
                else if (playerPosition.x < this.transform.position.x && directionModifier != -1)
                {
                    //left of me
                    directionModifier = -1;
                    moveLeft();
                }

                myRigidbody.velocity = new Vector2(directionModifier * runSpeed, myRigidbody.velocity.y);
            }
            else
            {
                if (directionModifier != 0)
                {
                    directionModifier = 0;
                    myRigidbody.velocity = new Vector2(0, 0);
                    greyDistanceActivated = true;
                    idle(0);
                }

                if (greyDistanceActivated && playerDistance > greyDistance)
                {
                    greyDistanceActivated = false;
                }
            }
        }
    }

    public void charge()
    {

    }

    public void spear()
    {

    }

    public void tackle()
    {
        //find a target
        //figure out our framing and where is the most optimal place to tackle from
        //tell update to travel to a given point
        //tell visual animator to trigger spin animation
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (playersInTriggerArea.Contains(collision.gameObject) == false)
            {
                playersInTriggerArea.Add(collision.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (playersInTriggerArea.Contains(collision.gameObject))
            {
                playersInTriggerArea.Remove(collision.gameObject);
            }
        }
    }

    private void fetchRandomTargetInArea()
    {
        if (isAttacking == false && playersInTriggerArea.Count > 0)
        {
            isAttacking = true;
            int chosenTarget = Random.Range(0, playersInTriggerArea.Count);
            playerTarget = playersInTriggerArea[chosenTarget];

            if (bossBehavior == 1)
            {
                if (playerTarget.transform.position.y > boundingBox_maxY.position.y)
                {
                    StartCoroutine(airSmackAttackDelay());
                }
                else
                {
                    StartCoroutine(smackAttackDelay());
                }
            }
            else if (bossBehavior == 2)
            {
                allowedToMove = false;
                directionModifier = 0;
                myRigidbody.velocity = new Vector2(0, 0);
                greyDistanceActivated = true;
                idle(3);
                myVisualAnimator.SetTrigger("Stab");
                StartCoroutine(stabAttackDelay());
            }
            else if (bossBehavior == 3)
            {
                allowedToMove = false;
                directionModifier = 0;
                myRigidbody.velocity = new Vector2(0, 0);
                greyDistanceActivated = true;
                idle(3);
                myVisualAnimator.SetTrigger("Spin");
                StartCoroutine(spinAttackDelay());
            }
        }
    }

    private void fetchRandomPlayer()
    {
        int chosenPlayer = Random.Range(0, allPlayersInGame.Length);

        if (allPlayersInGame[chosenPlayer] != null)
        {
            float playerHeightFromGround = allPlayersInGame[chosenPlayer].transform.position.y;

            if (bossBehavior == 3 && playerHeightFromGround > boundingBox_maxY.position.y)
            {
                fetchRandomPlayer();
                return;
            }
            else
            {
                playerTarget = allPlayersInGame[chosenPlayer];
            }
        }
        else
        {
            fetchRandomPlayer();
        }
    }

    #region Smack Attack
    public void smack()
    {
        //find a target
        isAttacking = false;
        bossBehavior = 1;
        attackAttempts = 3;
        myVisualAnimator.SetTrigger("Smack");
        smackTrigger.SetActive(true);
        StartCoroutine(smackWindUpDelay());
    }

    IEnumerator smackWindUpDelay()
    {
        yield return new WaitForSeconds(1f);//allow this to be a unique number or have a function passed upstream somehow
        allowedToMove = true;
    }
    IEnumerator smackAttackDelay()
    {
        yield return new WaitForSeconds(0.2f);//replace with a unique attack delay
        releaseSmackAttack();
    }

    IEnumerator airSmackAttackDelay()
    {
        yield return new WaitForSeconds(0.2f);//replace with a unique attack delay
        releaseAirSmackAttack();
    }

    private void releaseSmackAttack()
    {
        attackAttempts--;
        allowedToMove = false;
        playerTarget = null;
        myRigidbody.velocity = new Vector2(0, 0);
        myVisualAnimator.SetTrigger("Attack");
        StartCoroutine(smackAttackCooldown());
    }

    private void releaseAirSmackAttack()
    {
        attackAttempts--;
        allowedToMove = false;
        playerTarget = null;
        myRigidbody.velocity = new Vector2(0, 0);
        myVisualAnimator.SetTrigger("Air Attack");
        StartCoroutine(smackAttackCooldown());
    }

    IEnumerator smackAttackCooldown()
    {
        yield return new WaitForSeconds(1f); //replace with a unique cooldown

        if (attackAttempts == 0 || attackAttempts < 0)
        {
            idle(0);
        }
        else
        {
            isAttacking = false;
            allowedToMove = true;
        }
    }

    #endregion

    #region Stab Attack
    public void stab()
    {
        allowedToMove = true;
        isAttacking = false;
        bossBehavior = 2;
        attackAttempts = 2;
        stabTrigger.SetActive(true);
    }

    IEnumerator stabAttackDelay()
    {
        yield return new WaitForSeconds(1f);//allow this to be a unique number or have a function passed upstream somehow
        releaseStabAttack();

    }

    private void releaseStabAttack()
    {
        attackAttempts--;
        myVisualAnimator.SetTrigger("Attack");
        StartCoroutine(stabAttackDuration());

        if (playerPosition.x > this.transform.position.x && directionModifier != 1)
        {
            //right of me
            directionModifier = 1;
            faceRight();
        }
        else if (playerPosition.x < this.transform.position.x && directionModifier != -1)
        {
            //left of me
            directionModifier = -1;
            faceLeft();
        }
    }

    IEnumerator stabAttackDuration()
    {
        yield return new WaitForSeconds(2f); //replace with a unique cooldown
        directionModifier = 0;
        myRigidbody.velocity = new Vector2(0, 0);
        myVisualAnimator.SetBool("Ready to Go Idle", true);
        playerTarget = null;
        StartCoroutine(stabAttackCooldown());
    }

    IEnumerator stabAttackCooldown()
    {
        yield return new WaitForSeconds(2f); //replace with a unique cooldown

        if (attackAttempts == 0 || attackAttempts < 0)
        {
            idle(0);
        }
        else
        {
            idle(2);
        }

        yield return new WaitForSeconds(5f);
        isAttacking = false;
        allowedToMove = true;
    }

    #endregion

    #region Spin Attack
    public void spin()
    {
        allowedToMove = true;
        isAttacking = false;
        bossBehavior = 3;
        attackAttempts = 1;
        spinTrigger.SetActive(true);
    }

    IEnumerator spinAttackDelay()
    {
        yield return new WaitForSeconds(2f);//replace with a unique attack delay  
        releaseSpinAttack();
    }

    private void releaseSpinAttack()
    {
        attackAttempts--;
        myVisualAnimator.SetTrigger("Attack");
        StartCoroutine(spinAttackDuration());

        if (playerPosition.x > this.transform.position.x && directionModifier != 1)
        {
            //right of me
            directionModifier = 1;
        }
        else if (playerPosition.x < this.transform.position.x && directionModifier != -1)
        {
            //left of me
            directionModifier = -1;
        }

        bossBehavior = 30;
        /*
        attackAttempts--;
        //bossBehavior = 30;
        //allowedToMove = true;
        */
    }

    IEnumerator spinAttackDuration()
    {
        yield return new WaitForSeconds(2f); //replace with a unique cooldown
        bossBehavior = 0;
        directionModifier = 0;
        myRigidbody.velocity = new Vector2(0, 0);
        myVisualAnimator.SetBool("Ready to Go Idle", true);
        playerTarget = null;
        StartCoroutine(spinAttackCooldown());
    }

    IEnumerator spinAttackCooldown()
    {
        yield return new WaitForSeconds(1f); //replace with a unique cooldown

        if (attackAttempts == 0 || attackAttempts < 0)
        {
            idle(0);
        }
        else
        {
            isAttacking = false;
            allowedToMove = true;
        }
    }
    #endregion

    public void idle(int intendedState)
    {
        if (facingRight || facingLeft)
        {
            myVisualAnimator.SetTrigger("Jump");
        }

        directionModifier = 0;
        myRigidbody.velocity = new Vector2(0, 0);
        facingRight = false;
        facingLeft = false;
        myRotationAnimator.SetTrigger("Idle");
        myVisualAnimator.SetBool("Move Left", false);
        myVisualAnimator.SetBool("Move Right", false);
        myVisualAnimator.SetBool("Facing Left", false);
        myVisualAnimator.SetBool("Facing Right", false);
        myVisualAnimator.SetFloat("Run Speed Modifier", 1);

        //do we want just the visual of the idle or do we want to reset EVERYTHING
        if (intendedState == 0)
        {
            bossBehavior = 0;
            attackAttempts = 0;
            playerTarget = null;
            allowedToMove = false;
            greyDistanceActivated = false;
            smackTrigger.SetActive(false);
            stabTrigger.SetActive(false);
            spinTrigger.SetActive(false);
            myVisualAnimator.SetBool("Ready to Go Idle", true);
        }

        StartCoroutine(cleanFunctions());
    }

    IEnumerator cleanFunctions()
    {
        yield return new WaitForSeconds(1);
        myRotationAnimator.ResetTrigger("Idle");
        myRotationAnimator.ResetTrigger("Turn Right");
        myRotationAnimator.ResetTrigger("Turn Left");
        myVisualAnimator.ResetTrigger("Attack");
        myVisualAnimator.ResetTrigger("Air Attack");
        myVisualAnimator.ResetTrigger("Smack");
        myVisualAnimator.ResetTrigger("Stab");
        myVisualAnimator.ResetTrigger("Spin");
        myVisualAnimator.SetBool("Ready to Go Idle", false);
    }

    public void turnRight()
    {
        if (facingRight == false)
        {
            myVisualAnimator.SetTrigger("Jump");

            facingRight = true;
            facingLeft = false;
            myRotationAnimator.SetTrigger("Turn Right");
            myVisualAnimator.SetBool("Facing Left", false);
            myVisualAnimator.SetBool("Facing Right", true);
        }
    }

    public void faceRight()
    {
        if (facingRight == false)
        {
            facingRight = true;
            facingLeft = false;
            myRotationAnimator.SetTrigger("Face Right");
            myVisualAnimator.SetBool("Facing Left", false);
            myVisualAnimator.SetBool("Facing Right", true);
        }
    }

    public void turnLeft()
    {
        if (facingLeft == false)
        {
            myVisualAnimator.SetTrigger("Jump");

            facingRight = false;
            facingLeft = true;
            myRotationAnimator.SetTrigger("Turn Left");
            myVisualAnimator.SetBool("Facing Left", true);
            myVisualAnimator.SetBool("Facing Right", false);
        }
    }

    public void faceLeft()
    {
        if (facingLeft == false)
        {
            facingRight = false;
            facingLeft = true;
            myRotationAnimator.SetTrigger("Face Left");
            myVisualAnimator.SetBool("Facing Left", true);
            myVisualAnimator.SetBool("Facing Right", false);
        }
    }

    public void moveLeft()
    {
        myVisualAnimator.SetBool("Move Left", true);
        myVisualAnimator.SetBool("Move Right", false);
    }

    public void moveRight()
    {
        myVisualAnimator.SetBool("Move Left", false);
        myVisualAnimator.SetBool("Move Right", true);
    }
}

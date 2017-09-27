using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Combo Vars")]
    public float timeInbetween;
    [Tooltip("The amount added to the combo counter (60 times a second)")]
    public float comboIncrease = .04f;

    [Header("Rapid Slash Vars")]
    public int slashAmount = 5;
    public float rapidSlashCooldown;
    [Tooltip("The amount added to the rapid slash cooldown (60 times a second)")]
    public float rapidSlashIncrease = .04f;
    public float multiSpeedMultiplier = .7f;

    [Header("Dash Vars")]
    public float dashTime = 1.0f;
    public float dashSpeed = 5.0f;
    public float dashCooldown = 0.5f;

    public bool canDash = true;
    public bool canDashAttack = true;
    public bool canTripleAttack = true;
    public bool canSpinAttack = true;

    [Header("Block Vars")]
    public float rotationSpeed = 5;
    public LayerMask enemyMask;

    [Header("Spin Charge Vars")]
    public float timeToCharge = 2;
    public float timeBetweenFlash = 1;
    public Color spinFlashColor;
    public bool spinChargeReady = false;

    [Header("Other Vars")]
    public float moveBackOnHit = 5;
    public bool blocking = false;
    public float speedWhenBurned = 10;
    public GameObject slashFX;

    private PlayerInputs input;
    private PlayerMove playerMove;
    private PlayerInformation playerInformation;
    private Health playerHealth;
    private Animator animator;

    [Header("Weapon Stuff")]
    public GameObject swordBone;
    public GameObject shieldBone;
    public ShieldStats shield;
    public SwordStats sword;

    private int comboAmount;
    private CharacterController characterController;
    private float comboCounter;
    private float rapidSlashCounter;
    private float rapidSlashTimer;

    private bool cancelDash = false;
    private bool comboStarted = false;
    private bool movingForward = false;

    private float spinCounter = 0;
    private float heldDownCounter = 0;

    private Vector3 targetPosition = Vector3.zero;

    private Coroutine spinChargeRoutine;

    void Start()
    {
        playerHealth = GetComponent<Health>();
        animator = GetComponentInChildren<Animator>();
        characterController = GetComponent<CharacterController>();
        playerInformation = GetComponent<PlayerInformation>();
        playerMove = GetComponent <PlayerMove>();

        if (InputManager.Instance)
        {
            input = InputManager.GetPlayerInput(playerInformation.playerIndex);
        }
        else
        {
            input = new PlayerInputs();
            input.AddKeyboardBindings();
            input.AddControllerBindings();
        }
    }

    void OnDisable()
    {
        cancelDash = true;
    }

    void OnEnable()
    {
        cancelDash = false;
    }

    IEnumerator ChargeSpinFlash()
    {
        yield return new WaitForSeconds(.2f);
        while (spinCounter < (timeToCharge * 60))
        {
            if (!animator.GetCurrentAnimatorStateInfo(1).IsTag("SpinCharge"))
                break;
            playerHealth.SetColor(spinFlashColor);
            yield return new WaitForSeconds(timeBetweenFlash);
            playerHealth.PlayerSetOG();
            yield return new WaitForSeconds(timeBetweenFlash);
        }
    }

    void FixedUpdate()
    {
        CountCombo();
        if (animator.GetCurrentAnimatorStateInfo(1).IsTag("SpinCharge"))
        {
            spinCounter++;
        }

        //check if attack button is held
        if (canSpinAttack)
        {
            if (input.BasicAttack.IsPressed)
            {
                //check what state you're in
                if (animator.GetCurrentAnimatorStateInfo(1).IsTag("Attacking") || animator.GetCurrentAnimatorStateInfo(1).IsTag("SecondAttack"))
                {
                    heldDownCounter++;
                    if (heldDownCounter > 30)
                    {
                        animator.SetBool("SpinCharge", true);
                        sword.GetComponent<SwordCollision>().DoChargeParticle();
                        spinChargeRoutine = StartCoroutine(ChargeSpinFlash());
                        StartCoroutine(ChargeSpinFlash());
                    }
                }
            }
        }
    }

    void Update()
    {
        if (spinCounter < (timeToCharge * 60))
        {
            spinChargeReady = false;
        }
        else
        {
            spinChargeReady = true;
        }

        if (comboStarted)
        {
            animator.SetBool("Attacking", true);
            //check if combo has ended
            if (comboCounter > timeInbetween)
            {
                ResetCombo();
            }
        }
        else
        {
            animator.SetBool("Attacking", false);
        }
            
        //check if can actually attack
        //do basic attack
        if (!playerHealth.isDead)
        {
            if (input.BasicAttack.WasReleased)
            {
                if (spinCounter < (timeToCharge * 60))
                {
                    animator.SetBool("SpinCharge", false);
                    spinCounter = 0;
                    if( spinChargeRoutine != null)
                        StopCoroutine(spinChargeRoutine);
                }
                else
                {
                    StopCoroutine(spinChargeRoutine);
                    spinCounter = 0;
                    animator.SetBool("SpinCharge", false);
                    animator.SetTrigger("Spin");
                    playerHealth.InvincibilityForSecs(2);
                }
            }
            if (input.BasicAttack.WasPressed)
            {
                if (canTripleAttack)
                {
                    CheckCombo();
                    //if combo is equal to or greater than 3, do rapid slash
                    if (comboAmount >= slashAmount)
                    {
                        doRapidSlash();
                    }
                }
                //basic slash
                if (comboAmount < slashAmount)
                {
                    //checks if player is in idle to do the first attack
                    if (animator.GetCurrentAnimatorStateInfo(1).IsTag("Idle"))
                    {
                        doSlash();
                    }
                    //if the user tries to attack when the user is already attacking, it'll do the second attack once completed
                    else if (animator.GetCurrentAnimatorStateInfo(1).IsTag("Attacking"))
                    {
                        doSecondSlash();
                    }
                    else if (animator.GetCurrentAnimatorStateInfo(1).IsTag("SecondAttack"))
                    {
                        doSlash();
                    }
                }
            } 
            if (input.DashSlash.WasPressed)
            {
                if (!playerInformation.HasCharmBool("cantDash"))
                {
                    //dash
                    doDash();
                }
            } 
            
            if (input.Block)
            {
                if (!playerInformation.HasCharmBool("cantBlock"))
                {
                    //block
                    if (shield)
                    {
                        if (!animator.GetCurrentAnimatorStateInfo(1).IsTag("Attacking"))
                            DoBlock();
                    }
                }
            }

            if (input.Block.WasReleased)
            {
                //block
                if (shield)
                {
                    StopBlock();
                }
            }

            if (movingForward)
            {
                MoveForward(2);
            }

            if (playerHealth.isBurned)
            {
                MoveForward(speedWhenBurned);
            }

            if (animator.GetCurrentAnimatorStateInfo(1).IsTag("Idle") && !animator.GetCurrentAnimatorStateInfo(0).IsTag("DashAttack") && !animator.GetCurrentAnimatorStateInfo(0).IsTag("Spinning"))
            {
                animator.SetBool("SpinCharge", false);
                DisableSword();
            }
        }
    }

    void AddSwordPlayerComponenets(SwordCollision newSword)
    {
        newSword.playerInfo = playerInformation;
        newSword.playerHealth = playerHealth;
        newSword.playerAttack = this;
        newSword.animator = animator;
    }

    IEnumerator ThrowDroppedSword(Transform newSword)
    {
        newSword.transform.eulerAngles = new Vector3(0, 0, 0);

        newSword.GetComponent<SwordCollision>().trail.SetActive(true);

        //sets the target position above the player
        Vector3 targetPosition = new Vector3(sword.transform.position.x, 6, sword.transform.position.z);

        //adds right to the vector so its not directly ontop of the player
        targetPosition += newSword.transform.right;

        sword = null;

        //moves the sword to the target position with rotations
        while (newSword.transform.position != targetPosition)
        {
            newSword.transform.position = Vector3.MoveTowards(newSword.transform.position, targetPosition, 50 * Time.deltaTime);
            newSword.transform.eulerAngles = Vector3.Lerp(newSword.transform.eulerAngles, new Vector3(0, 0, 0), 10 * Time.deltaTime);
            yield return new WaitForSeconds(.01f);
        }

        //once completed, wait
        yield return new WaitForEndOfFrame();

        //sets the target position to the floor
        targetPosition = new Vector3(newSword.transform.position.x, 2, newSword.transform.position.z);

        //moves the sword to the floor
        while (newSword.transform.position != targetPosition)
        {
            //if not target position, move towards it
            if (newSword.transform.position != targetPosition)
            {
                newSword.transform.position = Vector3.MoveTowards(newSword.transform.position, targetPosition, 50 * Time.deltaTime);
            }
            //if the rotations doesn't equal the target rotation, lerp towards it
            newSword.transform.eulerAngles = Vector3.Lerp(newSword.transform.eulerAngles, new Vector3(0, 0, 180), 50 * Time.deltaTime);
            yield return new WaitForSeconds(.01f);
        }

        newSword.GetComponent<SwordCollision>().trail.SetActive(false);

        newSword.GetComponent<SwordPickup>().canPickUp = true;

    }

    public void DropSword()
    {
        if (sword)
        {
            //sets the swords parent to be nothing
            sword.transform.parent = null;
            StartCoroutine(ThrowDroppedSword(sword.transform));
        }
    }

    public void AddSword(SwordStats newSword)
    {
        if (sword)
        {
            DropSword();
        }
        sword = newSword;
        //sets the swords parent to be the sword bone
        sword.transform.parent = swordBone.transform;
        //resets the position
        sword.transform.localPosition = new Vector3(0, 0, 0);
        //sets the rotation
        sword.transform.localEulerAngles = new Vector3(-117.677f, -48.953f, 25.159f);
        sword.GetComponent<SwordPickup>().canPickUp = false;
        AddSwordPlayerComponenets(sword.GetComponent<SwordCollision>());
    }

    public void EnableSword()
    {
        //enables collider and trail
        sword.GetComponent<BoxCollider>().enabled = true;
        sword.transform.GetChild(0).GetComponent<TrailRenderer>().enabled = true;
    }

    public void DisableSword()
    {
        //disables collider and trail
        sword.GetComponent<BoxCollider>().enabled = false;
        sword.transform.GetChild(0).GetComponent<TrailRenderer>().enabled = false;
    }

    //-------------------------- Combo stuff

    void CheckCombo()
    {
        comboStarted = true;
        //if you attack quick enough, it is counted as a combo
        if (comboCounter < timeInbetween)
        {
            AddCombo();
        } 
		//if you attack to late, it restarts the combo
		else
        {
            ResetCombo();
        }
    }

    void ResetCombo()
    {
        //resets everything to do with combo
        if (comboAmount >= slashAmount)
        {
            animator.SetBool("TripleAttack", false);
            playerInformation.ResetMoveSpeed();
        }
        comboAmount = 0;
        comboCounter = 0;
        comboStarted = false;
    }

    void AddCombo()
    {
        //adds 1 to combo amount
        comboAmount++;
        comboCounter = 0;
    }
		

    //-------------------------- Block

    Transform GetClosestEnemy()
    {
        GameObject closestEnemy = null;
        float maxDistance = float.MaxValue;
        //gets all enemies in a specific radius
        Collider[] enemies = Physics.OverlapSphere(transform.position, 500, enemyMask);
        foreach (Collider enemy in enemies)
        {
            //loops through each enemy and finds which enemy is closest
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < maxDistance)
            {
                if (enemy.name != name)
                {
                    if (enemy.gameObject.layer == 11)
                    {
                        closestEnemy = enemy.gameObject;
                    }
                }
                maxDistance = dist;
            }
        }
        //returns the closest enemy
        if (closestEnemy)
            return closestEnemy.transform;
        return transform;
    }

    void DoBlock()
    {
        //do block things
        playerInformation.SetMoveSpeed(playerInformation.GetOriginalMoveSpeed() * shield.speedDamping * playerInformation.GetCharmFloat("blockSpeedMultiplier"));

        if (GetClosestEnemy() != transform)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(GetClosestEnemy().position - transform.position), rotationSpeed * Time.deltaTime);
        }
		
        if (animator)
        {
            animator.SetBool("Blocking", true);
        }
        blocking = true;

        //Debug.Log ("Blocking");
    }

    void StopBlock()
    {
        //stop block things
        playerInformation.ResetMoveSpeed();
        if (animator)
        {
            animator.SetBool("Blocking", false);
        }
        blocking = false;
    }


    //-------------------------- Attacks

    public float criticalHit()
    {
        if (playerInformation)
        {
            if (playerInformation.HasCharmFloat("critChance"))
            {
                float randomPercent = Random.Range(0, 101);
                if (randomPercent >= playerInformation.GetCharmFloat("critChance"))
                {
                    return playerInformation.GetCharmFloat("critMultiplier");
                }
                else
                {
                    return 1.0f;
                }
            }
            return 1.0f;
        }
        return 1.0f;
    }

    IEnumerator boolWait(string boolName)
    {
        animator.SetBool(boolName, true);
        yield return new WaitForSeconds(.1f);
        animator.SetBool(boolName, false);
    }

    void MoveForward(float speed)
    {
        targetPosition = Vector3.Lerp(targetPosition, transform.forward * speed, playerInformation.maxMoveSpeed * Time.deltaTime);
        characterController.Move(targetPosition * Time.deltaTime);
    }

    IEnumerator waitForSeconds(float seconds, bool movingBool)
    {
        movingBool = true;
        targetPosition = new Vector3(0, 0, 0);
        yield return new WaitForSeconds(seconds);
        movingBool = false;
    }

    void doSlash()
    {
        animator.SetTrigger("Attack");
        //do slash things
    }

    void doSecondSlash()
    {
        StartCoroutine(boolWait("SecondAttack"));
    }

    IEnumerator slashWait(string boolName)
    {
        animator.SetBool(boolName, true);
        yield return animator.GetCurrentAnimatorStateInfo(1);
        yield return new WaitForSeconds(.1f);
        animator.SetBool(boolName, false);
    }

    void doRapidSlash()
    {
        //do rapid slash things
        animator.SetBool("TripleAttack", true);
        if (animator.GetCurrentAnimatorStateInfo(1).IsTag("RapidAttack"))
        {
            playerInformation.SetMoveSpeed(playerInformation.GetOriginalMoveSpeed() * multiSpeedMultiplier);
        }
    }

    void doDash()
    {
        if (canDash)
        {
            if (canDashAttack)
            {
                animator.SetTrigger("DashAttack");
                playerHealth.InvincibilityForSecs(dashTime + 1);
            }
            else
            {
                animator.SetTrigger("Dash");
            }
            canDash = false;
            StartCoroutine(dash());
            StartCoroutine(dashCooldownTimer());
        }
    }

    IEnumerator dashCooldownTimer()
    {
        int i = 0;
        float cooldown = dashCooldown * playerInformation.GetCharmFloat("dashCooldown");

        while (i < cooldown)
        {
            yield return new WaitForSeconds(1);
            i++;
        }
        canDash = true;
    }

    IEnumerator dash()
    {
        playerMove.enabled = false;
        Vector3 startingPos = transform.position;
        float elapsedTime = 0;
        float timer = dashTime * playerInformation.GetCharmFloat("dashTime");
        while (elapsedTime < timer)
        {
            if (cancelDash)
                break;

            characterController.Move(transform.forward * dashSpeed * playerInformation.GetCharmFloat("dashSpeed") * Time.deltaTime);
            yield return new WaitForEndOfFrame();
            elapsedTime += Time.deltaTime;
        }
        playerMove.enabled = true;
    }


    //-------------------------- Counters

    void CountCombo()
    {
        comboCounter += comboIncrease;
    }

    void RapidSlashCooldownCounter()
    {
        rapidSlashCounter += rapidSlashIncrease;
    }

}

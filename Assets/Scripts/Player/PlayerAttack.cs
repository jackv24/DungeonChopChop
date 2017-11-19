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
    public float rapidDmgMultiplier = 1;
    public int slashAmount = 5;
    [Tooltip("The amount added to the rapid slash cooldown (60 times a second)")]
    public float rapidSlashCooldown;
    public float rapidSlashIncrease = .04f;
    public float multiSpeedMultiplier = .7f;
    public float rapidJuicePerFrame = .1f;

    [Header("Dash Vars")]
    public float dashAtkDmgMultiplier = 2;
    public float dashTime = 1.0f;
    public float dashSpeed = 5.0f;
    public float dashCooldown = 0.5f;
    public float juicePerDash;

    [Header("Block Vars")]
    public bool autoBlock = false;
    public float rotationSpeed = 5;
    public LayerMask enemyMask;

    [Header("Spin Vars")]
    public float spinDmgMultiplier = 2;
    public bool spinChargeReady = false;
    public float timeToCharge = 2;
    public float flashDuration = 1;
    [Tooltip("0 == no white, 1 == fully white")]
    public float whiteVal;
    [Tooltip("The amount it fades back from the flash amount per milisecond")]
    public float amountOfFadeBack = .01f;
    public float juicePerSpin;

    [Header("Projectile Vars")]
    public GameObject projectile;
    [Tooltip("The position of which the projectile comes out from")]
    public GameObject shootPosition;
    public float thrust;

    [Header("Other Vars")]
    public float moveBackOnHit = 5;
    public bool blocking = false;
    public float speedWhenBurned = 10;
    public GameObject slashFX;

    [Header("Dungeon Abilities")]
    public bool canDash = true;
    public bool canDashAttack = true;
    public bool canTripleAttack = true;
    public bool canSpinAttack = true;

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

    [Space()]
    [Header("Sounds")]
    public SoundEffect firstSlashSounds;
    public SoundEffect secondSlashSounds;
    public SoundEffect rapidSounds;
    public SoundEffect dashSounds;
    public SoundEffect dashAttackSounds;
    public SoundEffect startBlockSounds;
    public SoundEffect stopBlockSounds;
    public SoundEffect spinSounds;
    public SoundEffect chargeSpinSounds;
    public SoundEffect chargeReadySounds;
    [Tooltip("Sound when getting hit whilst blocking")]
    public SoundEffect hitBlockSound;

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
    private GameObject currentProj;

    void Start()
    {
        playerHealth = GetComponent<Health>();
        animator = GetComponentInChildren<Animator>();
        characterController = GetComponent<CharacterController>();
        playerInformation = GetComponent<PlayerInformation>();
        playerMove = GetComponent <PlayerMove>();

        input = new PlayerInputs();

        if (playerInformation.playerIndex == 0)
        {
            input.AddKeyboardBindings();
            if (GameManager.Instance.players.Count > 1)
            {
                if (InControl.InputManager.Devices.Count > 1)
                {
                    input.AssignDevice(InControl.InputManager.Devices[0]);
                    input.SetupBindings();
                }
            } 
            else 
            {
                if (InControl.InputManager.Devices.Count == 1)
                {
                    input.AssignDevice(InControl.InputManager.Devices[0]);
                    input.SetupBindings();
                }
                else
                {
                    input.AssignDevice(InControl.InputManager.Devices[0]);
                    input.SetupBindings();
                }
            }
        } 
        else if (playerInformation.playerIndex == 1)
        {
            if (InControl.InputManager.Devices.Count <= 1)
            {
                input.AssignDevice(InControl.InputManager.Devices[0]);
                input.AddControllerBindings();
            } 
            else 
            {
                input.AssignDevice(InControl.InputManager.Devices[1]);
                input.SetupBindings();
            }
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

    void ResetSpin()
    {
        spinCounter = 0;
        animator.SetBool("SpinCharge", false);
        spinChargeReady = false;
        sword.GetComponent<SwordCollision>().chargeCoroutine = null;
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
                //make sure not in rapid attack state
                if (!animator.GetCurrentAnimatorStateInfo(1).IsTag("RapidAttack"))
                {
                    heldDownCounter++;
                    //make sure not in spinning state
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Spinning"))
                    {
                        if (comboAmount <= 0)
                        {
                            if (heldDownCounter > 30)
                            {
                                if (playerInformation.currentCureAmount >= juicePerSpin)
                                {
                                    animator.SetBool("SpinCharge", true);
                                    //make sure in spin charge state
                                    if (animator.GetCurrentAnimatorStateInfo(1).IsTag("SpinCharge"))
                                    {
                                        //do spin charge stuff
                                        SoundManager.PlaySound(chargeSpinSounds, transform.position);
                                        sword.GetComponent<SwordCollision>().DoChargeParticle();
                                        heldDownCounter = 0;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        if (animator.GetCurrentAnimatorStateInfo(1).IsName("GetAttacked"))
        {
            ResetSpin();
        }
    }

    void Update()
    {
        //make sure game is not paused
        if (Time.timeScale > 0)
        {
            timeToCharge = playerInformation.GetCharmFloat("spinChargeTime");

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
                //check if holding down attack
                if (input.BasicAttack.WasReleased)
                {
                    //make sure not already spinning
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Spinning"))
                    {
                        //if the amount of time you're holding down attack is less then the charge time, cancel it
                        if (spinCounter < (timeToCharge * 60))
                        {
                            animator.SetBool("SpinCharge", false);
                            spinCounter = 0;
                        }
                    //otherwise do spin
                    else
                        {
                            spinCounter = 0;
                            animator.SetBool("SpinCharge", false);
                            //play sound
                            SoundManager.PlaySound(spinSounds, transform.position);
                            //do spin
                            animator.SetTrigger("Spin");
                            //set invincibility
                            playerHealth.InvincibilityForSecs(2);

                            playerInformation.currentCureAmount -= juicePerDash;
                        }
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
                            if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Spinning"))
                                doSlash();
                            else
                            {
                                //check the current frame of animation
                                int currentFrame = ((int)(animator.GetCurrentAnimatorStateInfo(0).normalizedTime * (17))) % 17;
                                if (currentFrame > 10)
                                    doSlash();
                            }
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
                        if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Spinning") && !animator.GetCurrentAnimatorStateInfo(1).IsTag("SpinCharge"))
                        {
                            //dash
                            doDash();
                        }
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
            }

            if (movingForward)
            {
                MoveForward(2);
            }

            if (playerHealth.isBurned)
            {
                MoveForward(speedWhenBurned);
            }

            if (animator.GetCurrentAnimatorStateInfo(1).IsTag("Idle") && !animator.GetCurrentAnimatorStateInfo(0).IsTag("Spinning") && !animator.GetCurrentAnimatorStateInfo(0).IsTag("DashAttack"))
            {
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

    void Shoot()
    {
        //create the projecticle
        GameObject proj = ObjectPooler.GetPooledObject(projectile);

        currentProj = proj;

        //sets the projectiles position to be the point of shoot position
        proj.transform.position = shootPosition.transform.position;

        proj.transform.rotation = transform.rotation;

        //sets the strength of the projectile
        proj.GetComponent<ProjectileCollision>().damageMultiplyer = playerInformation.strength;

        //pushes the proj
        proj.GetComponent<Rigidbody>().AddForce(proj.transform.forward * thrust, ForceMode.Impulse);

        proj.GetComponent<ProjectileCollision>().thrust = thrust;
    }

    public void ShootProjectile()
    {
        //check if the player can actually do this move
        if (ItemsManager.Instance.hasGauntles)
        {
        //check if there is a current projectile
            if (currentProj)
            {
                //check if the projectile is active
                if (!currentProj.activeSelf)
                {
                    Shoot();
                }
            }
            else
            {
                Shoot();
            }
        }
    }

    public void DropShield()
    {
        if (shield)
        {
            shield.transform.parent = null;
            Destroy(shield.gameObject);
            shield = null;
        }
    }

    public void DropSword()
    {
        if (sword)
        {
            if (sword.GetComponent<SwordCollision>().goldenSword)
            {
                playerInformation.absorbDistance /= sword.GetComponent<SwordCollision>().absorbMultiplier;
                ItemsManager.Instance.itemDropMultiplier -= sword.GetComponent<SwordCollision>().lootMultiplier;
            }
            
            //sets the swords parent to be nothing
            sword.transform.parent = null;
            sword.gameObject.SetActive(false);
            sword = null;
            //StartCoroutine(ThrowDroppedSword(sword.transform));
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

        if (sword.GetComponent<SwordCollision>().goldenSword)
        {
            playerInformation.absorbDistance *= sword.GetComponent<SwordCollision>().absorbMultiplier;
            ItemsManager.Instance.itemDropMultiplier += sword.GetComponent<SwordCollision>().lootMultiplier;
        }
    }

    public void AddShield(ShieldStats newShield)
    {
        if (newShield)
        {
            DropShield();
        }

        shield = newShield;
        shield.transform.parent = shieldBone.transform;
        shield.transform.localPosition = new Vector3(0, 0, 0);
        shield.transform.localEulerAngles = new Vector3(0, 0, shield.transform.localEulerAngles.z);
        shield.GetComponent<ShieldPickup>().canPickUp = false;

        if (shield.GetComponent<Collider>())
            shield.GetComponent<Collider>().enabled = false;
    }

    public void EnableSword()
    {
        //enables collider and trail
        sword.GetComponent<BoxCollider>().enabled = true;
        if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("DashAttack"))
            sword.transform.GetComponentInChildren<TrailRenderer>().enabled = true;
    }

    public void DisableSword()
    {
        //disables collider and trail
        sword.GetComponent<BoxCollider>().enabled = false;
        sword.transform.GetComponentInChildren<TrailRenderer>().enabled = false;
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
        //do block sound
        if (!blocking)
            SoundManager.PlaySound(startBlockSounds, transform.position);

        //sets the players move speed
        playerInformation.SetMoveSpeed(playerInformation.GetOriginalMoveSpeed() * shield.speedDamping * playerInformation.GetCharmFloat("blockSpeedMultiplier"));

        //auto look at the closest player
        if (GetClosestEnemy() != transform)
        {
            if (autoBlock)
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
        SoundManager.PlaySound(stopBlockSounds, transform.position);
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

    public void doAttackSound()
    {
        SoundManager.PlaySound(firstSlashSounds, transform.position);
    }

    void doSlash()
    {
        animator.SetTrigger("Attack");
        //do slash things
    }

    void doSecondSlash()
    {
        StartCoroutine(boolWait("SecondAttack"));
        SoundManager.PlaySound(secondSlashSounds, transform.position);
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
        if (playerInformation.currentCureAmount >= rapidJuicePerFrame)
        {
            //do rapid slash things
            SoundManager.PlaySound(rapidSounds, transform.position);
            animator.SetBool("TripleAttack", true);
            if (animator.GetCurrentAnimatorStateInfo(1).IsTag("RapidAttack"))
            {
                playerInformation.SetMoveSpeed(playerInformation.GetOriginalMoveSpeed() * multiSpeedMultiplier);
            }

            playerInformation.currentCureAmount -= rapidJuicePerFrame;
        }
        else
        {
            ResetCombo();
            ResetSpin();
        }
    }

    void doDash()
    {
        if (playerInformation.currentCureAmount >= juicePerDash)
        {
            //make sure the player is not frozen
            if (!playerHealth.isFrozen)
            {
                if (canDash)
                {
                    if (canDashAttack)
                    {
                        SoundManager.PlaySound(dashAttackSounds, transform.position);
                        animator.SetTrigger("DashAttack");
                        playerHealth.InvincibilityForSecs(dashTime + 1);
                    }
                    else
                    {
                        SoundManager.PlaySound(dashSounds, transform.position);
                        animator.SetTrigger("Dash");
                    }

                    playerHealth.RemoveAilment();

                    playerInformation.currentCureAmount -= juicePerDash;

                    canDash = false;

                    cancelDash = false;

                    StartCoroutine(dash());
                    StartCoroutine(dashCooldownTimer());
                }
            }
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

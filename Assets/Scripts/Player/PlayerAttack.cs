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
    public bool canDashAttack = true;

    [Header("Other Vars")]
    public bool blocking = false;
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

    private bool canDash = true;
	private bool cancelDash = false;
    private bool rapidSlashCoolingDown = false;
    private bool comboStarted = false;

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
            input.SetupBindings();
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

    void FixedUpdate()
    {
        CountCombo();
    }

    void Update()
    {
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

        if (animator.GetCurrentAnimatorStateInfo(1).IsTag("Idle"))
        {
            DisableSword();
        }

        //check if can actually attack
        //do basic attack
        if (input.BasicAttack.WasPressed)
        {
            CheckCombo();
            //if combo is equal to or greater than 3, do rapid slash
            if (comboAmount >= slashAmount)
            {
                doRapidSlash();
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
                //dash slash
                if (canDash)
                {
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

    void AddSwordPlayerComponenets(SwordCollision newSword)
    {
        newSword.playerInfo = playerInformation;
        newSword.playerHealth = playerHealth;
        newSword.playerAttack = this;
        newSword.animator = animator;
    }

    public void DropSword()
    {
        if (sword)
        {
            //sets the swords parent to be nothing
            sword.transform.parent = null;
            //places the sword in the world
            //sword.transform.localPosition = transform.right;
            sword.transform.position = new Vector3(sword.transform.position.x, 2, sword.transform.position.z);
            sword.transform.eulerAngles = new Vector3(21.458f, 90, 180);
            sword.GetComponent<SwordPickup>().canPickUp = true;
            sword = null;
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
        sword.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void DisableSword()
    {
        //disables collider and trail
        sword.GetComponent<BoxCollider>().enabled = false;
        sword.transform.GetChild(0).gameObject.SetActive(false);
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

    void DoBlock()
    {
        //do block things
        playerInformation.SetMoveSpeed(playerInformation.GetOriginalMoveSpeed() * shield.speedDamping * playerInformation.GetCharmFloat("blockSpeedMultiplier"));
		
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
        yield return new WaitForEndOfFrame();
        animator.SetBool(boolName, false);
    }

    void doSlash()
    {
        animator.SetTrigger("Attack");
        //StartCoroutine(slashWait("Attack"));
        //do slash things
    }

    void doSecondSlash()
    {
        animator.SetTrigger("SecondAttack");
        //StartCoroutine(slashWait("SecondAttack"));
    }

    IEnumerator slashWait(string boolName)
    {
        animator.SetBool(boolName, true);
        yield return animator.GetCurrentAnimatorStateInfo(1);
        yield return new WaitForSeconds(.1f);
        animator.SetBool(boolName, false);
    }

    public void DisplaySlash()
    {
//        GameObject slash = ObjectPooler.GetPooledObject(slashFX);
//        slash.transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
//        slash.transform.eulerAngles = transform.eulerAngles - slashFX.transform.eulerAngles;
//        slash.transform.localScale = slashFX.transform.localScale;
//        slash.GetComponent<Slash>().direction = transform.forward;
//        slash.GetComponent<Slash>().cc = characterController;
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

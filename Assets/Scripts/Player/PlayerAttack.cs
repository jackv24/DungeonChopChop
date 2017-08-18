using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    float attackMinAngle = 130;
    float attackDistance = 5;

    [Header("Combo Vars")]
    public float timeInbetween;
    [Tooltip("The amount added to the combo counter (60 times a second)")]
    public float comboIncrease = .04f;

    [Header("Rapid Slash Vars")]
    public int slashAmount;
    public float rapidSlashCooldown;
    [Tooltip("The amount added to the rapid slash cooldown (60 times a second)")]
    public float rapidSlashIncrease = .04f;

    [Header("Dash Vars")]
    public float dashTime = 1.0f;
    public float dashSpeed = 5.0f;
    public float dashCooldown = 0.5f;

    [Header("Other Vars")]
    public bool blocking = false;
    public GameObject slashFX;


    bool canAttack = true;

    private PlayerInputs input;
    private PlayerMove playerMove;
    private PlayerInformation playerInformation;
    private Animator animator;

    public ShieldStats shield;

    private int comboAmount;
    private CharacterController characterController;
    private float comboCounter;
    private float rapidSlashCounter;
    private float normalMoveSpeed;

    private bool canDash = true;
    private bool rapidSlashCoolingDown = false;
    private bool comboStarted = false;

    void Start()
    {
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
        normalMoveSpeed = playerInformation.maxMoveSpeed;
    }


    void Update()
    {
        //sets attack values
        attackMinAngle = playerInformation.attackMinAngle;
        attackDistance = playerInformation.attackDistance;
        //check if can actually attack
        //if (canAttack) 
        //{
        //do basic attack
        if (input.BasicAttack.WasPressed)
        {
            //CheckCombo();
            //if combo is equal to or greater than 3, do rapid slash
            if (comboAmount >= 3)
            {
                //rapid slash
                //doRapidSlash();
            } 
            //basic slash
            if (canAttack)
            {
                if (animator.GetCurrentAnimatorStateInfo(1).IsTag("Idle"))
                {
                    doSlash();
                }
            }
        } 
        if (input.DashSlash.WasPressed)
        {
            //dash slash
            if (canDash)
            {
                doDash();
            }
        } 
        if (input.Block)
        {
            //block
            if (shield)
            {
                if (!animator.GetCurrentAnimatorStateInfo(1).IsTag("Attacking"))
                    DoBlock();
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
        //}
    }

    void FixedUpdate()
    {
        if (comboStarted)
        {
            CountCombo();
            //check if combo has ended
            if (comboCounter > timeInbetween)
            {
                ResetCombo();
            }
        }

        if (rapidSlashCoolingDown)
        {
            RapidSlashCooldownCounter();
            //check if rapid slash cooldown is over
            if (rapidSlashCounter > rapidSlashCooldown)
            {
                ResetRapidSlash();
            }
        }
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
        if (playerInformation.maxMoveSpeed == normalMoveSpeed)
        {
            playerInformation.maxMoveSpeed = playerInformation.maxMoveSpeed * shield.speedDamping;
        }
        if (playerInformation.HasCharmFloat("blockSpeedMultiplier"))
            playerInformation.maxMoveSpeed = playerInformation.maxMoveSpeed * shield.speedDamping * playerInformation.GetCharmFloat("blockSpeedMultiplier");
		
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
        playerInformation.maxMoveSpeed = normalMoveSpeed;
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
        StartCoroutine(boolWait("Attack"));
        //do slash things
    }

    public void DisplaySlash()
    {
        GameObject slash = ObjectPooler.GetPooledObject(slashFX);
        slash.transform.position = transform.position;
        slash.transform.eulerAngles = transform.eulerAngles - slashFX.transform.eulerAngles;
        slash.transform.localScale = slashFX.transform.localScale;
        slash.GetComponent<Slash>().direction = transform.forward;
        slash.GetComponent<Slash>().cc = characterController;
    }

    void doRapidSlash()
    {
        //do rapid slash things
        rapidSlashCoolingDown = true;
        canAttack = false;
        ResetCombo();
        //Debug.Log ("Rapid Slash");
    }

    void ResetRapidSlash()
    {
        rapidSlashCoolingDown = false;
        rapidSlashCounter = 0;
        canAttack = true;
    }

    void doDash()
    {
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

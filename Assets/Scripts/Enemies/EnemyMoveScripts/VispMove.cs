using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VispMove : EnemyMove
{
    [Header("Moving values during states")]
    [Tooltip("The speed when in the shoot radius (On Visp Attack")]
    public float speedDuringShootRadius = 3.5f;
    [Tooltip("The speed when in the attack radius (On Visp Attack")]
    public float speedDuringAttackRadius = 3.5f;

    [Header("Leep Values")]
    [Tooltip("The distance in which the enemy will stop and 'leep'")]
    public float radiusAttack = 5;
    [Tooltip("Time until the enemy leeps at player")]
    public float waitTillLeep = 1;
    [Tooltip("The speed when leeping")]
    public float leepSpeed = 5;
    [Space()]
    public float minTimeBetweenLeep = 2;
    public float maxTimeBetweenLeep = 3;

    private bool inLeeping = false;
    private bool doingLeep = false;
    private bool moveBack = false;
    private bool canLeep = true;

    private int leepCounter = 0;
    private float timeBetweenLeep = 0;

    private Vector3 leepTarget;

    private VispAttack vispAttack;
    private TrailRenderer trailRenderer;

    // Use this for initialization
    void Start()
    {
        Setup();
        vispAttack = GetComponent<VispAttack>();
        trailRenderer = GetComponentInChildren<TrailRenderer>();
    }

    void OnEnable()
    {
        ResetEnable();
        inLeeping = false;
        doingLeep = false;
        moveBack = false;
        canLeep = true;
    }

    public override void FixedUpdate()
    {
        if (!canLeep)
        {
            //can leep or not counter
            leepCounter++;

            if (leepCounter > timeBetweenLeep * 60)
            {
                canLeep = true;
                //reset values
                leepCounter = 0;
                timeBetweenLeep = Random.Range(minTimeBetweenLeep, maxTimeBetweenLeep);

            }
        }

        base.FixedUpdate();
    }
	
    // Update is called once per frame
    void Update()
    {
        //sets the speed of the visp depending on the range of the player
        if (!doingLeep)
        {
            if (InDistanceBetweenTwoRadius(vispAttack.shootRadius, vispAttack.attackRadius))
            {
                agent.speed = speedDuringShootRadius;
            }
            else if (InDistance(vispAttack.attackRadius))
            {
                agent.speed = speedDuringAttackRadius;
            }
        }

        //checks if the visp is in the radius of the player to do leep
        if (!doingLeep)
        {
            if (!InDistance(radiusAttack))
            {
                FollowPlayer();
            }
            else
            {
                //check if can leep, is leeping and if the coroutine is already running
                if (canLeep)
                {
                    if (!inLeeping)
                    {
                        StartCoroutine(LeepAtEnemy(waitTillLeep));
                    }
                }
            }
        }
        //follow the player when leeping
        if (doingLeep)
        {
            GoToTarget(leepTarget);
        }
        //then move back
        if (moveBack)
        {
            RunAwayFromPlayer(false);
        }
    }

    IEnumerator LeepAtEnemy(float waitTillLeep)
    {
        //sets some values to be true, which starts the leep
        trailRenderer.enabled = true;

        inLeeping = true;
        moveBack = true;

        //stops the agent
        agent.speed = .5f;

        yield return new WaitForSeconds(waitTillLeep);

        //set leap target
        leepTarget = GetClosestPlayer().position;

        //the waiting has finished, now leep
        moveBack = false;
        doingLeep = true;

        //do the attack animation
        animator.SetTrigger("Attack");

        //speed the agent up
        agent.speed = originalSpeed * leepSpeed;

        yield return new WaitForSeconds(1);

        trailRenderer.enabled = false;

        //leep is now over, reset the speed
        agent.speed = originalSpeed;

        canLeep = false;

        inLeeping = false;
        doingLeep = false;
    }
}

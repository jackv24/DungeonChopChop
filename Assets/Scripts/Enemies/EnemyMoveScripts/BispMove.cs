using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BispMove : EnemyMove {

    [Header("Leep Values")]
    [Tooltip("The distance in which the enemy will stop and 'leep'")]
    public float radiusAttack;
    [Tooltip("Time until the enemy leeps at player")]
    public float waitTillLeep;
    [Tooltip("The speed when leeping")]
    public float leepSpeed = 5;
    [Tooltip("The time the enemy leeps for")]
    public float leepTime;
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

    void Start()
    {
        Setup();
    }

    void OnEnable()
    {
        inLeeping = false;
        doingLeep = false;
        moveBack = false;

        base.OnEnable();
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
	void Update () {
        
        //checks if the bisp is in the radius of the player to do leep
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
        inLeeping = true;
        moveBack = true;

        //stops the agent
        agent.speed = .5f;

        //set leap target
        leepTarget = GetClosestPlayer().position;

        yield return new WaitForSeconds(waitTillLeep);

        //the waiting has finished, now leep
        moveBack = false;
        doingLeep = true;

        //speed the agent up
        agent.speed = originalSpeed * leepSpeed;

        yield return new WaitForSeconds(leepTime);

        //leep is now over, reset the speed
        agent.speed = originalSpeed;

        canLeep = false;

        inLeeping = false;
        doingLeep = false;
    }
}

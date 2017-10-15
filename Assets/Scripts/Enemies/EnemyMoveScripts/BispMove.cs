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

    private bool inLeeping = false;
    private bool doingLeep = false;
    private bool moveBack = false;

    void Start()
    {
        Setup();
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
                //if so, do that leep
                if (!inLeeping)
                    StartCoroutine(LeepAtEnemy(waitTillLeep));
            }
        }
        //follow the player when leeping
        if (doingLeep)
        {
            FollowPlayer();
        }
        //then move back
        if (moveBack)
        {
            RunAwayFromPlayer(true);
        }
	}

    IEnumerator LeepAtEnemy(float waitTillLeep)
    {
        //sets some values to be true, which starts the leep
        inLeeping = true;
        moveBack = true;

        //stops the agent
        agent.speed = 0;

        yield return new WaitForSeconds(waitTillLeep);

        //the waiting has finished, now leep
        moveBack = false;
        doingLeep = true;

        //speed the agent up
        agent.speed = originalSpeed * leepSpeed;

        yield return new WaitForSeconds(1);

        //leep is now over, reset the speed
        agent.speed = originalSpeed;

        inLeeping = false;
        doingLeep = false;
    }
}

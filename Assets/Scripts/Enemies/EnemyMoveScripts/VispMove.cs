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

    private bool inLeeping = false;
    private bool doingLeep = false;
    private bool moveBack = false;

    private VispAttack vispAttack;

    // Use this for initialization
    void Start()
    {
        Setup();
        vispAttack = GetComponent<VispAttack>();
    }
	
    // Update is called once per frame
    void Update()
    {
        //sets the speed of the visp depending on the range of the player
        if (InDistanceBetweenTwoRadius(vispAttack.shootRadius, vispAttack.attackRadius))
        {
            agent.speed = speedDuringShootRadius;
        }
        else if (InDistance(vispAttack.attackRadius))
        {
            agent.speed = speedDuringAttackRadius;
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

        //do the attack animation
        animator.SetTrigger("Attack");

        //speed the agent up
        agent.speed = originalSpeed * leepSpeed;

        yield return new WaitForSeconds(1);

        //leep is now over, reset the speed
        agent.speed = originalSpeed;

        inLeeping = false;
        doingLeep = false;
    }
}

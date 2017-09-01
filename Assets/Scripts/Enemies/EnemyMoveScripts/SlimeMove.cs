﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeMove : EnemyMove {

    [Tooltip("In seconds")]
    public float damageToEnemies = 1; 
    public float radiusAttack = 5;
    public float waitTillLeep = 1;
    public float leepPower = 10;

    private int hopCounter = 0;
    private float currentTimeBetweenHop = 0;

    private BoxCollider col;
    private Rigidbody rb;

    public bool friendly = false;

    private bool attacking = false;
    private bool inLeeping = false;
    private bool doingLeep = false;

	// Use this for initialization
	void Awake () {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();
        Setup();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (!friendly)
        {
            if (!runAway)
                AttackPlayer();
            else
                RunAwayFromPlayer();
        }
        else
        {
            FollowEnemy();
        }

        foreach (PlayerInformation player in players)
        {
            if (player.HasCharmBool("slimesAreFriends"))
            {
                friendly = true;
                break;
            }
            else
            {
                friendly = false;
            }
        }

        //checks if the slime is in mid air, if so disable the collider
        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Midair"))
        {
            col.enabled = false;
        }
        else
        {
            //renable the collider
            if (!col.enabled)
            {
                col.enabled = true;
            }
        }
	}

    void AttackPlayer()
    {
        if (!InDistance(radiusAttack))
        {
            FollowPlayer();
        }
        else
        {
            if (!inLeeping)
                StartCoroutine(LeepAtEnemy(waitTillLeep));
        }
        if (doingLeep)
        {
            FollowPlayer();
        }
    }

    IEnumerator LeepAtEnemy(float waitTillLeep)
    {
        inLeeping = true;
        agent.speed = 0;
        yield return new WaitForSeconds(waitTillLeep);
        animator.SetTrigger("Hop");
        doingLeep = true;
        agent.speed = originalSpeed * 2;
        yield return new WaitForSeconds(1);
        agent.speed = originalSpeed;
        inLeeping = false;
        doingLeep = false;
    }

    void Hop()
    {
        animator.SetTrigger("Hop");
    }

    void OnTriggerEnter(Collider col)
    {
        if (friendly)
        {
            if (col.GetComponent<Collider>().tag != "Player" || col.GetComponent<Collider>().tag != "Slime")
            {
                if (col.gameObject.GetComponent<Health>())
                {
                    col.gameObject.GetComponent<Health>().AffectHealth(-damageToEnemies);
                }
            }
        }
    }
}

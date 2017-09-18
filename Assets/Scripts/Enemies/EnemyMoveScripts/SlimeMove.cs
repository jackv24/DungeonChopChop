﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum SlimeType
{
    Green,
    Blue,
    Red
}

public class SlimeMove : EnemyMove {

    public SlimeType type;

    public float damageToEnemies = 1; 

    [Header("Movement Vars")]
    public float followProximity = 20;
    public float radiusAttack = 5;
    public float waitTillLeep = 1;
    public float timeBetweenRoam = 2;

    [Space()]
    public bool friendly = false;

    private bool inLeeping = false;
    private bool doingLeep = false;

	// Use this for initialization
	void Awake () {
        Setup();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (!friendly)
        {
            if (!runAway)
            {
                DoMove();
            }
            else
            {
                RunAwayFromPlayer();
            }
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
	}

    void DoMove()
    {
        //if red, attack player
        if (type == SlimeType.Red)
        {
            AttackPlayer();
        }
        //if blue, roam until in distance, then attac
        else if (type == SlimeType.Blue)
        {
            if (!InDistance(followProximity))
                Roam(timeBetweenRoam);
            else
                AttackPlayer();
        }
        //if green, just roam
        else if (type == SlimeType.Green)
        {
            Roam(timeBetweenRoam);
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

    void OnTriggerEnter(Collider col)
    {
        if (friendly)
        {
            if (col.GetComponent<Collider>().tag != "Player")
            {
                if (col.gameObject.GetComponent<Health>())
                {
                    col.gameObject.GetComponent<Health>().AffectHealth(-damageToEnemies);
                }
            }
        }
    }
}

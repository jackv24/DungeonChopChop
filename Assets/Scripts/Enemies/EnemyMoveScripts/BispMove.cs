using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BispMove : EnemyMove {

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
        if (!InDistance(radiusAttack))
        {
            Debug.Log("hi");
            FollowPlayer();
        }
        else
        {
            if (!inLeeping)
                StartCoroutine(LeepAtEnemy(waitTillLeep));
        }
//        if (doingLeep)
//        {
//            FollowPlayer();
//        }
//
//        if (moveBack)
//        {
//            RunAwayFromPlayer(true);
//        }
	}

    IEnumerator LeepAtEnemy(float waitTillLeep)
    {
        inLeeping = true;
        agent.speed = 0;
        moveBack = true;
        yield return new WaitForSeconds(waitTillLeep);
        moveBack = false;
        //animator.SetTrigger("Hop");
        doingLeep = true;
        agent.speed = originalSpeed * leepSpeed;
        yield return new WaitForSeconds(1);
        agent.speed = originalSpeed;
        inLeeping = false;
        doingLeep = false;
    }
}

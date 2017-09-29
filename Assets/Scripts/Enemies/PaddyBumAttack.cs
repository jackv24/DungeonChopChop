using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum PaddyType
{
    RadiusExplode,
    HitExplode,
    TouchExplode,
}

public class PaddyBumAttack : EnemyAttack {

    public PaddyType paddyType;
    public float radiusTillExplode;
    public float timeTillExplode = 3;
    public float animatorMultiplier = 2;

    private bool waitingToExplode = false;
    private int counter = 0;

	// Update is called once per frame
	void Update () {
        
        if (paddyType == PaddyType.RadiusExplode)
        {
            if (!enemyHealth.isDead)
            {
                if (!waitingToExplode)
                {
                    if (enemyMove.InDistance(radiusTillExplode))
                    {
                        animator.SetTrigger("Exploding");
                        waitingToExplode = true;
                    }
                }
            }
        }
	}

    void FixedUpdate()
    {  
        if (waitingToExplode)
        {
            counter++;
            if (counter > (timeTillExplode * 60))
            {
                waitingToExplode = false;
                enemyHealth.AffectHealth(-enemyHealth.maxHealth);
                counter = 0;
            }
        }
    }

    void OnCollisionEnter(Collision col)
    {
        //check if player collided
        if (col.collider.tag == "Player1" || col.collider.tag == "Player2")
        {
            //if the type is touch explode
            if (paddyType == PaddyType.TouchExplode)
            {
                //kill itself
                if (!waitingToExplode)
                {
                    enemyHealth.SetColorSeconds(Color.red, .5f);
                    animator.SetTrigger("Exploding");
                    waitingToExplode = true;
                }
            }
        }
        //check if sword is colliding
        if (col.gameObject.layer == 16)
        {
            //if the type is hit explode
            if (paddyType == PaddyType.HitExplode)
            {
                //kill itself
                if (!waitingToExplode)
                {
                    enemyHealth.SetColorSeconds(Color.red, .5f);
                    animator.SetTrigger("Exploding");
                    waitingToExplode = true;
                }
            }
        }
    }
}

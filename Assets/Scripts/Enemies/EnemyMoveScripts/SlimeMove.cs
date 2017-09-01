using System.Collections;
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

    private Collider col;
    private Rigidbody rb;

    public bool friendly = false;

    private bool attacking = false;

	// Use this for initialization
	void Awake () {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        Setup();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
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
            AttackPlayer();
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

    void AttackPlayer()
    {
        if (!InDistance(radiusAttack))
        {
            FollowPlayer();
        }
        else
        {
            if (!attacking)
            {
                StartCoroutine(LeepAtEnemy(radiusAttack, waitTillLeep));
            }
        }
    }

    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Midair"))
        {
            col.enabled = false;
        }
        else
        {
            if (!col.enabled)
            {
                col.enabled = true;
            }
        }
    }

    IEnumerator LeepAtEnemy(float radius, float waitTillLeep)
    {
        attacking = true;
        agent.velocity = agent.velocity / 5;
        yield return new WaitForSeconds(waitTillLeep);
        Hop();
        rb.AddForce(transform.forward * leepPower, ForceMode.Impulse);
        attacking = false;
    }

    void Hop()
    {
        animator.SetTrigger("Hop");
    }

    void OnCollisionEnter(Collision col)
    {
        if (friendly)
        {
            if (col.collider.tag != "Player" || col.collider.tag != "Slime")
            {
                if (col.gameObject.GetComponent<Health>())
                {
                    col.gameObject.GetComponent<Health>().AffectHealth(-damageToEnemies);
                }
            }
        }
    }
}

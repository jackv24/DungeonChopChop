using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossVispAttack : EnemyAttack {

    [Header("Boss Visp Values")]

    [Header("Strike Values")]
    public float minTimeBetweenStrike = 2;
    public float maxTimeBetweenStrike = 4;

    [Header("Shooting Values")]
    public float rotateSpeed = 5;
    public float minTimeBetweenShot = .1f;
    public float maxTimeBetweenShot = 1;

    private float timeBetweenShot = 0;
    private float timeBetweenStrike = 0;

    private int shootCounter = 0;
    private int strikeCounter = 0;

    private bool striking = false;

    private Animator childAnimator;

	// Use this for initialization
	void Awake () 
    {
        childAnimator = transform.GetChild(0).GetComponent<Animator>();

        timeBetweenStrike = Random.Range(minTimeBetweenStrike, maxTimeBetweenStrike);
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        if (!striking)
        {
            //wait until its ready to shoot
            shootCounter++;
            if (shootCounter > timeBetweenShot * 60)
            {
                shootCounter = 0;

                childAnimator.SetTrigger("Shoot");

                timeBetweenShot = Random.Range(minTimeBetweenShot, maxTimeBetweenShot);
            }

            //look at the player
            enemyMove.LookAtClosestPlayer(rotateSpeed);
        }

        if (!striking)
        {
            strikeCounter++;

            transform.parent.position = Vector3.Lerp(transform.parent.position, new Vector3(enemyMove.GetClosestPlayer().transform.position.x, 0, transform.position.z), 1.1f * Time.fixedDeltaTime);

            if (strikeCounter > timeBetweenStrike * 60)
            {
                strikeCounter = 0;

                StartCoroutine(BoolFlick("Strike"));

                timeBetweenStrike = Random.Range(minTimeBetweenStrike, maxTimeBetweenStrike);
            }
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Striking"))
            striking = true;
        else
            striking = false;
	}

    IEnumerator BoolFlick(string boolname)
    {
        animator.SetBool(boolname, true);
        yield return new WaitForEndOfFrame();
        animator.SetBool(boolname, false);
    }
}

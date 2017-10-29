using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleShooterMove : EnemyMove {

    [Header("Apple Shooter Values")]
    public float rotateSpeed = 5;
    public int hitsTillShoot = 2;

    private int hits = 0;

    public override void OnEnable()
    {
        base.OnEnable();
    }

    void Start()
    {
        Setup();

        enemyHealth.OnHealthChange += AttackOverride;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
	
	// Update is called once per frame
	void Update () {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Attacking"))
        {
            LookAtClosestPlayer(rotateSpeed);
        }
	}

    void AttackOverride()
    {
        //count how many hits, if so shoot
        hits++;

        if (hits > hitsTillShoot)
        {
            animator.SetTrigger("Attack");
            hits = 0;
        }
    }
}

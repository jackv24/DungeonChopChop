using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VispAttack : EnemyAttack {

    [Tooltip("Shoots if player is in this radius")]
    public float shootRadius = 20;
    [Tooltip("Start attacking if player is in this radius")]
    public float attackRadius = 10;

    private int counter = 0;
    private VispMove vispMove;
    private float shootInterval;

	// Use this for initialization
	void Start () {
        vispMove = GetComponent<VispMove>();
        animator = GetComponentInChildren<Animator>();

        shootInterval = Random.Range(minInterval, maxInterval);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        counter++;

        if (vispMove)
        {
            if (vispMove.InDistanceBetweenTwoRadius(shootRadius, attackRadius))
            {
                if (counter > (shootInterval * 60))
                {
                    animator.SetTrigger("Shoot");
                    counter = 0;
                    shootInterval = Random.Range(minInterval, maxInterval);
                }
            }
        }
	}
}

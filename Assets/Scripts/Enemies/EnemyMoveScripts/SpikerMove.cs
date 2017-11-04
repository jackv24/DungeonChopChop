using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikerMove : EnemyMove {

    [Header("Spiker Values")]
    public float attackRadius;
    public float rotateSpeed = .1f;

	// Use this for initialization
	void Start () {
        Setup();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        if (GetClosestPlayerRadius(attackRadius) != null)
        {
            animator.SetBool("Attack", true);
            transform.Rotate(0, rotateSpeed, 0);
        }
        else 
            animator.SetBool("Attack", false);
            
	}
}

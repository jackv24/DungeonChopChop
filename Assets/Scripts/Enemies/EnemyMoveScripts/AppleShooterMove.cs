using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleShooterMove : EnemyMove {

    public float rotateSpeed = 5;

	// Use this for initialization
	void Start () {
        Setup();
	}
	
	// Update is called once per frame
	void Update () {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Attacking"))
        {
            LookAtClosestPlayer(rotateSpeed);
        }
	}
}

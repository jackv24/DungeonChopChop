using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleShooterMove : EnemyMove {

    public float rotateSpeed = 5;

    void OnEnable()
    {
        ResetEnable();
    }

    void Start()
    {
        Setup();
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
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapEyeMove : EnemyMove 
{
    public float radius;

    void OnEnable()
    {
        base.OnEnable();
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
	void Update ()
    {
        if (InDistance(radius))
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Idle"))
            {
                animator.SetTrigger("Triggered");
            }
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Attacking"))
        {
            if (!runAway)
                FollowPlayer();
            else
                RunAwayFromPlayer(false);
        }
    }
}

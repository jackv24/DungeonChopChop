using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapEyeMove : EnemyMove 
{

    public float radius;

	// Use this for initialization
	void Start () 
    {
        if (IsInDistanceOfPlayer(radius))
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Idle"))
            {
                animator.SetTrigger("Triggered");
            }
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Attacking"))
        {
            FollowPlayer();
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

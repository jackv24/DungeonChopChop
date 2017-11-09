using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushBoomMove : EnemyMove 
{

	// Use this for initialization
	void Start () 
	{
        Setup();
	}

    void OnEnable()
    {
        base.OnEnable();
    }
	
	// Update is called once per frame
	void FixedUpdate () 
	{
        if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Attacking"))
        {
            if (agent.isOnNavMesh)
                agent.isStopped = false;
            Roam();
        }
        else
        {
            if (agent)
            {
                if (agent.isOnNavMesh)
                    agent.isStopped = true;
            }
        }
	}
}

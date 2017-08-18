using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeMove : EnemyMove {

    [Tooltip("In seconds")]
    public float minTimeBetweenHops;
    public float maxTimeBetweenHops;

    private int hopCounter = 0;
    private float currentTimeBetweenHop = 0;

    private Collider col;

	// Use this for initialization
	void Awake () {
        col = GetComponent<Collider>();
        currentTimeBetweenHop = Random.Range(minTimeBetweenHops, maxTimeBetweenHops);
        Setup();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        DoHop();
        FollowPlayer();
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

    void DoHop()
    {
        hopCounter++;
        if (hopCounter > currentTimeBetweenHop * 60)
        {
            Hop();
            currentTimeBetweenHop = Random.Range(minTimeBetweenHops, maxTimeBetweenHops);
            hopCounter = 0;
        }
    }

    void Hop()
    {
        animator.SetTrigger("Hop");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoatMove : EnemyMove {

    public float radius;
    public float chargeSpeed = 10;
    public float timeBetweenRoamChange = 2;

    private float OGSpeed;

	// Use this for initialization
	void Awake () {
        Setup();
        OGSpeed = agent.speed;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!IsInDistanceOfPlayer(radius))
        {
            Roam(timeBetweenRoamChange);
            agent.speed = OGSpeed;
        }
        else
        {
            Charge();
        }
	}

    void Charge()
    {
        agent.speed = chargeSpeed;
        StartCoroutine(animWait());
        FollowPlayer();
    }

    IEnumerator animWait()
    {
        animator.SetBool("Charging", true);
        yield return new WaitForEndOfFrame();
        animator.SetBool("Charging", false);
    }
}

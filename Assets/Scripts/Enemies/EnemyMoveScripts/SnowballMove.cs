using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballMove : EnemyMove {

    [Tooltip("The health increases overtime by this amount")]
    public float healthIncrease = .01f;

	// Use this for initialization
	void Start () {
        Setup();
	}

    void FixedUpdate()
    {
        FollowPlayer();

        transform.localScale = new Vector3(enemyHealth.health, enemyHealth.health, enemyHealth.health) * 1.5f;

        enemyHealth.health += healthIncrease;
    }
}

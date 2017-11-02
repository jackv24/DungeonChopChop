using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballMove : EnemyMove {

    [Tooltip("The health increases overtime by this amount")]
    public float healthIncrease = .01f;
    public float rotateSpeed = .5f;

    private float rotatation = 0;

    private GameObject snowBallModel;

    void OnEnable()
    {
        base.OnEnable();
    }

	// Use this for initialization
	void Start () {
        
        snowBallModel = transform.GetChild(0).gameObject;

        Setup();
	}

    public override void FixedUpdate()
    {
        FollowPlayer();

        rotatation = agent.velocity.magnitude * rotateSpeed;

        transform.Rotate(0, 0, rotatation * Time.fixedDeltaTime);

        if (agent.velocity.magnitude > 3)
        {
            enemyHealth.health += healthIncrease;
        }

        transform.localScale = new Vector3(enemyHealth.health, enemyHealth.health, enemyHealth.health) * 1.5f;

        base.FixedUpdate();
    }
}

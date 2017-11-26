using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballMove : EnemyMove {

    [Tooltip("The health increases overtime by this amount")]
    public float healthIncrease = .01f;
    [Tooltip("The min speed the snowball has to be moving to start increasing in size")]
    public float minSpeedToIncrease = 2.5f;

    private float rotatation = 0;

    private GameObject snowBallModel;
    private Vector3 prevPos;
    private float currentSpeed;
    private Vector3 originalScale;

    void OnEnable()
    {
        transform.localScale = originalScale;

        enemyHealth.health = enemyHealth.maxHealth / 2;

        base.OnEnable();
    }

	// Use this for initialization
	void Awake () {
        
        originalScale = transform.localScale;
        
        snowBallModel = transform.GetChild(0).gameObject;

        Setup();

        enemyHealth.OnHealthChange += ChangeScale;
	}

    void ChangeScale()
    {
        if (enemyHealth.health > (originalScale.x / 2))
            transform.localScale = new Vector3(enemyHealth.health, enemyHealth.health, enemyHealth.health);
    }

    void Update()
    {
        Vector3 move = transform.position - prevPos;
        currentSpeed = move.magnitude / Time.deltaTime;
        prevPos = transform.position;

        snowBallModel.transform.Rotate(0, currentSpeed, currentSpeed);
    }

    public override void FixedUpdate()
    {
        FollowPlayer();

        if (currentSpeed > minSpeedToIncrease)
        {
            enemyHealth.health += healthIncrease;
            enemyHealth.HealthChanged();
        }

        base.FixedUpdate();
    }
}

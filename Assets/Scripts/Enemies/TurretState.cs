using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretState : MonoBehaviour {

    public bool active = true;

    private EnemyAttack enemyAttack;

	// Use this for initialization
	void Start () 
    {
        enemyAttack = GetComponent<EnemyAttack>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if (active)
        {
            enemyAttack.enabled = true;
        }
        else
        {
            enemyAttack.enabled = false;
        }
	}
}

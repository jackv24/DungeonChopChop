using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleShooterAnimEvent : MonoBehaviour {

    EnemyAttack enemyAttack;

    // Use this for initialization
    void Start () {
        enemyAttack = GetComponentInParent<EnemyAttack>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Shoot()
    {
        enemyAttack.Shootforward();
    }
}

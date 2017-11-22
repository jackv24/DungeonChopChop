using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemySpawner : MonoBehaviour {

    private EnemySpawner enemySpawner;

	// Use this for initialization
	void Start () 
    {
        enemySpawner = GetComponent<EnemySpawner>();	

        enemySpawner.OnEnemiesDefeated += SpawnEnemies;
	}
	
	// Update is called once per frame
	void SpawnEnemies () 
    {
        enemySpawner.Spawn(true);
	}
}

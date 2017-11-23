
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemySpawner : MonoBehaviour {

    public float minTimeBetweenSpawn = 5;
    public float maxTimeBetweenSpawn = 10;

    private EnemySpawner enemySpawner;
    private LevelTile tile;

    private bool active = false;
    private int counter = 0;
    private float time = 0;

	// Use this for initialization
	void Start () 
    {
        enemySpawner = GetComponent<EnemySpawner>();	
        tile = GetComponent<LevelTile>();

        tile.OnTileEnter += SetActive;
        tile.OnTileExit += SetDeactive;
	}

    void SetActive()
    {
        active = true;
    }

    void SetDeactive()
    {
        active = false;
    }

    void FixedUpdate()
    {
        if (active)
        {
            counter++;

            if (counter > time * 60)
            {
                counter = 0;

                time = Random.Range(minTimeBetweenSpawn, maxTimeBetweenSpawn);

                SpawnEnemies();
            }
        }
    }
	
	// Update is called once per frame
    void SpawnEnemies () 
    {
        enemySpawner.SetRandomProfile();

        enemySpawner.Spawn(true);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	public Transform[] spawnPoints;

	public Helper.Probability[] enemies;

	private List<GameObject> spawnedEnemies = new List<GameObject>();

	[Space()]
	[Tooltip("How many tiles must be entered until this one respawns enemies?")]
	public int respawnTileCount = 5;
	private int tilesLeftUntilRespawn = 0;

	private class EnemySpawnPair
	{
		public EnemySpawnPair(GameObject prefab, Transform spawnPoint)
		{
			this.prefab = prefab;
			this.spawnPoint = spawnPoint;
		}

		public GameObject prefab;
		public Transform spawnPoint;
	}
	private List<EnemySpawnPair> undefeatedEnemies = new List<EnemySpawnPair>();

	private bool shouldSpawn = false;

	public void Spawn()
	{
		if (tilesLeftUntilRespawn > 0)
			return;

		shouldSpawn = true;

		StartCoroutine(SpawnWithEffect());
	}

	IEnumerator SpawnWithEffect()
	{
		bool newEnemies = false;

		List<EnemySpawnPair> toSpawn = new List<EnemySpawnPair>();

		//If there were no undefeated enemies...
		if (undefeatedEnemies.Count <= 0)
		{
			//Spawn a new random set
			foreach (Transform spawn in spawnPoints)
			{
				GameObject enemyPrefab = Helper.GetRandomByProbability(enemies);

				//Add to list to be spawned
				if (enemyPrefab)
					toSpawn.Add(new EnemySpawnPair(enemyPrefab, spawn));
			}

			newEnemies = true;
		}
		else
		{
			//If there were enemies undefeated, just respawn them
			foreach (EnemySpawnPair undefeatedEnemy in undefeatedEnemies)
				toSpawn.Add(undefeatedEnemy);
		}

		if (LevelVars.Instance)
		{
			//Keep a list of spawned particles
			List<ParticleSystem> particles = new List<ParticleSystem>();

			if (LevelVars.Instance.enemySpawnEffect)
			{
				//Get particle effect from pool
				GameObject effectPrefab = LevelVars.Instance.enemySpawnEffect;

				foreach(EnemySpawnPair spawn in toSpawn)
				{
					//Spawn an effect where enemies will spawn
					if (spawn.prefab)
					{
						GameObject effect = ObjectPooler.GetPooledObject(effectPrefab);

						if (effect)
						{
							effect.transform.position = spawn.spawnPoint.position;

							ParticleSystem system = effect.GetComponent<ParticleSystem>();
							particles.Add(system);
						}
					}
				}
			}

			//Wait for set time
			yield return new WaitForSeconds(LevelVars.Instance.enemySpawnDelay);

			//Stop playing all particle effects (let the particles disperse before being disabled in other script)
			foreach (ParticleSystem system in particles)
				system.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		}

		//After delay, actually spawn the enemies
		if (shouldSpawn)
		{
			foreach (EnemySpawnPair spawn in toSpawn)
			{
				GameObject enemy = ObjectPooler.GetPooledObject(spawn.prefab);

				if (enemy)
				{
					enemy.transform.position = spawn.spawnPoint.position + Vector3.up;
					enemy.transform.rotation = Quaternion.identity;

					spawnedEnemies.Add(enemy);

					if (newEnemies)
						undefeatedEnemies.Add(spawn);
				}
			}
		}
	}

	public void Despawn()
	{
		//Interrupt spawning routine if yet to happen
		shouldSpawn = false;
		List<EnemySpawnPair> toRemove = new List<EnemySpawnPair>();

		//Make sure enemies have actually spawned
		if (spawnedEnemies.Count > 0)
		{
			for (int i = 0; i < undefeatedEnemies.Count; i++)
			{
				//Add defeated enemies to a list so that iterator is not edited
				if (!spawnedEnemies[i].activeSelf)
					toRemove.Add(undefeatedEnemies[i]);
			}
		}

		//Remove defeated enemies
		foreach (EnemySpawnPair e in toRemove)
			undefeatedEnemies.Remove(e);

		toRemove.Clear();

		foreach (GameObject enemy in spawnedEnemies)
			enemy.SetActive(false);
	
		spawnedEnemies.Clear();

		//If tile was just cleared, setup tile reset counter
		if (undefeatedEnemies.Count <= 0 && tilesLeftUntilRespawn <= 0)
		{
			tilesLeftUntilRespawn = respawnTileCount;

			LevelGenerator.Instance.OnTileEnter += MinusTilesLeft;
		}
	}

	void MinusTilesLeft()
	{
		tilesLeftUntilRespawn--;

		if (tilesLeftUntilRespawn <= 0)
		{
			//If time to reset tile, just clear undefeated enemies
			undefeatedEnemies.Clear();

			//If no tiles are left, remove reset counter
			LevelGenerator.Instance.OnTileEnter -= MinusTilesLeft;
		}
	}
}

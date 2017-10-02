﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
	public delegate void NormalEvent();
	public event NormalEvent OnEnemiesDefeated;
	public event NormalEvent OnEnemiesSpawned;

	[HideInInspector]
	public bool spawned = false;

	public Transform[] spawnPoints;

	[Space()]
	public int minSpawns = 0;
	public int maxSpawns = 0;

	[Space()]
	public Helper.ProbabilityGameObject[] enemies;

	private List<GameObject> spawnedEnemies = new List<GameObject>();

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

	private bool cleared = false;

	void Start()
	{
		//Make sure all spawn points are on ground
		if (LevelVars.Instance)
		{
			foreach (Transform spawn in spawnPoints)
			{
				if (spawn)
				{
					RaycastHit hit;

					if (Physics.Raycast(spawn.position + Vector3.up, Vector3.down, out hit, 100f, LevelVars.Instance.groundLayer))
					{
						spawn.position = hit.point;
					}
				}
			}
		}
	}

	//Only has to happen occasionally
	void FixedUpdate()
	{
		if(spawned)
		{
			int aliveCount = 0;

			foreach(var enemy in spawnedEnemies)
			{
				if (enemy.activeSelf)
					aliveCount++;
			}

			if(aliveCount <= 0)
			{
				if (OnEnemiesDefeated != null)
					OnEnemiesDefeated();

				if (LevelGenerator.Instance)
					LevelGenerator.Instance.ClearTile();

				spawned = false;
			}
		}
	}

	public void Spawn()
	{
		if (cleared)
			return;

		shouldSpawn = true;

		if (OnEnemiesSpawned != null)
			OnEnemiesSpawned();

		StartCoroutine(SpawnWithEffect());
	}

	IEnumerator SpawnWithEffect()
	{
		bool newEnemies = false;

		List<EnemySpawnPair> toSpawn = new List<EnemySpawnPair>();

		//If there were no undefeated enemies...
		if (undefeatedEnemies.Count <= 0)
		{
			//Cache spawn points for non-destructive editing
			List<Transform> potentialSpawns = new List<Transform>(spawnPoints);

			if (maxSpawns > 0)
			{
				//Check if min/max is valid
				if (maxSpawns <= minSpawns || minSpawns < 0)
					Debug.LogWarning("Enemy spawner min/max spawns mismatch!");
				else
				{
					//Randomly choose amount to spawn
					int spawnAmount = Random.Range(minSpawns, maxSpawns + 1);

					//Remove random elements until desired spawn amount is reached
					while (potentialSpawns.Count > spawnAmount)
						potentialSpawns.RemoveAt(Random.Range(0, potentialSpawns.Count));
				}
			}

			//Spawn a new random set
			foreach (Transform spawn in potentialSpawns)
			{
				GameObject enemyPrefab = Helper.GetRandomGameObjectByProbability(enemies);

				//Add to list to be spawned
				if (enemyPrefab)
					toSpawn.Add(new EnemySpawnPair(enemyPrefab, spawn));
			}

			newEnemies = true;
		}
		else
		{
			//Copy list of spawn points for non-destructive removal
			List<Transform> spawns = new List<Transform>(spawnPoints);

			//If there were enemies undefeated, just respawn them
			foreach (EnemySpawnPair undefeatedEnemy in undefeatedEnemies)
			{
				//Get random spawn and remove from temp list
				Transform spawn = spawns[Random.Range(0, spawns.Count)];
				spawns.Remove(spawn);

				//Enemy should spawn at new random point
				undefeatedEnemy.spawnPoint = spawn;

				toSpawn.Add(undefeatedEnemy);
			}
		}

		if (LevelVars.Instance)
		{
			if (LevelVars.Instance.enemySpawnEffect)
			{
				//Get particle effect from pool
				GameObject effectPrefab = LevelVars.Instance.enemySpawnEffect;

				foreach(EnemySpawnPair spawn in toSpawn)
				{
					//Spawn an effect where enemies will spawn
					if (spawn.prefab && spawn.spawnPoint)
					{
						GameObject effect = ObjectPooler.GetPooledObject(effectPrefab);

						if (effect)
						{
							effect.transform.position = spawn.spawnPoint.position;
							effect.transform.rotation = effectPrefab.transform.rotation;
						}
					}
				}
			}

			//Wait for set time
			yield return new WaitForSeconds(LevelVars.Instance.enemySpawnDelay);
		}

		//After delay, actually spawn the enemies
		if (shouldSpawn)
		{
			foreach (EnemySpawnPair spawn in toSpawn)
			{
				if (spawn.prefab && spawn.spawnPoint)
				{
					Vector3 pos = spawn.spawnPoint.position;

					GameObject enemy = ObjectPooler.GetPooledObject(spawn.prefab);

					if (enemy)
					{
						spawnedEnemies.Add(enemy);

						if (newEnemies)
							undefeatedEnemies.Add(spawn);

						enemy.transform.position = pos;

						NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();

						if (agent)
						{
							agent.enabled = true;
							agent.Warp(pos);
						}
					}
				}
			}

			spawned = true;
		}
	}

	public void Despawn()
	{
		spawned = false;

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

		//If tile was just cleared, don't allow respawning
		if (undefeatedEnemies.Count <= 0)
		{
			cleared = true;
		}
	}
}

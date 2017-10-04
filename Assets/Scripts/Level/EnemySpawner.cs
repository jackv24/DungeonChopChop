using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public class EnemySpawner : MonoBehaviour
{
	public delegate void NormalEvent();
	public event NormalEvent OnEnemiesDefeated;
	public event NormalEvent OnEnemiesSpawned;

	[HideInInspector]
	public bool spawned = false;

	[System.Serializable]
	public struct Profile
	{
		[System.Serializable]
		public class Spawn
		{
			public GameObject enemyPrefab;
			public Vector3 position;
		}

		public Spawn[] spawns;
	}

	public int currentProfileIndex = 0;
	public Profile[] profiles;
	private Profile currentProfile;

	private List<GameObject> spawnedEnemies = new List<GameObject>();
	private List<Profile.Spawn> undefeatedEnemies = new List<Profile.Spawn>();

	private bool shouldSpawn = false;

	private bool cleared = false;

	void Start()
	{
		if(profiles.Length > 0)
			currentProfile = profiles[currentProfileIndex];
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

		List<Profile.Spawn> toSpawn = new List<Profile.Spawn>();

		//If there were no undefeated enemies...
		if (undefeatedEnemies.Count <= 0)
		{
			//Select random profile
			currentProfile = profiles[Random.Range(0, profiles.Length)];

			toSpawn.AddRange(currentProfile.spawns);

			newEnemies = true;
		}
		else
		{
			toSpawn.AddRange(undefeatedEnemies);
		}

		if (LevelVars.Instance)
		{
			if (LevelVars.Instance.enemySpawnEffect)
			{
				//Get particle effect from pool
				GameObject effectPrefab = LevelVars.Instance.enemySpawnEffect;

				foreach(Profile.Spawn spawn in toSpawn)
				{
					//Spawn an effect where enemies will spawn
					if (spawn.enemyPrefab)
					{
						GameObject effect = ObjectPooler.GetPooledObject(effectPrefab);

						if (effect)
						{
							effect.transform.position = transform.TransformPoint(spawn.position);
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
			foreach (Profile.Spawn spawn in toSpawn)
			{
				if (spawn.enemyPrefab)
				{
					GameObject enemy = ObjectPooler.GetPooledObject(spawn.enemyPrefab);

					if (enemy)
					{
						spawnedEnemies.Add(enemy);

						if (newEnemies)
							undefeatedEnemies.Add(spawn);

						enemy.transform.position = transform.TransformPoint(spawn.position);

						NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();

						if (agent)
						{
							agent.enabled = true;
							agent.Warp(transform.TransformPoint(spawn.position));
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
		List<Profile.Spawn> toRemove = new List<Profile.Spawn>();

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
		foreach (Profile.Spawn e in toRemove)
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

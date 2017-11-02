using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
	public delegate void NormalEvent();
	public event NormalEvent OnEnemiesDefeated;
	public event NormalEvent OnEnemiesSpawned;

	public bool spawned = false;

    public bool waitForSpawnMessage = false;
    private bool receivedMessage = false;

    [System.Serializable]
	public class Profile
	{
		public bool randomised;

		[System.Serializable]
		public class Spawn
		{
			public GameObject enemyPrefab;
			public Vector3 position;

			public Spawn()
			{
				enemyPrefab = null;
				position = Vector3.zero;
			}

			public Spawn(GameObject prefab, Vector3 pos)
			{
				enemyPrefab = prefab;
				position = pos;
			}

			public Spawn(Spawn other)
			{
				enemyPrefab = other.enemyPrefab;
				position = other.position;
			}
		}

		public Spawn[] spawns;

		public Profile()
		{
			randomised = true;
		}

		public Profile(Profile other)
		{
			randomised = other.randomised;

			//Setup array of spawnpoints to match length of other, before copying data
			spawns = new Spawn[other.spawns.Length];

			//Copy spawn points using their copy constructors
			for(int i = 0; i < spawns.Length; i++)
			{
				spawns[i] = new Spawn(other.spawns[i]);
			}
		}
	}

	public int currentProfileIndex = 0;
	public List<Profile> profiles = new List<Profile>();
	private Profile currentProfile;

	public List<GameObject> spawnedEnemies = new List<GameObject>();
	private List<Profile.Spawn> undefeatedEnemies = new List<Profile.Spawn>();

	private bool shouldSpawn = false;

	private bool cleared = false;

	void Start()
	{
		if(profiles.Count > 0)
			currentProfile = profiles[currentProfileIndex];

        if (!LevelGenerator.Instance)
        {
            Spawn();
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
		//Don't spawn if tile already cleared
		if (cleared)
			return;

		//Don't spawn if we should wait for a message and have not received it
		if(waitForSpawnMessage && !receivedMessage)
            return;

        shouldSpawn = true;

		if (OnEnemiesSpawned != null)
			OnEnemiesSpawned();

		StartCoroutine(SpawnWithEffect());
	}

	public void SetSpawnMessage()
	{
		//If spawn was waiting for this message, then spawn
		if(waitForSpawnMessage && !receivedMessage)
		{
            receivedMessage = true;

            Spawn();
        }
	}

	IEnumerator SpawnWithEffect()
	{
		bool newEnemies = false;

		List<Profile.Spawn> toSpawn = new List<Profile.Spawn>();

		//If there were no undefeated enemies...
		if (undefeatedEnemies.Count <= 0)
		{
			//Select random profile
			if (profiles.Count > 0)
			{
				currentProfile = profiles[Random.Range(0, profiles.Count)];

				toSpawn.AddRange(currentProfile.spawns);
			}
			else
				Debug.LogWarning("No Enemy Spawner profiles assigned to " + gameObject.name);

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
		if (shouldSpawn && currentProfile != null)
		{
			//Randomise enemy positions if desired
			if (currentProfile.randomised)
			{
				//Create new list at the size of old list, and cache old list count for iterating
				List<Profile.Spawn> newSpawns = new List<Profile.Spawn>(toSpawn.Count);
				int count = toSpawn.Count;

				//Copy spawn positions to list, since toSpawn list will be removed from
				Vector3[] positions = new Vector3[count];
				for (int i = 0; i < count; i++)
					positions[i] = toSpawn[i].position;

				for(int i = 0; i < count; i++)
				{
					//Get random spawnpoint from remaining list, then remove
					Profile.Spawn spawn = toSpawn[Random.Range(0, toSpawn.Count)];
					toSpawn.Remove(spawn);

					//Create and add a new spawn with the new random enemy at the same point
					Profile.Spawn newSpawn = new Profile.Spawn(spawn.enemyPrefab, positions[i]);
					newSpawns.Add(newSpawn);
				}

				//Swap old empty list out for new randomised list
				toSpawn = newSpawns;
			}

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

        if (LevelGenerator.Instance)
            LevelGenerator.Instance.EnemiesSpawned();
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

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

	public bool sendDefeatedMessage = false;
	public string defeatedMessage = "";

    [System.Serializable]
	public class Profile
	{
		public bool randomised;

		[System.Serializable]
		public class Spawn
		{
			public GameObject enemyPrefab;
			public Vector3 position;
            public Vector3 rotation;

			public Spawn()
			{
				enemyPrefab = null;
				position = Vector3.zero;
                rotation = Vector3.zero;
			}

            public Spawn(GameObject prefab, Vector3 pos, Vector3 rot)
			{
				enemyPrefab = prefab;
				position = pos;
                rotation = rot;
			}

			public Spawn(Spawn other)
			{
				enemyPrefab = other.enemyPrefab;
				position = other.position;
                rotation = other.rotation;
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
	public List<Profile.Spawn> undefeatedEnemies = new List<Profile.Spawn>();

	private bool shouldSpawn = false;

	private bool cleared = false;

    private Coroutine spawnRoutine;

	void Start()
	{
		if(profiles.Count > 0)
			currentProfile = profiles[currentProfileIndex];

        if (!LevelGenerator.Instance)
        {
            Spawn();
        }
	}

	void Update()
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

				if (sendDefeatedMessage)
					BroadcastMessage(defeatedMessage);

				spawned = false;
			}
		}
	}

    public void Spawn(bool overrideChecks = false)
    {
        if (!overrideChecks)
        {
            //Don't spawn if tile already cleared
            if (cleared)
                return;
        }

        shouldSpawn = true;

		spawnRoutine = StartCoroutine(SpawnWithEffect());
	}

	public void SetSpawnMessage()
	{
		//If spawn was waiting for this message, then spawn
		if(waitForSpawnMessage && !receivedMessage)
		{
            receivedMessage = true;
        }
	}

    public void SetRandomProfile()
    {
        currentProfile = profiles[Random.Range(0, profiles.Count)];
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
                SetRandomProfile();

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
                    Profile.Spawn newSpawn = new Profile.Spawn(spawn.enemyPrefab, positions[i], spawn.rotation);
					newSpawns.Add(newSpawn);
				}

				//Swap old empty list out for new randomised list
				toSpawn = newSpawns;
			}

            //Pre-spawn enemies
            List<GameObject> preSpawned = new List<GameObject>(toSpawn.Count);

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

                        enemy.transform.position = transform.TransformPoint(new Vector3(spawn.position.x, 0, spawn.position.z));
                        enemy.transform.rotation = transform.rotation;
                        enemy.transform.Rotate(spawn.rotation);

                        //set up the enemy values
                        enemy.GetComponentInChildren<EnemyAttack>().ChangeHealth();
                        enemy.GetComponentInChildren<EnemyAttack>().ChangeStrength();
                        enemy.GetComponentInChildren<EnemyMove>().ChangeMoveSpeed();

                        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();

                        if (agent)
                        {
                            agent.enabled = true;
                            agent.Warp(transform.TransformPoint(spawn.position));
                        }

                        preSpawned.Add(enemy);
                    }
                }
            }

            yield return new WaitForEndOfFrame();

            foreach (GameObject obj in preSpawned)
                obj.SetActive(false);

            if(waitForSpawnMessage)
            {
                while (!receivedMessage)
                    yield return new WaitForEndOfFrame();
            }

            //Spawn effects and wait for delay
            if (LevelVars.Instance)
			{
				if (LevelVars.Instance.enemySpawnEffect)
				{
					//Get particle effect from pool
					GameObject effectPrefab = LevelVars.Instance.enemySpawnEffect;

					foreach (Profile.Spawn spawn in toSpawn)
					{
						//Spawn an effect where enemies will spawn
						if (spawn.enemyPrefab)
						{
							GameObject effect = ObjectPooler.GetPooledObject(effectPrefab);

							if (effect)
							{
								effect.transform.position = transform.TransformPoint(spawn.position);
							}
						}
					}
				}

				//Wait for set time
				yield return new WaitForSeconds(LevelVars.Instance.enemySpawnDelay);
			}

            foreach (GameObject obj in preSpawned)
                obj.SetActive(true);

            if (OnEnemiesSpawned != null)
                OnEnemiesSpawned();

            spawned = true;
		}

        if (LevelGenerator.Instance)
            LevelGenerator.Instance.EnemiesSpawned();
    }

	public void Despawn()
	{
		spawned = false;

        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileParticles : MonoBehaviour
{
	public Vector3 size;
	public Vector3 offset;

	[Tooltip("Doesn't need to be set. If set, overrides default GameObject transform.")]
	public Transform origin;

	private LevelTile tile;
	private GameObject spawnedParticles;

    public bool HasParticles { get { return spawnedParticles; } }

	private void Start()
	{
		tile = GetComponent<LevelTile>();

		if (tile)
		{
			//Subscribe to tile enter/exit events
			tile.OnTileEnter += OnEnter;
			tile.OnTileExit += OnExit;
		}
		else
			Debug.LogWarning("Tile particles script could not find LevelTile component.");
	}

	void OnEnter()
	{
		//Get correct prefab for biome
		GameObject prefab = GetParticles();

		if(prefab)
		{
			//Spawn particle prefab
			GameObject obj = ObjectPooler.GetPooledObject(prefab);
			obj.transform.position = origin ? origin.position : transform.position;
			obj.transform.rotation = origin ? origin.rotation : transform.rotation;
			spawnedParticles = obj;

			//Particle effect can have multiple children systems
			ParticleSystem[] systems = obj.GetComponentsInChildren<ParticleSystem>();

			foreach(ParticleSystem system in systems)
			{
				var shape = system.shape;
				
				//make emission shape fit set tile shape
				shape.position = offset;
				shape.scale = new Vector3(size.x, shape.scale.y, size.z);

				//Play particle system prewarmed
				system.Simulate(system.main.duration);
				system.Play();
			}

            if (ItemsManager.Instance.hasGoggles)
            {
                ParticleEmmissionScaler scaler = obj.GetComponent<ParticleEmmissionScaler>();

                if (scaler)
                {
                    scaler.multiplier = .3f;
                }
            }
		}
	}

	public void OnExit()
	{
		if(spawnedParticles)
		{
			ParticleSystem[] systems = spawnedParticles.GetComponentsInChildren<ParticleSystem>();

			foreach (ParticleSystem system in systems)
			{
				//Make sure to stop and clear particles, as they'll be reused
				system.Stop();
				system.Clear();
			}

			//Disable GameObject for object pooling
			spawnedParticles.SetActive(false);
			spawnedParticles = null;
		}
	}

	GameObject GetParticles()
	{
		GameObject obj = null;

		if (LevelVars.Instance)
		{
			switch (tile.Biome)
			{
				case LevelTile.Biomes.Grass:
					obj = LevelVars.Instance.grassParticles;
					break;
				case LevelTile.Biomes.Desert:
					obj = LevelVars.Instance.desertParticles;
					break;
				case LevelTile.Biomes.Forest:
					obj = LevelVars.Instance.forestParticles;
					break;
				case LevelTile.Biomes.Ice:
					obj = LevelVars.Instance.iceParticles;
					break;
				case LevelTile.Biomes.Fire:
					obj = LevelVars.Instance.fireParticles;
					break;
				case LevelTile.Biomes.Dungeon:
					obj = LevelVars.Instance.dungeonParticles;
					break;
			}
		}

		return obj;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.matrix = transform.localToWorldMatrix;

		Gizmos.DrawWireCube((origin ? origin.localPosition : Vector3.zero) + offset, size);
	}
}

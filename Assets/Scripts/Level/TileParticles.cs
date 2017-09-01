using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileParticles : MonoBehaviour
{
	public Vector3 size;
	public Vector3 offset;
	public Transform origin;

	private LevelTile tile;
	private GameObject spawnedParticles;

	private void Start()
	{
		tile = GetComponent<LevelTile>();

		if (tile)
		{
			tile.OnTileEnter += OnEnter;
			tile.OnTileExit += OnExit;
		}
		else
			Debug.LogWarning("Tile particles script could not find LevelTile component.");
	}

	void OnEnter()
	{
		GameObject prefab = GetParticles();

		if(prefab)
		{
			GameObject obj = ObjectPooler.GetPooledObject(prefab, origin ? origin.position : transform.position);
			spawnedParticles = obj;

			ParticleSystem[] systems = obj.GetComponentsInChildren<ParticleSystem>();

			foreach(ParticleSystem system in systems)
			{
				var shape = system.shape;
				
				shape.position = offset;
				shape.scale = size;

				system.Simulate(system.main.duration);
				system.Play();
			}
		}
	}

	void OnExit()
	{
		if(spawnedParticles)
		{
			ParticleSystem[] systems = spawnedParticles.GetComponentsInChildren<ParticleSystem>();

			foreach (ParticleSystem system in systems)
			{
				system.Stop();
				system.Clear();
			}

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
		Gizmos.DrawWireCube((origin ? origin.position : transform.position) + offset, size);
	}
}

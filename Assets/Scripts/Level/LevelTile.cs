using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTile : MonoBehaviour
{
	public delegate void NormalEvent();
	public event NormalEvent OnTileEnter;
	public event NormalEvent OnTileExit;

	public int index = 0;

	public List<Transform> doors = new List<Transform>();
	public BoxCollider layoutCollider;

	[Space()]
	public bool limitToHorizontal = false;

	[Space()]
	[Tooltip("The prefab to spawn when blocking off doors on this tile.")]
	public GameObject blockedDoorPrefab;
	public int minBlocks = 1;
	public int maxBlocks = 2;

	[Space()]
	public Transform tileOrigin;

    [Header("Alternate Tiles")]
    [Tooltip("The graphic that will be replaced by the below prefabs.")]
    public GameObject currentGraphic;
	private Biomes biome;
	public Biomes Biome { get { return biome; } }

    [Space()]
    public Helper.ProbabilityGameObject[] grassTiles;
	public Helper.ProbabilityGameObject[] desertTiles;
	public Helper.ProbabilityGameObject[] fireTiles;
	public Helper.ProbabilityGameObject[] iceTiles;
	public Helper.ProbabilityGameObject[] forestTiles;
	public Helper.ProbabilityGameObject[] dungeonTiles;

	private MapTile mapTile;

	private bool layoutReplaced = false;

	public enum Biomes
    {
        Grass,
        Fire,
        Desert,
		Ice,
		Forest,
		Dungeon
    }

	void Start()
	{
		mapTile = GetComponentInChildren<MapTile>();
	}

    public bool IsIntersecting(LayerMask layerMask)
	{
		bool intersected = false;

		BoxCollider col = layoutCollider;

		//Get list of all colliders in layer overlapping this one
		List<Collider> others = new List<Collider>(Physics.OverlapBox(col.center + col.transform.position, col.size / 2, col.transform.rotation, layerMask));

		//Remove self from list
		others.Remove(col);

		//If there are colliders in list, it is intersecting
		if (others.Count > 0)
		{
			intersected = true;
		}

		return intersected;
	}

	public Vector3 GetPosInTile(float width, float height)
	{
		BoxCollider col = layoutCollider;

		Vector3 pos = Vector3.zero;

		pos.x = Random.Range(col.bounds.min.x + width / 2, col.bounds.max.x - width / 2);
		pos.z = Random.Range(col.bounds.min.z + height / 2, col.bounds.max.z - height / 2);

		return pos;
	}

	public void BlockDoors(int ignoreIndex)
	{
		if (minBlocks < 0)
			minBlocks = 0;

		if (maxBlocks >= doors.Count)
			maxBlocks = doors.Count - 1;

		int blockAmount = Random.Range(minBlocks, maxBlocks + 1);

		int spawnedBlockAmount = 0;

		while(spawnedBlockAmount < blockAmount)
		{
			int index = Random.Range(0, doors.Count);

			//Don't block the door that this tile was spawned from
			if (index == ignoreIndex)
				continue;

            if (blockedDoorPrefab)
            {
                GameObject doorObj = Instantiate(blockedDoorPrefab, transform);
                doorObj.transform.position = doors[index].position;
                doorObj.transform.rotation = doors[index].rotation;
            }

			DestroyImmediate(doors[index].gameObject);
			doors.RemoveAt(index);

			spawnedBlockAmount++;
		}
	}

	public void ReplaceDoors()
	{
		for(int i = 0; i < doors.Count; i++)
		{
			GameObject oldObj = doors[i].gameObject;

			ReplaceWithPrefab replace = doors[i].GetComponent<ReplaceWithPrefab>();

			if(replace)
			{
				GameObject newObj = replace.Replace();

				if(newObj)
				{
					doors[i] = newObj.transform;

					DestroyImmediate(oldObj);
				}
			}
		}
	}

    public void Replace(Biomes biome)
    {
		this.biome = biome;

        if(currentGraphic)
        {
            GameObject newGraphic = null;

            //Select prefab to replace based on biome
            switch(biome)
            {
                case Biomes.Grass:
                    newGraphic = Helper.GetRandomGameObjectByProbability(grassTiles);
                    break;
                case Biomes.Desert:
                    newGraphic = Helper.GetRandomGameObjectByProbability(desertTiles);
                    break;
                case Biomes.Fire:
                    newGraphic = Helper.GetRandomGameObjectByProbability(fireTiles);
                    break;
				case Biomes.Ice:
					newGraphic = Helper.GetRandomGameObjectByProbability(iceTiles);
					break;
				case Biomes.Forest:
					newGraphic = Helper.GetRandomGameObjectByProbability(forestTiles);
					break;
				case Biomes.Dungeon:
					newGraphic = Helper.GetRandomGameObjectByProbability(dungeonTiles);
					break;
			}

            //If prefab exists then replace with it
            if(newGraphic)
            {
                GameObject obj = Instantiate(newGraphic, transform);
                obj.transform.localPosition = currentGraphic.transform.localPosition;
                obj.transform.localRotation = currentGraphic.transform.localRotation;

				//Destroy old graphic
				if (layoutReplaced)
					DestroyImmediate(currentGraphic);
				else
					layoutReplaced = true;

				currentGraphic = obj;
            }
        }

        LevelBlock[] levelBlocks = GetComponentsInChildren<LevelBlock>();

        for(int i = 0; i < levelBlocks.Length; i++)
        {
            levelBlocks[i].Replace(biome);
        }
    }

	public void EnsureBiomeContinuity()
	{
		bool connected = false;

		List<Biomes> biomeOptions = new List<Biomes>();

		//Check if this tile connects to any other tiles of the same biome
		foreach(Transform door in doors)
		{
			LevelDoor d = door.GetComponent<LevelDoor>();

			if (d.targetTile.biome == biome)
				connected = true;
			else
				biomeOptions.Add(d.targetTile.biome);
		}

		//If not, replace it with a random biome that it is connected to
		if(!connected && biomeOptions.Count > 0)
		{
			Biomes newBiome = biomeOptions[Random.Range(0, biomeOptions.Count)];

			Replace(newBiome);
		}
	}

	public void SetCurrent(LevelTile oldTile)
	{
		StartCoroutine(SwitchTile(this, oldTile));
	}

	public void ShowTile(bool inside, bool revealMap = true)
	{
		//Show/hide meshes
		Renderer[] rends = GetComponentsInChildren<Renderer>();

		foreach (Renderer rend in rends)
		{
			rend.enabled = inside;
		}

		if (revealMap)
		{
			//Show tile on map
			if (mapTile)
			{
				mapTile.biome = biome;

				if (inside)
					mapTile.SetInside();
				else
					mapTile.SetOutside();
			}

			if (LevelVars.Instance)
				LevelVars.Instance.levelData.SetClearedTile(index);

			//Show doors on map
			foreach (Transform door in doors)
			{
				LevelDoor d = door.GetComponent<LevelDoor>();

				if (d)
					d.ShowOnMap();
			}
		}
	}

	IEnumerator SwitchTile(LevelTile newTile, LevelTile oldTile)
	{
		if (FadeScreen.Instance)
			FadeScreen.Instance.FadeInOut();

		//Wait until screen has faded until moving to new tile
		if (FadeScreen.Instance)
			yield return new WaitForSeconds(FadeScreen.Instance.fadeOutTime);

		///Exit old tile
		if (oldTile)
		{
			//Despawn enemies
			EnemySpawner oldSpawner = oldTile.GetComponentInChildren<EnemySpawner>();
			if (oldSpawner)
				oldSpawner.Despawn();

			if (oldTile.OnTileExit != null)
				oldTile.OnTileExit();

			//Disable rendering of old tile
			oldTile.ShowTile(false);
		}

		///Enter new tile
		if (newTile)
		{
			//Enable rendering of new tile
			ShowTile(true);

			//Spawn enemies in new tile
			EnemySpawner newSpawner = GetComponentInChildren<EnemySpawner>();
			if (newSpawner)
				newSpawner.Spawn();

			if (OnTileEnter != null)
				OnTileEnter();

			//Move camera to new tile
			if (newTile.layoutCollider)
				CameraFollow.Instance.UpdateCameraBounds(newTile.layoutCollider.bounds);
		}

		//Set as current tile and call events
		LevelGenerator.Instance.currentTile = this;
		LevelGenerator.Instance.EnterTile();
	}
}

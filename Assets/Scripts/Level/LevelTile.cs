using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTile : MonoBehaviour
{
	public delegate void NormalEvent();
	public event NormalEvent OnTileEnter;

	public List<Transform> doors = new List<Transform>();
	public BoxCollider layoutCollider;

	[Space()]
	[Tooltip("The prefab to spawn when blocking off doors on this tile.")]
	public GameObject blockedDoorPrefab;
	public int minBlocks = 1;
	public int maxBlocks = 2;

	[Space()]
	public Transform tileOrigin;

	[Space()]
	public GameObject walls;

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

	//Shared material that all walls return to after fading (set by first wall fade)
	private static Material wallMaterial = null;

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
		if (oldTile)
		{
			StartCoroutine(FadeWalls(CameraFollow.Instance.wallFadeOutTime, CameraFollow.Instance.wallFadeInTime, CameraFollow.Instance.wallFadeDelay, this, oldTile));

			EnemySpawner oldSpawner = oldTile.GetComponentInChildren<EnemySpawner>();
			EnemySpawner newSpawner = GetComponentInChildren<EnemySpawner>();

			if (oldSpawner)
				oldSpawner.Despawn();

			if (newSpawner)
				newSpawner.Spawn();

			if (oldTile.mapTile)
			{
				oldTile.mapTile.biome = oldTile.biome;
				oldTile.mapTile.SetOutside();
			}

			LevelGenerator.Instance.currentTile = this;
			LevelGenerator.Instance.EnterTile();
		}
		else
		{
			walls.SetActive(true);

			if (layoutCollider)
				CameraFollow.Instance.UpdateCameraBounds(layoutCollider.bounds);
		}

		//Show tile on map
		if (mapTile)
		{
			mapTile.biome = biome;
			mapTile.SetInside();
		}

		//Show doors on map
		foreach(Transform door in doors)
		{
			LevelDoor d = door.GetComponent<LevelDoor>();

			if (d)
				d.ShowOnMap();
		}

		if (OnTileEnter != null)
			OnTileEnter();
	}

	IEnumerator FadeWalls(float fadeOutTime, float fadeInTime, float wallFadeDelay, LevelTile newTile, LevelTile oldTile)
	{
		GameObject newWalls = newTile.walls;
		GameObject oldWalls = oldTile.walls;

		//Get mesh renderers
		MeshRenderer[] newRends = newWalls.GetComponentsInChildren<MeshRenderer>();
		MeshRenderer[] oldRends = oldWalls.GetComponentsInChildren<MeshRenderer>();

		//Cache shared material to return at end
		if(wallMaterial == null)
			wallMaterial = newRends[0].sharedMaterial;

		//Fade old out
		{
			Material oldMat = oldRends[0].material;

			for (int i = 0; i < oldRends.Length; i++)
				oldRends[i].sharedMaterial = oldMat;

			Color startColor = wallMaterial.GetColor("_TintColor");
			Color endColor = oldMat.GetColor("_TintColor");
			endColor.a = 0;

			float elapsedTime = 0;

			while (elapsedTime <= fadeOutTime)
			{
				yield return new WaitForEndOfFrame();
				elapsedTime += Time.deltaTime;

				oldMat.SetColor("_TintColor", Color.Lerp(startColor, endColor, elapsedTime / fadeOutTime));
			}

			oldWalls.SetActive(false);
		}

		//Fade new in
		{
			Material newMat = newRends[0].material;

			for (int i = 0; i < newRends.Length; i++)
				newRends[i].sharedMaterial = newMat;

			Color startColor = wallMaterial.GetColor("_TintColor");
			startColor.a = 0;
			Color endColor = newMat.GetColor("_TintColor");

			newMat.SetColor("_TintColor", startColor);

			newWalls.SetActive(true);

			if(newTile.layoutCollider)
				CameraFollow.Instance.UpdateCameraBounds(newTile.layoutCollider.bounds);

			yield return new WaitForSeconds(wallFadeDelay);

			float elapsedTime = 0;

			while (elapsedTime <= fadeInTime)
			{
				yield return new WaitForEndOfFrame();
				elapsedTime += Time.deltaTime;

				newMat.SetColor("_TintColor", Color.Lerp(startColor, endColor, elapsedTime / fadeInTime));
			}

		}

		//Restore shared material
		foreach(MeshRenderer rend in oldRends)
			rend.sharedMaterial = wallMaterial;

		foreach (MeshRenderer rend in newRends)
			rend.sharedMaterial = wallMaterial;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTile : MonoBehaviour
{
	public List<Transform> doors = new List<Transform>();
	public BoxCollider[] layoutColliders;

	[Space()]
	[Tooltip("The prefab to spawn when blocking off doors on this tile.")]
	public GameObject blockedDoorPrefab;
	public int minBlocks = 1;
	public int maxBlocks = 2;

	[Space()]
	public Transform tileOrigin;

    [Header("Alternate Graphics")]
    [Tooltip("The graphic that will be replaced by the below prefabs.")]
    public GameObject defaultGraphic;

    [Space()]
    public GameObject grassGraphicPrefab;
    public GameObject desertGraphicPrefab;
    public GameObject fireGraphicPrefab;

    public enum Biomes
    {
        Grass,
        Fire,
        Desert,
		Ice
    }

    public bool IsIntersecting(LayerMask layerMask)
	{
		bool intersected = false;

		//Check every layout collider
		foreach(BoxCollider col in layoutColliders)
		{
			//Get list of all colliders in layer overlapping this one
			List<Collider> others = new List<Collider>(Physics.OverlapBox(col.center + col.transform.position, col.size / 2, col.transform.rotation, layerMask));

			//Remove self from list
			others.Remove(col);

			//If there are colliders in list, it is intersecting
			if (others.Count > 0)
			{
				intersected = true;

				break;
			}
		}

		return intersected;
	}

	public void BlockDoors()
	{
		if (maxBlocks > doors.Count)
			maxBlocks = doors.Count;

		int blockAmount = Random.Range(minBlocks, maxBlocks + 1);

		int spawnedBlockAmount = 0;

		while(spawnedBlockAmount < blockAmount)
		{
			int index = Random.Range(0, doors.Count);

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

    public void Replace(Biomes biome)
    {
        if(defaultGraphic)
        {
            GameObject newGraphic = null;

            //Select prefab to replace based on biome
            switch(biome)
            {
                case Biomes.Grass:
                    newGraphic = grassGraphicPrefab;
                    break;
                case Biomes.Desert:
                    newGraphic = desertGraphicPrefab;
                    break;
                case Biomes.Fire:
                    newGraphic = fireGraphicPrefab;
                    break;
            }

            //If prefab exists then replace with it
            if(newGraphic)
            {
                GameObject obj = Instantiate(newGraphic, transform);
                obj.transform.localPosition = defaultGraphic.transform.localPosition;
                obj.transform.localRotation = defaultGraphic.transform.localRotation;

                //Destroy layout graphic
                DestroyImmediate(defaultGraphic);
            }
        }

        LevelBlock[] levelBlocks = GetComponentsInChildren<LevelBlock>();

        for(int i = 0; i < levelBlocks.Length; i++)
        {
            levelBlocks[i].Replace(biome);
        }
    }

	public void EnableStaticBatching()
	{
		//Only combine meshes in-game (prevents saved scene files becoming too large)
		if(Application.isPlaying)
			StaticBatchingUtility.Combine(gameObject);
	}

	public void SetCurrent(LevelTile oldTile)
	{
		//Add current tile to camera targets
		CameraFollow.Instance.targets.Add(tileOrigin ? tileOrigin : transform);

		//Remove old tile from camera targets
		if (oldTile)
			CameraFollow.Instance.targets.Remove(oldTile.tileOrigin ? oldTile.tileOrigin : oldTile.transform);
	}
}

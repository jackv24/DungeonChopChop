using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTile : MonoBehaviour
{
	public List<Transform> doors = new List<Transform>();
	public BoxCollider[] layoutColliders;

	[Space()]
	public GameObject blockedDoorPrefab;
	public int minBlocks = 1;
	public int maxBlocks = 2;

	[System.Serializable]
	public class Requirement
	{
		public enum Type
		{
			Door,
			Dungeon
		}

		public Type type;

		public int amount = 1;
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
		if(!blockedDoorPrefab)
		{
			Debug.LogWarning(gameObject.name + " does not have a door block prefab assigned!");
			return;
		}

		if (maxBlocks > doors.Count)
			maxBlocks = doors.Count;

		int blockAmount = Random.Range(minBlocks, maxBlocks + 1);

		int spawnedBlockAmount = 0;

		while(spawnedBlockAmount < blockAmount)
		{
			int index = Random.Range(0, doors.Count);

			GameObject doorObj = (GameObject)Instantiate(blockedDoorPrefab, transform);
			doorObj.transform.position = doors[index].position;
			doorObj.transform.rotation = doors[index].rotation;

			DestroyImmediate(doors[index].gameObject);
			doors.RemoveAt(index);

			spawnedBlockAmount++;
		}
	}

	public void EnableStaticBatching()
	{
		//Only combine meshes in-game (prevents saved scene files becoming too large)
		if(Application.isPlaying)
			StaticBatchingUtility.Combine(gameObject);
	}
}

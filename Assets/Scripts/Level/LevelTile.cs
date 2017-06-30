﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTile : MonoBehaviour
{
	public Transform[] doors;
	public Collider[] layoutColliders;

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

	[Space()]
	public List<Requirement> requirements = new List<Requirement>();

	public Transform GetRandomDoor()
	{
		int index = Random.Range(0, doors.Length);

		return doors[index];
	}

	public bool IsIntersecting(LayerMask layerMask)
	{
		bool intersected = false;

		//Check every layout collider
		foreach(Collider col in layoutColliders)
		{
			//Get list of all colliders in layer overlapping this one
			List<Collider> others = new List<Collider>(Physics.OverlapBox(col.bounds.center, col.bounds.size / 2, col.transform.rotation, layerMask));

			//Remove self from list
			others.Remove(col);

			Debug.Log(others.Count);

			//If there are colliders in list, it is intersecting
			if (others.Count > 0)
			{
				intersected = true;

				break;
			}
		}

		Debug.Log(intersected);

		return intersected;
	}
}

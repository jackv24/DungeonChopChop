using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTile : MonoBehaviour
{
	public Transform[] doors;

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
}

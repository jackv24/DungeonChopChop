using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTile : MonoBehaviour
{
	public Transform[] doors;
	public BoxCollider[] layoutColliders;

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

	public void EnableStaticBatching()
	{
		//Only combine meshes in-game (prevents saved scene files becoming too large)
		if(!Application.isEditor)
			StaticBatchingUtility.Combine(gameObject);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestSpawn : MonoBehaviour
{
	public enum Type
	{
		Normal,
		Special
	}

	public Type type;

	public bool Spawn()
	{
		if(LevelVars.Instance)
		{
			GameObject prefab = null;

			switch(type)
			{
				case Type.Normal:
					prefab = LevelVars.Instance.normalChestPrefab;
					break;
				case Type.Special:
					prefab = LevelVars.Instance.specialChestPrefab;
					break;
			}

			if (prefab)
			{
				GameObject obj = Instantiate(prefab, transform.parent);
				obj.transform.localPosition = transform.localPosition;

				return true;
			}
			else
				Debug.LogWarning("Chest could not spawn, no prefab assigned in LevelVars!");
		}

		return false;
	}
}

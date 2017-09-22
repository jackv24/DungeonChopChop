using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonKeyTile : MonoBehaviour
{
	public enum Type
	{
		Key,
		Chest
	}

	public GameObject keyReplaceTile;
	public GameObject chestReplaceTile;

	public GameObject Replace(Type replaceType)
	{
		GameObject replaceTile = replaceType == Type.Key ? keyReplaceTile : chestReplaceTile;

		if(replaceTile)
		{
			//Spawn new tile in this one's place
			GameObject obj = (GameObject)Instantiate(replaceTile, transform.parent);
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localRotation = Quaternion.identity;

			//Remove this tile graphic
			Destroy(gameObject);

			return obj;
		}

		return null;
	}
}

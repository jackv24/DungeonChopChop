using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonTile : MonoBehaviour
{
	public GameObject replaceTile;

	public GameObject Replace()
	{
		if(replaceTile)
		{
			//Spawn new tile in this one's place
			GameObject obj = (GameObject)Instantiate(replaceTile, transform.parent);
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localRotation = Quaternion.identity;

			//Remove this tile graphic
			Destroy(gameObject);

			//Make sure this dungeon tile wont be replaced with a special tile
            SpecialTile special = obj.GetComponentInParent<SpecialTile>();
			if(special)
                DestroyImmediate(special);

            return obj;
		}

		return null;
	}
}

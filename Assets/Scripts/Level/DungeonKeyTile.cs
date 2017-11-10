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

	private LevelTile tile;

    private void Awake()
    {
        tile = GetComponentInParent<LevelTile>();
    }

	public GameObject Replace(Type replaceType)
	{
		GameObject replaceTile = replaceType == Type.Key ? keyReplaceTile : chestReplaceTile;

		if(replaceTile)
		{
			//Spawn new tile in this one's place
			GameObject obj = (GameObject)Instantiate(replaceTile, transform.parent);
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localRotation = Quaternion.identity;

            //Reset rotation +180 to face forward
            float angle = obj.transform.eulerAngles.y + 180;
            //Rotate around tile origion so at to keep position correcty
            obj.transform.RotateAround(tile.tileOrigin.position, Vector3.up, -angle);

			//Remove this tile graphic
			Destroy(gameObject);

			return obj;
		}

		return null;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonKeyTile : MonoBehaviour
{
	private LevelTile tile;

    private void Awake()
    {
        tile = GetComponentInParent<LevelTile>();
    }

	public GameObject Replace(GameObject tilePrefab)
	{
		if(tilePrefab)
		{
			//Spawn new tile in this one's place
			GameObject obj = (GameObject)Instantiate(tilePrefab, transform.parent);
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

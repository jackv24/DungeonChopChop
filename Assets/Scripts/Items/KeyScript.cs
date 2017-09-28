using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyScript : MonoBehaviour
{
	public enum Type { Normal, Dungeon }
	public Type type = Type.Normal;

	public InventoryItem keyItem;

    void OnTriggerEnter(Collider col)
    {
        //checks if the player collides with the item
        if (col.tag == "Player1" || col.tag == "Player2") {
			Pickup(col.GetComponent<PlayerInformation>());
        }
    }

	void Pickup(PlayerInformation playerInfo)
	{
		if (type == Type.Normal)
			ItemsManager.Instance.Keys += 1 * (int)playerInfo.GetCharmFloat("keyMultiplier");
		else
			ItemsManager.Instance.DungeonKeys += 1;

		gameObject.SetActive(false);
	}
}

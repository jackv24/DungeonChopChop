using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyScript : MonoBehaviour
{
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
		ItemsManager.Instance.Keys += 1 * (int)playerInfo.GetCharmFloat("keyMultiplier");
		gameObject.SetActive(false);
	}
}

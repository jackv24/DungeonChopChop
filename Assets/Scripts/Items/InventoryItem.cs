using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Data/Items/Item")]
public class InventoryItem : BaseItem
{
	public GameObject itemPrefab;
	public bool usePrefabForPickup = false;

    [System.Serializable]
    public class Item
    {
        public string itemKey = "";
        public float floatValue = 1.0f;
    }

    public Item[] items;

	public override void Pickup(PlayerInformation playerInfo)
	{
        base.Pickup(playerInfo);

        if (items.Length > 0)
        {
            foreach (Item itemFloat in items)
            {
                if (itemFloat.itemKey != "")
                {
                    playerInfo.SetItemFloat (itemFloat.itemKey, itemFloat.floatValue);
                }
            }
        }
	}

	public override void Drop(PlayerInformation playerInfo)
	{
		base.Drop(playerInfo);
	}
}

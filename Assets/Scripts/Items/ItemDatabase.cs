using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Database", menuName = "Data/Items/Item Database")]
public class ItemDatabase : ScriptableObject
{
	[System.Serializable]
	public class Group
	{
		[System.Serializable]
		public class Tier
		{
			public List<InventoryItem> items = new List<InventoryItem>();
		}

		public List<Tier> tiers = new List<Tier>();
	}

	public enum ItemType
	{
		Consumables,
		Weapons,
		Armour
	}

	public Group consumables;
	public Group weapons;
	public Group armour;

	public InventoryItem GetItem(ItemType type, int tier)
	{
		Group group = null;

		switch(type)
		{
			case ItemType.Consumables:
				group = consumables;
				break;
			case ItemType.Weapons:
				group = weapons;
				break;
			case ItemType.Armour:
				group = armour;
				break;
		}

		if(group != null)
		{
			if (tier > group.tiers.Count - 1)
				tier = group.tiers.Count - 1;

			if (tier >= 0)
			{
				Group.Tier itemTier = group.tiers[tier];

				InventoryItem item = itemTier.items[Random.Range(0, itemTier.items.Count)];

				if (item)
					return item;
				else
					Debug.LogError("Empty item found in database!");
			}
		}

		return null;
	}
}

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
			public List<BaseItem> items = new List<BaseItem>();
		}

		public List<Tier> tiers = new List<Tier>();
	}

	public enum ItemType
	{
		Consumables,
		Weapons,
		Armour,
		Charms,
		Map
	}

	public Group consumables;
	public Group weapons;
	public Group armour;
	public Group charms;
	public Group maps;

	public BaseItem GetItem(ItemType type, int tier, List<BaseItem> excludeItems = null)
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
			case ItemType.Charms:
				group = charms;
				break;
			case ItemType.Map:
				group = maps;
				break;
		}

		if(group != null)
		{
			if (tier > group.tiers.Count - 1)
				tier = group.tiers.Count - 1;

			if (tier >= 0)
			{
				Group.Tier itemTier = group.tiers[tier];

				List<BaseItem> possibleItems = new List<BaseItem>(itemTier.items);

				//Remove items that are excluded
				foreach(BaseItem i in excludeItems)
				{
					if (possibleItems.Contains(i))
						possibleItems.Remove(i);
				}

				//if resulting list is empty, just use any item
				if(possibleItems.Count <= 0)
					possibleItems = new List<BaseItem>(itemTier.items);

				BaseItem item = possibleItems[Random.Range(0, possibleItems.Count)];

				if (item)
					return item;
				else
					Debug.LogError("Empty item found in database!");
			}
		}

		return null;
	}
}

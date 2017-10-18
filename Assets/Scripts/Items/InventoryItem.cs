using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum ArmourType
{
    Boots,
    ChestPiece,
    Helmet
}

[CreateAssetMenu(fileName = "New Item", menuName = "Data/Items/Item")]
public class InventoryItem : BaseItem
{
	public GameObject itemPrefab;
	public bool usePrefabForPickup = false;
    public Charm charm;
    public ArmourType armourType;

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

                if (charm)
                {
                    foreach (Charm.CharmFloat charmFloat in charm.charmFloats)
                    {
                        if (charmFloat.floatKey != "")
                        {
                            playerInfo.SetItemCharmFloat (charmFloat.floatKey, charmFloat.floatValue);
                        }
                    }
                }
            }
        }
	}

	public override void Drop(PlayerInformation playerInfo)
	{
        base.Drop(playerInfo);

        foreach (Charm.CharmFloat charmFloat in charm.charmFloats)
        {
            if (charmFloat.floatKey != "")
            {
                playerInfo.SetItemCharmFloat (charmFloat.floatKey, 1.0f);
                playerInfo.RemoveItemCharmFloats(charmFloat.floatKey);
            }
        }

        foreach (Charm.CharmBool charmBool in charm.charmBools)
        {
            if (charmBool.boolKey != "")
            {
                playerInfo.SetItemCharmBool(charmBool.boolKey, false);
                playerInfo.RemoveItemCharmBools(charmBool.boolKey);
            }
        }
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum Rarity
{
    Common,
    Rare,
    Mythic,
    Legendary
}

[System.Serializable]
public enum ArmourType
{
    Nothing,
    Boots,
    ChestPiece,
}

[CreateAssetMenu(fileName = "New Item", menuName = "Data/Items/Item")]
public class InventoryItem : BaseItem
{
	public GameObject itemPrefab;

    [System.Serializable]
    public class Offset
    {
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;

        public Offset(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }
    }
    [Space()]
    public Offset shopOffset = new Offset(Vector3.zero, new Vector3(-60, 180, 0), Vector3.one);
    public Offset popupOffset = new Offset(Vector3.zero, new Vector3(0, 180, 0), Vector3.one);

    [Space()]
    public bool usePrefabForPickup = false;
    public Charm charm;
    public ArmourType armourType;
    public Rarity rarity;

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

        if (charm)
        {
            foreach (Charm.CharmFloat charmFloat in charm.charmFloats)
            {
                if (charmFloat.floatKey != "")
                {
                    playerInfo.SetItemCharmFloat(charmFloat.floatKey, 1.0f);
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
}

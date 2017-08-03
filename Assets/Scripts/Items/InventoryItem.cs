using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Data/Items/Item")]
public class InventoryItem : BaseItem
{
	public override void Pickup(PlayerInformation playerInfo)
	{
		base.Pickup(playerInfo);
	}

	public override void Drop(PlayerInformation playerInfo)
	{
		base.Drop(playerInfo);
	}
}

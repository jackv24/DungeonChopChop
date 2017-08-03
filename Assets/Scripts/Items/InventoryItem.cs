using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Data/Items/Item")]
public class InventoryItem : BaseItem
{
	public override void Pickup()
	{
		base.Pickup();
	}

	public override void Drop()
	{
		base.Drop();
	}
}

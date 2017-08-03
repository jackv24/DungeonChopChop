using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Charm", menuName = "Data/Items/Charm")]
public class Charm : BaseItem
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Charm", menuName = "Data/Items/Charm")]
public class Charm : BaseItem
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

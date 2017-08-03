using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Charm", menuName = "Data/Items/Magnetic Charm")]
public class MagneticCharm : Charm
{
	public float magnetizeRadius = 5;
	public float absorbSpeed = 5;
	public LayerMask layerMask;

	public override void Pickup(PlayerInformation playerInfo)
	{
		base.Pickup(playerInfo);
		playerInfo.canMagnetize = true;
	}

	public override void Drop(PlayerInformation playerInfo)
	{
		base.Drop(playerInfo);

		playerInfo.canMagnetize = false;
	}
}

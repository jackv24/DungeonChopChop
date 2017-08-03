using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Charm", menuName = "Data/Items/Dash Charm")]
public class DashCharm : Charm
{
	public AnimationCurve dashCurve;
	public float dashDistance = 5;
	public float dashCooldown = 4;
	public float dashTime = 1;

	public override void Pickup(PlayerInformation playerInfo)
	{
		base.Pickup(playerInfo);
		playerInfo.canDash = true;
	}

	public override void Drop(PlayerInformation playerInfo)
	{
		base.Drop(playerInfo);

		playerInfo.canDash = false;
	}
}

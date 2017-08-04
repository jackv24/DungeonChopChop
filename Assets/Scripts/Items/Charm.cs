using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Charm", menuName = "Data/Items/Charm")]
public class Charm : BaseItem
{
	[Header("Multipler")]
	public string multiplierKey = "";
	public float multiplierValue = 1.0f;
	[Space()]
	public bool global = false;

	public override void Pickup(PlayerInformation playerInfo)
	{
		base.Pickup(playerInfo);

		if (multiplierKey != "")
		{
			if(global)
				GameManager.Instance.SetGlobalMultiplier(multiplierKey, multiplierValue);
			else
				playerInfo.SetMultiplier(multiplierKey, multiplierValue);
		}
	}

	public override void Drop(PlayerInformation playerInfo)
	{
		base.Drop(playerInfo);

		if (multiplierKey != "")
		{
			if (global)
				GameManager.Instance.SetGlobalMultiplier(multiplierKey, 1.0f);
			else
				playerInfo.SetMultiplier(multiplierKey, 1.0f);
		}
	}
}

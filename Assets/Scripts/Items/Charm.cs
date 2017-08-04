using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Charm", menuName = "Data/Items/Charm")]
public class Charm : BaseItem
{
	[System.Serializable]
	public class Multiplier
	{
		public string multiplierKey = "";
		public float multiplierValue = 1.0f;
		[Space()]
		public bool global = false;
	}

	[Header("Multipliers")]
	public Multiplier[] multipliers;

	public override void Pickup(PlayerInformation playerInfo)
	{
		base.Pickup(playerInfo);

		foreach (Multiplier multiplier in multipliers)
		{
			if (multiplier.multiplierKey != "") {
				if (multiplier.global)
					GameManager.Instance.SetGlobalMultiplier (multiplier.multiplierKey, multiplier.multiplierValue);
				else
					playerInfo.SetMultiplier (multiplier.multiplierKey, multiplier.multiplierValue);
			}
		}
	}

	public override void Drop(PlayerInformation playerInfo)
	{
		base.Drop(playerInfo);

		foreach (Multiplier multiplier in multipliers)
		{
			if (multiplier.multiplierKey != "") {
				if (multiplier.global)
					GameManager.Instance.SetGlobalMultiplier (multiplier.multiplierKey, 1.0f);
				else
					playerInfo.SetMultiplier (multiplier.multiplierKey, 1.0f);
			}
		}
	}
}

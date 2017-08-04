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

	[System.Serializable]
	public class Chance
	{
		public string chanceKey = "";
		[Tooltip("0 = no chance, 50 = 50% chance")]
		public float chanceValue = 50.0f;
		[Space()]
		public bool global = false;
	}

	[Header("Multipliers")]
	public Multiplier[] multipliers;
	[Header("Random Chances")]
	public Chance[] chances;

	public override void Pickup(PlayerInformation playerInfo)
	{
		base.Pickup(playerInfo);

		foreach (Multiplier multiplier in multipliers)
		{
			if (multiplier.multiplierKey != "") 
			{
				if (multiplier.global)
					GameManager.Instance.SetGlobalMultiplier (multiplier.multiplierKey, multiplier.multiplierValue);
				else
					playerInfo.SetMultiplier (multiplier.multiplierKey, multiplier.multiplierValue);
			}
		}

		foreach (Chance chance in chances)
		{
			if (chance.chanceKey != "") 
			{
				if (chance.global)
					GameManager.Instance.SetGlobalMultiplier (chance.chanceKey, chance.chanceValue);
				else
					playerInfo.SetChance (chance.chanceKey, chance.chanceValue);
			}
		}
	}

	public override void Drop(PlayerInformation playerInfo)
	{
		base.Drop(playerInfo);

		foreach (Multiplier multiplier in multipliers)
		{
			if (multiplier.multiplierKey != "") 
			{
				if (multiplier.global)
					GameManager.Instance.SetGlobalMultiplier (multiplier.multiplierKey, 1.0f);
				else
					playerInfo.SetMultiplier (multiplier.multiplierKey, 1.0f);
			}
		}

		foreach (Chance chance in chances)
		{
			if (chance.chanceKey != "") 
			{
				if (chance.global)
					GameManager.Instance.SetGlobalMultiplier (chance.chanceKey, 1.0f);
				else
					playerInfo.SetChance (chance.chanceKey, 1.0f);
			}
		}

	}
}

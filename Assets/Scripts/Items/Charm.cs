using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Charm", menuName = "Data/Items/Charm")]
public class Charm : BaseItem
{
	[System.Serializable]
	public class Multiplier
	{
		public string multiplierKey = "";
		public float multiplierValue = 1.0f;
		[Space ()]
		public bool global = false;
	}

	[System.Serializable]
	public class Chance
	{
		public string chanceKey = "";
		[Tooltip ("0 = no chance, 50 = 50% chance")]
		public float chanceValue = 50.0f;
		[Space ()]
		public bool global = false;
	}

	[System.Serializable]
	public class Tick
	{
		public string tickKey = "";
		public float tickValue = 0.5f;
		[Space ()]
		public bool global = false;
	}

	[System.Serializable]
	public class Radial
	{
		public string radialKey = "";
		public float radialValue = 0.5f;
		[Space ()]
		public bool global = false;
	}

	[Header ("Multipliers")]
	public Multiplier[] multipliers;
	[Header ("Random Chances")]
	public Chance[] chances;
	[Header ("Something per tick")]
	public Tick[] ticks;
	[Header ("Layer Mask")]
	public Radial[] radials;
	public LayerMask layerMask;

	public override void Pickup (PlayerInformation playerInfo)
	{
		base.Pickup (playerInfo);

		if (multipliers.Length > 0)
		{
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
		}

		if (chances.Length > 0)
		{
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

		if (ticks.Length > 0)
		{
			foreach (Tick tick in ticks)
			{
				if (tick.tickKey != "")
				{
					if (tick.global)
						GameManager.Instance.SetGlobalMultiplier (tick.tickKey, tick.tickValue);
					else
						playerInfo.SetTick (tick.tickKey, tick.tickValue);
				}
			}
		}

		if (radials.Length > 0)
		{
			foreach (Radial radial in radials)
			{
				if (radial.radialKey != "")
				{
					if (radial.global)
						GameManager.Instance.SetGlobalMultiplier (radial.radialKey, radial.radialValue);
					else
						playerInfo.SetRadial (radial.radialKey, radial.radialValue);
				}
			}
		}
		playerInfo.SetLayerMask (layerMask);
	}

	public override void Drop (PlayerInformation playerInfo)
	{
		base.Drop (playerInfo);

		if (multipliers.Length > 0)
		{
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
		}

		if (chances.Length > 0)
		{
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

		if (ticks.Length > 0)
		{
			foreach (Tick tick in ticks)
			{
				if (tick.tickKey != "")
				{
					if (tick.global)
						GameManager.Instance.SetGlobalMultiplier (tick.tickKey, 1.0f);
					else
						playerInfo.SetTick (tick.tickKey, 1.0f);
				}
			}
		}

		if (radials.Length > 0)
		{
			foreach (Radial radial in radials)
			{
				if (radial.radialKey != "")
				{
					if (radial.global)
						GameManager.Instance.SetGlobalMultiplier (radial.radialKey, 0);
					else
						playerInfo.SetRadial (radial.radialKey, 0);
				}
			}
		}
	}
}

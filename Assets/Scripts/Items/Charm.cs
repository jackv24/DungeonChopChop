using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Charm", menuName = "Data/Items/Charm")]
public class Charm : BaseItem
{
	[System.Serializable]
	public class CharmFloat
	{
		public string floatKey = "";
		public float floatValue = 1.0f;
		[Space ()]
		public bool global = false;
	}

	[System.Serializable]
	public class CharmBool
	{
		public string boolKey = "";
		public bool boolValue = false;
		[Space()]
		public bool global = false;
	}

	[Header ("CharmFloats")]
	public CharmFloat[] charmFloats;

	[Header("CharmBools")]
	public CharmBool[] charmBools;

	public LayerMask layerMask;

	public override void Pickup (PlayerInformation playerInfo)
	{
		base.Pickup (playerInfo);

		if (charmFloats.Length > 0)
		{
			foreach (CharmFloat charmFloat in charmFloats)
			{
				if (charmFloat.floatKey != "")
				{
					if (charmFloat.global)
						GameManager.Instance.SetGlobalMultiplier (charmFloat.floatKey, charmFloat.floatValue);
					else
						playerInfo.SetCharmFloat (charmFloat.floatKey, charmFloat.floatValue);
				}
			}
		}

		if (charmBools.Length > 0)
		{
			foreach (CharmBool charmBool in charmBools)
			{
				if (charmBool.boolKey != "")
				{
					if (charmBool.global)
						GameManager.Instance.SetGlobalBool (charmBool.boolKey, charmBool.boolValue);
					else
						playerInfo.SetCharmBool (charmBool.boolKey, charmBool.boolValue);
				}
			}
		}

		playerInfo.SetLayerMask (layerMask);
	}

	public override void Drop (PlayerInformation playerInfo)
	{
		base.Drop (playerInfo);

		if (charmFloats.Length > 0)
		{
			foreach (CharmFloat charmFloat in charmFloats)
			{
				if (charmFloat.floatKey != "")
				{
					if (charmFloat.global)
						GameManager.Instance.SetGlobalMultiplier (charmFloat.floatKey, 1.0f);
					else
						playerInfo.SetCharmFloat (charmFloat.floatKey, 1.0f);
				}
			}
		}

		if (charmBools.Length > 0)
		{
			foreach (CharmBool charmBool in charmBools)
			{
				if (charmBool.boolKey != "")
				{
					if (charmBool.global)
						GameManager.Instance.SetGlobalBool (charmBool.boolKey, false);
					else
						playerInfo.SetCharmBool (charmBool.boolKey, false);
				}
			}
		}

		GameObject obj = ObjectPooler.GetPooledObject(LevelVars.Instance.droppedCharmPrefab);
		obj.transform.position = playerInfo.transform.position + Vector3.up;

		CharmPickup pickup = obj.GetComponent<CharmPickup>();

		if(pickup)
			pickup.representingCharm = this;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItem : ScriptableObject
{
	public string displayName = "";

	public virtual void Pickup(PlayerInformation playerInfo)
	{

	}

	public virtual void Drop(PlayerInformation playerInfo)
	{

	}
}

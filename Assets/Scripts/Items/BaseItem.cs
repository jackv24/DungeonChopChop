using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItem : ScriptableObject
{
	public string displayName = "";
    public string itemInfo = "";
	public Sprite itemIcon;

	public int cost = 0;

	public virtual void Pickup(PlayerInformation playerInfo)
	{
        
	}

	public virtual void Drop(PlayerInformation playerInfo)
	{

	}
}

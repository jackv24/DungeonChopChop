﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
	[HideInInspector]
    public bool opened = false;
    public bool requireKeys = false;

	public KeyScript.Type keyType = KeyScript.Type.Normal;

	public Helper.ProbabilityItem[] possibleItems;
	public BaseItem containingItem;
	private bool randomise = true;

	public float releaseItemDelay = 1.5f;
	public float releaseItemForce = 10.0f;

    private Animator animator;

	void Start ()
	{
        animator = GetComponentInChildren<Animator>();

		if(randomise)
			containingItem = Helper.GetRandomItemByProbability(possibleItems);
	}

	public void SetItem(BaseItem item)
	{
		randomise = false;
		containingItem = item;
	}

    void OnCollisionEnter(Collision col)
    {
        if (!opened)
        {
            if (col.collider.gameObject.layer == 14)
            {
				if (ItemsManager.Instance)
				{
                    if (requireKeys)
                    {
                        if ((keyType == KeyScript.Type.Normal && ItemsManager.Instance.Keys > 0) || (keyType == KeyScript.Type.Dungeon && ItemsManager.Instance.DungeonKeys > 0))
                        {
                            Open();

							if(keyType == KeyScript.Type.Normal)
								ItemsManager.Instance.Keys -= 1;
							else
								ItemsManager.Instance.DungeonKeys -= 1;
						}
                    }
                    else
                    {
                        Open();
                    }
				}
				else //If there is no items manager, just open the chest anyway (for testing)
					Open();
            }
        }
    }

	void Open()
	{
        //opens chest and plays animation
		animator.SetTrigger("Open");
		opened = true;

		if(containingItem)
			StartCoroutine(ReleaseItems());

		//Remove icon from map once opened
		MapTracker icon = GetComponent<MapTracker>();
		if (icon)
			icon.Remove();

		EventSender events = GetComponentInParent<EventSender>();
        if (events)
            events.SendDisabledEvent();
	}

	IEnumerator ReleaseItems()
	{
		yield return new WaitForSeconds(releaseItemDelay);

		//Spawn object
		GameObject obj = null;

		//Set containing item
		if (containingItem is Charm)
		{
			obj = ObjectPooler.GetPooledObject(LevelVars.Instance.droppedCharmPrefab);

			CharmPickup pickup = obj.GetComponentInChildren<CharmPickup>();
			if (pickup)
				pickup.representingCharm = (Charm)containingItem;
		}
		else if(containingItem is InventoryItem)
		{
			InventoryItem item = (InventoryItem)containingItem;

			if(item.itemPrefab)
			{
				obj = ObjectPooler.GetPooledObject(item.itemPrefab);
			}
		}

		if (obj)
		{
			obj.transform.position = transform.position + Vector3.up;

			//Throw out of chest
			Rigidbody body = obj.GetComponent<Rigidbody>();
			if(body)
				body.AddForce(Vector3.up * releaseItemForce, ForceMode.Impulse);
		}
	}
}

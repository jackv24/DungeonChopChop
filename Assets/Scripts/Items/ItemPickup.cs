using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
	public SpriteRenderer iconRenderer;

	[Space()]
	[Tooltip("The item that this GameObject represents.")]
	public BaseItem representingItem;
	private BaseItem oldItem = null;

	[Space()]
	public float pickupDelay = 1.0f;
	private float pickupTime;

	void OnEnable()
	{
		pickupTime = Time.time + pickupDelay;
	}

	void Update()
	{
		if (iconRenderer)
		{
			if (representingItem != oldItem)
			{
				oldItem = representingItem;

				if (representingItem.itemIcon)
					iconRenderer.sprite = representingItem.itemIcon;
				else
					iconRenderer.sprite = null;
			}
		}
	}

	void OnTriggerEnter(Collider col)
	{
		if (representingItem && Time.time >= pickupTime)
		{
			PlayerInformation playerInfo = col.GetComponent<PlayerInformation>();

			if (playerInfo)
			{
				//if item is a charm, add charm
                if ((Charm)representingItem)
                    playerInfo.PickupCharm((Charm)representingItem);
                //if item is a item, add item
                else if ((InventoryItem)representingItem)
                    playerInfo.PickupItem((InventoryItem)representingItem);
				else
					Debug.Log("item was not a charm");

				gameObject.SetActive(false);
			}
		}
	}
}

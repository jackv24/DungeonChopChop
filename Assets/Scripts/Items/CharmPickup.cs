using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharmPickup : MonoBehaviour
{
	public SpriteRenderer iconRenderer;

	[Space()]
	[Tooltip("The item that this GameObject represents.")]
	public Charm representingCharm;
	private Charm oldCharm = null;

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
			if (representingCharm != oldCharm)
			{
				oldCharm = representingCharm;

				if (representingCharm.itemIcon)
					iconRenderer.sprite = representingCharm.itemIcon;
			}
		}
	}

	void OnTriggerEnter(Collider col)
	{
		if (representingCharm && Time.time >= pickupTime)
		{
			PlayerInformation playerInfo = col.GetComponent<PlayerInformation>();

			if (playerInfo)
			{
				playerInfo.PickupCharm (representingCharm);

				gameObject.SetActive(false);
			}
		}
	}
}

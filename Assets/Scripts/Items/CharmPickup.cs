using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharmPickup : MonoBehaviour
{
	public MeshRenderer[] iconRenderers;

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
		if (representingCharm != oldCharm)
		{
			oldCharm = representingCharm;

			if(iconRenderers.Length > 0)
			{
				Material mat = iconRenderers[0].material;

				if (representingCharm.itemIcon)
					mat.mainTexture = representingCharm.itemIcon.texture;
				else
					mat.mainTexture = null;

				foreach (MeshRenderer iconRenderer in iconRenderers)
				{
					iconRenderer.sharedMaterial = mat;
				}
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
				playerInfo.PickupCharm(representingCharm);

				gameObject.SetActive(false);
			}
		}
	}
}

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

	[Space()]
	public GameObject pickupUIPrefab;
	public GameObject pickupEffect;

	public static Vector3 lastPickupPos;

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
			lastPickupPos = transform.position;

			PlayerInformation playerInfo = col.GetComponent<PlayerInformation>();

			if (playerInfo)
			{
				playerInfo.PickupCharm(representingCharm);

				if(pickupUIPrefab)
				{
					GameObject obj = ObjectPooler.GetPooledObject(pickupUIPrefab);

					CharmPopup popup = obj.GetComponent<CharmPopup>();
					if(popup)
						popup.Init(representingCharm, playerInfo.transform);
				}

				if(pickupEffect)
				{
					GameObject obj = ObjectPooler.GetPooledObject(pickupEffect);
					obj.transform.position = transform.position;
				}

				transform.parent.gameObject.SetActive(false);
			}
		}
	}
}

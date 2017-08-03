using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
	[Tooltip("The item that this GameObject represents.")]
	public BaseItem representingItem;

	void OnTriggerEnter(Collider col)
	{
		if (representingItem)
		{
			PlayerInformation playerInfo = col.GetComponent<PlayerInformation>();

			if (playerInfo)
			{
				//Call pickup function on item
				Charm charm = (Charm)representingItem;

				if (charm)
					playerInfo.PickupCharm (charm);

				gameObject.SetActive(false);
			}
		}
	}
}

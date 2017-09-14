using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
	public Transform itemSpawn;

	public InventoryItem sellingItem;
	private GameObject itemGraphic;

	private DialogueSpeaker speaker;

	private bool canPurchase = false;
	private PlayerInputs input;
	private PlayerInformation playerInfo;

	void Update()
	{
		if(canPurchase && sellingItem && input != null && input.Purchase.WasPressed)
		{
			if(ItemsManager.Instance.Coins >= sellingItem.cost)
			{
				ItemsManager.Instance.Coins -= sellingItem.cost;

				sellingItem.Pickup(playerInfo);

				if(sellingItem.usePrefabForPickup && sellingItem.itemPrefab)
				{
					GameObject obj = ObjectPooler.GetPooledObject(sellingItem.itemPrefab);
					obj.transform.position = playerInfo.transform.position;
				}

				Destroy(itemGraphic);
				sellingItem = null;

				speaker.Close(true);
			}
		}
	}

	public void SpawnItem(InventoryItem item)
	{
		sellingItem = item;

		if (itemSpawn && item.itemPrefab)
		{
			itemGraphic = Instantiate(item.itemPrefab, itemSpawn);
			itemGraphic.transform.localPosition = Vector3.zero;

			//Only need to display this item, don't need any behaviours
			Component[] components = itemGraphic.GetComponentsInChildren<Component>();
			for(int i = components.Length - 1; i >= 0; i--)
			{
				if (!(components[i] is MeshRenderer || components[i] is MeshFilter || components[i] is Transform))
					DestroyImmediate(components[i], false);
			}
		}

		if (!speaker)
		{
			speaker = GetComponent<DialogueSpeaker>();

			speaker.OnGetPlayer += AllowPurchase;
		}

		if(speaker && speaker.lines.Length > 0)
		{
			speaker.enabled = true;

			//Dialogue box should only show piece of text - the shop text
			string text = speaker.lines[0];
			speaker.lines = new string[] { string.Format(text, item.displayName, item.cost) };
		}
	}

	void AllowPurchase(PlayerInformation playerInfo, bool value)
	{
		canPurchase = value;
		this.playerInfo = playerInfo;

		input = InputManager.GetPlayerInput(playerInfo.playerIndex);
	}
}

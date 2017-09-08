using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
	public Transform itemSpawn;

	public InventoryItem sellingItem;

	public void SpawnItem(InventoryItem item)
	{
		sellingItem = item;

		if(itemSpawn && item.itemPrefab)
		{
			GameObject obj = Instantiate(item.itemPrefab, itemSpawn);
			obj.transform.localPosition = Vector3.zero;
		}

		DialogueSpeaker speaker = GetComponent<DialogueSpeaker>();

		if(speaker && speaker.lines.Length > 0)
		{
			string text = speaker.lines[0];

			speaker.lines = new string[] { string.Format(text, item.displayName, item.cost) };
		}
	}
}

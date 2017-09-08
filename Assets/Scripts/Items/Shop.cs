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

			Component[] components = obj.GetComponentsInChildren<Component>();

			for(int i = components.Length - 1; i >= 0; i--)
			{
				if (!(components[i] is MeshRenderer || components[i] is MeshFilter || components[i] is Transform))
					DestroyImmediate(components[i], false);
			}
		}

		DialogueSpeaker speaker = GetComponent<DialogueSpeaker>();

		if(speaker && speaker.lines.Length > 0)
		{
			string text = speaker.lines[0];

			speaker.lines = new string[] { string.Format(text, item.displayName, item.cost) };
		}
	}

	public void SetGroup(List<Shop> shopGroup)
	{
		List<DialogueSpeaker> speakers = new List<DialogueSpeaker>();

		foreach(Shop shop in shopGroup)
		{
			DialogueSpeaker speaker = shop.GetComponent<DialogueSpeaker>();

			if (speaker)
				speakers.Add(speaker);
		}

		foreach(DialogueSpeaker speaker in speakers)
		{
			speaker.group = speakers;
		}
	}
}

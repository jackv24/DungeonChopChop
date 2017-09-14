using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopSpawner : MonoBehaviour
{
	[Header("Prefabs")]
	public GameObject shopPrefab;
	public GameObject[] decoratorPrefabs;

	[Header("Spacing")]
	public float shopSpacing = 3.0f;
	public float decoratorSpacing = 1.0f;

	public enum Direction { Right, Left }
	[Space()]
	public Direction direction;
	private Vector3 OffsetDirection { get { return (direction == Direction.Right ? Vector3.right : Vector3.left); } }

	[System.Serializable]
	public class ShopStall
	{
		public ItemDatabase.ItemType itemType;

		[Tooltip("If an item of this type and tier is not found, it will try a lower tier.")]
		public int itemTier = 0;
	}

	[Header("Shop Values")]
	public ItemDatabase itemDatabase;
	[Space()]
	public List<ShopStall> shopStalls = new List<ShopStall>();

	void Start()
	{
		Generate();
	}

	public void Generate()
	{
		DeleteChildren();

		if (shopPrefab)
		{
			for (int i = 0; i < shopStalls.Count; i++)
			{
				//Spawn stalls
				GameObject shopObj = Instantiate(shopPrefab, transform);
				shopObj.transform.localPosition = OffsetDirection * shopSpacing * i;

				if (itemDatabase)
				{
					InventoryItem item = itemDatabase.GetItem(shopStalls[i].itemType, shopStalls[i].itemTier);

					if(item)
					{
						Shop shop = shopObj.GetComponent<Shop>();

						if (shop)
							shop.SpawnItem(item);
					}
				}

				//Spawn decorations between stalls
				if(i < shopStalls.Count - 1)
				{
					GameObject decoratorObj = Instantiate(GetRandomDecorator(), transform);
					decoratorObj.transform.localPosition = shopObj.transform.localPosition + OffsetDirection * decoratorSpacing;
				}
			}
		}
		else
			Debug.LogError("Shop spawner has no shop prefab!");
	}

	public void DeleteChildren()
	{
		for (int i = transform.childCount - 1; i >= 0; i--)
		{
			DestroyImmediate(transform.GetChild(i).gameObject, false);
		}
	}

	GameObject GetRandomDecorator()
	{
		if (decoratorPrefabs.Length <= 0)
			Debug.LogError("Shop spawner has no decorator prefabs!");

		return decoratorPrefabs[Random.Range(0, decoratorPrefabs.Length)];
	}
}

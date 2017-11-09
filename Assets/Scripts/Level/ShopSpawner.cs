﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopSpawner : MonoBehaviour
{
    public delegate void ShopEvent();
    public event ShopEvent OnItemPurchased;

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

	private List<BaseItem> spawnedItems = new List<BaseItem>();

    private List<Shop> spawnedShops = new List<Shop>();

	public void Generate()
	{
		DeleteChildren();

		spawnedItems.Clear();

		if (shopPrefab)
		{
			for (int i = 0; i < shopStalls.Count; i++)
			{
				//Spawn stalls
				GameObject shopObj = Instantiate(shopPrefab, transform);
				shopObj.transform.localPosition = OffsetDirection * shopSpacing * i;

				if (itemDatabase)
				{
					BaseItem item = itemDatabase.GetItem(shopStalls[i].itemType, shopStalls[i].itemTier, spawnedItems);
					spawnedItems.Add(item);

					if(item)
					{
						Shop shop = shopObj.GetComponent<Shop>();

                        if (shop)
                        {
                            shop.SpawnItem(item);
                            spawnedShops.Add(shop);
                        }
					}
				}

				//Spawn decorations between stalls
				if(i < shopStalls.Count - 1)
				{
					GameObject decoratorObj = Instantiate(GetRandomDecorator(), transform);
					decoratorObj.transform.localPosition = shopObj.transform.localPosition + OffsetDirection * decoratorSpacing;
				}
			}

            if (Application.isPlaying)
            {
                foreach (Shop shop in spawnedShops)
                {
                    shop.OnItemPurchased += ItemPurchased;
                }
            }
		}
		else
			Debug.LogError("Shop spawner has no shop prefab!");
	}

	public void DeleteChildren()
	{
        if (Application.isPlaying)
        {
            foreach (Shop shop in spawnedShops)
            {
                shop.OnItemPurchased -= ItemPurchased;
            }
        }

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

    void ItemPurchased()
    {
        if (OnItemPurchased != null)
            OnItemPurchased();
    }
}

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

	[Header("Shop Values")]
	public int shopStalls = 3;

	void Start()
	{
		Generate();
	}

	public void Generate()
	{
		DeleteChildren();

		if (shopPrefab)
		{
			for (int i = 0; i < shopStalls; i++)
			{
				GameObject shopObj = Instantiate(shopPrefab, transform);
				shopObj.transform.localPosition = OffsetDirection * shopSpacing * i;

				if(i < shopStalls - 1)
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ObjectSpawner : MonoBehaviour
{
	public GameObject prefab;
	private GameObject oldPrefab;

	void Start()
	{
		Replace();
		oldPrefab = prefab;
	}

	void Update()
	{
		if(prefab != oldPrefab)
		{
			oldPrefab = prefab;
			Replace();
		}
	}

	public void Replace()
	{
		//Remove children
		int childAmount = transform.childCount;
		for (int i = 0; i < childAmount; i++)
		{
			DestroyImmediate(transform.GetChild(0).gameObject);
		}

		if (prefab)
		{
			GameObject obj = Instantiate(prefab, transform);
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localRotation = Quaternion.identity;
		}
	}
}

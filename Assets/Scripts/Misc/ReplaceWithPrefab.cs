using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceWithPrefab : MonoBehaviour
{
	public GameObject prefab;

	public bool replaceOnAwake = false;
	public bool pooled = false;

	void Awake()
	{
		if (replaceOnAwake)
			Replace();
	}

	public void Replace()
	{
		if (prefab)
		{
			GameObject obj = null;

			if (pooled && Application.isPlaying)
				obj = ObjectPooler.GetPooledObject(prefab);
			else
				obj = Instantiate(prefab);

			if (obj)
			{
				obj.transform.parent = transform.parent;
				obj.transform.localPosition = transform.localPosition;
				obj.transform.localRotation = transform.localRotation;
			}

			DestroyImmediate(gameObject);
		}
	}
}

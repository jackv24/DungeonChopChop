using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ObjectSpawner : MonoBehaviour
{
	public GameObject prefab;
	private GameObject oldPrefab;

	public bool spawnAfterMerging = false;

	void Start()
	{
		if (transform.childCount <= 0 || Application.isPlaying)
		{
			if (spawnAfterMerging && Application.isPlaying && LevelGenerator.Instance)
				LevelGenerator.Instance.OnGenerationFinished += Replace;
			else
				Replace();
		}
	}

	void OnDestroy()
	{
		if (spawnAfterMerging && Application.isPlaying && LevelGenerator.Instance)
			LevelGenerator.Instance.OnGenerationFinished -= Replace;
	}

	void Update()
	{
		if (!Application.isPlaying)
		{
			if (prefab != oldPrefab)
			{
				Replace();
			}
		}
	}

	public void Replace()
	{
		oldPrefab = prefab;

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

            PersistentObject persistent = GetComponent<PersistentObject>();
			if(persistent)
			{
                persistent.Setup();
            }
        }
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
	public GameObject prefab;
	private GameObject oldPrefab;

	public bool spawnAfterMerging = false;

    [Space()]
    public bool respawnWithMessage = false;
	public GameObject respawnPrefab;
    public MonoBehaviour enableBehaviour;

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
        ReplaceWith(prefab);

		if (respawnWithMessage && enableBehaviour)
            enableBehaviour.enabled = false;
    }

	public void ReplaceMessage()
	{
		if(respawnWithMessage)
		{
            ReplaceWith(respawnPrefab);

			if(enableBehaviour)
                enableBehaviour.enabled = true;
        }
	}

	void ReplaceWith(GameObject p)
	{
		oldPrefab = p;

		//Remove children
		int childAmount = transform.childCount;
		for (int i = childAmount - 1; i >= 0; i--)
		{
			if(Application.isPlaying)
				Destroy(transform.GetChild(i).gameObject);
			else
				DestroyImmediate(transform.GetChild(i).gameObject);
		}

		if (p)
		{
			GameObject obj = Instantiate(p, transform);
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

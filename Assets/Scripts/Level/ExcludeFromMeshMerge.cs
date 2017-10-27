using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExcludeFromMeshMerge : MonoBehaviour
{
    private Transform oldParent;

    void Awake()
	{
		if(LevelGenerator.Instance)
		{
            LevelGenerator.Instance.OnBeforeMergeMeshes += UnParent;
            LevelGenerator.Instance.OnGenerationFinished += Parent;
        }
    }

	void OnDisable()
	{
		if (LevelGenerator.Instance)
        {
            LevelGenerator.Instance.OnBeforeMergeMeshes -= UnParent;
            LevelGenerator.Instance.OnGenerationFinished -= Parent;
        }
	}

	void UnParent()
	{
        oldParent = transform.parent;

        transform.SetParent(null, true);
    }

	void Parent()
	{
		if(oldParent)
            transform.SetParent(oldParent, true);
    }
}

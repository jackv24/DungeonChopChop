using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceForwardInTile : MonoBehaviour
{
	void Start()
	{
		if(LevelGenerator.Instance)
			LevelGenerator.Instance.OnBeforeMergeMeshes += FaceForward;
	}

	void OnDestroy()
	{
		if (LevelGenerator.Instance)
			LevelGenerator.Instance.OnBeforeMergeMeshes -= FaceForward;
	}

	void FaceForward()
	{
		transform.LookAt(transform.position - Vector3.forward);
	}
}

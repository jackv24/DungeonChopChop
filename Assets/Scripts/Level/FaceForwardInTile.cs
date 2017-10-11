using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceForwardInTile : MonoBehaviour
{
	public bool beforeMerge = true;

	void Start()
	{
		//if (LevelGenerator.Instance)
		//{
		//	if(beforeMerge)
		//		LevelGenerator.Instance.OnBeforeMergeMeshes += FaceForward;
		//	else
		//		LevelGenerator.Instance.OnGenerationFinished += FaceForward;
		//}

		FaceForward();
	}

	//void OnDestroy()
	//{
	//	if (LevelGenerator.Instance)
	//	{
	//		if (beforeMerge)
	//			LevelGenerator.Instance.OnBeforeMergeMeshes -= FaceForward;
	//		else
	//			LevelGenerator.Instance.OnGenerationFinished -= FaceForward;
	//	}
	//}

	void FaceForward()
	{
		transform.LookAt(transform.position - Vector3.forward);
	}
}

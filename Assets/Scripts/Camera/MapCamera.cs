using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCamera : MonoBehaviour
{
	private float height;

	private CameraFollow cam;

	void Start()
	{
		cam = FindObjectOfType<CameraFollow>();

		height = transform.position.y;
	}

	void LateUpdate()
	{
		if(cam)
		{
			transform.position = cam.targetPos + Vector3.up * height;
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTracker : MonoBehaviour
{
	void Start()
	{
		if(MapCamera.Instance)
		{
			MapCamera.Instance.town = transform;
		}
	}
}

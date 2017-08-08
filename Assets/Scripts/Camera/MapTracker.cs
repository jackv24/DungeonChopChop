using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTracker : MonoBehaviour
{
	public Sprite sprite;
	public Color color = Color.white;

	[Space()]
	public bool setLastSibling = false;

	void Start()
	{
		if(sprite && MapCamera.Instance)
		{
			MapCamera.Instance.RegisterIcon(sprite, transform, color, setLastSibling);
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour
{
	public Material insideMaterial;
	public Material outsideMaterial;

	private MeshRenderer rend;

	void Awake()
	{
		rend = GetComponentInChildren<MeshRenderer>();
	}

	void Start()
	{
		if (rend)
			rend.enabled = false;
	}

	public void SetInside()
	{
		if (rend)
		{
			rend.enabled = true;
			rend.sharedMaterial = insideMaterial;
		}
	}

	public void SetOutside()
	{
		if (rend)
		{
			rend.enabled = true;
			rend.sharedMaterial = outsideMaterial;
		}
	}
}

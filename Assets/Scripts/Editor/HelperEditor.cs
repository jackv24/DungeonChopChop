using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class HelperEditor
{
	[MenuItem("Helper/Reduce Materials")]
	private static void ReduceMaterials()
	{
		MeshRenderer[] renderers = GameObject.FindObjectsOfType<MeshRenderer>();

		foreach(MeshRenderer rend in renderers)
		{
			Material mat = rend.materials[0];

			rend.materials = null;

			rend.materials = new Material[1];

			rend.materials[0] = mat;
		}
	}
}

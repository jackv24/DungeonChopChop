using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleToFitOrthoCamera : MonoBehaviour
{
	void Start()
	{
        Camera cam = GetComponentInParent<Camera>();

		if(cam)
		{
            Bounds bounds = GetBounds();

            Vector3 offset = bounds.center - cam.transform.position;
            offset.z = 0;

            transform.position += offset;
        }
    }

	Bounds GetBounds()
	{
        Bounds bounds = new Bounds();

        Renderer[] renderers = GetComponentsInChildren<Renderer>();

		foreach(Renderer rend in renderers)
		{
            Vector3 min = Vector3.Min(bounds.min, rend.bounds.min);
			Vector3 max = Vector3.Max(bounds.max, rend.bounds.max);

            bounds.min = min;
            bounds.max = max;
        }

        return bounds;
    }
}

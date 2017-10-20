using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomRender : MonoBehaviour
{
	public Shader renderShader;

    [Space()]
    public string excludeTag = "Player";
    private List<Renderer> excludeRenderers = new List<Renderer>();

    private Camera cam;

	void Awake()
	{
		cam = GetComponent<Camera>();
	}

	void Start()
	{
		if(cam)
		{
            cam.SetReplacementShader(renderShader, "RenderType");
        }

        GameObject[] exclude = GameObject.FindGameObjectsWithTag(excludeTag);

        foreach(GameObject obj in exclude)
		{
            Renderer[] rends = obj.GetComponentsInChildren<Renderer>();

			foreach(Renderer rend in rends)
			{
				if(!excludeRenderers.Contains(rend))
                    excludeRenderers.Add(rend);
            }
        }
    }

	void OnPreCull()
	{
        EnableRenderers(false);
    }

	void OnPostRender()
	{
		EnableRenderers(true);
	}

	void EnableRenderers(bool value)
	{
		foreach(Renderer rend in excludeRenderers)
        {
        	rend.enabled = value;
        }
	}
}

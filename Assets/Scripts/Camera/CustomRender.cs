using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomRender : MonoBehaviour
{
	public Shader renderShader;

	private Camera cam;

	void Awake()
	{
		cam = GetComponent<Camera>();
	}

	void Start()
	{
		if(cam)
		{
			cam.enabled = false;
		}
	}

	void Update()
	{
		if(cam)
		{
			cam.RenderWithShader(renderShader, "RenderType");
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTexture : MonoBehaviour
{
	public bool createInstance = false;
	public bool scrollNormal = false;

    private MeshRenderer rend;
    public float xIncrease;
    public float yIncrease;

	private Vector2 offset;

	void Start ()
	{
        rend = GetComponent<MeshRenderer>();
	}
	
	void FixedUpdate ()
	{
		if (rend)
		{
			Material mat = rend.sharedMaterial;

			if (createInstance)
				mat = rend.material;

			offset = new Vector2(offset.x + xIncrease * Time.deltaTime, offset.y + yIncrease * Time.deltaTime);

			if (!scrollNormal)
				mat.mainTextureOffset = offset;
			else
			{
				mat.SetTextureOffset("_BumpMap", offset);
			}
		}
	}
}

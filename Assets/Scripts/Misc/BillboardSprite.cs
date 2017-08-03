using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardSprite : MonoBehaviour
{
	void Update()
	{
		transform.LookAt(transform.position - Camera.main.transform.forward, Camera.main.transform.up);
	}
}

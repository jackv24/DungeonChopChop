using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGameObjectState : MonoBehaviour
{
	void Start()
	{
        Debug.Log("<b>" + gameObject.name + " start!</b>");
    }

	void OnDisable()
    {
        Debug.Log("<b>" + gameObject.name + " disable!</b>");
    }
}
